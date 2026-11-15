using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 数据库操作类型枚举，用于固定连接字符串时区分读操作或写操作
    /// </summary>
    public enum DBOperationEnum { Default, Read, Write }
    /// <summary>
    /// 标记Controller或Action使用固定的连接字符串，不受其他设定控制
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class FixConnectionAttribute : Attribute
    {
        /// <summary>
        /// 连接字符串名称
        /// </summary>
        public string CsName { get; set; }

        /// <summary>
        /// 数据库类型，为null时使用appsettings中的设定
        /// </summary>
        public DBTypeEnum? DbType { get; set; }

        /// <summary>
        /// 操作类型，读或写
        /// </summary>
        public DBOperationEnum Operation { get; set; }

        /// <summary>
        /// 新建固定连接字符串标记
        /// </summary>
        /// <param name="Operation">Operation</param>
        /// <param name="CsName">the key of the ConnectionString in appsettings</param>
        public FixConnectionAttribute(DBOperationEnum Operation = DBOperationEnum.Default, string CsName = "")
        {
            this.CsName = CsName;
            this.Operation = Operation;
            this.DbType = null;
        }

        /// <summary>
        /// 操作类型，读或写
        /// </summary>
        /// <param name="DbType">the database type, if t is Default, the value in appsettings will be used</param>
        /// <param name="Operation">Operation</param>
        /// <param name="CsName">the key of the ConnectionString in appsettings</param>
        public FixConnectionAttribute(DBTypeEnum DbType, DBOperationEnum Operation = DBOperationEnum.Default, string CsName = "")
        {
            this.CsName = CsName;
            this.Operation = Operation;
            this.DbType = DbType;
        }

    }
}
