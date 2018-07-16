using System;
using System.Net;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// append file to storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_APPEND_FILE 24
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file name length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: append file body length
    ///     @ file name
    ///     @ append body 
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    /// </summary>
    public class APPEND_FILE : FDFSRequest
    {
        private static APPEND_FILE _instance = null;
        public static APPEND_FILE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new APPEND_FILE();
                return _instance;
            }
        }
        private APPEND_FILE()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,string        FileName
        /// 3,byte[]        File bytes
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            IPEndPoint endPoint = (IPEndPoint)paramList[0];

            string fileName = (string)paramList[1];
            byte[] contentBuffer = (byte[])paramList[2];

            APPEND_FILE result = new APPEND_FILE();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);

            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + fileName.Length + contentBuffer.Length;
            byte[] bodyBuffer = new byte[length];

            byte[] fileNameLenBuffer = FDFSUtil.LongToBuffer(fileName.Length);
            Array.Copy(fileNameLenBuffer, 0, bodyBuffer, 0, fileNameLenBuffer.Length);

            byte[] fileSizeBuffer = FDFSUtil.LongToBuffer(contentBuffer.Length);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE, fileSizeBuffer.Length);

            byte[] fileNameBuffer = FDFSUtil.StringToByte(fileName);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, fileNameBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + fileNameBuffer.Length, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_APPEND_FILE, 0);
            return result;
        }

        public class Response
        {
            public string GroupName;
            public string FileName;
            public Response(byte[] responseBody)
            {
                /*
                byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                byte[] fileNameBuffer = new byte[responseBody.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
                FileName = Util.ByteToString(fileNameBuffer).TrimEnd('\0');
                 * */
            }
        }

    }
}
