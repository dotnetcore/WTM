using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DFS 业务类型与Group映射关系
    /// </summary>
    public class SupportedBusinessTypeMapping
    {
        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 该业务类型对应的 Group 与内外网地址映射列表
        /// </summary>
        public List<SupportedGroupMapping> GroupMappings { get; set; }
    }

    /// <summary>
    /// DFS Group与内外网地址映射关系
    /// </summary>
    public class SupportedGroupMapping
    {
        /// <summary>
        /// 群组名
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 内网地址头
        /// </summary>
        public string InsideUrlHeader { get; set; }
        /// <summary>
        /// 外网地址头
        /// </summary>
        public string OutsideUrlHeader { get; set; }
    }
}
