using System;
using System.Net;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// upload file to storage server
    /// 
    /// Reqeust 
    ///     Cmd: UPLOAD_APPEND_FILE 23
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
    ///     @ filename
    ///     @ file bytes: file content 
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename   
    /// </summary>
    public class UPLOAD_APPEND_FILE : FDFSRequest
    {
        private static UPLOAD_APPEND_FILE _instance = null;
        public static UPLOAD_APPEND_FILE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UPLOAD_APPEND_FILE();
                return _instance;
            }
        }
        private UPLOAD_APPEND_FILE()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,Byte          StorePathIndex
        /// 3,long          FileSize
        /// 4,string        File Ext
        /// 5,byte[FileSize]    File Content
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 5)
                throw new FDFSException("param count is wrong");
            IPEndPoint endPoint = (IPEndPoint)paramList[0];

            byte storePathIndex = (byte)paramList[1];
            int fileSize = (int)paramList[2];
            string ext = (string)paramList[3];
            byte[] contentBuffer = (byte[])paramList[4];

            #region 拷贝后缀扩展名值
            byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = FDFSUtil.StringToByte(ext);
            int ext_name_len = bse.Length;
            if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, ext_name_len);
            #endregion

            UPLOAD_APPEND_FILE result = new UPLOAD_APPEND_FILE();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);
            if (ext.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new FDFSException("file ext is too long");

            long length = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + contentBuffer.Length;
            byte[] bodyBuffer = new byte[length];
            bodyBuffer[0] = storePathIndex;

            byte[] fileSizeBuffer = FDFSUtil.LongToBuffer(fileSize);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, 1, fileSizeBuffer.Length);

            Array.Copy(extBuffer, 0, bodyBuffer, 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE, extBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer, 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE, 0);
            return result;
        }

        public class Response
        {
            public string GroupName;
            public string FileName;
            public Response(byte[] responseBody)
            {
                byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
                GroupName = FDFSUtil.ByteToString(groupNameBuffer).TrimEnd('\0');

                byte[] fileNameBuffer = new byte[responseBody.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
                FileName = FDFSUtil.ByteToString(fileNameBuffer).TrimEnd('\0');
            }
        }

    }
}
