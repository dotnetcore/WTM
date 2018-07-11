using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class Pool
    {
        private List<Connection> inUse;
        private Stack<Connection> idle;

        private AutoResetEvent autoEvent = null;
        private IPEndPoint endPoint = null;
        private int maxConnection = 0;
        public Pool(IPEndPoint endPoint, int maxConnection)
        {
            autoEvent = new AutoResetEvent(false);
            inUse = new List<Connection>(maxConnection);
            idle = new Stack<Connection>(maxConnection);
            this.maxConnection = maxConnection;
            this.endPoint = endPoint;
        }
        private Connection GetPooldConncetion()
        {
            Connection result = null;
            lock ((idle as ICollection).SyncRoot)
            {
                if (idle.Count > 0)
                    result = idle.Pop();
                if (result != null && (int)(DateTime.Now - result.LastUseTime).TotalSeconds > FDFSConfig.Connection_LifeTime)
                {
                    result.Close();
                }
            }
            lock ((inUse as ICollection).SyncRoot)
            {
                if (inUse.Count == maxConnection)
                    return null;
                if (result == null)
                {
                    result = new Connection();
                    result.Connect(endPoint);
                    result.Pool = this;
                }
                inUse.Add(result);
            }
            return result;
        }

        public Connection GetConnection()
        {
            int timeOut = FDFSConfig.ConnectionTimeout * 1000;
            Connection result = null;
            Stopwatch watch = Stopwatch.StartNew();
            while (timeOut > 0)
            {
                result = GetPooldConncetion();
                if (result != null)
                    return result;
                if (!autoEvent.WaitOne(timeOut, false))
                    break;
                watch.Stop();
                timeOut = timeOut - (int)watch.ElapsedMilliseconds;
            }
            throw new FDFSException(FDFSErrorCode.ConnectionTimeOut, "Connection Time Out");
        }

        public void ReleaseConnection(Connection conn)
        {
            if (!conn.InUse)
            {
                try
                {
                    FDFSHeader header = new FDFSHeader(0, Consts.FDFS_PROTO_CMD_QUIT, 0);
                    byte[] buffer = header.ToByte();
                    conn.GetStream().Write(buffer, 0, buffer.Length);
                    conn.GetStream().Close();
                }
                catch
                {
                }
            }

            conn.Close();

            lock ((inUse as ICollection).SyncRoot)
            {
                inUse.Remove(conn);
            }
            autoEvent.Set();
        }
        public void CloseConnection(Connection conn)
        {
            conn.InUse = false;
            lock ((inUse as ICollection).SyncRoot)
            {
                inUse.Remove(conn);
            }
            lock ((idle as ICollection).SyncRoot)
            {
                idle.Push(conn);
            }
            autoEvent.Set();
        }
    }
    public class ConnectionManager
    {
        public static DFS DFSConfig { get; set; }
        public static Dictionary<IPEndPoint, Pool> trackerPools = new Dictionary<IPEndPoint, Pool>();
        public static Dictionary<IPEndPoint, Pool> storePools = new Dictionary<IPEndPoint, Pool>();
        private static List<IPEndPoint> listTrackers = new List<IPEndPoint>();
        private static bool _inited = false;
        public static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (IPEndPoint point in trackers)
            {
                try
                {
                    if (!trackerPools.ContainsKey(point))
                        trackerPools.Add(point, new Pool(point, FDFSConfig.Tracker_MaxConnection));
                }
                catch
                {
                }
            }
            listTrackers = trackers;
            _inited = true;
            return true;
        }

        public static Connection GetTrackerConnection()
        {
            if (_inited == false)
            {
                Initialize(FDFSConfig.Trackers);
            }
            Random random = new Random();
            int index = random.Next(trackerPools.Count);
            Pool pool = trackerPools[listTrackers[index]];
            return pool.GetConnection();
        }

        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            if (_inited == false)
            {
                Initialize(FDFSConfig.Trackers);
            }
            lock ((storePools as ICollection).SyncRoot)
            {
                if (!storePools.ContainsKey(endPoint))
                {
                    Pool pool = new Pool(endPoint, FDFSConfig.Storage_MaxConnection);
                    storePools.Add(endPoint, pool);
                }
            }
            return storePools[endPoint].GetConnection();
        }
    }
}
