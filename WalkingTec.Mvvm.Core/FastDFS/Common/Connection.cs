using System;
using System.Net.Sockets;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class Connection : TcpClient
    {
        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }
        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        private DateTime _lastUseTime;
        public DateTime LastUseTime
        {
            get { return _lastUseTime; }
            set { _lastUseTime = value; }
        }

        private bool _inUse = false;
        public bool InUse
        {
            get { return _inUse; }
            set { _inUse = value; }
        }
        public void Open()
        {
            if (_inUse)
                throw new FDFSException("the connection is already in user");
            _inUse = true;
            this._lastUseTime = DateTime.Now;
        }
        public new void Close()
        {
            _pool.CloseConnection(this);
        }

        public void Release()
        {
            _pool.ReleaseConnection(this);
        }
    }
}
