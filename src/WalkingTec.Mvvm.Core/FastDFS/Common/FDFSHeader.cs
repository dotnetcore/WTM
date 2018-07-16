using System;
using System.IO;
namespace WalkingTec.Mvvm.Core.FDFS
{
    public class FDFSHeader
    {
        private long _length;
        private byte _command;
        private byte _status;

        /// <summary>
        /// Pachage Length
        /// </summary>        
        public long Length
        {
            set { _length = value; }
            get { return _length; }
        }
        /// <summary>
        /// Command
        /// </summary>
        public byte Command
        {
            set { _command = value; }
            get { return _command; }
        }
        /// <summary>
        /// Status
        /// </summary>
        public byte Status
        {
            set { _status = value; }
            get { return _status; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="command"></param>
        /// <param name="status"></param>
        public FDFSHeader(long length, byte command, byte status)
        {
            _length = length;
            _command = command;
            _status = status;
        }
        public FDFSHeader(Stream stream)
        {
            int count = Consts.FDFS_PROTO_PKG_LEN_SIZE + 2;
            byte[] headerBuffer = new byte[count];
            var offset = 0;
            while (count > 0)
            {
                var readCount = stream.Read(headerBuffer, offset, count);
                if (readCount <= 0)
                    throw new FDFSException("Get Response Header Error,readCount <= 0");

                offset += readCount;
                count -= readCount;
            }


            //int bytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);

            //if (bytesRead == 0)
            //    throw new FDFSException("Init Header Exeption : Cann't Read Stream");

            _length = FDFSUtil.BufferToLong(headerBuffer, 0);
            _command = headerBuffer[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            _status = headerBuffer[Consts.FDFS_PROTO_PKG_LEN_SIZE + 1];
        }
        public byte[] ToByte()
        {
            byte[] result = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE + 2];
            byte[] pkglen = FDFSUtil.LongToBuffer(this._length);
            Array.Copy(pkglen, 0, result, 0, pkglen.Length);
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE] = this._command;
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE + 1] = this._status;
            return result;
        }
    }
}
