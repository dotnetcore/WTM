using System;
using System.Net;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// query file info from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_QUERY_FILE_INFO 22
    ///     Body:   
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file create timestamp
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file CRC32 signature
    /// </summary>
    public class QUERY_FILE_INFO : FDFSRequest
    {
        private static QUERY_FILE_INFO _instance = null;
        public static QUERY_FILE_INFO Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QUERY_FILE_INFO();
                return _instance;
            }
        }
        private QUERY_FILE_INFO()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,string fileName
        /// 3,string fileBytes
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            IPEndPoint endPoint = (IPEndPoint)paramList[0];

            string groupName = (string)paramList[1];
            string fileName = (string)paramList[2];

            QUERY_FILE_INFO result = new QUERY_FILE_INFO();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);

            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
                throw new FDFSException("groupName is too long");

            long length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileName.Length;
            byte[] bodyBuffer = new byte[length];
            byte[] groupNameBuffer = FDFSUtil.StringToByte(groupName);
            byte[] fileNameBuffer = FDFSUtil.StringToByte(fileName);

            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_QUERY_FILE_INFO, 0);
            return result;
        }
    }

    public class FDFSFileInfo
    {
        public long FileSize;
        public DateTime CreateTime;
        public long Crc32;

        public FDFSFileInfo(byte[] responseByte)
        {
            byte[] fileSizeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] createTimeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            byte[] crcBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

            Array.Copy(responseByte, 0, fileSizeBuffer, 0, fileSizeBuffer.Length);
            Array.Copy(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE, createTimeBuffer, 0, createTimeBuffer.Length);
            Array.Copy(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, crcBuffer, 0, crcBuffer.Length);

            FileSize = FDFSUtil.BufferToLong(responseByte, 0);
            CreateTime = new System.DateTime(1970, 1, 1).AddSeconds(FDFSUtil.BufferToLong(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE));

            Crc32 = FDFSUtil.BufferToLong(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE);
        }
    }
}
