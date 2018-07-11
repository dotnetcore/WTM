using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class FDFSConfig
    {
        public static int Storage_MaxConnection { get; set; }
        public static int Tracker_MaxConnection { get; set; }
        public static int ConnectionTimeout { get; set; }
        public static int Connection_LifeTime { get; set; }
        public static Encoding Charset = Encoding.UTF8;
        public static List<IPEndPoint> Trackers { get; set; }
    }
}
