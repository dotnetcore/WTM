using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkMenuControllerTest
    {
        private FrameworkMenuController _controller;
        private string _seed;
        public FrameworkMenuControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<FrameworkMenuController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
            _controller.Wtm.ConfigInfo.Connections = new List<CS> { new CS { Key = "default", Value = _seed, DbType = DBTypeEnum.Memory } };
        }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new BaseSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }
        [TestMethod] public void ExportTest() { Assert.IsNotNull(_controller.ExportExcel(new BaseSearcher())); }

        [TestMethod]
        public void CreateTest_AllFields()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkMenuVM2>();
            vm.Entity = new FrameworkMenu
            {
                PageName = "用户管理",
                ActionName = "User",
                ModuleName = "Admin",
                FolderOnly = false,
                IsInherit = true,
                ShowOnMenu = true,
                IsPublic = false,
                DisplayOrder = 1,
                IsInside = true,
                TenantAllowed = true,
                Url = "/admin/user",
                Icon = "icon-user"
            };
            Assert.IsInstanceOfType(_controller.Add(vm), typeof(OkObjectResult));

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var m = c.Set<FrameworkMenu>().First();
            Assert.AreEqual("用户管理", m.PageName);
            Assert.AreEqual("User", m.ActionName);
            Assert.AreEqual("Admin", m.ModuleName);
            Assert.IsFalse(m.FolderOnly);
            Assert.IsTrue(m.IsInherit);
            Assert.IsTrue(m.ShowOnMenu);
            Assert.IsFalse(m.IsPublic);
            Assert.AreEqual(1, m.DisplayOrder);
            Assert.IsTrue(m.IsInside.Value);
            Assert.IsTrue(m.TenantAllowed.Value);
            Assert.AreEqual("icon-user", m.Icon);
        }

        [TestMethod]
        public void CreateTest_TreeStructure()
        {
            // 创建顶层菜单
            var parent = _controller.Wtm.CreateVM<FrameworkMenuVM2>();
            parent.Entity = new FrameworkMenu { PageName = "系统管理", FolderOnly = true, ShowOnMenu = true, IsPublic = false, DisplayOrder = 1, IsInside = true };
            _controller.Add(parent);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var parentId = c.Set<FrameworkMenu>().First().ID;

            // 创建子菜单
            var child = _controller.Wtm.CreateVM<FrameworkMenuVM2>();
            child.Entity = new FrameworkMenu { PageName = "用户管理", ParentId = parentId, ActionName = "User", FolderOnly = false, ShowOnMenu = true, IsPublic = false, DisplayOrder = 1, IsInside = true };
            _controller.Add(child);

            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var menus = c2.Set<FrameworkMenu>().OrderBy(x => x.DisplayOrder).ToList();
            Assert.AreEqual(2, menus.Count);
            Assert.AreEqual("系统管理", menus[0].PageName);
            Assert.IsNull(menus[0].ParentId);
            Assert.AreEqual("用户管理", menus[1].PageName);
            Assert.AreEqual(parentId, menus[1].ParentId);
        }

        [TestMethod]
        public void CreateTest_FolderOnlyMenu()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkMenuVM2>();
            vm.Entity = new FrameworkMenu
            {
                PageName = "目录菜单",
                FolderOnly = true,
                ShowOnMenu = true,
                IsPublic = true,
                DisplayOrder = 99,
                IsInside = true,
                Icon = "icon-folder"
            };
            _controller.Add(vm);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var m = c.Set<FrameworkMenu>().First();
            Assert.IsTrue(m.FolderOnly);
            Assert.IsTrue(m.IsPublic);
            Assert.AreEqual("icon-folder", m.Icon);
        }

        [TestMethod]
        public void GetTest()
        {
            var m = new FrameworkMenu { PageName = "测试菜单", ActionName = "test", ModuleName = "testModule", FolderOnly = false, ShowOnMenu = true, IsPublic = false, DisplayOrder = 5, IsInside = true, Url = "/test", Icon = "icon-test" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkMenu>().Add(m); c.SaveChanges(); }

            var vm = _controller.Get(m.ID);
            Assert.AreEqual("测试菜单", vm.Entity.PageName);
            Assert.AreEqual("test", vm.Entity.ActionName);
            Assert.AreEqual("testModule", vm.Entity.ModuleName);
            Assert.AreEqual("/test", vm.Entity.Url);
            Assert.AreEqual("icon-test", vm.Entity.Icon);
            Assert.IsFalse(vm.Entity.FolderOnly);
            Assert.IsTrue(vm.Entity.ShowOnMenu);
            Assert.AreEqual(5, vm.Entity.DisplayOrder);
        }
    }
}
