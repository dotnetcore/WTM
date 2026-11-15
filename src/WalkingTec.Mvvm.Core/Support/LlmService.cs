using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 大模型（LLM）服务，封装对话补全接口的调用，
    /// 支持普通对话、知识库上下文对话、多轮对话及图像理解
    /// </summary>
    public class LlmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _chatPath;
        private readonly HttpClient _visionClient;
        private readonly string _visionPath;
        private readonly LlmOption _options;

        /// <summary>
        /// 构造函数，根据配置分别初始化对话与图像理解两个 HTTP 客户端
        /// </summary>
        /// <param name="options">大模型配置项</param>
        public LlmService(LlmOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            (_httpClient, _chatPath) = CreateClient(options.ChatApiUrl, options.ApiKey);
            // 图像理解接口未单独配置时，回退使用对话接口的地址与密钥
            var visionUrl = !string.IsNullOrEmpty(options.VisionApiUrl) ? options.VisionApiUrl : options.ChatApiUrl;
            var visionKey = !string.IsNullOrEmpty(options.VisionApiKey) ? options.VisionApiKey : options.ApiKey;
            (_visionClient, _visionPath) = CreateClient(visionUrl, visionKey);
        }

        // 将完整的接口地址拆分为基址与路径，并创建携带 Bearer 认证头的 HTTP 客户端
        private static (HttpClient client, string path) CreateClient(string fullUrl, string apiKey)
        {
            var uri = new Uri(fullUrl);
            var baseAddr = $"{uri.Scheme}://{uri.Authority}";
            var path = uri.PathAndQuery;
            var client = new HttpClient { BaseAddress = new Uri(baseAddr) };
            if (!string.IsNullOrEmpty(apiKey))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            return (client, path);
        }

        /// <summary>
        /// 发起单轮对话，可附带系统提示词
        /// </summary>
        /// <param name="question">用户提问内容</param>
        /// <param name="systemMessage">系统提示词，可为空</param>
        /// <returns>模型返回的回答文本</returns>
        public Task<string> ChatAsync(string question, string systemMessage = null)
        {
            var messages = new List<object>();
            if (!string.IsNullOrEmpty(systemMessage)) messages.Add(new { role = "system", content = systemMessage });
            messages.Add(new { role = "user", content = question });
            return PostAsync(_httpClient, _chatPath, _options.Model, messages);
        }

        /// <summary>
        /// 发起单轮对话，支持按调用方覆盖模型、温度与最大Token数；
        /// 未指定的参数回退使用构造时的配置，供Agent编排引擎的LLM节点使用
        /// </summary>
        /// <param name="systemMessage">系统提示词，可为空</param>
        /// <param name="userMessage">用户提示词</param>
        /// <param name="model">模型名称，空则使用配置的默认模型</param>
        /// <param name="temperature">温度，空则使用默认值</param>
        /// <param name="maxTokens">最大生成Token数，空则使用默认值</param>
        /// <returns>模型返回的回答文本</returns>
        public Task<string> ChatAsync(string systemMessage, string userMessage, string model = null, double? temperature = null, int? maxTokens = null)
        {
            var messages = new List<object>();
            if (!string.IsNullOrEmpty(systemMessage)) messages.Add(new { role = "system", content = systemMessage });
            messages.Add(new { role = "user", content = userMessage });
            return PostAsync(_httpClient, _chatPath,
                string.IsNullOrEmpty(model) ? _options.Model : model,
                messages, temperature, maxTokens);
        }

        /// <summary>
        /// 基于知识库内容的增强对话，将检索到的知识库分块拼入系统提示词后发起请求
        /// </summary>
        /// <param name="question">用户提问内容</param>
        /// <param name="contextChunks">从知识库检索到的上下文分块列表</param>
        /// <returns>模型返回的回答文本</returns>
        public async Task<string> ChatWithContextAsync(string question, List<string> contextChunks)
        {
            var systemMessage = "你是一个知识库助手。请严格基于以下提供的知识库内容回答问题。\n\n### 知识库内容:\n"
                + string.Join("\n---\n", contextChunks);
            return await PostAsync(_httpClient, _chatPath, _options.Model,
                new List<object> { new { role = "system", content = systemMessage }, new { role = "user", content = question } });
        }

        /// <summary>
        /// 发起多轮对话，按顺序传入历史对话消息
        /// </summary>
        /// <param name="turns">多轮对话的消息列表</param>
        /// <returns>模型返回的回答文本</returns>
        public Task<string> ChatMultiTurnAsync(List<ChatTurn> turns)
        {
            return PostAsync(_httpClient, _chatPath, _options.Model,
                turns.Select(t => new { role = t.Role, content = t.Content }).Cast<object>().ToList());
        }

        /// <summary>
        /// 发起带图像的多模态对话，将问题与图像一并提交给视觉模型
        /// </summary>
        /// <param name="question">用户提问内容</param>
        /// <param name="imageDataUrl">图像的 Data URL 地址</param>
        /// <returns>模型返回的回答文本</returns>
        public async Task<string> ChatWithImageAsync(string question, string imageDataUrl)
        {
            return await PostAsync(_visionClient, _visionPath,
                !string.IsNullOrEmpty(_options.VisionModel) ? _options.VisionModel : _options.Model,
                new List<object> { new { role = "user", content = new object[] {
                    new { type = "text", text = question },
                    new { type = "image_url", image_url = new { url = imageDataUrl, detail = "auto" } }
                }}});
        }

        /// <summary>
        /// 发起带工具定义的对话（OpenAI function calling 协议）。
        /// messages为原始消息对象列表（支持 system/user/assistant/tool 及含tool_calls的消息，原样透传），
        /// 模型要求调用工具时返回ToolCalls列表，否则返回最终文本Content；
        /// RawResponse保留接口原始JSON、AssistantMessage保留choices[0].message原样元素，供Agent循环回填与调试展示
        /// </summary>
        /// <param name="messages">完整的消息历史（原始对象，原样序列化）</param>
        /// <param name="tools">可调用的工具定义列表</param>
        /// <param name="model">模型名称，空则使用配置的默认模型</param>
        /// <param name="temperature">温度，空则使用默认值</param>
        /// <param name="maxTokens">最大生成Token数，空则使用默认值</param>
        /// <returns>模型返回的消息内容与工具调用请求</returns>
        public async Task<LlmToolCallResult> ChatWithToolsAsync(List<object> messages, List<LlmToolDefinition> tools, string model = null, double? temperature = null, int? maxTokens = null)
        {
            var body = new Dictionary<string, object>
            {
                ["model"] = string.IsNullOrEmpty(model) ? _options.Model : model,
                ["messages"] = messages,
                ["temperature"] = temperature ?? 0.3,
                ["max_tokens"] = maxTokens ?? 2000,
                ["stream"] = false,
                ["tool_choice"] = "auto",
                ["tools"] = tools.Select(t => (object)new Dictionary<string, object>
                {
                    ["type"] = "function",
                    ["function"] = new Dictionary<string, object>
                    {
                        ["name"] = t.Name,
                        ["description"] = t.Description,
                        ["parameters"] = JsonSerializer.Deserialize<JsonElement>(t.ParametersJsonSchema)
                    }
                }).ToList()
            };
            ApplyExtraBody(body, _options.ExtraBodyJson);
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_chatPath, content);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"LLM API {(int)response.StatusCode}: {json}");

            var result = new LlmToolCallResult { RawResponse = PrettyJson(json) };
            using (var doc = JsonDocument.Parse(json))
            {
                var message = doc.RootElement.GetProperty("choices")[0].GetProperty("message");
                result.AssistantMessage = message.Clone();
                if (message.TryGetProperty("content", out var c) && c.ValueKind == JsonValueKind.String)
                    result.Content = c.GetString();
                if (message.TryGetProperty("tool_calls", out var tcs) && tcs.ValueKind == JsonValueKind.Array)
                {
                    foreach (var tc in tcs.EnumerateArray())
                    {
                        result.ToolCalls.Add(new LlmToolCall
                        {
                            Id = tc.TryGetProperty("id", out var id) ? id.GetString() : null,
                            Name = tc.TryGetProperty("function", out var fn) && fn.TryGetProperty("name", out var name) ? name.GetString() : null,
                            ArgumentsJson = fn.TryGetProperty("arguments", out var args) && args.ValueKind == JsonValueKind.String ? args.GetString() : "{}"
                        });
                    }
                }
                //提取token用量与缓存命中（DeepSeek返回prompt_cache_hit_tokens），供前端展示缓存效果
                if (doc.RootElement.TryGetProperty("usage", out var usage))
                {
                    if (usage.TryGetProperty("prompt_tokens", out var pt) && pt.ValueKind == JsonValueKind.Number)
                        result.PromptTokens = pt.GetInt32();
                    if (usage.TryGetProperty("prompt_cache_hit_tokens", out var hit) && hit.ValueKind == JsonValueKind.Number)
                        result.CacheHitTokens = hit.GetInt32();
                    if (usage.TryGetProperty("completion_tokens", out var ct) && ct.ValueKind == JsonValueKind.Number)
                        result.CompletionTokens = ct.GetInt32();
                }
            }
            return result;
        }

        /// <summary>
        /// 将JSON字符串格式化为带缩进的形式，便于调试展示；
        /// 使用宽松编码器避免中文被转义成\uXXXX形式
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <returns>带缩进的JSON字符串，解析失败时原样返回</returns>
        private static string PrettyJson(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch
            {
                return json;
            }
        }

        // 构造对话补全请求体并调用大模型接口，解析返回的首条回复内容
        private Task<string> PostAsync(HttpClient client, string path, string model, List<object> messages)
        {
            return PostAsync(client, path, model, messages, null, null);
        }

        // 构造对话补全请求体并调用大模型接口，解析返回的首条回复内容；可覆盖温度与最大Token数
        private async Task<string> PostAsync(HttpClient client, string path, string model, List<object> messages, double? temperature, int? maxTokens)
        {
            var body = new Dictionary<string, object>
            {
                ["model"] = model,
                ["messages"] = messages,
                ["temperature"] = temperature ?? 0.3,
                ["max_tokens"] = maxTokens ?? 2000,
                ["stream"] = false,
            };
            ApplyExtraBody(body, _options.ExtraBodyJson);
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(path, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatCompletionResponse>(json);
            return result?.Choices?[0]?.Message?.Content
                ?? throw new InvalidOperationException("LLM API returned empty response");
        }

        /// <summary>
        /// 把配置的供应商专属参数（ExtraBodyJson）合并进请求体，同键覆盖默认值；
        /// 解析失败时忽略，不影响正常请求
        /// </summary>
        private static void ApplyExtraBody(Dictionary<string, object> body, string extraJson)
        {
            if (string.IsNullOrWhiteSpace(extraJson))
                return;
            try
            {
                using var doc = JsonDocument.Parse(extraJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    return;
                foreach (var p in doc.RootElement.EnumerateObject())
                    body[p.Name] = p.Value.Clone();
            }
            catch
            {
                /* ExtraBodyJson不是合法JSON对象时忽略 */
            }
        }

        /// <summary>
        /// 对话补全接口的返回结构
        /// </summary>
        private class ChatCompletionResponse { [JsonPropertyName("choices")] public List<Choice> Choices { get; set; } }
        /// <summary>
        /// 单条候选回复
        /// </summary>
        private class Choice { [JsonPropertyName("message")] public MessageContent Message { get; set; } }
        /// <summary>
        /// 回复消息内容
        /// </summary>
        private class MessageContent { [JsonPropertyName("content")] public string Content { get; set; } }
    }

    /// <summary>
    /// 对话轮次类，包含消息角色（system/user/assistant）与消息内容
    /// </summary>
    public class ChatTurn { public string Role { get; set; } public string Content { get; set; } }

    /// <summary>
    /// 工具定义（OpenAI function calling 格式），ParametersJsonSchema为JSON Schema字符串
    /// </summary>
    public class LlmToolDefinition
    {
        /// <summary>工具名称，模型调用时引用</summary>
        public string Name { get; set; }
        /// <summary>工具功能描述，供模型判断何时调用</summary>
        public string Description { get; set; }
        /// <summary>参数的JSON Schema定义</summary>
        public string ParametersJsonSchema { get; set; }
    }

    /// <summary>
    /// 模型发起的一次工具调用请求（含调用ID、工具名与JSON字符串形式的参数）
    /// </summary>
    public class LlmToolCall
    {
        /// <summary>调用ID，回填tool消息时必须原样带回</summary>
        public string Id { get; set; }
        /// <summary>工具名称</summary>
        public string Name { get; set; }
        /// <summary>参数的JSON字符串</summary>
        public string ArgumentsJson { get; set; }
    }

    /// <summary>
    /// 带工具对话的返回结果：文本内容、工具调用列表、
    /// 原始响应JSON与原样的assistant消息元素（供Agent循环回填历史）
    /// </summary>
    public class LlmToolCallResult
    {
        /// <summary>模型返回的文本内容（要求调用工具时可能为空）</summary>
        public string Content { get; set; }
        /// <summary>模型请求的工具调用列表</summary>
        public List<LlmToolCall> ToolCalls { get; set; } = new List<LlmToolCall>();
        /// <summary>接口原始响应JSON（格式化后）</summary>
        public string RawResponse { get; set; }
        /// <summary>choices[0].message 的原样JSON元素（含tool_calls，可直接回填消息历史）</summary>
        public JsonElement AssistantMessage { get; set; }
        /// <summary>本次请求的输入token数（含缓存命中的部分）</summary>
        public int? PromptTokens { get; set; }
        /// <summary>前缀缓存命中的输入token数（DeepSeek自动上下文缓存）</summary>
        public int? CacheHitTokens { get; set; }
        /// <summary>本次生成的输出token数</summary>
        public int? CompletionTokens { get; set; }
    }
}
