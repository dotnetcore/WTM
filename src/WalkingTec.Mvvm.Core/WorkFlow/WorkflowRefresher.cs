using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// WTM 工作流上下文刷新器，负责在 Elsa 流程执行过程中加载和保存与流程关联的业务实体，
    /// 加载时优先通过匹配的 CRUD VM 按主键获取实体，以填充导航属性等关联数据
    /// </summary>
    /// <typeparam name="T">与工作流关联的业务实体类型，须继承 TopBasePoco 并实现 IWorkflow 接口</typeparam>
    public class WorkflowRefresher<T> : WorkflowContextRefresher<T> where T : TopBasePoco,IWorkflow
    {

        /// <summary>
        /// WTM 上下文，用于创建数据库上下文和定位 CRUD VM
        /// </summary>
        private WTMContext _wtm;

        /// <summary>
        /// 构造函数，注入 WTM 上下文
        /// </summary>
        /// <param name="wtm">WTM 上下文对象</param>
        public WorkflowRefresher(WTMContext wtm)
        {
            this._wtm = wtm;
        }

        /// <summary>
        /// 加载工作流上下文：优先查找实体类型对应的 CRUD VM 并通过 VM 按主键加载实体
        /// （以便填充导航属性），找不到对应 VM 时直接从数据库按 ID 查询
        /// </summary>
        /// <param name="context">加载工作流上下文，携带上下文 ID 与工作流执行信息</param>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>加载到的业务实体；加载失败时返回 null</returns>
        public override async ValueTask<T> LoadAsync(LoadWorkflowContext context, CancellationToken cancellationToken = default)
        {
            // 若工作流执行上下文中已存在实体则直接复用
            T rv = context.WorkflowExecutionContext.WorkflowContext as T;
            if (rv == null)
            {
                // 反射查找所有 VM 类型中实体类型为 T 的 CRUD VM，用于通过 VM 加载实体
                var vmType = Utils.GetAllVms().Where(x => typeof(IBaseCRUDVM<TopBasePoco>).IsAssignableFrom(x) && x.GetInterface("IBaseCRUDVM`1")?.GenericTypeArguments[0] == typeof(T)).FirstOrDefault();
                IBaseCRUDVM<TopBasePoco> vm = null;
                if (vmType != null)
                {
                    // 反射创建 VM 实例（要求 VM 提供无参构造函数）
                    vm = vmType.GetConstructor(System.Type.EmptyTypes).Invoke(null) as IBaseCRUDVM<TopBasePoco>;
                }
                // 使用新建的数据库上下文，避免长生命周期上下文的缓存影响
                using (var dc = _wtm.DC.ReCreate())
                {
                    if (vm != null)
                    {
                        // 为 VM 注入 WTM 上下文和数据库上下文后按 ID 加载实体
                        (vm as BaseVM).Wtm = this._wtm;
                        (vm as BaseVM).DC = dc;

                        vm.SetEntityById(context.ContextId);
                        rv = vm.Entity as T;
                    }
                    else
                    {
                        // 未找到对应 VM 时直接按 ID 从数据库查询实体
                        rv = await dc.Set<T>().CheckID(context.ContextId).FirstOrDefaultAsync(cancellationToken);

                    }
                }
            }
            return rv;
        }

        /// <summary>
        /// 保存工作流上下文：将流程中被修改的业务实体重新加载到新建的数据库上下文，
        /// 把最新值复制到数据库现有记录后保存
        /// </summary>
        /// <param name="context">保存工作流上下文，携带待保存的业务实体</param>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>保存后的业务实体 ID 字符串</returns>
        public override async ValueTask<string> SaveAsync(SaveWorkflowContext<T> context, CancellationToken cancellationToken = default)
        {
            var data = context.Context;
                using (var dc = _wtm.DC.ReCreate(_wtm.LoggerFactory))
                {
                    // 从数据库重新加载现有实体，将工作流上下文中的最新值复制过去后保存
                    var existing = await dc.Set<T>().CheckID(data.GetID()).FirstOrDefaultAsync(cancellationToken);
                    (dc as DbContext).Entry(existing).CurrentValues.SetValues(data);
                    await dc.SaveChangesAsync(cancellationToken);
                }
            
            return data.GetID().ToString();
        }
    }
}
