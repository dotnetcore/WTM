using System;
namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE 104
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ 1 byte: store path index on the storage server
    /// </summary>
    public class QUERY_STORE_WITH_GROUP_ONE : FDFSRequest
    {
        private static QUERY_STORE_WITH_GROUP_ONE _instance = null;
        public static QUERY_STORE_WITH_GROUP_ONE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QUERY_STORE_WITH_GROUP_ONE();
                return _instance;
            }
        }
        private QUERY_STORE_WITH_GROUP_ONE()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,string groupName-->the storage groupName
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length == 0)
                throw new FDFSException("GroupName is null");
            QUERY_STORE_WITH_GROUP_ONE result = new QUERY_STORE_WITH_GROUP_ONE();

            byte[] groupName = FDFSUtil.StringToByte((string)paramList[0]);
            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new FDFSException("GroupName is too long");
            }
            byte[] body = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(groupName, 0, body, 0, groupName.Length);
            result.Body = body;
            result.Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN,
                Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);
            return result;
        }

        public class Response
        {
            public string GroupName;
            public string IPStr;
            public int Port;
            public byte StorePathIndex;
            public Response(byte[] responseByte)
            {
                byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseByte, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
                GroupName = FDFSUtil.ByteToString(groupNameBuffer).TrimEnd('\0');
                byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
                IPStr = new string(FDFSConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');
                byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
                    portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
                Port = (int)FDFSUtil.BufferToLong(portBuffer, 0);

                StorePathIndex = responseByte[responseByte.Length - 1];
            }
        }
    }
}
