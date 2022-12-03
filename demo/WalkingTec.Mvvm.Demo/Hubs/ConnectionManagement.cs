using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Demo.Hubs
{
    public static class ConnectionManagement
    {
        public static List<SignalRConn> SignalRConns { get; set; } = new();

        public static void AddConnInfo(SignalRConn conn)
        {
            SignalRConns.Add(conn);
        }
        public static void DelConnInfo(string connid)
        {
            var signalRConns = SignalRConns.FirstOrDefault(x => x.ConnectionId == connid);
            SignalRConns.Remove(signalRConns);
        }
        public static void EditConnInfo(string userid, string newconnid)
        {
            var signalRConns = SignalRConns.FirstOrDefault(x => x.UserId.ToLower() == userid.ToLower());
            signalRConns.ConnectionId = newconnid;
        }

        public static bool IsConn(string userid)
        {
            var b = SignalRConns.FirstOrDefault(x => x.UserId.ToLower() == userid.ToLower());
            if (b != null)
            {
                return true;
            }
            return false;
        }
    }
}
