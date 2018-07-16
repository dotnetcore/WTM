//using System;
//using System.Diagnostics;

//namespace WalkingTec.Mvvm.Core
//{
//    /// <summary>
//    /// LogDebug 只有定义了DEBUG常量才会生效，可以理解为只在Debug模式下才会输出
//    /// </summary>
//    public sealed class LogDebug
//    {
//        /// <summary>
//        /// Debug
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("DEBUG")]
//        public static void Debug(string message, Exception ex = null)
//        {
//            LogTrace.Debug(message, ex);
//        }
//        /// <summary>
//        /// Info
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("DEBUG")]
//        public static void Info(string message, Exception ex = null)
//        {
//            LogTrace.Info(message, ex);
//        }
//        /// <summary>
//        /// Warn
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("DEBUG")]
//        public static void Warn(string message, Exception ex = null)
//        {
//            LogTrace.Warn(message, ex);
//        }
//        /// <summary>
//        /// Error
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("DEBUG")]
//        public static void Error(string message, Exception ex = null)
//        {
//            LogTrace.Error(message, ex);
//        }
//        /// <summary>
//        /// Fatal
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="ex"></param>
//        [Conditional("DEBUG")]
//        public static void Fatal(string message, Exception ex = null)
//        {
//            LogTrace.Fatal(message, ex);
//        }
//    }
//}
