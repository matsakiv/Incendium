namespace Incendium
{
    /// <summary>
    /// Represents result type that can contain a not null success value or error
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    public readonly struct Result<T>
    {
        /// <summary>
        /// Not null success value
        /// </summary>
        public T Value { get; init; }
        /// <summary>
        /// Error value
        /// </summary>
        public Error? Error { get; init; }

        /// <summary>
        /// Initialize result instance from success value
        /// </summary>
        /// <param name="value">Value if success</param>
        public static implicit operator Result<T>(T value) => new() { Value = value };
        /// <summary>
        /// Initialize result instance from error value
        /// </summary>
        /// <param name="error">Error value</param>
        public static implicit operator Result<T>(Error error) => new() { Error = error };
        /// <summary>
        /// Initialize result instance from nullable error value
        /// </summary>
        /// <param name="error">Nullable error value</param>
        public static implicit operator Result<T>(Error? error) => new() { Error = error };

        public void Deconstruct(out T value, out Error? error)
        {
            value = Value;
            error = Error;
        }
    }

    /// <summary>
    /// Represents result type that can contain a nullable success value or error
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct NullableResult<T>
    {
        /// <summary>
        /// Not null success value
        /// </summary>
        public T? Value { get; init; }
        /// <summary>
        /// Error value
        /// </summary>
        public Error? Error { get; init; }

        /// <summary>
        /// Initialize result instance from success value
        /// </summary>
        /// <param name="value">Value if success</param>
        public static implicit operator NullableResult<T>(T value) => new() { Value = value };
        /// <summary>
        /// Initialize result instance from error value
        /// </summary>
        /// <param name="error">Error value</param>
        public static implicit operator NullableResult<T>(Error error) => new() { Error = error };
        /// <summary>
        /// Initialize result instance from nullable error value
        /// </summary>
        /// <param name="error">Nullable error value</param>
        public static implicit operator NullableResult<T>(Error? error) => new() { Error = error };

        public void Deconstruct(out T? value, out Error? error)
        {
            value = Value;
            error = Error;
        }
    }
}
