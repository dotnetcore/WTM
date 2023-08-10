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
    }

    public class WtmApproveBookmarkProvider : BookmarkProvider<WtmApproveBookmark, WtmApproveActivity>
    {
        public override IEnumerable<BookmarkResult> GetBookmarks(BookmarkProviderContext<WtmApproveActivity> context)
        {
            yield return Result(new WtmApproveBookmark());
        }
    }
}
