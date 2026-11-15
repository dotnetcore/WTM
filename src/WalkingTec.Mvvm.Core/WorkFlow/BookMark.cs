using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// WTM 审批书签，记录等待审批的用户/角色/部门标识及流程信息，
    /// Elsa 通过书签在外部审批触发时定位并恢复挂起的审批活动
    /// </summary>
    public class WtmApproveBookmark : IBookmark
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public WtmApproveBookmark()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="user">审批用户 ITCode 或角色/部门标识</param>
        /// <param name="name">流程名称或业务实体类型全名</param>
        /// <param name="tag">审批节点标签</param>
        /// <param name="endityid">业务实体 ID</param>
        public WtmApproveBookmark(string user,string name,string tag,string endityid)
        {
            User = user;
            Tag = tag;
            Name = name;
            EntityId = endityid;
        }

        /// <summary>
        /// 审批用户 ITCode 或角色/部门标识
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 审批节点标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 流程名称或业务实体类型全名（书签匹配键）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 业务实体 ID
        /// </summary>
        public string EntityId { get; set; }
    }

    /// <summary>
    /// WTM 审批书签提供器，为审批活动生成书签，供外部审批触发时匹配并恢复流程
    /// </summary>
    public class WtmApproveBookmarkProvider : BookmarkProvider<WtmApproveBookmark, WtmApproveActivity>
    {
        /// <summary>
        /// 根据活动中配置的审批用户、角色、部门信息生成书签：
        /// 每类审批人分别生成按流程名称和按业务实体类型全名作为键的书签，两种键均可用于触发匹配
        /// </summary>
        /// <param name="context">书签提供器上下文，包含活动配置与工作流执行信息</param>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>生成的书签结果集合；未配置任何审批人时返回空集合</returns>
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<WtmApproveActivity> context, CancellationToken cancellationToken)
        {
            // 读取活动中配置的审批用户、角色、部门、主管列表及标签
            var userinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveUsers, cancellationToken))?.ToList() ?? new List<string>();
            var roleinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveRoles, cancellationToken))?.ToList() ?? new List<string>();
            var groupinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveGroups, cancellationToken))?.ToList() ?? new List<string>();
            var managerinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveManagers, cancellationToken))?.ToList() ?? new List<string>();
            var tag = (await context.ReadActivityPropertyAsync<WtmApproveActivity, string>(x => x.Tag, cancellationToken));
            // 获取流程名称、业务实体类型全名和业务实体 ID，作为书签的匹配键
            var name = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.Name??"";
            var model = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.ContextOptions?.ContextType?.FullName;
            var id = context.ActivityExecutionContext.WorkflowExecutionContext.ContextId?.ToString()??"";
            if(tag == "")
            {
                tag = null;
            }
            // 每类审批人生成两份书签：一份以流程名称为键，一份以业务实体类型全名为键
            List<BookmarkResult> rv = new List<BookmarkResult>();
            if (userinfo?.Any() == true)
            {
                rv.AddRange(userinfo.Select(x => Result(new WtmApproveBookmark(x, name,tag,id))));
                if(string.IsNullOrEmpty(model) == false)
                {
                    rv.AddRange(userinfo.Select(x => Result(new WtmApproveBookmark(x, model, tag, id))));
                }
            }
            if (roleinfo?.Any() == true)
            {
                rv.AddRange(roleinfo.Select(x => Result(new WtmApproveBookmark(x, name, tag, id))));
                if (string.IsNullOrEmpty(model) == false)
                {
                    rv.AddRange(roleinfo.Select(x => Result(new WtmApproveBookmark(x, model, tag, id))));
                }
            }
            if (groupinfo?.Any() == true)
            {
                rv.AddRange(groupinfo.Select(x => Result(new WtmApproveBookmark(x, name, tag, id))));
                if (string.IsNullOrEmpty(model) == false)
                {
                    rv.AddRange(groupinfo.Select(x => Result(new WtmApproveBookmark(x, model, tag, id))));
                }
            }
            if(rv.Count > 0)
            {
                return rv;
            }
            // 未配置任何审批人时返回空数组
            return new BookmarkResult[0];
        }

    }
}
