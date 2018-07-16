using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DFS 业务类型与Group映射关系
    /// </summary>
    public class SupportedBusinessTypeMapping
    {
        public string BusinessTypeName { get; set; }
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
