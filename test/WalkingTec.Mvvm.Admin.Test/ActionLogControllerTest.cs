using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.Controllers;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;
using Microsoft.AspNetCore.Hosting;
using WalkingTec.Mvvm.TagHelpers.LayUI.Common;

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
            _controller = MockController.CreateController<ActionLogController>(_seed,"user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco,BaseSearcher>));
        }

        [TestMethod]
        public void DetailsTest()
        {
            ActionLog l = new ActionLog();
            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                context.Set<ActionLog>().Add(l);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(l.ID);
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(l.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.ID);
        }

        [TestMethod]
        public void DetailsFailTest()
        {
            Assert.ThrowsException<Exception>(() => _controller.Details(Guid.Empty), "数据不存在");
        }

    }
}
