using System;

namespace Incendium
{
    /// <summary>
    /// Represents error struct that contains code, message and optionally exception
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="exception">Inner exception</param>
    public readonly struct Error(int code, string message, Exception? exception)
    {
        /// <summary>
        /// Gets error code
        /// </summary>
        public int Code { get; init; } = code;
        /// <summary>
        /// Gets error message
        /// </summary>
        public string Message { get; init; } = message;
        /// <summary>
        /// Gets optional inner exception
        /// </summary>
        public Exception? Exception { get; init; } = exception;

        /// <summary>
        /// Initialize a new instance of the Error struct with a specified error code and message
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public Error(int code, string message)
            : this(code, message, exception: null)
        {
        }
    }
}
