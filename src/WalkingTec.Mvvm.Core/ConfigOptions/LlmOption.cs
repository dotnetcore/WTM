namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 通用 LLM 配置 — 兼容 OpenAI / DeepSeek / Ollama / 任何 OpenAI-compatible API
    /// 在 appsettings.json 中配置:
    /// "Llm": { "ChatApiUrl": "https://api.deepseek.com/v1/chat/completions", "ApiKey": "[redacted]", "Model": "deepseek-chat" }
    /// </summary>
    public class LlmOption
    {
        /// <summary>是否启用</summary>
        public bool Enabled { get; set; } = false;

        // ========== 对话 ==========

        /// <summary>完整 API URL，如 https://api.deepseek.com/v1/chat/completions</summary>
        public string ChatApiUrl { get; set; } = "https://api.deepseek.com/v1/chat/completions";
        /// <summary>API Key</summary>
        public string ApiKey { get; set; } = "";
        /// <summary>模型名</summary>
        public string Model { get; set; } = "deepseek-chat";
        /// <summary>采样温度（0 到 2），值越低输出越确定、越高越发散，默认 0.3</summary>
        public float Temperature { get; set; } = 0.3f;
        /// <summary>单次响应允许的最大 Token 数，默认 2000</summary>
        public int MaxTokens { get; set; } = 2000;
        /// <summary>
        /// 追加到对话请求体的供应商专属参数（JSON对象字符串），合并覆盖默认请求体。
        /// 用于不改代码适配各供应商的开关，例如通义开启联网搜索：{"enable_search":true}；
        /// 智谱：{"tools":[{"type":"web_search","web_search":{"enable":true}}]}
        /// </summary>
        public string ExtraBodyJson { get; set; } = "";

        // ========== 视觉 ==========

        /// <summary>视觉 API 完整 URL，默认同 ChatApiUrl</summary>
        public string VisionApiUrl { get; set; } = "";
        /// <summary>视觉 Key，默认同 ApiKey</summary>
        public string VisionApiKey { get; set; } = "";
        /// <summary>视觉模型</summary>
        public string VisionModel { get; set; } = "deepseek-vl2";
    }
}
