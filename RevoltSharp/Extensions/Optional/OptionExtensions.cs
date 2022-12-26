using System;

namespace Optionals
{
    public static class OptionExtensions
    {
        /// <summary>
        /// Wraps an existing value in an Option&lt;T&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T> Some<T>(this T value) => Optional.Some(value);

        /// <summary>
        /// Wraps an existing value in an Option&lt;T, TException&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> Some<T, TException>(this T value) =>
            Optional.Some<T, TException>(value);

        /// <summary>
        /// Creates an empty Option&lt;T&gt; instance from a specified value.
        /// </summary>
        /// <param name="value">A value determining the type of the optional.</param>
        /// <returns>An empty optional.</returns>
        internal static Optional<T> None<T>(this T value) => Optional.None<T>();

        /// <summary>
        /// Creates an empty Option&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An empty optional.</returns>
        internal static Optional<T, TException> None<T, TException>(this T value, TException exception) =>
            Optional.None<T, TException>(exception);

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value does not satisfy the given predicate, 
        /// an empty optional is returned.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T> SomeWhen<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return predicate(value) ? Optional.Some(value) : Optional.None<T>();
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value does not satisfy the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> SomeWhen<T, TException>(this T value, Func<T, bool> predicate, TException exception)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return predicate(value) ? Optional.Some<T, TException>(value) : Optional.None<T, TException>(exception);
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value does not satisfy the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> SomeWhen<T, TException>(this T value, Func<T, bool> predicate, Func<TException> exceptionFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return predicate(value) ? Optional.Some<T, TException>(value) : Optional.None<T, TException>(exceptionFactory());
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value satisfies the given predicate, 
        /// an empty optional is returned.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T> NoneWhen<T>(this T value, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return value.SomeWhen(val => !predicate(val));
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value satisfies the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> NoneWhen<T, TException>(this T value, Func<T, bool> predicate, TException exception)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return value.SomeWhen(val => !predicate(val), exception);
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value does satisfy the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> NoneWhen<T, TException>(this T value, Func<T, bool> predicate, Func<TException> exceptionFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.SomeWhen(val => !predicate(val), exceptionFactory);
        }

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value is null, an empty optional is returned.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T> SomeNotNull<T>(this T value) => value.SomeWhen(val => val != null);

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value is null, an empty optional is returned, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> SomeNotNull<T, TException>(this T value, TException exception) =>
            value.SomeWhen(val => val != null, exception);

        /// <summary>
        /// Creates an Option&lt;T&gt; instance from a specified value. 
        /// If the value is null, an empty optional is returned, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        internal static Optional<T, TException> SomeNotNull<T, TException>(this T value, Func<TException> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.SomeWhen(val => val != null, exceptionFactory);
        }

        /// <summary>
        /// Converts a Nullable&lt;T&gt; to an Option&lt;T&gt; instance.
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <returns>The Option&lt;T&gt; instance.</returns>
        internal static Optional<T> ToOption<T>(this T? value) where T : struct =>
            value.HasValue ? Optional.Some(value.Value) : Optional.None<T>();

        /// <summary>
        /// Converts a Nullable&lt;T&gt; to an Option&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>The Option&lt;T, TException&gt; instance.</returns>
        internal static Optional<T, TException> ToOption<T, TException>(this T? value, TException exception) where T : struct =>
            value.HasValue ? Optional.Some<T, TException>(value.Value) : Optional.None<T, TException>(exception);

        /// <summary>
        /// Converts a Nullable&lt;T&gt; to an Option&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>The Option&lt;T, TException&gt; instance.</returns>
        internal static Optional<T, TException> ToOption<T, TException>(this T? value, Func<TException> exceptionFactory) where T : struct
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.HasValue ? Optional.Some<T, TException>(value.Value) : Optional.None<T, TException>(exceptionFactory());
        }

        /// <summary>
        /// Returns the existing value if present, or the attached 
        /// exceptional value.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The existing or exceptional value.</returns>
        internal static T ValueOrException<T>(this Optional<T, T> option) => option.HasValue ? option.Value : option.Exception;

        /// <summary>
        /// Flattens two nested optionals into one. The resulting optional
        /// will be empty if either the inner or outer optional is empty.
        /// </summary>
        /// <param name="option">The nested optional.</param>
        /// <returns>A flattened optional.</returns>
        internal static Optional<T> Flatten<T>(this Optional<Optional<T>> option) =>
            option.FlatMap(innerOption => innerOption);

        /// <summary>
        /// Flattens two nested optionals into one. The resulting optional
        /// will be empty if either the inner or outer optional is empty.
        /// </summary>
        /// <param name="option">The nested optional.</param>
        /// <returns>A flattened optional.</returns>
        internal static Optional<T, TException> Flatten<T, TException>(this Optional<Optional<T, TException>, TException> option) =>
            option.FlatMap(innerOption => innerOption);
    }
}
