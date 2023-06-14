using Optionals;
using RevoltSharp.Commands.Builders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

/// <summary>
///     Provides the information of a command.
/// </summary>
/// <remarks>
///     This object contains the information of a command. This can include the module of the command, various
///     descriptions regarding the command.
/// </remarks>
[DebuggerDisplay("{Name,nq}")]
public class CommandInfo
{
    private static readonly System.Reflection.MethodInfo _convertParamsMethod = typeof(CommandInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
    private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<IEnumerable<object>, object>>();

    private readonly CommandService _commandService;
    private readonly Func<CommandContext, object[], IServiceProvider, CommandInfo, Task> _action;

    /// <summary>
    ///     Gets the module that the command belongs in.
    /// </summary>
    public ModuleInfo Module { get; }
    /// <summary>
    ///     Gets the name of the command. If none is set, the first alias is used.
    /// </summary>
    public string Name { get; }
    /// <summary>
    ///     Gets the summary of the command.
    /// </summary>
    /// <remarks>
    ///     This field returns the summary of the command. <see cref="Summary"/> and <see cref="Remarks"/> can be
    ///     useful in help commands and various implementation that fetches details of the command for the user.
    /// </remarks>
    public string Summary { get; }
    /// <summary>
    ///     Gets the remarks of the command.
    /// </summary>
    /// <remarks>
    ///     This field returns the summary of the command. <see cref="Summary"/> and <see cref="Remarks"/> can be
    ///     useful in help commands and various implementation that fetches details of the command for the user.
    /// </remarks>
    public string Remarks { get; }
    /// <summary>
    ///     Gets the priority of the command. This is used when there are multiple overloads of the command.
    /// </summary>
    public int Priority { get; }
    /// <summary>
    ///     Indicates whether the command accepts a <see langword="params"/> <see cref="Type"/>[] for its
    ///     parameter.
    /// </summary>
    public bool HasVarArgs { get; }
    /// <summary>
    ///     Indicates whether extra arguments should be ignored for this command.
    /// </summary>
    public bool IgnoreExtraArgs { get; }

    /// <summary>
    ///     Gets a list of aliases defined by the <see cref="AliasAttribute" /> of the command.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }
    /// <summary>
    ///     Gets a list of information about the parameters of the command.
    /// </summary>
    public IReadOnlyList<ParameterInfo> Parameters { get; }
    /// <summary>
    ///     Gets a list of preconditions defined by the <see cref="PreconditionAttribute" /> of the command.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions { get; }
    /// <summary>
    ///     Gets a list of attributes of the command.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    internal CommandInfo(CommandBuilder builder, ModuleInfo module, CommandService service)
    {
        Module = module;

        Name = builder.Name;
        Summary = builder.Summary;
        Remarks = builder.Remarks;

        Priority = builder.Priority;

        Aliases = module.Aliases
            .Permutate(builder.Aliases, (first, second) =>
            {
                if (first == "")
                    return second;
                else if (second == "")
                    return first;
                else
                    return first + service._separatorChar + second;
            })
            .Select(x => service._caseSensitive ? x : x.ToLowerInvariant())
            .ToImmutableArray();

        Preconditions = builder.Preconditions.ToImmutableArray();
        Attributes = builder.Attributes.ToImmutableArray();

        Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
        HasVarArgs = builder.Parameters.Count > 0 && builder.Parameters[builder.Parameters.Count - 1].IsMultiple;
        IgnoreExtraArgs = builder.IgnoreExtraArgs;

        _action = builder.Callback;
        _commandService = service;
    }

    public async Task<PreconditionResult> CheckPreconditionsAsync(CommandContext context, IServiceProvider services = null)
    {
        services = services ?? EmptyServiceProvider.Instance;

        async Task<PreconditionResult> CheckGroups(IEnumerable<PreconditionAttribute> preconditions, string type)
        {
            foreach (IGrouping<string, PreconditionAttribute> preconditionGroup in preconditions.GroupBy(p => p.Group, StringComparer.Ordinal))
            {
                if (preconditionGroup.Key == null)
                {
                    foreach (PreconditionAttribute precondition in preconditionGroup)
                    {
                        PreconditionResult result = await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false);
                        if (!result.IsSuccess)
                            return result;
                    }
                }
                else
                {
                    List<PreconditionResult> results = new List<PreconditionResult>();
                    foreach (PreconditionAttribute precondition in preconditionGroup)
                        results.Add(await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false));

                    if (!results.Any(p => p.IsSuccess))
                        return PreconditionGroupResult.FromError($"{type} precondition group {preconditionGroup.Key} failed.", results);
                }
            }
            return PreconditionGroupResult.FromSuccess();
        }

        PreconditionResult moduleResult = await CheckGroups(Module.Preconditions, "Module").ConfigureAwait(false);
        if (!moduleResult.IsSuccess)
            return moduleResult;

        PreconditionResult commandResult = await CheckGroups(Preconditions, "Command").ConfigureAwait(false);
        if (!commandResult.IsSuccess)
            return commandResult;

