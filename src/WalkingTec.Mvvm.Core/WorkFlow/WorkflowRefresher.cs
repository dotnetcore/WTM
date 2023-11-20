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
    public class WorkflowRefresher<T> : WorkflowContextRefresher<T> where T : TopBasePoco,IWorkflow
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
                var vmType = Utils.GetAllVms().Where(x => typeof(IBaseCRUDVM<TopBasePoco>).IsAssignableFrom(x) && x.GetInterface("IBaseCRUDVM`1")?.GenericTypeArguments[0] == typeof(T)).FirstOrDefault();
                IBaseCRUDVM<TopBasePoco> vm = null;
                if (vmType != null)
                {
                    vm = vmType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as IBaseCRUDVM<TopBasePoco>;
                }
                using (var dc = _wtm.DC.ReCreate())
                {
                    if (vm != null)
                    {
                        (vm as BaseVM).DC = dc;
                        vm.SetEntityById(context.ContextId);
                        rv = vm.Entity as T;
                    }
                    else
                    {
                        rv = await dc.Set<T>().CheckID(context.ContextId).FirstOrDefaultAsync(cancellationToken);

                    }
                }
            }
            return rv;
        }

        public override async ValueTask<string> SaveAsync(SaveWorkflowContext<T> context, CancellationToken cancellationToken = default)
        {
            var data = context.Context;
                using (var dc = _wtm.DC.ReCreate(_wtm.LoggerFactory))
                {
                    var existing = await dc.Set<T>().CheckID(data.GetID()).FirstOrDefaultAsync(cancellationToken);
                    (dc as DbContext).Entry(existing).CurrentValues.SetValues(data);
                    await dc.SaveChangesAsync(cancellationToken);
                }
            
            return data.GetID().ToString();
        }
    }
}
