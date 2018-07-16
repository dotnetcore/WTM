namespace WalkingTec.Mvvm.Core.FDFS.Common
{
    /// <summary>
    /// list storage servers of a group
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVER_LIST_STORAGE 92
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: the group name to query
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:(n)
    /*   @ 1 byte: status
       @ FDFS_IPADDR_SIZE bytes: ip address
       @ FDFS_DOMAIN_NAME_MAX_SIZE  bytes : domain name of the web server
       @ IP_ADDRESS_SIZE bytes: source storage server ip address
       @ FDFS_VERSION_SIZE bytes: storage server version
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: join time (join in timestamp)
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: up time (start timestamp)
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total space in MB
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: free space in MB
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: upload priority
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: store path count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: subdir count per path
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: current write path[
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage http port
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total upload count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success upload count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total set metadata count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success set metadata count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total delete count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success delete count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total download count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success download count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total get metadata count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success get metadata count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total create link count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success create link count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: total delete link count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: success delete link count
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: last source update timestamp
       @ FDFS_PROTO_PKG_LEN_SIZE bytes: last sync update timestamp
       @ FDFS_PROTO_PKG_LEN_SIZE bytes:  last synced timestamp
       @ FDFS_PROTO_PKG_LEN_SIZE bytes:  last heart beat timestamp
    */
    /// </summary>
    public class LIST_STORAGE : FDFSRequest
    {
        private LIST_STORAGE()
        {

        }
    }
}