        return PreconditionResult.FromSuccess();
    }

    public async Task<ParseResult> ParseAsync(CommandContext context, int startIndex, SearchResult searchResult, PreconditionResult preconditionResult = null, IServiceProvider services = null)
    {
        services = services ?? EmptyServiceProvider.Instance;

        if (!searchResult.IsSuccess)
            return ParseResult.FromError(searchResult);
        if (preconditionResult != null && !preconditionResult.IsSuccess)
            return ParseResult.FromError(preconditionResult);

        string input = searchResult.Text.Substring(startIndex);

        return await CommandParser.ParseArgsAsync(this, context, _commandService._ignoreExtraArgs, services, input, 0, _commandService._quotationMarkAliasMap).ConfigureAwait(false);
    }

    public Task<IResult> ExecuteAsync(CommandContext context, ParseResult parseResult, IServiceProvider services)
    {
        if (!parseResult.IsSuccess)
            return Task.FromResult((IResult)ExecuteResult.FromError(parseResult));

        object[] argList = new object[parseResult.ArgValues.Count];
        for (int i = 0; i < parseResult.ArgValues.Count; i++)
        {
            if (!parseResult.ArgValues[i].IsSuccess)
                return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ArgValues[i]));
            argList[i] = parseResult.ArgValues[i].Values.First().Value;
        }

        object[] paramList = new object[parseResult.ParamValues.Count];
        for (int i = 0; i < parseResult.ParamValues.Count; i++)
        {
            if (!parseResult.ParamValues[i].IsSuccess)
                return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ParamValues[i]));
            paramList[i] = parseResult.ParamValues[i].Values.First().Value;
        }

        return ExecuteAsync(context, argList, paramList, services);
    }
    public async Task<IResult> ExecuteAsync(CommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
    {
        services = services ?? EmptyServiceProvider.Instance;

        try
        {
            object[] args = GenerateArgs(argList, paramList);

            for (int position = 0; position < Parameters.Count; position++)
            {
                ParameterInfo parameter = Parameters[position];
                object argument = args[position];
                PreconditionResult result = await parameter.CheckPreconditionsAsync(context, argument, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    Module.Service.InvokeCommandExecuted(Optional.Some(this), context, result);
                    return ExecuteResult.FromError(result);
                }
            }

            _ = Task.Run(async () =>
            {
                await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
            });
            return ExecuteResult.FromSuccess();
        }
        catch (Exception ex)
        {
            return ExecuteResult.FromError(ex);
        }
    }

    private async Task<IResult> ExecuteInternalAsync(CommandContext context, object[] args, IServiceProvider services)
    {
        //await Module.Service._cmdLogger.DebugAsync($"Executing {GetLogText(context)}").ConfigureAwait(false);
        try
        {
            Task task = _action(context, args, services, this);
            if (task is Task<IResult> resultTask)
            {
                IResult result = await resultTask.ConfigureAwait(false);
                Module.Service.InvokeCommandExecuted(Optional.Some(this), context, result);
                if (result is RuntimeResult execResult)
                    return execResult;
            }
            else if (task is Task<ExecuteResult> execTask)
            {
                ExecuteResult result = await execTask.ConfigureAwait(false);
                Module.Service.InvokeCommandExecuted(Optional.Some(this), context, result);
                return result;
            }
            else
            {
                await task.ConfigureAwait(false);
                ExecuteResult result = ExecuteResult.FromSuccess();
                Module.Service.InvokeCommandExecuted(Optional.Some(this), context, result);
            }

            ExecuteResult executeResult = ExecuteResult.FromSuccess();
            return executeResult;
        }
        catch (Exception ex)
        {
            Exception originalEx = ex;
            while (ex is TargetInvocationException) //Happens with void-returning commands
                ex = ex.InnerException;

            CommandException wrappedEx = new CommandException(this, context, ex);
            //await Module.Service._cmdLogger.ErrorAsync(wrappedEx).ConfigureAwait(false);

            ExecuteResult result = ExecuteResult.FromError(ex);
            Module.Service.InvokeCommandExecuted(Optional.Some(this), context, result);

            return result;
        }
        finally
        {
            //await Module.Service._cmdLogger.VerboseAsync($"Executed {GetLogText(context)}").ConfigureAwait(false);
        }
    }

    private object[] GenerateArgs(IEnumerable<object> argList, IEnumerable<object> paramsList)
    {
        int argCount = Parameters.Count;
        object[] array = new object[Parameters.Count];
        if (HasVarArgs)
            argCount--;

        int i = 0;
        foreach (object arg in argList)
        {
            if (i == argCount)
                throw new InvalidOperationException("Command was invoked with too many parameters.");
            array[i++] = arg;
        }
        if (i < argCount)
            throw new InvalidOperationException("Command was invoked with too few parameters.");

        if (HasVarArgs)
        {
            Func<IEnumerable<object>, object> func = _arrayConverters.GetOrAdd(Parameters[Parameters.Count - 1].Type, t =>
            {
                MethodInfo method = _convertParamsMethod.MakeGenericMethod(t);
                return (Func<IEnumerable<object>, object>)method.CreateDelegate(typeof(Func<IEnumerable<object>, object>));
            });
            array[i] = func(paramsList);
        }

        return array;
    }

    private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList)
        => paramsList.Cast<T>().ToArray();

    internal string GetLogText(CommandContext context)
    {
        if (context.Server != null)
            return $"\"{Name}\" for {context.User} in {context.Server}/{context.Channel}";
        else
            return $"\"{Name}\" for {context.User} in {context.Channel}";
    }
}
