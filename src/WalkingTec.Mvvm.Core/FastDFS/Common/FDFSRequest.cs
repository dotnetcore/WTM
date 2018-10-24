using System;
using System.Net.Sockets;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class FDFSRequest
    {
        private FDFSHeader _header;
        public FDFSHeader Header
        {
            set { _header = value; }
            get { return _header; }
        }
        private byte[] _body;
        public byte[] Body
        {
            set { _body = value; }
            get { return _body; }
        }
        private Connection _connection;
        public Connection Connection
        {
            get { return _connection; }
            set { this._connection = value; }
        }

        public FDFSRequest()
        {

        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] GetResponse()
        {
            if (this._connection == null)
                this._connection = ConnectionManager.GetTrackerConnection();
            _connection.Open();
            try
            {
                NetworkStream stream = this._connection.GetStream();
                byte[] headerBuffer = this._header.ToByte();
                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(this._body, 0, this._body.Length);

                FDFSHeader header = new FDFSHeader(stream);
                if (header.Status != 0)
                    throw new FDFSException(string.Format("Get Response Error,Error Code:{0}", header.Status));

                int count = (int)header.Length;
                byte[] body = new byte[count];
                var offset = 0;
                while (count > 0)
                {
                    var readCount = stream.Read(body, offset, count);
                    if (readCount <= 0)
                        throw new FDFSException("Get Response Error,readCount <= 0");

                    offset += readCount;
                    count -= readCount;
                }

                _connection.Close();
                return body;
            }
            catch (Exception)
            {
                _connection.Release();
                //throw ex;//可以看Storage节点的log看
                //22    -〉下载字节数超过文件长度 invalid download file bytes: 10 > file remain bytes: 4
                //      -> 或者 pkg length is not correct
                //2     -〉没有此文件 error info: No such file or directory.
            }
            return null;
        }
    }
}