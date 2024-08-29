using System;

namespace Incendium
{
    /// <summary>
    /// Represents error type that contains code, message, exception and inner error
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="exception">Inner exception</param>
    /// <param name="innerError">Inner error</param>
    public class Error(int code, string? message, Exception? exception, Error? innerError)
    {
        /// <summary>
        /// Gets error code
        /// </summary>
        public int Code { get; init; } = code;
        /// <summary>
        /// Gets error message
        /// </summary>
        public string? Message { get; init; } = message;
        /// <summary>
        /// Gets inner exception
        /// </summary>
        public Exception? Exception { get; init; } = exception;
        /// <summary>
        /// Gets inner error
        /// </summary>
        public Error? InnerError { get; init; } = innerError;

        /// <summary>
        /// Initialize a new instance of the error
        /// </summary>
        public Error()
            : this(0, message: null, exception: null, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error code
        /// </summary>
        /// <param name="code">Error code</param>
        public Error(int code)
            : this(code, message: null, exception: null, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error message
        /// </summary>
        /// <param name="message">Error message</param>
        public Error(string message)
            : this(code: 0, message: message, exception: null, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified inner exception
        /// </summary>
        /// <param name="exception">Inner exception</param>
        public Error(Exception exception)
            : this(code: 0, message: null, exception: exception, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error code and message
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public Error(int code, string message)
            : this(code, message, exception: null, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error code, message and exception
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="exception">Inner exception</param>
        public Error(int code, string message, Exception? exception)
            : this(code, message, exception, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error message and inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Inner exception</param>
        public Error(string message, Exception exception)
            : this(code: 0, message, exception: exception, innerError: null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the error with a specified error code, message and inner error
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="error">Inner error</param>
        public Error(int code, string message, Error? error)
            : this(code, message, exception: null, innerError: error)
        {
        }
    }
}
