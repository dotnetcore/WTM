/****************************************************************************************************************
*                                                                                                               *
* Copyright (C) hezx                                                                                            *
*                                                                                                               *
*                                                                                                               *
*                                                                                                               *
* Author:G小星星                                                                                                *
*                                                                                                               *
****************************************************************************************************************/

using System;
using System.Net;

namespace WalkingTec.Mvvm.Core.FDFS
{
    /// <summary>
    /// upload slave file to storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE 21
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: master filename length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_FILE_PREFIX_MAX_LEN bytes: filename prefix
    ///     @ FDFS_FILE_EXT_NAME_MAX_LEN bytes: file ext name, do not include dot (.)
    ///     @ master filename bytes: master filename
    ///     @ file size bytes: file content
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// </summary>
    public class UPLOAD_SLAVE_FILE : FDFSRequest
    {
        private static UPLOAD_SLAVE_FILE _instance = null;
        public static UPLOAD_SLAVE_FILE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UPLOAD_SLAVE_FILE();
                return _instance;
            }
        }

        private UPLOAD_SLAVE_FILE()
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
            if (paramList.Length != 6)
                throw new FDFSException("param count is wrong");
            IPEndPoint endPoint = (IPEndPoint)paramList[0];

            int fileSize = (int)paramList[1];
            string masterFilename = paramList[2].ToString();
            string prefix_name = (string)paramList[3];
            string ext = (string)paramList[4];
            byte[] contentBuffer = (byte[])paramList[5];
            byte[] sizeBytes;
            byte[] hexLenBytes;
            byte[] masterFilenameBytes = FDFSUtil.StringToByte(masterFilename);
            int offset;

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

            UPLOAD_SLAVE_FILE result = new UPLOAD_SLAVE_FILE();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);
            if (ext.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new FDFSException("file ext is too long");

            sizeBytes = new byte[2 * Consts.FDFS_PROTO_PKG_LEN_SIZE];
            //long length = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + contentBuffer.Length;
            long length = sizeBytes.Length + +Consts.FDFS_FILE_PREFIX_MAX_LEN + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + masterFilenameBytes.Length + contentBuffer.Length;
            //body_len = sizeBytes.length + ProtoCommon.FDFS_FILE_PREFIX_MAX_LEN + ProtoCommon.FDFS_FILE_EXT_NAME_MAX_LEN
            //             + masterFilenameBytes.length + file_size;
            byte[] bodyBuffer = new byte[length];
            hexLenBytes = FDFSUtil.LongToBuffer(masterFilename.Length);
            offset = hexLenBytes.Length;

            //Array.Copy(hexLenBytes, 0, sizeBytes, 0, hexLenBytes.Length);

            Array.Copy(hexLenBytes, 0, bodyBuffer, 0, hexLenBytes.Length);
            //System.arraycopy(sizeBytes, 0, wholePkg, header.length, sizeBytes.length);

            byte[] fileSizeBuffer = FDFSUtil.LongToBuffer(fileSize);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, offset, fileSizeBuffer.Length);

            offset = sizeBytes.Length;

            byte[] prefix_name_bs = new byte[Consts.FDFS_FILE_PREFIX_MAX_LEN];
            byte[] bs = FDFSUtil.StringToByte(prefix_name);
            int prefix_name_len = bs.Length;
            if (prefix_name_len > Consts.FDFS_FILE_PREFIX_MAX_LEN)
            {
                prefix_name_len = Consts.FDFS_FILE_PREFIX_MAX_LEN;
            }
            if (prefix_name_len > 0)
            {
                Array.Copy(bs, 0, prefix_name_bs, 0, prefix_name_len);
            }
            Array.Copy(prefix_name_bs, 0, bodyBuffer, offset, prefix_name_bs.Length);

            offset += prefix_name_bs.Length;

            Array.Copy(extBuffer, 0, bodyBuffer, offset, extBuffer.Length);

            offset += extBuffer.Length;

            Array.Copy(masterFilenameBytes, 0, bodyBuffer, offset, masterFilenameBytes.Length);
            offset += masterFilenameBytes.Length;

            Array.Copy(contentBuffer, 0, bodyBuffer, offset, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE, 0);
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
