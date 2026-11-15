using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 内置嵌入服务 — 字符 N-gram 哈希算法，零依赖，即时可用。
    /// 原理同 fastText：字符 unigram/bigram/trigram → FNV-1a 哈希 → 累加 → L2 归一化。
    /// 中文效果尤其好，维度 256，无需任何外部 API 或 GPU。
    /// </summary>
    public class BuiltInEmbeddingService : IEmbeddingService
    {
        private const int Dim = 256;

        /// <summary>
        /// 嵌入向量的维度
        /// </summary>
        public int Dimension => Dim;

        /// <summary>
        /// 计算文本的嵌入向量：提取字符n-gram并哈希映射到固定维度上累加计数，最后做L2归一化
        /// </summary>
        /// <param name="text">待向量化的文本</param>
        /// <returns>256维的归一化嵌入向量；空白文本返回全零向量</returns>
        public Task<float[]> GetEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Task.FromResult(new float[Dim]);

            var normalized = Normalize(text);
            var vec = new float[Dim];
            var ngrams = ExtractNgrams(normalized, maxN: 3);

            //将每个n-gram哈希到某一维度并累加计数
            foreach (var ng in ngrams)
            {
                int idx = HashToDim(ng, Dim);
                vec[idx] += 1f;
            }

            L2Normalize(vec);
            return Task.FromResult(vec);
        }

        /// <summary>
        /// 文本规范化：将连续空白字符合并为单个空格，并将字符统一转为小写
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <returns>规范化后的文本</returns>
        private static string Normalize(string text)
        {
            var sb = new StringBuilder(text.Length);
            bool lastWasSpace = false;
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!lastWasSpace) { sb.Append(' '); lastWasSpace = true; }
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                    lastWasSpace = false;
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 提取1到maxN的字符n-gram特征：在文本首尾添加边界标记以保留头尾字符信息，同时把完整单词也作为特征加入
        /// </summary>
        /// <param name="text">规范化后的文本</param>
        /// <param name="maxN">n-gram的最大长度</param>
        /// <returns>n-gram特征集合</returns>
        private static HashSet<string> ExtractNgrams(string text, int maxN)
        {
            var result = new HashSet<string>();
            var padded = $"<{text}>";
            for (int n = 1; n <= maxN; n++)
                for (int i = 0; i <= padded.Length - n; i++)
                    result.Add(padded.Substring(i, n));

            foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                if (word.Length > 1) result.Add(word);

            return result;
        }

        /// <summary>
        /// 使用FNV-1a哈希算法将字符串映射到指定维度的向量下标
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <param name="dim">向量维度</param>
        /// <returns>0到dim-1之间的向量下标</returns>
        private static int HashToDim(string s, int dim)
        {
            uint hash = 2166136261u;
            foreach (char c in s) { hash ^= c; hash *= 16777619u; }
            return (int)(hash % (uint)dim);
        }

        /// <summary>
        /// L2归一化：将向量缩放为单位长度，使其可直接用于余弦相似度计算
        /// </summary>
        /// <param name="vec">待归一化的向量</param>
        private static void L2Normalize(float[] vec)
        {
            float norm = 0f;
            for (int i = 0; i < vec.Length; i++) norm += vec[i] * vec[i];
            norm = MathF.Sqrt(norm);
            if (norm > 1e-10f)
                for (int i = 0; i < vec.Length; i++) vec[i] /= norm;
        }
    }
}
