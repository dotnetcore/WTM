namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// get metat data from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_GET_METADATA 15
    ///     Body:   
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ meta data buff, each meta data seperated by \x01, name and value seperated by \x02
    /// </summary>
    public class GET_METADATA : FDFSRequest
    {
        private GET_METADATA()
        {

        }
    }
}
