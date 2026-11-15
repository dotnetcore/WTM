using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{

    /// <summary>
    /// 操作日志的精简传输类，保存单条操作日志的关键信息，用于日志的序列化传输和 API 输出
    /// </summary>
    public class SimpleLog
    {

        /// <summary>
        /// 将当前精简对象的各字段值复制为一个新的 ActionLog 实体对象
        /// </summary>
        /// <returns>复制了本对象各字段值的 ActionLog 实体对象</returns>
        public ActionLog GetActionLog()
        {
            return new ActionLog
            {
                ActionName = this.ActionName,
                ActionTime = this.ActionTime,
                ActionUrl = this.ActionUrl,
                CreateBy = this.CreateBy,
                CreateTime = this.CreateTime,
                Duration = this.Duration,
                IP = this.IP,
                ITCode = this.ITCode,
                LogType = this.LogType,
                ModuleName = this.ModuleName,
                Remark = this.Remark,
                UpdateBy = this.UpdateBy,
                UpdateTime = this.UpdateTime,
            };
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 操作用户的账号(ITCode)
        /// </summary>
        public string ITCode { get; set; }

        /// <summary>
        /// 操作对应的 Url
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// 操作发生的时间
        /// </summary>
        public DateTime ActionTime { get; set; }

        /// <summary>
        /// 操作耗时(毫秒)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 客户端 IP 地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public ActionLogTypesEnum LogType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateBy
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// UpdateBy
        /// </summary>
        public string UpdateBy { get; set; }
    }
}
