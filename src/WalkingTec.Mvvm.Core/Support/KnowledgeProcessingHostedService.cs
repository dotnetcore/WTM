using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 知识库后台处理服务，按固定间隔扫描待处理的知识文档并执行分块与向量化处理
    /// </summary>
    public class KnowledgeProcessingHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private WTMContext _wtm;
        private static readonly TimeSpan _scanInterval = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 构造函数，初始化服务提供者并创建全局使用的 WTM 上下文
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public KnowledgeProcessingHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            using var scope = serviceProvider.CreateScope();
            _wtm = scope.ServiceProvider.GetRequiredService<WTMContext>();
            _wtm.SetServiceProvider(serviceProvider);
        }

        /// <summary>
        /// 后台任务主入口，循环处理待处理文档并按固定间隔轮询，直到收到停止信号
        /// </summary>
        /// <param name="stoppingToken">用于通知后台任务停止的取消令牌</param>
        /// <returns>表示后台任务执行完成的任务对象</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //服务启动时，把上次进程崩溃遗留的"处理中"文档重置为待处理，否则它们永远不会被重新扫描
            try
            {
                var orphans = await _wtm.DC.Set<KnowledgeDoc>()
                    .Where(d => d.Status == DocStatusEnum.Processing)
                    .ToListAsync(stoppingToken);
                if (orphans.Count > 0)
                {
                    foreach (var o in orphans) o.Status = DocStatusEnum.Pending;
                    await _wtm.DC.SaveChangesAsync(stoppingToken);
                    _wtm.DoLog($"重置{orphans.Count}条遗留的处理中文档为待处理", ActionLogTypesEnum.Normal, "知识库后台处理");
                }
            }
            catch { }

            // 持续轮询，直到服务被停止
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 处理当前所有待处理文档，异常记录到日志后继续下一轮轮询
                    await ProcessPendingDocuments(stoppingToken);
                }
                catch (Exception ex)
                {
                    _wtm.DoLog(ex.ToString(), ActionLogTypesEnum.Exception, "知识库后台处理");
                }

                await Task.Delay(_scanInterval, stoppingToken);
            }
        }

        // 依次取出最早的待处理文档并逐个执行处理管线，直到没有待处理文档或收到取消请求
        private async Task ProcessPendingDocuments(CancellationToken ct)
        {
            var processService = new KnowledgeProcessService();

            while (!ct.IsCancellationRequested)
            {
                // 按创建时间顺序取出最早的一条待处理文档
                var pending = await _wtm.DC.Set<KnowledgeDoc>()
                    .Where(d => d.Status == DocStatusEnum.Pending)
                    .OrderBy(d => d.CreateTime)
                    .FirstOrDefaultAsync(ct);

                if (pending == null) break;

                await processService.ProcessDocumentAsync(_wtm, pending.ID);

                //处理后状态若仍为待处理（异常路径），跳出本轮等待下个扫描周期，避免对同一文档紧密循环重试
                var after = await _wtm.DC.Set<KnowledgeDoc>().AsNoTracking()
                    .Where(d => d.ID == pending.ID)
                    .Select(d => (DocStatusEnum?)d.Status)
                    .FirstOrDefaultAsync(ct);
                if (after == null || after == DocStatusEnum.Pending) break;
            }
        }
    }
}
