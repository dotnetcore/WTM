using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkGroupControllerTest
    {
        private FrameworkGroupController _controller;
        private string _seed;
        public FrameworkGroupControllerTest() { _seed = Guid.NewGuid().ToString(); _controller = MockController.CreateApi<FrameworkGroupController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user"); }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new FrameworkGroupSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }
        [TestMethod] public void ExportTest() { Assert.IsNotNull(_controller.ExportExcel(new FrameworkGroupSearcher())); }

        [TestMethod]
        public void CreateTest_AllFields()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkGroupVM>();
            vm.Entity = new FrameworkGroup { GroupCode = "001", GroupName = "管理组", GroupRemark = "管理用户组", Manager = "admin" };
            Assert.IsInstanceOfType(_controller.Add(vm), typeof(OkObjectResult));

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var g = c.Set<FrameworkGroup>().First();
            Assert.AreEqual("001", g.GroupCode);
            Assert.AreEqual("管理组", g.GroupName);
            Assert.AreEqual("管理用户组", g.GroupRemark);
            Assert.AreEqual("admin", g.Manager);
        }

        [TestMethod]
        public void CreateTest_TreeStructure()
        {
            // 创建父组
            var parent = _controller.Wtm.CreateVM<FrameworkGroupVM>();
            parent.Entity = new FrameworkGroup { GroupCode = "001", GroupName = "父组" };
            _controller.Add(parent);

            // 创建子组（关联父组）
            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var parentId = c.Set<FrameworkGroup>().First().ID;

            var child = _controller.Wtm.CreateVM<FrameworkGroupVM>();
            child.Entity = new FrameworkGroup { GroupCode = "002", GroupName = "子组", ParentId = parentId };
            _controller.Add(child);

            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var groups = c2.Set<FrameworkGroup>().OrderBy(x => x.GroupCode).ToList();
            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual("001", groups[0].GroupCode);
            Assert.IsNull(groups[0].ParentId);
            Assert.AreEqual("002", groups[1].GroupCode);
            Assert.AreEqual(parentId, groups[1].ParentId);
        }

        [TestMethod]
        public void EditTest_UpdateAllFields()
        {
            var g = new FrameworkGroup { GroupCode = "001", GroupName = "oldName", GroupRemark = "old", Manager = "oldMgr" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkGroup>().Add(g); c.SaveChanges(); }

            var vm = _controller.Wtm.CreateVM<FrameworkGroupVM>();
            vm.Entity = new FrameworkGroup { ID = g.ID, GroupCode = "001", GroupName = "newName", GroupRemark = "newRemark", Manager = "newMgr" };
            vm.FC = new Dictionary<string, object> { { "Entity.GroupName", null }, { "Entity.GroupRemark", null }, { "Entity.Manager", null } };
            _controller.Edit(vm);

            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var updated = c2.Set<FrameworkGroup>().First();
            Assert.AreEqual("newName", updated.GroupName);
            Assert.AreEqual("newRemark", updated.GroupRemark);
            Assert.AreEqual("newMgr", updated.Manager);
        }

        [TestMethod]
        public void GetTest()
        {
            var g = new FrameworkGroup { GroupCode = "001", GroupName = "test", GroupRemark = "remark", Manager = "mgr" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkGroup>().Add(g); c.SaveChanges(); }
            var vm = _controller.Get(g.ID);
            Assert.AreEqual("001", vm.Entity.GroupCode);
            Assert.AreEqual("test", vm.Entity.GroupName);
            Assert.AreEqual("remark", vm.Entity.GroupRemark);
            Assert.AreEqual("mgr", vm.Entity.Manager);
            Assert.IsNotNull(_controller.GetParents());
        }
    }
}
