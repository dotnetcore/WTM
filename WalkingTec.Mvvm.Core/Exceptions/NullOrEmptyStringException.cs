using System;
using System.Runtime.Serialization;

namespace WalkingTec.Mvvm.Core.Exceptions
{
    /// <summary>
    /// 对象为 null 或 空字符串 异常
    /// </summary>
    public class NullOrEmptyStringException : Exception
    {
        public const string DEFAULT_EXCEPTION_MSG = "对象为 null 或 空字符串 异常";
        public NullOrEmptyStringException() : base(DEFAULT_EXCEPTION_MSG) { }
        public NullOrEmptyStringException(string message) : base(message) { }
        public NullOrEmptyStringException(string message, Exception innerException) : base(message, innerException) { }
        protected NullOrEmptyStringException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
