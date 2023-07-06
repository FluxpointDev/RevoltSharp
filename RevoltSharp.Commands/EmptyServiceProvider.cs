using System;

namespace RevoltSharp.Commands;


internal class EmptyServiceProvider : IServiceProvider
{
    public static readonly EmptyServiceProvider Instance = new EmptyServiceProvider();

    public object? GetService(Type serviceType) => null;
}