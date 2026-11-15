using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// OpenAI 兼容嵌入服务 — 调用 /v1/embeddings API。
    /// 根据 EmbeddingOption 配置连接外部嵌入模型（OpenAI/Ollama/等）。
    /// </summary>
    public class OpenAIEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingOption _options;

        /// <summary>
        /// 构造函数，根据配置初始化 HTTP 客户端并设置接口地址、超时与认证头
        /// </summary>
        /// <param name="options">嵌入服务配置项</param>
        public OpenAIEmbeddingService(EmbeddingOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.ApiEndpoint.TrimEnd('/')),
                Timeout = TimeSpan.FromMinutes(2)
            };
            if (!string.IsNullOrEmpty(options.ApiKey))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.ApiKey);
        }

        /// <summary>
        /// 嵌入向量维度，配置未指定（小于等于 0）时默认为 1536
        /// </summary>
        public int Dimension => _options.Dimension > 0 ? _options.Dimension : 1536;

        /// <summary>
        /// 调用 OpenAI 兼容的嵌入接口，将文本转换为向量
        /// </summary>
        /// <param name="text">要向量化的文本</param>
        /// <returns>文本的嵌入向量</returns>
        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            // 构造请求体，未指定模型时默认使用 text-embedding-3-small
            var requestBody = new
            {
                model = string.IsNullOrEmpty(_options.Model) ? "text-embedding-3-small" : _options.Model,
                input = text,
                dimensions = _options.Dimension > 0 ? _options.Dimension : (int?)null
            };

            // 序列化请求体并调用 embeddings 接口
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/v1/embeddings", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<EmbeddingResponse>(responseJson);
            return result?.Data?[0]?.Embedding
                ?? throw new InvalidOperationException("Embedding API returned empty result");
        }

        /// <summary>
        /// 嵌入接口的返回结构
        /// </summary>
        private class EmbeddingResponse
        {
            [JsonPropertyName("data")] public List<EmbeddingData> Data { get; set; }
        }
        /// <summary>
        /// 单条嵌入数据，包含向量数组
        /// </summary>
        private class EmbeddingData
        {
            [JsonPropertyName("embedding")] public float[] Embedding { get; set; }
        }
    }
}
