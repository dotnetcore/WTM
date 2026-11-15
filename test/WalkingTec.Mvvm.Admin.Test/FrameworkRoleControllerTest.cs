using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkRoleControllerTest
    {
        private FrameworkRoleController _controller;
        private string _seed;
        public FrameworkRoleControllerTest() { _seed = Guid.NewGuid().ToString(); _controller = MockController.CreateApi<FrameworkRoleController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user"); }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new FrameworkRoleSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }
        [TestMethod] public void ExportTest() { Assert.IsNotNull(_controller.ExportExcel(new FrameworkRoleSearcher())); }

        [TestMethod]
        public void CreateTest_AllFields()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkRoleVM>();
            vm.Entity = new FrameworkRole { RoleCode = "001", RoleName = "管理员", RoleRemark = "系统管理员角色" };
            Assert.IsInstanceOfType(_controller.Add(vm), typeof(OkObjectResult));

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var r = c.Set<FrameworkRole>().First();
            Assert.AreEqual("001", r.RoleCode);
            Assert.AreEqual("管理员", r.RoleName);
            Assert.AreEqual("系统管理员角色", r.RoleRemark);
        }

        [TestMethod]
        public void CreateTest_MultipleRoles()
        {
            var vm1 = _controller.Wtm.CreateVM<FrameworkRoleVM>();
            vm1.Entity = new FrameworkRole { RoleCode = "001", RoleName = "role1" };
            _controller.Add(vm1);
            var vm2 = _controller.Wtm.CreateVM<FrameworkRoleVM>();
            vm2.Entity = new FrameworkRole { RoleCode = "002", RoleName = "role2" };
            _controller.Add(vm2);
            var vm3 = _controller.Wtm.CreateVM<FrameworkRoleVM>();
            vm3.Entity = new FrameworkRole { RoleCode = "003", RoleName = "role3" };
            _controller.Add(vm3);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var roles = c.Set<FrameworkRole>().OrderBy(x => x.RoleCode).ToList();
            Assert.AreEqual(3, roles.Count);
            Assert.AreEqual("001", roles[0].RoleCode);
            Assert.AreEqual("002", roles[1].RoleCode);
            Assert.AreEqual("003", roles[2].RoleCode);
        }

        [TestMethod]
        public void EditTest_UpdateAllFields()
        {
            var r = new FrameworkRole { RoleCode = "001", RoleName = "old", RoleRemark = "old remark" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkRole>().Add(r); c.SaveChanges(); }

            var vm = _controller.Wtm.CreateVM<FrameworkRoleVM>();
            vm.Entity = new FrameworkRole { ID = r.ID, RoleCode = "001", RoleName = "新名称", RoleRemark = "新备注" };
            vm.FC.Add("Entity.RoleName", null);
            vm.FC.Add("Entity.RoleRemark", null);
            _controller.Edit(vm);

            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var updated = c2.Set<FrameworkRole>().First();
            Assert.AreEqual("001", updated.RoleCode);
            Assert.AreEqual("新名称", updated.RoleName);
            Assert.AreEqual("新备注", updated.RoleRemark);
        }

        [TestMethod]
        public void GetTest()
        {
            var r = new FrameworkRole { RoleCode = "001", RoleName = "测试角色", RoleRemark = "备注" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkRole>().Add(r); c.SaveChanges(); }
            var vm = _controller.Get(r.ID);
            Assert.AreEqual("001", vm.Entity.RoleCode);
            Assert.AreEqual("测试角色", vm.Entity.RoleName);
            Assert.AreEqual("备注", vm.Entity.RoleRemark);
        }
    }
}
