using System;
using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace Agridea.Core
{
    /// <summary>
    /// Simple Log class to use with Log4net
    /// Enables logging Info, Verbose, Warning and Error and other TraceEventType severities
    /// </summary>
    public static class Log
    {
        #region Members

        private static readonly ILog log2_;

        #endregion Members

        #region Initialization

        static Log()
        {
            log2_ = LogManager.GetLogger("Acorda2");
        }

        #endregion Initialization

        #region Helpers

        private static void WriteException(TraceEventType severity, Exception exception)
        {
            if (severity == TraceEventType.Warning && log2_.IsWarnEnabled)
                log2_.Warn(GetMessageFor(exception));
            if (severity == TraceEventType.Error && log2_.IsErrorEnabled)
                log2_.Error(GetMessageFor(exception));
        }

        #endregion Helpers

        #region Services

        public static void Write(TraceEventType severity, string format, params object[] args)
        {
            Write(severity, string.Format(format, args));
        }

        public static void Write(TraceEventType severity, string message)
        {
            if (log2_.IsInfoEnabled)
                log2_.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public static void Info(string message)
        {
            if (log2_.IsInfoEnabled)
                log2_.Info(message);
        }

        public static void Verbose(string format, params object[] args)
        {
            Verbose(string.Format(format, args));
        }

        public static void Verbose(string message)
        {
            if (log2_.IsDebugEnabled)
                log2_.Debug(message);
        }

        public static void Warning(string format, params object[] args)
        {
            Warning(string.Format(format, args));
        }

        public static void Warning(string message)
        {
            if (log2_.IsWarnEnabled)
                log2_.Warn(message);
        }

        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public static void Error(string message)
        {
            if (log2_.IsErrorEnabled)
                log2_.Error(message);
        }

        public static void Warning(Exception exception)
        {
            WriteException(TraceEventType.Warning, exception);
        }

        public static void Error(Exception exception)
        {
            WriteException(TraceEventType.Error, exception);
        }

        public static string GetMessageFor(Exception exception)
        {
            return string.Format("'{0}' -  '{2}'{1}  InnerExceptions{1}{3}{1}  StackTrace{1}{4}",
                                 exception.GetType().Name,
                                 Environment.NewLine,
                                 exception.Message,
                                 GetInnerExceptionsMessage(exception),
                                 exception.StackTrace);
        }

        public static void Flush()
        {
            var logger = log2_.Logger as Logger;
            if (logger != null)
            {
                foreach (IAppender appender in logger.Appenders)
                {
                    var buffered = appender as BufferingAppenderSkeleton;
                    if (buffered != null)
                        buffered.Flush();
                }
            }
        }

        public static string GetInnerExceptionsMessage(Exception exception)
        {
            string message = string.Empty;
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                message += string.Format("    '{0}' - '{1}'{2}", innerException.GetType().Name, innerException.Message, Environment.NewLine);
                innerException = innerException.InnerException;
            }
            return message;
        }

        #endregion Services
    }
}