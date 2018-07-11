namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// set metat data from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_SET_METADATA 13
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename length 
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: meta data size 
    ///     @ 1 bytes: operation flag,  
    ///         'O' for overwrite all old metadata 
    ///         'M' for merge, insert when the meta item not exist, otherwise update it
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name 
    ///     @ filename bytes: filename
    ///     @ meta data bytes: each meta data seperated by \x01,
    ///         name and value seperated by \x02
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///         
    /// </summary>
    public class SET_METADATA : FDFSRequest
    {
        private SET_METADATA()
        {

        }
    }
}
