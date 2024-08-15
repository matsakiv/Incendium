using System;

namespace Incendium
{
    /// <summary>
    /// Represents a nullable result type, which can contain a nullable success value or an error value
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    public readonly struct NullableResult<T>
    {
        /// <summary>
        /// Gets nullable success value
        /// </summary>
        public T? Value { get; init; }
        /// <summary>
        /// Gets nullable error value
        /// </summary>
        public Error? Error { get; init; }

        /// <summary>
        /// Initialize a result instance from the success nullable value
        /// </summary>
        /// <param name="value">Nullable success value</param>
        public NullableResult(T? value)
        {
            Value = value;
        }

        /// <summary>
        /// Initialize a result instance from the error value
        /// </summary>
        /// <param name="error">Error</param>
        public NullableResult(Error error)
        {
            Error = error;
        }

        /// <summary>
        /// Initialize a result instance from the non-null error value with nullable type
        /// </summary>
        /// <param name="error">Non-null error value with nullable type</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NullableResult(Error? error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        /// <summary>
        /// Initialize a result instance from the success nullable value
        /// </summary>
        /// <param name="value">Nullable success value</param>
        public static implicit operator NullableResult<T>(T? value) => new(value);
        /// <summary>
        /// Initialize a result instance from the error value
        /// </summary>
        /// <param name="error">Error</param>
        public static implicit operator NullableResult<T>(Error error) => new(error);
        /// <summary>
        /// Initialize a result instance from the non-null error value with nullable type
        /// </summary>
        /// <param name="error">Non-null error value with nullable type</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static implicit operator NullableResult<T>(Error? error) => new(error);

        /// <summary>
        /// Deconstruct struct to success value and error value
        /// </summary>
        /// <param name="value">Nullable success value</param>
        /// <param name="error">Nullable error value</param>
        public void Deconstruct(out T? value, out Error? error)
        {
            value = Value;
            error = Error;
        }
    }
}
