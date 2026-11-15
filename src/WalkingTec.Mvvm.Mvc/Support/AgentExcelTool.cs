using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Agent;

namespace WalkingTec.Mvvm.Mvc.Support
{
    /// <summary>
    /// Agent生成Excel工具（Mvc层）：负责解析模型参数与两种数据来源——
    /// ①rows=模型内联提供的数据；②query=按查询工具名在服务端取数（数据不经过大模型），
    /// 数据就绪后调用Core的AgentExcelService生成xlsx并保存为框架附件返回下载链接
    /// </summary>
    public static class AgentExcelTool
    {
        /// <summary>
        /// 构造generate_excel工具
        /// </summary>
        /// <param name="wtm">WTM上下文</param>
        /// <param name="baseUrl">本服务基址</param>
        /// <param name="authorization">当前用户令牌（服务端查询继承权限）</param>
        /// <returns>Excel生成工具</returns>
        public static AgentTool Build(WTMContext wtm, string baseUrl, string authorization)
        {
            return new AgentTool
            {
                Definition = new LlmToolDefinition
                {
                    Name = "generate_excel",
                    Description = "生成Excel文件(.xlsx)并返回下载链接。数据两种来源二选一：①rows直接给表头和行数据（适合小数据）；②query={tool:查询工具名(如query_Student), args:该工具的查询参数, limit:行数}由服务端取数，数据不占用对话，适合大量数据。style可按用户要求定制外观：表头背景色headerBackColor、表头字色headerFontColor（hex如#4472C4）、隔行变色zebra、边框border、冻结表头freezeHeader、列宽自适应autoWidth。生成后把url告诉用户下载。",
                    ParametersJsonSchema = BuildSchema()
                },
                Executor = async (args, ct) =>
                {
                    try
                    {
                        return await GenerateAsync(wtm, baseUrl, authorization, args);
                    }
                    catch (Exception ex)
                    {
                        return new { error = $"生成Excel失败：{ex.Message}" };
                    }
                }
            };
        }

