using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using NPOI.HPSF;
using NPOI.SS.Formula;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    public class WtmMiddleware
    {
        private readonly RequestDelegate _next;

        public WtmMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WTMContext wtm)
        {
            var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (max.IsReadOnly == false)
            {
                max.MaxRequestBodySize = wtm.ConfigInfo.FileUploadOptions.UploadLimit;
            }
            if (context.Request.Path == "/v1/activities" && context.Request.QueryString.Value != "?inneruse=1")
            {
                var txt = wtm.CallAPI("", $"{wtm.HostAddress}/v1/activities?inneruse=1").Result.Data;
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //var loc = "WalkingTec.Mvvm.Mvc.Workflow.json";
                //var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
                //string content = textStreamReader.ReadToEnd();
                //textStreamReader.Close();
                //foreach (var c in context.Request.Cookies)
                //{
                //    context.Response.Cookies.Append(c.Key,c.Value);

                //}
                await context.Response.WriteAsync(this.TranslateWorkflow(txt));
                return;
            }
            if (context.Request.Path == "/")
            {
                context.Response.Cookies.Append("pagemode", wtm.ConfigInfo.PageMode.ToString());
                context.Response.Cookies.Append("tabmode", wtm.ConfigInfo.TabMode.ToString());
            }
            if (context.Request.ContentLength > 0 && context.Request.HasFormContentType == false)
            {
                try
                {
                    context.Request.EnableBuffering();
                    context.Request.Body.Position = 0;
                    StreamReader tr = new StreamReader(context.Request.Body);
                    string body = tr.ReadToEndAsync().Result;
                    context.Request.Body.Position = 0;
                    if (context.Items.ContainsKey("DONOTUSE_REQUESTBODY") == false)
                    {
                        context.Items.Add("DONOTUSE_REQUESTBODY", body);
                    }
                    else
                    {
                        context.Items["DONOTUSE_REQUESTBODY"] = body;
                    }
                }
                catch {
                    context.Request.Body.Position = 0;
                }
            }
            await _next(context);
            if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync(string.Empty);
            }
        }

        private string TranslateWorkflow(string txt)
        {
            if(txt == null)
            {
                return "";
            }
            string rv = txt
                .Replace("\"displayName\":\"Read Line\"", "\"displayName\":\"读取\"")
                .Replace("\"description\":\"Read text from standard in.\"", "\"description\":\"从标准输入中读取\"")
                .Replace("\"category\":\"Console\"", "\"category\":\"命令行\"")
                .Replace("\"displayName\":\"Write Line\"", "\"displayName\":\"写入\"")
                .Replace("\"description\":\"Write text to standard out.\"", "\"description\":\"向标准输出写入\"")
                .Replace("\"label\":\"Text\"", "\"label\":\"文本\"")
                .Replace("\"hint\":\"The text to write.\"", "\"hint\":\"要写入的文本\"")
                .Replace("\"label\":\"Script\"", "\"label\":\"脚本\"")
                .Replace("\"hint\":\"The JavaScript to run.\"", "\"hint\":\"要运行的Javascript脚本\"")
                .Replace("\"label\":\"Possible Outcomes\"", "\"label\":\"可能的输出\"")
                .Replace("\"hint\":\"The possible outcomes that can be set by the script.\"", "\"hint\":\"脚本可能设置的输出\"")
                .Replace("\"displayName\":\"HTTP Endpoint\"", "\"displayName\":\"HTTP 端点\"")
                .Replace("\"description\":\"Handle an incoming HTTP request.\"", "\"description\":\"处理HTTP请求\"")
                .Replace("\"label\":\"Path\"", "\"label\":\"路径\"")
                .Replace("\"hint\":\"The relative path that triggers this activity.\"", "\"hint\":\"触发动作的相对路径\"")
                .Replace("\"label\":\"Methods\"", "\"label\":\"请求方式\"")
                .Replace("\"label\":\"Read Content\"", "\"label\":\"读取内容\"")
                .Replace("\"label\":\"Target Type\"", "\"label\":\"目标类型\"")
                .Replace("\"displayName\":\"HTTP Response\"", "\"displayName\":\"HTTP 输出\"")
                .Replace("\"description\":\"Write an HTTP response.\"", "\"description\":\"输出HTTP内容\"")
                .Replace("\"label\":\"Status Code\"", "\"label\":\"状态码\"")
                .Replace("\"label\":\"Content\"", "\"label\":\"内容\"")
                .Replace("\"displayName\":\"Send HTTP Request\"", "\"displayName\":\"发送Http请求\"")
                .Replace("\"description\":\"Send an HTTP request.\"", "\"description\":\"送Http请求\"")
                .Replace("\"displayName\":\"Redirect\"", "\"displayName\":\"重定向\"")
                .Replace("\"description\":\"Write an HTTP redirect response.\"", "\"description\":\"重定向Http请求\"")
                .Replace("\"displayName\":\"Send Email\"", "\"displayName\":\"发送Email\"")
                .Replace("\"description\":\"Send an email message.\"", "\"description\":\"发送电子邮件\"")
                .Replace("\"category\":\"Timers\"", "\"category\":\"定时\"")
                .Replace("\"displayName\":\"Timer\"", "\"displayName\":\"定时器\"")
                .Replace("\"description\":\"Triggers at a specified interval.\"", "\"description\":\"指定时间间隔触发\"")
                .Replace("\"displayName\":\"Start at\"", "\"displayName\":\"开始\"")
                .Replace("\"description\":\"Triggers at a specified moment in time.\"", "\"description\":\"指定时间开始\"")
                .Replace("\"displayName\":\"Clear Timer\"", "\"displayName\":\"清除定时器\"")
                .Replace("\"description\":\"Cancel a timer (Cron, StartAt, Timer) so that it is not executed.\"", "\"description\":\"清除一个定时器\"")
                .Replace("\"category\":\"Workflows\"", "\"category\":\"工作流\"")
                .Replace("\"displayName\":\"Run Workflow\"", "\"displayName\":\"运行子流程\"")
                .Replace("\"description\":\"Runs a child workflow.\"", "\"description\":\"运行一个子流程\"")
                .Replace("\"category\":\"Miscellaneous\"", "\"category\":\"其他\"")
                .Replace("\"displayName\":\"Workflow\"", "\"displayName\":\"流程\"")
                .Replace("\"category\":\"State Machine\"", "\"category\":\"状态机\"")
                .Replace("\"displayName\":\"State\"", "\"displayName\":\"状态\"")
                .Replace("\"description\":\"Puts the workflow into the specified state.\"", "\"description\":\"给流程设置状态\"")
                .Replace("\"displayName\":\"Interrupt Trigger\"", "\"displayName\":\"中断触发器\"")
                .Replace("\"description\":\"Resumes suspended workflows that are blocked on a specific trigger.\"", "\"description\":\"继续一个被某个触发器中断的流程\"")
                .Replace("\"displayName\":\"Send Signal\"", "\"displayName\":\"发送信号\"")
                .Replace("\"description\":\"Sends the specified signal.\"", "\"description\":\"发送一个信号\"")
                .Replace("\"displayName\":\"Signal Received\"", "\"displayName\":\"接收信号\"")
                .Replace("\"description\":\"Suspend workflow execution until the specified signal is received.\"", "\"description\":\"暂停工作流直到接收到某个信号\"")
                .Replace("\"displayName\":\"Fault\"", "\"displayName\":\"失败\"")
                .Replace("\"description\":\"Put the workflow in a faulted state.\"", "\"description\":\"将工作流标记为失败\"")
                .Replace("\"displayName\":\"Inline\"", "\"displayName\":\"内联\"")
                .Replace("\"category\":\"Primitives\"", "\"category\":\"底层\"")
                .Replace("\"displayName\":\"Set Context ID\"", "\"displayName\":\"设置上下文ID\"")
                .Replace("\"description\":\"Set context ID on the workflow.\"", "\"description\":\"设置工作流的上下文ID\"")
                .Replace("\"displayName\":\"Set Name\"", "\"displayName\":\"设置名称\"")
                .Replace("\"description\":\"Set the name of the workflow instance.\"", "\"description\":\"设置工作流实例的名称\"")
                .Replace("\"displayName\":\"Set Transient Variable\"", "\"displayName\":\"设置临时变量\"")
                .Replace("\"description\":\"Set a transient variable on the current workflow execution context.\"", "\"description\":\"设置一个当前运行流程内的临时变量\"")
                .Replace("\"displayName\":\"Set Variable\"", "\"displayName\":\"设置变量\"")
                .Replace("\"description\":\"Set variable on the workflow.\"", "\"description\":\"设置流程变量\"")
                .Replace("\"displayName\":\"Correlate\"", "\"displayName\":\"关联ID\"")
                .Replace("\"description\":\"Set the CorrelationId of the workflow to a given value.\"", "\"description\":\"设置流程的关联ID\"")
                .Replace("\"description\":\"Triggers periodically based on a specified CRON expression.\"", "\"description\":\"使用CRON表达式触发\"")
                .Replace("\"category\":\"Control Flow\"", "\"category\":\"控制流\"")
                .Replace("\"description\":\"Break out of a While, For or ForEach loop.\"", "\"description\":\"中断While, For 或者 ForEach 循环\"")
                .Replace("\"displayName\":\"Finish\"", "\"displayName\":\"结束\"")
                .Replace("\"description\":\"Removes any blocking activities from the current container (workflow or composite activity).\"", "\"description\":\"取消所有暂停的动作，标记流程结束\"")
                .Replace("\"description\":\"Iterate between two numbers.\"", "\"description\":\"在两个数字之间循环\"")
                .Replace("\"description\":\"Iterate over a collection.\"", "\"description\":\"循环一个集合\"")
                .Replace("\"displayName\":\"Fork\"", "\"displayName\":\"分叉\"")
                .Replace("\"description\":\"Fork workflow execution into multiple branches.\"", "\"description\":\"将一个流程分叉成多个\"")
                .Replace("\"description\":\"Evaluate a Boolean expression and continue execution depending on the result.\"", "\"description\":\"根据条件判断流程走向\"")
                .Replace("\"displayName\":\"Join\"", "\"displayName\":\"合并\"")
                .Replace("\"description\":\"Merge workflow execution back into a single branch.\"", "\"description\":\"合并多个分支为一个\"")
                .Replace("\"label\":\"Mode\"", "\"label\":\"模式\"")
                .Replace("\"hint\":\"WaitAll:wait for all incoming activities to have executed. WaitAny:continue execution as soon as any of the incoming activity has executed.\"", "\"hint\":\"等待全部:等待所有动作都完成. 等候任意:任意动作完成就可以继续\"")
                .Replace("\"text\":\"WaitAll\"", "\"text\":\"等候全部\"")
                .Replace("\"text\":\"WaitAny\"", "\"text\":\"等候任意\"")
                .Replace("\"displayName\":\"Parallel for Each\"","\"displayName\":\"并行 for Each\"")
                .Replace("\"description\":\"Iterate over a collection in parallel.\"", "\"description\":\"并行循环一个集合\"")
                .Replace("\"displayName\":\"Switch\"", "\"displayName\":\"开关\"")
                .Replace("\"description\":\"Evaluate multiple conditions and continue execution depending on the results.\"", "\"description\":\"评估多个条件的值，并且根据值运行\"")
                .Replace("\"description\":\"Execute while a given condition is true.\"", "\"description\":\"条件为真时循环\"")
                .Replace("\"category\":\"Compensation\"", "\"category\":\"补偿\"")
                .Replace("\"displayName\":\"Compensable\"", "\"displayName\":\"可补偿\"")
                .Replace("\"description\":\"Allow work that executed after this activity to be undone.\"", "\"description\":\"允许之后的动作被回滚\"")
                .Replace("\"displayName\":\"Compensate\"", "\"displayName\":\"补偿\"")
                .Replace("\"description\":\"Invoke a specific compensable activity.\"", "\"description\":\"运行一个可补偿的动作\"")
                .Replace("\"displayName\":\"Confirm", "\"displayName\":\"确认")
                .Replace("\"description\":\"Confirm a specific compensable activity.\"", "\"description\":\"确认一个可补偿的动作\"")
                .Replace("\"category\":\"Scripting\"", "\"category\":\"脚本\"")
                .Replace("\"displayName\":\"Run JavaScript\"", "\"displayName\":\"运行Javascript\"")
                .Replace("\"description\":\"Run JavaScript code.\"", "\"description\":\"运行Javascript脚本\"")
                .Replace("\"category\":\"Email\"", "\"category\":\"电子邮件\"")
                .Replace("\"displayName\":\"Composite Activity\"", "\"displayName\":\"组合动作\"");

            return rv;
        }
    }

    public static class WtmMiddlewareExtensions
    {
        public static IApplicationBuilder UseWtm(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WtmMiddleware>();
        }
    }
}
