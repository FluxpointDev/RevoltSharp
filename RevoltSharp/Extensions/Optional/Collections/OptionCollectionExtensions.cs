// Note: Several of the below implementations are closely inspired by the corefx source code for FirstOrDefault, etc.

using System;
using System.Collections.Generic;

namespace Optionals.Collections;

public static class OptionCollectionExtensions
{

    /// <summary>
    /// Flattens a sequence of optionals into a sequence containing all inner values.
    /// Empty elements are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    internal static IEnumerable<T> Values<T>(this IEnumerable<Optional<T>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (Optional<T> option in source)
        {
            if (option.HasValue)
            {
                yield return option.Value;
            }
        }
    }

    /// <summary>
    /// Flattens a sequence of optionals into a sequence containing all inner values.
    /// Empty elements and their exceptional values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    internal static IEnumerable<T> Values<T, TException>(this IEnumerable<Optional<T, TException>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (Optional<T, TException> option in source)
        {
            if (option.HasValue)
            {
                yield return option.Value;
            }
        }
    }

    /// <summary>
    /// Flattens a sequence of optionals into a sequence containing all exceptional values.
    /// Non-empty elements and their values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of exceptional values.</returns>
    internal static IEnumerable<TException> Exceptions<T, TException>(this IEnumerable<Optional<T, TException>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (Optional<T, TException> option in source)
        {
            if (!option.HasValue)
            {
                yield return option.Exception;
            }
        }
    }

    /// <summary>
    /// Returns the value associated with the specified key if such exists.
    /// A dictionary lookup will be used if available, otherwise falling
    /// back to a linear scan of the enumerable.
    /// </summary>
    /// <param name="source">The dictionary or enumerable in which to locate the key.</param>
    /// <param name="key">The key to locate.</param>
    /// <returns>An Option&lt;TValue&gt; instance containing the associated value if located.</returns>
    internal static Optional<TValue> GetValueOrNone<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.TryGetValue(key, out TValue? value) ? value.Some() : value.None();
        }
#if !NET35
        else if (source is IReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
        {
            return readOnlyDictionary.TryGetValue(key, out TValue? value) ? value.Some() : value.None();
        }
#endif

        return source
            .FirstOrNone(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key))
            .Map(pair => pair.Value);
    }

    /// <summary>
    /// Returns the first element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the first element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    internal static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IList<TSource> list)
        {
            if (list.Count > 0)
            {
                return list[0].Some();
            }
        }
#if !NET35
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            if (readOnlyList.Count > 0)
            {
                return readOnlyList[0].Some();
            }
        }
#endif
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current.Some();
                }
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns the first element of a sequence, satisfying a specified predicate, 
    /// if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the first element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    internal static Optional<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        foreach (TSource? element in source)
        {
            if (predicate(element))
            {
                return element.Some();
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns the last element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    internal static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IList<TSource> list)
        {
            int count = list.Count;
            if (count > 0)
            {
                return list[count - 1].Some();
            }
        }
#if !NET35
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            int count = readOnlyList.Count;
            if (count > 0)
            {
                return readOnlyList[count - 1].Some();
            }
        }
#endif
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    TSource result;
                    do
                    {
                        result = enumerator.Current;
                    }
                    while (enumerator.MoveNext());

                    return result.Some();
                }
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns the last element of a sequence, satisfying a specified predicate, 
    /// if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    internal static Optional<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        if (source is IList<TSource> list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                TSource? result = list[i];
                if (predicate(result))
                {
                    return result.Some();
                }
            }
        }
#if !NET35
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            for (int i = readOnlyList.Count - 1; i >= 0; --i)
            {
                TSource? result = readOnlyList[i];
                if (predicate(result))
                {
                    return result.Some();
                }
            }
        }
#endif
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TSource? result = enumerator.Current;
                    if (predicate(result))
                    {
                        while (enumerator.MoveNext())
                        {
                            TSource? element = enumerator.Current;
                            if (predicate(element))
                            {
                                result = element;
                            }
                        }

                        return result.Some();
                    }
                }
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns a single element from a sequence, if it exists 
    /// and is the only element in the sequence.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    internal static Optional<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IList<TSource> list)
        {
            switch (list.Count)
            {
                case 0: return Optional.None<TSource>();
                case 1: return list[0].Some();
            }
        }
#if !NET35
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            switch (readOnlyList.Count)
            {
                case 0: return Optional.None<TSource>();
                case 1: return readOnlyList[0].Some();
            }
        }
#endif
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return Optional.None<TSource>();
                }

                TSource? result = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    return result.Some();
                }
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns a single element from a sequence, satisfying a specified predicate, 
    /// if it exists and is the only element in the sequence.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    internal static Optional<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                TSource? result = enumerator.Current;
                if (predicate(result))
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return Optional.None<TSource>();
                        }
                    }

                    return result.Some();
                }
            }
        }

        return Optional.None<TSource>();
    }

    /// <summary>
    /// Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    internal static Optional<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (index >= 0)
        {
            if (source is IList<TSource> list)
            {
                if (index < list.Count)
                {
                    return list[index].Some();
                }
            }
#if !NET35
            else if (source is IReadOnlyList<TSource> readOnlyList)
            {
                if (index < readOnlyList.Count)
                {
                    return readOnlyList[index].Some();
                }
            }
#endif
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (index == 0)
                        {
                            return enumerator.Current.Some();
                        }

                        index--;
                    }
                }
            }
        }

        return Optional.None<TSource>();
    }
}
