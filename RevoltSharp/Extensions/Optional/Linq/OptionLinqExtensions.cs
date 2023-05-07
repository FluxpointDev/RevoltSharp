using System;

namespace Optionals.Linq;

public static class OptionLinqExtensions
{
    internal static Optional<TResult> Select<TSource, TResult>(this Optional<TSource> source, Func<TSource, TResult> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return source.Map(selector);
    }

    internal static Optional<TResult> SelectMany<TSource, TResult>(this Optional<TSource> source, Func<TSource, Optional<TResult>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return source.FlatMap(selector);
    }

    internal static Optional<TResult> SelectMany<TSource, TCollection, TResult>(
            this Optional<TSource> source,
            Func<TSource, Optional<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
    {
        if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
        return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
    }

    internal static Optional<TSource> Where<TSource>(this Optional<TSource> source, Func<TSource, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return source.Filter(predicate);
    }

    internal static Optional<TResult, TException> Select<TSource, TException, TResult>(this Optional<TSource, TException> source, Func<TSource, TResult> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return source.Map(selector);
    }

    internal static Optional<TResult, TException> SelectMany<TSource, TException, TResult>(
            this Optional<TSource, TException> source,
            Func<TSource,
            Optional<TResult, TException>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return source.FlatMap(selector);
    }

    internal static Optional<TResult, TException> SelectMany<TSource, TException, TCollection, TResult>(
            this Optional<TSource, TException> source,
            Func<TSource, Optional<TCollection, TException>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
    {
        if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
        if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
        return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
    }
}
