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
            var info = (await context.ReadActivityPropertyAsync<WtmApproveActivity, ICollection<string>>(x => x.ApproveUsers, cancellationToken))?.ToList() ?? new List<string>();
            var tag = (await context.ReadActivityPropertyAsync<WtmApproveActivity, string>(x => x.Tag, cancellationToken));
            var name = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.Name??"";
            var model = context.ActivityExecutionContext.WorkflowExecutionContext.WorkflowBlueprint.ContextOptions.ContextType?.FullName;
            var id = context.ActivityExecutionContext.WorkflowExecutionContext.ContextId?.ToString()??"";
            List<BookmarkResult> rv = new List<BookmarkResult>();
            if (info?.Any() == true)
            {
                rv.AddRange(info.Select(x => Result(new WtmApproveBookmark(x, name,tag,id))));
                if(string.IsNullOrEmpty(model) == false)
                {
                    rv.AddRange(info.Select(x => Result(new WtmApproveBookmark(x, model, tag, id))));
                }

                return rv;
            }
            return new BookmarkResult[0];
        }

    }
}
