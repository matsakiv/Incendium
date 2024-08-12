using System;

namespace Incendium.Extensions
{
    public static class ErrorExtensions
    {
        /// <summary>
        /// Gets error code
        /// </summary>
        /// <param name="error">Nullable error</param>
        /// <returns>Error code</returns>
        public static int Code(this Error? error) => error!.Value.Code;
        /// <summary>
        /// Gets error message
        /// </summary>
        /// <param name="error">Nullable error</param>
        /// <returns>Error message</returns>
        public static string Message(this Error? error) => error!.Value.Message;
        /// <summary>
        /// Gets inner exception
        /// </summary>
        /// <param name="error">Nullable error</param>
        /// <returns>Inner exception</returns>
        public static Exception? Exception(this Error? error) => error!.Value.Exception;
    }
}
