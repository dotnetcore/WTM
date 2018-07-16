using System;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL 107
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
    public class QUERY_STORE_WITH_GROUP_ALL : FDFSRequest
    {
        private QUERY_STORE_WITH_GROUP_ALL()
        {

        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public class Response
        {

        }
    }
}
