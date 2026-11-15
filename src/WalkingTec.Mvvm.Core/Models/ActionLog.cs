using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 操作日志类型枚举，用于区分普通日志、异常日志、调试日志和定时任务日志
    /// </summary>
    public enum ActionLogTypesEnum
    {
        /// <summary>
        /// 普通操作日志
        /// </summary>
        [Display(Name = "_Admin.Normal")]
        Normal,
        /// <summary>
        /// 异常日志
        /// </summary>
        [Display(Name = "_Admin.Exception")]
        Exception,
        /// <summary>
        /// 调试日志
        /// </summary>
        [Display(Name = "_Admin.Debug")]
        Debug,
        /// <summary>
        /// 定时任务日志
        /// </summary>
        [Display(Name = "_Admin.Job")]
        Job
    };

    /// <summary>
    /// ActionLog
    /// </summary>
    [Table("ActionLogs")]
    public class ActionLog : BasePoco,ITenant, ICloneable
    {
        /// <summary>
        /// 模块名称，即请求对应的控制器名称
        /// </summary>
        [Display(Name = "_Admin.Module")]
        [StringLength(255, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 动作名称，即请求对应的方法名称
        /// </summary>
        [Display(Name = "_Admin.Action")]
        [StringLength(255, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ActionName { get; set; }

        /// <summary>
        /// 操作用户的登录账号（ITCode）
        /// </summary>
        [Display(Name = "_Admin.Account")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ITCode { get; set; }

        /// <summary>
        /// 请求的Url地址
        /// </summary>
        [Display(Name = "Url")]
        [StringLength(250, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 操作发生的时间
        /// </summary>
        [Display(Name = "_Admin.ActionTime")]
        public DateTime ActionTime { get; set; }

        /// <summary>
        /// 操作耗时（秒）
        /// </summary>
        [Display(Name = "_Admin.Duration")]
        public double Duration { get; set; }

        /// <summary>
        /// 备注信息，异常日志中记录异常内容
        /// </summary>
        [Display(Name = "_Admin.Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        /// <summary>
        /// 日志类型，区分普通、异常、调试、定时任务日志
        /// </summary>
        [Display(Name = "_Admin.LogType")]
        public ActionLogTypesEnum LogType { get; set; }
        /// <summary>
        /// 租户编码，标识日志所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

        /// <summary>
        /// 创建当前对象的浅表副本
        /// </summary>
        /// <returns>当前 ActionLog 对象的浅拷贝副本</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 生成用于文件日志或控制台输出的多行格式化字符串，包含本次操作的关键信息
        /// </summary>
        /// <returns>包含操作时间、账号、IP、模块、方法、Url、耗时、备注信息的格式化字符串</returns>
        public string GetLogString()
        {
            return $@"
|-{Core.CoreProgram._localizer?["_Admin.ActionTime"]}：{this.ActionTime}
|-{Core.CoreProgram._localizer?["_Admin.Account"]}：{this.ITCode??""}
|-IP：{this.IP??""}
|-{Core.CoreProgram._localizer?["_Admin.Module"]}：{this.ModuleName??""}
|-{Core.CoreProgram._localizer?["_Admin.MethodName"]}：{this.ActionName ?? ""}
|-Url：{this.ActionUrl ?? ""}
|-{Core.CoreProgram._localizer?["_Admin.Duration"]}：{this.Duration.ToString("F2")+" s"}
|-{Core.CoreProgram._localizer?["_Admin.Remark"]}：{this.Remark}
";
        }
    }


}
