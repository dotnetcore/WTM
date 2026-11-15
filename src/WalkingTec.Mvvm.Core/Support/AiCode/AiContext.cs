using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WalkingTec.Mvvm.Core.Support.AiCode
{
    /// <summary>
    /// AI函数运行时上下文，作为脚本方法的可选参数注入：
    /// 脚本可以声明 AiContext 类型的参数来访问 WTM 上下文并记录运行日志
    /// </summary>
    public class AiContext
    {
        /// <summary>
        /// WTM上下文（可访问 DC、LoginUserInfo、Cache 等）
        /// </summary>
        public WTMContext Wtm { get; set; }

        /// <summary>
        /// 运行日志，脚本内通过 ctx.Log.AppendLine("...") 记录，
        /// 执行结束后由调用方通过日志回调取出
        /// </summary>
        public StringBuilder Log { get; } = new StringBuilder();

        /// <summary>
        /// 记录一行日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public void WriteLog(string message)
        {
            Log.AppendLine(message);
        }
    }

    /// <summary>
    /// AI附加参数定义四元组（沿用swp的设计），序列化保存在模型的 [AiArgs] 字段中
    /// </summary>
    public class AiArgDefinition
    {
        /// <summary>参数英文变量名（与脚本方法参数名一致）</summary>
        public string argName { get; set; }
        /// <summary>参数C#类型（int/double/bool/string/datetime等）</summary>
        public string argType { get; set; }
        /// <summary>参数中文说明（填值控件的显示列）</summary>
        public string argDes { get; set; }
        /// <summary>参数默认值</summary>
        public object argDF { get; set; }
    }

    /// <summary>
    /// AI附加参数的解析与合并：
    /// 解析 [AiArgs] 字段中的定义JSON，与调用时传入的覆盖值合并（缺省用默认值），
    /// 并按脚本方法参数的声明类型转换
    /// </summary>
    public static class AiArgParser
    {
        /// <summary>
        /// 解析参数定义JSON（容错：空串/非法JSON返回空列表）
        /// </summary>
        /// <param name="argsJson">[AiArgs] 字段内容</param>
        /// <returns>参数定义列表</returns>
        public static List<AiArgDefinition> ParseDefinitions(string argsJson)
        {
            if (string.IsNullOrWhiteSpace(argsJson))
                return new List<AiArgDefinition>();
            try
            {
                var text = CleanJson(argsJson);
                var defs = System.Text.Json.JsonSerializer.Deserialize<List<AiArgDefinition>>(text, JsonOpts);
                return defs ?? new List<AiArgDefinition>();
            }
            catch
            {
                return new List<AiArgDefinition>();
            }
        }

        /// <summary>
        /// 合并参数定义与调用时的覆盖值为一个参数名→值的字典：
        /// 覆盖值优先，未覆盖的用定义中的默认值
        /// </summary>
        /// <param name="definitions">参数定义列表</param>
        /// <param name="overrides">调用时的覆盖值（如下游业务用填值控件产出的JSON）</param>
        /// <returns>合并后的参数值字典</returns>
        public static Dictionary<string, object> BuildValues(List<AiArgDefinition> definitions, Dictionary<string, object> overrides)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (definitions != null)
            {
                foreach (var def in definitions)
                {
                    if (string.IsNullOrWhiteSpace(def?.argName))
                        continue;
                    dict[def.argName] = def.argDF;
                }
            }
            if (overrides != null)
            {
                foreach (var kv in overrides)
                {
                    if (kv.Value != null)
                        dict[kv.Key] = kv.Value;
                }
            }
            return dict;
        }

        /// <summary>
        /// 解析填值控件产出的JSON字符串为覆盖值字典（形如 {"minPrecision":2}），容错返回空字典
        /// </summary>
        /// <param name="valuesJson">值的JSON字符串</param>
        /// <returns>覆盖值字典</returns>
        public static Dictionary<string, object> ParseValuesJson(string valuesJson)
        {
            if (string.IsNullOrWhiteSpace(valuesJson))
                return new Dictionary<string, object>();
            try
            {
                var text = CleanJson(valuesJson);
                using var doc = System.Text.Json.JsonDocument.Parse(text);
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.ValueKind == System.Text.Json.JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.Deserialize<object>(JsonOpts);
                }
                return dict;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// 把值转换为目标参数类型（基元/可空/枚举/Guid/DateTime，兼容JsonElement），转换失败抛出带参数名的明确异常
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="argName">参数名（用于错误提示）</param>
        /// <returns>转换后的值</returns>
        public static object ConvertToType(object value, Type targetType, string argName)
        {
            if (value == null)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
            //JSON反序列化出的值是JsonElement，先解包为基元值
            if (value is System.Text.Json.JsonElement je)
                value = UnwrapJsonElement(je, targetType);

            var srcType = value.GetType();
            if (targetType.IsAssignableFrom(srcType))
                return value;

            //可空类型取底层类型
            var underlying = Nullable.GetUnderlyingType(targetType);
            if (underlying != null)
                targetType = underlying;

            try
            {
                if (targetType.IsEnum)
                {
                    var str = value.ToString();
                    if (int.TryParse(str, out var iv) && Enum.IsDefined(targetType, iv))
                        return Enum.ToObject(targetType, iv);
                    return Enum.Parse(targetType, str, true);
                }
                if (targetType == typeof(Guid))
                    return Guid.Parse(value.ToString());
                if (targetType == typeof(DateTime))
                    return value is DateTime dt ? dt : DateTime.Parse(value.ToString());
                if (targetType == typeof(string))
                    return value.ToString();
                return Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"AI参数 {argName} 的值 \"{value}\" 无法转换为 {targetType.Name}", ex);
            }
        }

        /// <summary>
        /// 按目标类型解包JsonElement为基元值
        /// </summary>
        private static object UnwrapJsonElement(System.Text.Json.JsonElement je, Type targetType)
        {
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            switch (je.ValueKind)
            {
                case System.Text.Json.JsonValueKind.Number:
                    if (underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short) || underlying == typeof(byte))
                        return Convert.ChangeType(je.GetDouble(), underlying, System.Globalization.CultureInfo.InvariantCulture);
                    return je.GetDouble();
                case System.Text.Json.JsonValueKind.True:
                case System.Text.Json.JsonValueKind.False:
                    return je.GetBoolean();
                case System.Text.Json.JsonValueKind.String:
                    return je.GetString();
                default:
                    return je.ToString();
            }
        }

        /// <summary>
        /// 去除LLM输出常见的Markdown代码围栏与前后杂字符
        /// </summary>
        internal static string CleanJson(string text)
        {
            var t = text.Trim();
            if (t.StartsWith("```"))
            {
                //去掉第一行的```json或```
                var firstLineEnd = t.IndexOf('\n');
                if (firstLineEnd > 0)
                    t = t.Substring(firstLineEnd + 1);
                var fenceEnd = t.LastIndexOf("```", StringComparison.Ordinal);
                if (fenceEnd >= 0)
                    t = t.Substring(0, fenceEnd);
            }
            return t.Trim();
        }

        private static readonly System.Text.Json.JsonSerializerOptions JsonOpts = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
