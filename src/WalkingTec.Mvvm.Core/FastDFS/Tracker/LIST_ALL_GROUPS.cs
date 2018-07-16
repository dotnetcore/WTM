namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// list all groups
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUPS 91
    ///     Body:
    ///     
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN+1 bytes: group name
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: free disk storage in MB
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server count
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server http port
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: active server count
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: current write server index
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: store path count on storage server
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: subdir count per path on storage server
    /// </summary>
    public class LIST_ALL_GROUPS : FDFSRequest
    {
        private LIST_ALL_GROUPS()
        {

        }
    }
}
