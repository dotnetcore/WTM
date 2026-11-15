using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkTenantControllerTest
    {
        private FrameworkTenantController _controller;
        private string _seed;
        public FrameworkTenantControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<FrameworkTenantController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
            _controller.Wtm.ConfigInfo.Connections = new List<CS> { new CS { Key = "default", Value = _seed, DbType = DBTypeEnum.Memory } };
        }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new FrameworkTenantSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }
        [TestMethod] public void ExportTest() { Assert.IsNotNull(_controller.ExportExcel(new FrameworkTenantSearcher())); }

        [TestMethod]
        public void GetTest()
        {
            var t = new FrameworkTenant { TCode = "t001", TName = "test" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkTenant>().Add(t); c.SaveChanges(); }
            Assert.AreEqual("test", _controller.Get(t.ID).Entity.TName);
        }
    }
}
