using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class WtmApproveBookmark : IBookmark
    {
        public WtmApproveBookmark()
        {
        }

        public WtmApproveBookmark(string user,string name,string tag,string endityid)
        {
            User = user;
            Tag = tag;
            Name = name;
            EntityId = endityid;
        }

        public string User { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string EntityId { get; set; }
    }

    public class WtmApproveBookmarkProvider : BookmarkProvider<WtmApproveBookmark, WtmApproveActivity>
    {
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<WtmApproveActivity> context, CancellationToken cancellationToken)
        {
            var userinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveUsers, cancellationToken))?.ToList() ?? new List<string>();
            var roleinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveRoles, cancellationToken))?.ToList() ?? new List<string>();
            var groupinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveGroups, cancellationToken))?.ToList() ?? new List<string>();
            var managerinfo = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveManagers, cancellationToken))?.ToList() ?? new List<string>();
            var tag = (await context.ReadActivityPropertyAsync<WtmApproveActivity, string>(x => x.Tag, cancellationToken));
            var name = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.Name??"";
            var model = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.ContextOptions?.ContextType?.FullName;
            var id = context.ActivityExecutionContext.WorkflowExecutionContext.ContextId?.ToString()??"";
            if(tag == "")
            {
                tag = null;
            }
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
            return new BookmarkResult[0];
        }

    }

    public class BackApproveBookmarkProvider : BookmarkProvider<WtmApproveBookmark, BackApproveActivity>
    {
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<BackApproveActivity> context, CancellationToken cancellationToken)
        {
            List<BookmarkResult> rv = new List<BookmarkResult>();

            var name = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.Name ?? "";
            var model = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.ContextOptions?.ContextType?.FullName;
            var id = context.ActivityExecutionContext.WorkflowExecutionContext.ContextId?.ToString() ?? "";
            var tag = "提交";

            var data = context.ActivityExecutionContext.WorkFlowApproveRecord();

            if (data != null && data.Approved != null)
            {
                rv.Add(Result(new WtmApproveBookmark(data.Approved, name, tag, id)));

                if (!string.IsNullOrEmpty(model))
                {
                    rv.Add(Result(new WtmApproveBookmark(data.Approved, model, tag, id)));
                }
            }

            return rv;
        }
    }
}
