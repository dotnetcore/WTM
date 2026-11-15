using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// Agent流程定时执行作业：继承WtmJob由框架Quartz宿主自动发现并调度，
    /// 每分钟扫描一次启用定时的流程，按各自的Cron表达式判断到点并执行；
    /// 执行前先更新LastScheduleTime防止重叠运行，执行结果写入AgentRuns
    /// </summary>
    [QuartzJob("AgentFlowScheduleJob")]
    [QuartzGroup("Agent")]
    [QuartzRepeat(60, 0, true, 30)]
    public class AgentFlowScheduleJob : WtmJob
    {
        /// <summary>
        /// 作业执行入口：扫描到点的定时流程并依次执行
        /// </summary>
        /// <param name="context">Quartz执行上下文</param>
        public override async Task Execute(IJobExecutionContext context)
        {
            //服务提供者由调度器通过JobDataMap传入（QuartzHostService构建作业时写入），作业需自行取出
            if (Sp == null)
                Sp = context.MergedJobDataMap?["Sp"] as IServiceProvider;
            if (Sp == null)
                return;
            try
            {
                var dc = Wtm.CreateDC();
                AgentTableHelper.EnsureTables(dc);
                var now = DateTime.Now;
                var flows = dc.Set<AgentFlow>().AsNoTracking()
                    .Where(x => x.IsValid == true && x.IsScheduled == true)
                    .ToList();
                foreach (var flow in flows)
                {
                    if (IsDue(flow, now) == false)
                        continue;
                    try
                    {
                        await RunFlowAsync(flow.ID, now);
                    }
                    catch (Exception ex)
                    {
                        Wtm.DoLog($"定时流程[{flow.Name}]执行异常：{ex.Message}", ActionLogTypesEnum.Exception, "Agent定时调度");
                    }
                }
            }
            catch
            {
                //调度作业自身异常静默，等待下一个周期
            }
        }

        /// <summary>
        /// 判断流程是否到点：从未运行过视为到点（启动后首个周期执行一次）；
        /// 之后优先按Cron计算上次运行后的下次触发时间，无Cron时默认每小时
        /// </summary>
        private static bool IsDue(AgentFlow flow, DateTime now)
        {
            var last = flow.LastScheduleTime;
            if (last == null)
                return true;
            if (last.Value > now)
                return false;
            if (string.IsNullOrWhiteSpace(flow.ScheduleCron))
                return last.Value.AddHours(1) <= now;
            try
            {
                var cron = new CronExpression(flow.ScheduleCron) { TimeZone = TimeZoneInfo.Local };
                var next = cron.GetNextValidTimeAfter(new DateTimeOffset(last.Value));
                return next != null && next.Value.LocalDateTime <= now;
            }
            catch
            {
                //Cron表达式非法时不执行，避免异常循环
                return false;
            }
        }

        /// <summary>
        /// 执行单个流程：先把LastScheduleTime置为当前时间（防止下个扫描周期重复执行），
        /// 再解析默认输入、运行引擎并把结果写入AgentRuns
        /// </summary>
        private async Task RunFlowAsync(Guid flowId, DateTime now)
        {
            var dc = Wtm.CreateDC();
            var flow = dc.Set<AgentFlow>().FirstOrDefault(x => x.ID == flowId && x.IsValid == true);
            if (flow == null)
                return;

            //占位更新上次运行时间，未到执行完成即提交，避免长流程被下一周期重复触发
            flow.LastScheduleTime = now;
            dc.SaveChanges();

            var inputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(flow.ScheduleInputsJson))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(flow.ScheduleInputsJson);
                    if (parsed != null)
                        inputs = new Dictionary<string, string>(parsed, StringComparer.OrdinalIgnoreCase);
                }
                catch { /* 输入JSON非法按空输入执行 */ }
            }

            var graph = AgentGraph.Parse(flow.GraphJson);
            var result = await AgentExecutor.RunAsync(graph, inputs, AgentRuntimeServices.FromWtm(Wtm));

            var run = new AgentRun
            {
                ID = Guid.NewGuid(),
                FlowId = flow.ID,
                FlowName = flow.Name,
                Status = result.Status,
                InputJson = JsonSerializer.Serialize(inputs),
                OutputJson = result.Output,
                LogsJson = JsonSerializer.Serialize(result.NodeLogs),
                Error = result.Error,
                DurationMs = result.TotalMs,
                CreateTime = DateTime.Now,
                CreateBy = "Agent定时调度",
                IsValid = true
            };
            dc.Set<AgentRun>().Add(run);
            dc.SaveChanges();
        }
    }
}
