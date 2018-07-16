using System;

namespace WalkingTec.Mvvm.Core
{
    public enum DBOperationEnum { Default, Read, Write}
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
        /// 操作类型，读或写
        /// </summary>
        public DBOperationEnum Operation { get; set; }
        /// <summary>
        /// 新建固定连接字符串标记
        /// </summary>
        /// <param name="Operation">Operation</param>
        /// <param name="CsName">连接字符串名称</param>
        public FixConnectionAttribute(DBOperationEnum Operation =  DBOperationEnum.Default, string CsName = "")
        {
            this.CsName = CsName;
            this.Operation = Operation;
        }

    }
}
