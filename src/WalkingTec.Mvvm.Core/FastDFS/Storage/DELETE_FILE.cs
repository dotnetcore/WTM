using System;
using System.Net;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// delete file from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_DELETE_FILE 12
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///         
    /// </summary>
    public class DELETE_FILE : FDFSRequest
    {
        private static DELETE_FILE _instance = null;
        public static DELETE_FILE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DELETE_FILE();
                return _instance;
            }
        }
        private DELETE_FILE()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,string groupName
        /// 3,string fileName
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            IPEndPoint endPoint = (IPEndPoint)paramList[0];

            string groupName = (string)paramList[1];
            string fileName = (string)paramList[2];

            DELETE_FILE result = new DELETE_FILE();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);

            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
                throw new FDFSException("groupName is too long");

            long length = Consts.FDFS_GROUP_NAME_MAX_LEN +
                fileName.Length;
            byte[] bodyBuffer = new byte[length];
            byte[] groupNameBuffer = FDFSUtil.StringToByte(groupName);
            byte[] fileNameBuffer = FDFSUtil.StringToByte(fileName);

            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_DELETE_FILE, 0);
            return result;
        }

        public class Response
        {
            public Response(byte[] responseBody)
            {
            }
        }

    }
}
