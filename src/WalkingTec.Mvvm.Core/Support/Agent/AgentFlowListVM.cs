using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// Agent流程列表搜索器
    /// </summary>
    public class AgentFlowSearcher : BaseSearcher
    {
        /// <summary>
        /// 流程名称关键字
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Agent流程列表VM（标准列表管道，供WtmTable的doSearch调用）
    /// </summary>
    public class AgentFlowListVM : BasePagedListVM<AgentFlow, AgentFlowSearcher>
    {
        protected override IEnumerable<IGridColumn<AgentFlow>> InitGridHeader()
        {
            return new List<GridColumn<AgentFlow>>
            {
                this.MakeGridHeader(x => x.Name, 180),
                this.MakeGridHeader(x => x.Description, 260),
                this.MakeGridHeader(x => x.IsSkill, 80),
                this.MakeGridHeader(x => x.IsScheduled, 80),
                this.MakeGridHeader(x => x.UpdateTime, 150),
                this.MakeGridHeader(x => x.CreateBy, 100),
            };
        }

        public override IOrderedQueryable<AgentFlow> GetSearchQuery()
        {
            var q = DC.Set<AgentFlow>()
                .Where(x => x.IsValid == true)
                .CheckContain(Searcher.Name, x => x.Name);
            return q.OrderByDescending(x => x.UpdateTime ?? x.CreateTime);
        }
    }
}
