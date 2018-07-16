using System;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class Consts
    {
        //Args
        public const byte FDFS_FILE_EXT_NAME_MAX_LEN = 6;   // common/fdfs_global.h
        public const byte FDFS_VERSION_SIZE = 6;

        public const byte FDFS_PROTO_PKG_LEN_SIZE = 8;

        public const byte IP_ADDRESS_SIZE = 16;
        public const byte FDFS_FILE_PREFIX_MAX_LEN = 16;    // tracker/tracker_types.h
        public const byte FDFS_GROUP_NAME_MAX_LEN = 16;

        public const Int16 FDFS_DOMAIN_NAME_MAX_SIZE = 128;

        //Command-Tracker
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_ONE_GROUP = 90;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_ALL_GROUPS = 91;
        public const byte TRACKER_PROTO_CMD_SERVER_LIST_STORAGE = 92;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE = 101;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE = 102;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE = 103;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE = 104;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ALL = 105;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ALL = 106;
        public const byte TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL = 107;

        //Command-Storage
        public const byte STORAGE_PROTO_CMD_UPLOAD_FILE = 11;
        public const byte STORAGE_PROTO_CMD_DELETE_FILE = 12;
        public const byte STORAGE_PROTO_CMD_SET_METADATA = 13;
        public const byte STORAGE_PROTO_CMD_DOWNLOAD_FILE = 14;
        public const byte STORAGE_PROTO_CMD_GET_METADATA = 15;
        public const byte STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE = 21;
        public const byte STORAGE_PROTO_CMD_QUERY_FILE_INFO = 22;
        public const byte STORAGE_PROTO_CMD_UPLOAD_APPENDER_FILE = 23;
        public const byte STORAGE_PROTO_CMD_APPEND_FILE = 24;
        public const byte STORAGE_PROTO_CMD_MODIFY_FILE = 34;  //modify appender file  3.06新增特性
        public const byte STORAGE_PROTO_CMD_TRUNCATE_FILE = 36;  //truncate appender file 3.06新增特性
        //Exit
        public const byte FDFS_PROTO_CMD_QUIT = 82;//未确认


    }
}
