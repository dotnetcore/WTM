//using System;
//using System.Diagnostics;
//using System.Reflection;
//using log4net;

//namespace WalkingTec.Mvvm.Core
//{
//    /// <summary>
//    /// Trace 只有定义了TRACE常量才会生效，可以理解为在Debug与Release下均会输出
//    /// </summary>
//    public sealed class LogTrace
//    {
//        private static ILog _logger;
//        private static ILog Logger
//        {
//            get
//            {
//                if (_logger == null)
//                {
//                    _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//                }
//                return _logger;
//            }
//        }

//        /// <summary>
//        /// Write
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        public static void Write(string message, Exception ex = null)
//        {
//            OutputWindowWriteLine(message);
//            if (ex == null)
//                Logger.Debug(message);
//            else
//                Logger.Debug(message, ex);
//        }
//        /// <summary>
//        /// Debug
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("TRACE")]
//        public static void Debug(string message, Exception ex = null)
//        {
//            Write(message, ex);
//        }
//        /// <summary>
//        /// Info
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("TRACE")]
//        public static void Info(string message, Exception ex = null)
//        {
//            OutputWindowWriteLine(message);
//            if (ex == null)
//                Logger.Info(message);
//            else
//                Logger.Info(message, ex);
//        }
//        /// <summary>
//        /// Warn
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("TRACE")]
//        public static void Warn(string message, Exception ex = null)
//        {
//            OutputWindowWriteLine(message);
//            if (ex == null)
//                Logger.Warn(message);
//            else
//                Logger.Warn(message, ex);
//        }
//        /// <summary>
//        /// Error
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("TRACE")]
//        public static void Error(string message, Exception ex = null)
//        {
//            OutputWindowWriteLine(message);
//            if (ex == null)
//                Logger.Error(message);
//            else
//                Logger.Error(message, ex);
//        }
//        /// <summary>
//        /// Fatal
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("TRACE")]
//        public static void Fatal(string message, Exception ex = null)
//        {
//            OutputWindowWriteLine(message);
//            if (ex == null)
//                Logger.Fatal(message);
//            else
//                Logger.Fatal(message, ex);
//        }

//        #region Output Window
//        [Conditional("DEBUG")]
//        private static void OutputWindowWriteLine(string message, int level = 0, string category = null)
//        {
//            if (System.Diagnostics.Debugger.IsLogging())
//            {
//                //调用Debugger.Log方法，这个方法可以输出信息到DebugView中
//                System.Diagnostics.Debugger.Log(level, category, $"{message}\r\n");
//            }
//        }
//        [Conditional("DEBUG")]
//        private static void OutputWindowWrite(string message, int level = 0, string category = null)
//        {
//            if (System.Diagnostics.Debugger.IsLogging())
//            {
//                System.Diagnostics.Debugger.Log(level, category, message);
//            }
//        }
//        #endregion
//    }
//}
