using System;

namespace Incendium
{
    /// <summary>
    /// Represents a result type, which can contain a non-null success value or an error value
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    public readonly struct Result<T>
    {
        /// <summary>
        /// Gets a non-zero value if the error is null, otherwise gets null
        /// </summary>
        public T Value { get; private init; }
        /// <summary>
        /// Gets nullable error value
        /// </summary>
        public Error? Error { get; private init; }

        /// <summary>
        /// Initialize a result instance from the success non-null value
        /// </summary>
        /// <param name="value">Non-null success value</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Result(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        /// <summary>
        /// Initialize a result instance from the error value
        /// </summary>
        /// <param name="error">Error</param>
        public Result(Error error)
        {
            Value = default!;
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        /// <summary>
        /// Initialize a result instance from the success non-null value
        /// </summary>
        /// <param name="value">Non-null success value</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static implicit operator Result<T>(T value) => new(value);
        /// <summary>
        /// Initialize a result instance from the error value
        /// </summary>
        /// <param name="error">Error</param>
        public static implicit operator Result<T>(Error error) => new(error);
        /// <summary>
        /// Deconstruct struct to success value and error value
        /// </summary>
        /// <param name="value">Success value</param>
        /// <param name="error">Nullable error value</param>
        public void Deconstruct(out T value, out Error? error)
        {
            value = Value;
            error = Error;
        }
    }
}
