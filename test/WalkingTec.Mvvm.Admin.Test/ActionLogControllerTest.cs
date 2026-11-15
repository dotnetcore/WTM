using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class ActionLogControllerTest
    {
        private ActionLogController _controller;
        private string _seed;

        public ActionLogControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<ActionLogController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            var rv = _controller.Search(new ActionLogSearcher());
            Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false);
        }

        [TestMethod]
        public void GetTest()
        {
            var log = new ActionLog();
            using (var ctx = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                ctx.Set<ActionLog>().Add(log);
                ctx.SaveChanges();
            }
            var vm = _controller.Get(log.ID);
            Assert.IsNotNull(vm);
            Assert.AreEqual(log.ID, vm.Entity.ID);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            var log = new ActionLog();
            using (var ctx = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                ctx.Set<ActionLog>().Add(log);
                ctx.SaveChanges();
            }
            var rv = _controller.BatchDelete(new[] { log.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ExportTest()
        {
            var rv = _controller.ExportExcel(new ActionLogSearcher());
            Assert.IsNotNull(rv);
        }
    }
}
