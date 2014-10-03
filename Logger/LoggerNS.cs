using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logger {

    public enum LogLevel {
        FATAL = 0,
        ERROR = 1,
        WARN = 2,
        INFO = 3,
        VERBOSE = 4
    }

    public interface ILogger {

        /// <summary>
        /// Write a message to the log
        /// </summary>
        /// <param name="Category">A String of the category to write</param>
        /// <param name="Level">A LogLevel value fo the level of this message</param>
        /// <param name="Message">A String of the message to write to the log</param>
        void WriteMessage(String Category, LogLevel Level, String Message);

    }

    internal class FileLogger : ILogger {
        public void WriteMessage(String Category, LogLevel Level, String Message) {

        }
    }

    /// <summary>
    /// Factory Class to get the appropriate ILogger based on what is specified in app.config file.
    /// </summary>
    public class LoggerFactory {
        #region Member Variables

        // referenced to the ILogger

        #endregion

    }
}
