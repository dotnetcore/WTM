using System;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public enum FDFSErrorCode
    {
        General = 0,
        ConnectionTimeOut = 1
    }


    public class FDFSException : Exception
    {
        private FDFSErrorCode _errorCode;

        public FDFSErrorCode ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        public FDFSException(string msg) : base(msg)
        {

        }

        public FDFSException(FDFSErrorCode errorCode, string msg)
            : base(msg)
        {
            _errorCode = errorCode;
        }


    }
}
