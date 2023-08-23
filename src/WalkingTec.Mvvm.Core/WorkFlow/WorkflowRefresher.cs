using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class WorkflowRefresher<T> : WorkflowContextRefresher<T> where T : TopBasePoco
    {

        private WTMContext _wtm;

        public WorkflowRefresher(WTMContext wtm)
        {
            this._wtm = wtm;
        }

        public override async ValueTask<T> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default)
        {            
            T rv = context.WorkflowExecutionContext.WorkflowContext as T;
            if (rv == null)
            {
                using (var dc = _wtm.DC.ReCreate())
                {
                    rv = await dc.Set<T>().CheckID(context.ContextId).FirstOrDefaultAsync(cancellationToken);
                }
            }
            return rv;
        }

        public override async ValueTask<string> SaveAsync(SaveWorkflowContext<T> context, CancellationToken cancellationToken = default)
        {
            var data = context.Context;
            if (context.WorkflowExecutionContext.ContextHasChanged == true)
            {
                using (var dc = _wtm.DC.ReCreate())
                {
                    var existing = await dc.Set<T>().CheckID(data.ID).FirstOrDefaultAsync(cancellationToken);
                    (dc as DbContext).Entry(existing).CurrentValues.SetValues(data);
                    await dc.SaveChangesAsync(cancellationToken);
                }
            }
            return data.ID.ToString();
        }
    }
}
