using System;

namespace Incendium
{
    /// <summary>
    /// Represents a result type that can contain either a nullable value or an error.
    /// This type is particularly useful when the success case may legitimately be null.
    /// </summary>
    /// <typeparam name="T">The type of the success value, which may be null</typeparam>
    /// <remarks>
    /// Unlike <see cref="Result{T}"/>, this type allows null values as valid success cases.
    /// Use this type when working with operations that can return null as a valid result.
    /// </remarks>
    public readonly struct NullableResult<T>
    {
        /// <summary>
        /// Gets the success value, which may be null even in a successful result.
        /// </summary>
        /// <remarks>
        /// A null value here doesn't necessarily indicate failure - check <see cref="Error"/>
        /// to determine if the result represents a failure.
        /// </remarks>
        public T? Value { get; init; }

        /// <summary>
        /// Gets the error if the result represents a failure, or null if the result is successful.
        /// </summary>
        /// <remarks>
        /// This property being null indicates success, even if <see cref="Value"/> is also null.
        /// </remarks>
        public Error? Error { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableResult{T}"/> struct representing a success.
        /// </summary>
        /// <param name="value">The success value, which may be null.</param>
        /// <remarks>
        /// Use this constructor to create a successful result, even when the value is null.
        /// </remarks>
        public NullableResult(T? value)
        {
            Value = value;
            Error = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableResult{T}"/> struct representing a failure.
        /// </summary>
        /// <param name="error">The error describing the failure.</param>
        /// <exception cref="ArgumentNullException">Thrown when the error is null.</exception>
        /// <remarks>
        /// The error parameter cannot be null - use the value constructor with null
        /// to represent a successful null result.
        /// </remarks>
        public NullableResult(Error error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
            Value = default;
        }

        /// <summary>
        /// Implicitly converts a nullable value to a successful result.
        /// </summary>
        /// <param name="value">The value to convert, which may be null.</param>
        /// <returns>A new successful result containing the value.</returns>
        public static implicit operator NullableResult<T>(T? value) => new(value);

        /// <summary>
        /// Implicitly converts an error to a failed result.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A new failed result containing the error.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the error is null.</exception>
        public static implicit operator NullableResult<T>(Error error) => new(error);

        /// <summary>
        /// Deconstructs the result into its success value and error components.
        /// </summary>
        /// <param name="value">When this method returns, contains the success value (which may be null)
        /// if the result was successful, or null if the result contained an error.</param>
        /// <param name="error">When this method returns, contains the error if the result was a failure,
        /// or null if the result was successful.</param>
        public void Deconstruct(out T? value, out Error? error)
        {
            value = Value;
            error = Error;
        }

        /// <summary>
        /// Maps the success value to a new result using the provided mapping function.
        /// If the current result contains an error, returns a new result with the same error.
        /// </summary>
        /// <typeparam name="TResult">The type of the mapped value</typeparam>
        /// <param name="mapper">A function to map the success value to a new value</param>
        /// <returns>A new result containing either the mapped value or the original error</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null</exception>
        public NullableResult<TResult> Map<TResult>(Func<T?, TResult?> mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (Error != null)
                return Error;

            return mapper(Value);
        }

        /// <summary>
        /// Binds the success value to a new result using the provided binding function.
        /// If the current result contains an error, returns a new result with the same error.
        /// </summary>
        /// <typeparam name="TResult">The type of the bound result</typeparam>
        /// <param name="binder">A function that returns a new result</param>
        /// <returns>The bound result or a new result containing the original error</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null</exception>
        public NullableResult<TResult> Bind<TResult>(Func<T?, NullableResult<TResult>> binder)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            if (Error != null)
                return Error;

            return binder(Value);
        }

        /// <summary>
        /// Matches the result to one of two functions depending on whether it contains a success value or an error.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the match operation</typeparam>
        /// <param name="onSuccess">The function to execute if the result is successful</param>
        /// <param name="onError">The function to execute if the result contains an error</param>
        /// <returns>The result of either the success or error function</returns>
        /// <exception cref="ArgumentNullException">Thrown when onSuccess or onError is null</exception>
        public TResult Match<TResult>(Func<T?, TResult> onSuccess, Func<Error, TResult> onError)
        {
            if (onSuccess == null)
                throw new ArgumentNullException(nameof(onSuccess));
            if (onError == null)
                throw new ArgumentNullException(nameof(onError));

            return Error != null
                ? onError(Error)
                : onSuccess(Value);
        }

        /// <summary>
        /// Executes one of two actions depending on whether the result contains a success value or an error.
        /// </summary>
        /// <param name="onSuccess">The action to execute if the result is successful</param>
        /// <param name="onError">The action to execute if the result contains an error</param>
        /// <exception cref="ArgumentNullException">Thrown when onSuccess or onError is null</exception>
        public void Match(Action<T?> onSuccess, Action<Error> onError)
        {
            if (onSuccess == null)
                throw new ArgumentNullException(nameof(onSuccess));
            if (onError == null)
                throw new ArgumentNullException(nameof(onError));

            if (Error != null)
                onError(Error);
            else
                onSuccess(Value);
        }

        /// <summary>
        /// Attempts to get the success value from the result.
        /// </summary>
        /// <param name="value">When this method returns, contains the success value (which may be null)
        /// if the result was successful, or null if the result contained an error</param>
        /// <returns>true if the result was successful (even if the value is null); otherwise, false</returns>
        public bool TryGetValue(out T? value)
        {
            value = Value;
            return Error == null;
        }

        /// <summary>
        /// Executes the provided action on a successful result and returns the original result.
        /// </summary>
        /// <param name="action">The action to execute on the success value</param>
        /// <returns>The original result</returns>
        /// <exception cref="ArgumentNullException">Thrown when action is null</exception>
        public NullableResult<T> Tap(Action<T?> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (Error == null)
                action(Value);

            return this;
        }

        /// <summary>
        /// Throws an exception if the result contains an error.
        /// </summary>
        /// <returns>The success value if the result is successful (may be null).</returns>
        /// <exception cref="InvalidOperationException">Thrown when the result contains an error.</exception>
        public T? ThrowIfError()
        {
            if (Error != null)
                throw new InvalidOperationException($"Result contains an error: {Error.Message}");

            return Value;
        }

        /// <summary>
        /// Creates a new successful result with the specified value.
        /// </summary>
        /// <param name="value">The value, which may be null.</param>
        /// <returns>A new NullableResult instance containing the success value.</returns>
        public static NullableResult<T> Success(T? value) => new(value);

        /// <summary>
        /// Creates a new failed result with the specified error.
        /// </summary>
        /// <param name="error">The error describing the failure.</param>
        /// <returns>A new NullableResult instance containing the error.</returns>
        /// <exception cref="ArgumentNullException">Thrown when error is null.</exception>
        public static NullableResult<T> Failure(Error error) => new(error);
    }
}