        /// <summary>
        /// 用对象序列化方式生成参数Schema，避免手写JSON的括号配平错误
        /// </summary>
        private static string BuildSchema()
        {
            var schema = new
            {
                type = "object",
                properties = new Dictionary<string, object>
                {
                    ["fileName"] = new { type = "string", description = "文件名，默认'导出数据.xlsx'" },
                    ["title"] = new { type = "string", description = "表格大标题（第一行合并居中显示），可选" },
                    ["headers"] = new { type = "array", items = new { type = "string" }, description = "列名（rows模式必填；query模式可选）" },
                    ["rows"] = new { type = "array", items = new { type = "array" }, description = "行数据，每行为与headers对应的值数组" },
                    ["query"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["tool"] = new { type = "string", description = "查询工具名，如query_Student" },
                            ["args"] = new { type = "object", description = "传给该查询工具的查询参数" },
                            ["limit"] = new { type = "integer", description = "最大行数，默认100，最大500" },
                        },
                        description = "服务端数据源：直接调用系统查询工具取数",
                    },
                    ["columns"] = new { type = "array", items = new { type = "string" }, description = "保留的列（字段名），缺省全部列" },
                    ["style"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["headerBackColor"] = new { type = "string", description = "表头背景色hex，默认#4472C4" },
                            ["headerFontColor"] = new { type = "string", description = "表头字色hex，默认#FFFFFF" },
                            ["zebra"] = new { type = "boolean", description = "隔行变色，默认true" },
                            ["border"] = new { type = "boolean", description = "全表边框，默认true" },
                            ["freezeHeader"] = new { type = "boolean", description = "冻结表头，默认true" },
                            ["autoWidth"] = new { type = "boolean", description = "列宽自适应，默认true" },
                        },
                    },
                },
                required = Array.Empty<string>(),
            };
            return JsonSerializer.Serialize(schema, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        /// <summary>
        /// 解析参数取数并生成：rows与query二选一
        /// </summary>
        private static async Task<object> GenerateAsync(WTMContext wtm, string baseUrl, string authorization, JsonElement args)
        {
            string fileName = GetString(args, "fileName");
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "导出数据.xlsx";
            if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                fileName += ".xlsx";

            var headers = new List<string>();
            var rows = new List<List<string>>();
            bool fromQuery = false;

            if (args.TryGetProperty("query", out var q) && q.ValueKind == JsonValueKind.Object
                && q.TryGetProperty("tool", out var toolNameEl) && toolNameEl.ValueKind == JsonValueKind.String)
            {
                fromQuery = true;
                var allowWrite = wtm?.ConfigInfo?.Agent?.AllowAllTools == true;
                var tool = AgentApiToolDiscovery.FindTool(toolNameEl.GetString(), allowWrite);
                if (tool == null)
                    return new { error = $"未找到查询工具 {toolNameEl.GetString()}，请使用工具列表中query_开头的工具" };

                int limit = 100;
                if (q.TryGetProperty("limit", out var l) && l.ValueKind == JsonValueKind.Number && l.TryGetInt32(out var li))
                    limit = Math.Clamp(li, 1, AgentExcelService.MaxRows);

                //查询参数 = 模型给的args + 分页（Limit受控）
                var merged = new Dictionary<string, object>();
                if (q.TryGetProperty("args", out var qa) && qa.ValueKind == JsonValueKind.Object)
                {
                    foreach (var p in qa.EnumerateObject())
                        merged[p.Name] = p.Value.Clone();
                }
                merged["Limit"] = limit;
                merged["Page"] = 1;

                var result = await AgentApiToolDiscovery.InvokeAsync(tool, JsonSerializer.SerializeToElement(merged), baseUrl, authorization);
                if (result is not JsonElement je || je.ValueKind != JsonValueKind.Object
                    || !je.TryGetProperty("Data", out var data) || data.ValueKind != JsonValueKind.Array)
                {
                    return new { error = "查询工具未返回数据（可能无权限或参数有误）" };
                }

                List<string> keepCols = null;
                if (args.TryGetProperty("columns", out var cols) && cols.ValueKind == JsonValueKind.Array)
                    keepCols = cols.EnumerateArray().Select(c => c.GetString()).Where(c => c != null).ToList();

                foreach (var item in data.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;
                    var allKeys = item.EnumerateObject().Select(p => p.Name).ToList();
                    if (headers.Count == 0)
                    {
                        headers = keepCols != null && keepCols.Count > 0
                            ? keepCols.Where(k => allKeys.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList()
                            : allKeys;
                        if (headers.Count == 0)
                            return new { error = "数据没有可导出的列" };
                    }
                    var row = headers.Select(h =>
                    {
                        foreach (var p in item.EnumerateObject())
                            if (string.Equals(p.Name, h, StringComparison.OrdinalIgnoreCase))
                                return p.Value.ValueKind == JsonValueKind.String ? p.Value.GetString() : p.Value.ToString();
                        return "";
                    }).ToList();
                    rows.Add(row);
                }
            }
            else if (args.TryGetProperty("rows", out var r) && r.ValueKind == JsonValueKind.Array)
            {
                foreach (var row in r.EnumerateArray())
                {
                    if (row.ValueKind != JsonValueKind.Array)
                        continue;
                    rows.Add(row.EnumerateArray().Select(c => c.ValueKind == JsonValueKind.String ? c.GetString() : c.ToString()).ToList());
                }
                if (args.TryGetProperty("headers", out var hs) && hs.ValueKind == JsonValueKind.Array)
                    headers = hs.EnumerateArray().Select(h => h.GetString()).ToList();
                //未提供headers时把首行当表头
                if (headers.Count == 0 && rows.Count > 0)
                {
                    headers = rows[0];
                    rows.RemoveAt(0);
                }
            }
            else
            {
                return new { error = "缺少数据：请提供rows（内联数据）或query（服务端查询）" };
            }

            var style = args.TryGetProperty("style", out var st) && st.ValueKind == JsonValueKind.Object ? st : (JsonElement?)null;
            return AgentExcelService.GenerateAndSave(wtm, baseUrl, fileName, GetString(args, "title"), headers, rows, style);
        }

        private static string GetString(JsonElement e, string name)
            => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    }
}
