using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test
{
    public abstract class CrudTestBase
    {
        private static bool _localizerInitialized;

        static CrudTestBase()
        {
            if (CoreProgram._localizer == null)
            {
                var factory = new ResourceManagerStringLocalizerFactory(
                    Options.Create(new LocalizationOptions { ResourcesPath = "Resources" }),
                    new Microsoft.Extensions.Logging.LoggerFactory());
                CoreProgram._localizer = factory.Create(typeof(Program));
            }
        }
        protected string Seed { get; private set; }

        /// <summary>
        /// 每个测试类实例获得唯一的 seed，确保内存数据库隔离
        /// </summary>
        protected void InitSeed()
        {
            Seed = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 创建新的内存数据库上下文（同一 seed 共享数据）
        /// </summary>
        protected DataContext NewContext()
        {
            return new DataContext(Seed, DBTypeEnum.Memory);
        }

        /// <summary>
        /// 创建模拟的 WTM 上下文并绑定内存数据库
        /// </summary>
        protected WTMContext NewWtmContext(string userCode = null)
        {
            return MockWtmContext.CreateWtmContext(NewContext(), userCode);
        }

        /// <summary>
        /// 为 VM 设置 Wtm 上下文
        /// </summary>
        protected void SetupVm(BaseVM vm, string userCode = null)
        {
            vm.Wtm = NewWtmContext(userCode);
        }

        #region Assertion Helpers

        /// <summary>
        /// 断言 VM 没有模型错误
        /// </summary>
        protected static void AssertNoModelError(BaseVM vm)
        {
            Assert.AreEqual(0, vm.MSD.Count, $"期望无模型错误，但实际有 {vm.MSD.Count} 个");
        }

        /// <summary>
        /// 断言指定字段存在模型错误
        /// </summary>
        protected static void AssertModelError(BaseVM vm, string fieldKey)
        {
            Assert.IsTrue(vm.MSD[fieldKey].Count > 0,
                $"期望字段 '{fieldKey}' 存在模型错误");
        }

        /// <summary>
        /// 断言指定字段不存在模型错误
        /// </summary>
        protected static void AssertNoModelError(BaseVM vm, string fieldKey)
        {
            Assert.AreEqual(0, vm.MSD[fieldKey].Count,
                $"期望字段 '{fieldKey}' 无模型错误");
        }

        /// <summary>
        /// 断言时间在指定秒数内（默认 10 秒容忍度）
        /// </summary>
        protected static void AssertRecent(DateTime? dt, int toleranceSeconds = 10)
        {
            Assert.IsNotNull(dt, "期望日期时间不为 null");
            var diff = DateTime.Now.Subtract(dt.Value).Duration();
            Assert.IsTrue(diff.TotalSeconds < toleranceSeconds,
                $"期望时间在 {toleranceSeconds}s 内，实际差距 {diff.TotalSeconds:F1}s");
        }

        /// <summary>
        /// 断言实体已创建审计字段（CreateBy/CreateTime）
        /// </summary>
        protected static void AssertCreatedAudit<T>(T entity, string expectedUser)
            where T : IBasePoco
        {
            Assert.AreEqual(expectedUser, entity.CreateBy, "CreateBy 不匹配");
            AssertRecent(entity.CreateTime);
        }

        /// <summary>
        /// 断言实体已更新审计字段（UpdateBy/UpdateTime）
        /// </summary>
        protected static void AssertUpdatedAudit<T>(T entity, string expectedUser)
            where T : IBasePoco
        {
            Assert.AreEqual(expectedUser, entity.UpdateBy, "UpdateBy 不匹配");
            AssertRecent(entity.UpdateTime);
        }

        /// <summary>
        /// 断言集合包含指定数量的元素
        /// </summary>
        protected static void AssertCount<T>(System.Collections.Generic.IEnumerable<T> collection, int expected)
        {
            Assert.AreEqual(expected, System.Linq.Enumerable.Count(collection));
        }

        #endregion
    }
}
