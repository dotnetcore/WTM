using System;
using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Agent编排流程定义，保存流程图（节点+连线）的JSON表示，
    /// 由前端可视化设计器编辑，由AgentExecutor引擎执行
    /// </summary>
    public class AgentFlow : PersistPoco
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Name { get; set; }

        /// <summary>
        /// 流程说明
        /// </summary>
        [Display(Name = "说明")]
        [StringLength(500, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Description { get; set; }

        /// <summary>
        /// 流程图JSON（节点、连线、节点配置），结构见 AgentGraph
        /// </summary>
        [Display(Name = "流程图")]
        public string GraphJson { get; set; }

        /// <summary>
        /// 是否发布为AI助手技能：通用对话中模型可按用户意图自动调用本流程
        /// </summary>
        [Display(Name = "AI技能")]
        public bool IsSkill { get; set; }

        /// <summary>
        /// 是否定时运行：由框架的Quartz定时作业按Cron到点自动执行
        /// </summary>
        [Display(Name = "定时运行")]
        public bool IsScheduled { get; set; }

        /// <summary>
        /// 定时运行的Quartz Cron表达式（7段式，如每天8点：0 0 8 * * ?）；为空时默认每小时运行
        /// </summary>
        [Display(Name = "定时表达式")]
        [StringLength(100)]
        public string ScheduleCron { get; set; }

        /// <summary>
        /// 定时运行时的默认输入变量JSON（{变量名:值}）
        /// </summary>
        [Display(Name = "定时输入")]
        public string ScheduleInputsJson { get; set; }

        /// <summary>
        /// 上次定时运行时间（调度作业据此判断Cron是否到点，运行前更新防止重叠执行）
        /// </summary>
        [Display(Name = "上次定时运行")]
        public DateTime? LastScheduleTime { get; set; }
    }

    /// <summary>
    /// Agent流程的一次执行记录，保存输入、输出、每个节点的执行日志，
    /// 供前端查看运行历史与调试
    /// </summary>
    public class AgentRun : PersistPoco
    {
        /// <summary>
        /// 所属流程ID
        /// </summary>
        [Display(Name = "流程ID")]
        public Guid? FlowId { get; set; }

        /// <summary>
        /// 执行时的流程名称（流程可能被改名或删除，冗余保存名称便于历史查看）
        /// </summary>
        [Display(Name = "流程名称")]
        [StringLength(100)]
        public string FlowName { get; set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        [Display(Name = "状态")]
        public AgentRunStatusEnum Status { get; set; }

        /// <summary>
        /// 输入变量JSON
        /// </summary>
        [Display(Name = "输入")]
        public string InputJson { get; set; }

        /// <summary>
        /// 最终输出内容
        /// </summary>
        [Display(Name = "输出")]
        public string OutputJson { get; set; }

        /// <summary>
        /// 各节点执行日志JSON（AgentNodeLog列表）
        /// </summary>
        [Display(Name = "节点日志")]
        public string LogsJson { get; set; }

        /// <summary>
        /// 失败时的错误信息
        /// </summary>
        [Display(Name = "错误")]
        public string Error { get; set; }

        /// <summary>
        /// 总耗时（毫秒）
        /// </summary>
        [Display(Name = "耗时")]
        public long DurationMs { get; set; }
    }

    /// <summary>
    /// Agent对话的一条消息记录，按用户保存对话历史；
    /// 因框架登出/令牌过期会清空浏览器本地存储，历史以服务端为准
    /// </summary>
    public class AgentChatMessage : TopBasePoco
    {
        /// <summary>
        /// 所属用户账号（ITCode）
        /// </summary>
        [Display(Name = "用户")]
        [StringLength(50)]
        public string UserCode { get; set; }

        /// <summary>
        /// 消息角色：user / assistant
        /// </summary>
        [Display(Name = "角色")]
        [StringLength(20)]
        public string Role { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "时间")]
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// Agent流程执行状态
    /// </summary>
    public enum AgentRunStatusEnum
    {
        /// <summary>
        /// 执行中
        /// </summary>
        Running = 0,
        /// <summary>
        /// 执行成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 执行失败
        /// </summary>
        Failed = 2
    }
}
