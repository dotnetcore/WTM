namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 嵌入服务配置 — 在 appsettings.json 中配置
    /// </summary>
    public class EmbeddingOption
    {
        /// <summary>API 地址，默认 Ollama 本地</summary>
        public string ApiEndpoint { get; set; } = "http://localhost:11434";

        /// <summary>API Key（Ollama 不需要）</summary>
        public string ApiKey { get; set; } = "";

        /// <summary>模型名，默认 nomic-embed-text（Ollama 免费嵌入模型）</summary>
        public string Model { get; set; } = "nomic-embed-text";

        /// <summary>向量维度，0 使用模型默认值</summary>
        public int Dimension { get; set; } = 0;
    }
}
