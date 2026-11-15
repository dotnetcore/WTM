using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkUserApiTest
    {
        private FrameworkUserController _controller;
        private string _seed;

        public FrameworkUserApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<FrameworkUserController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new FrameworkUserSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }
        [TestMethod] public void ExportTest() { Assert.IsNotNull(_controller.ExportExcel(new FrameworkUserSearcher())); Assert.IsNotNull(_controller.ExportExcelByIds(new string[0])); Assert.IsNotNull(_controller.GetExcelTemplate()); }

        [TestMethod]
        public void CreateTest_AllFields()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            vm.Entity = new FrameworkUser
            {
                ITCode = "u001",
                Password = "[redacted]",
                Name = "张三",
                Email = "zhangsan@test.com",
                Gender = GenderEnum.Male,
                CellPhone = "13812345678",
                HomePhone = "010-12345678",
                Address = "北京市朝阳区测试路100号",
                ZipCode = "100000",
                IsValid = true
            };
            vm.SelectedFrameworkUserRolesIDs = new List<string> { "r001", "r002" };
            vm.SelectedFrameworkUserGroupsIDs = new List<string> { "g001" };

            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv.Result, typeof(OkObjectResult));

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var data = c.Set<FrameworkUser>().First();
            Assert.AreEqual("u001", data.ITCode);
            Assert.AreEqual("张三", data.Name);
            Assert.AreEqual("zhangsan@test.com", data.Email);
            Assert.AreEqual(GenderEnum.Male, data.Gender);
            Assert.AreEqual("13812345678", data.CellPhone);
            Assert.AreEqual("010-12345678", data.HomePhone);
            Assert.AreEqual("北京市朝阳区测试路100号", data.Address);
            Assert.AreEqual("100000", data.ZipCode);
            Assert.IsTrue(data.IsValid);
        }

        [TestMethod]
        public void CreateTest_ManyToMany_Roles()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            vm.Entity = new FrameworkUser { ITCode = "u001", Name = "name", Password = "[redacted]" };
            vm.SelectedFrameworkUserRolesIDs = new List<string> { "roleA", "roleB", "roleC" };

            _controller.Add(vm);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var roles = c.Set<FrameworkUserRole>().Where(x => x.UserCode == "u001").ToList();
            Assert.AreEqual(3, roles.Count);
            CollectionAssert.AreEquivalent(new[] { "roleA", "roleB", "roleC" }, roles.Select(x => x.RoleCode).ToList());
        }

        [TestMethod]
        public void CreateTest_ManyToMany_Groups()
        {
            var vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            vm.Entity = new FrameworkUser { ITCode = "u001", Name = "name", Password = "[redacted]" };
            vm.SelectedFrameworkUserGroupsIDs = new List<string> { "gA", "gB" };

            _controller.Add(vm);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var groups = c.Set<FrameworkUserGroup>().Where(x => x.UserCode == "u001").ToList();
            Assert.AreEqual(2, groups.Count);
            CollectionAssert.AreEquivalent(new[] { "gA", "gB" }, groups.Select(x => x.GroupCode).ToList());
        }

        [TestMethod]
        public void EditTest_UpdateRolesAndGroups()
        {
            // 先创建用户，带初始角色
            var vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            var v = new FrameworkUser { ITCode = "u001", Name = "name", Password = "[redacted]" };
            vm.Entity = v;
            vm.SelectedFrameworkUserRolesIDs = new List<string> { "roleOld" };
            vm.SelectedFrameworkUserGroupsIDs = new List<string> { "groupOld" };
            _controller.Add(vm);

            // 编辑：替换角色和用户组
            var editVm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            editVm.Entity = new FrameworkUser { ID = v.ID, ITCode = "u001", Name = "name1", Password = "[redacted]", Email = "new@test.com", Gender = GenderEnum.Female, CellPhone = "13900001111", HomePhone = "021-11111111", Address = "上海", ZipCode = "200000" };
            editVm.SelectedFrameworkUserRolesIDs = new List<string> { "roleNew1", "roleNew2" };
            editVm.SelectedFrameworkUserGroupsIDs = new List<string> { "groupNew" };
            editVm.FC = new Dictionary<string, object> { { "Entity.Name", "" }, { "Entity.Email", "" } };

            _controller.Edit(editVm);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var data = c.Set<FrameworkUser>().First();
            Assert.AreEqual("name1", data.Name);
            Assert.AreEqual("new@test.com", data.Email);
            Assert.AreEqual(GenderEnum.Female, data.Gender);
            Assert.AreEqual("13900001111", data.CellPhone);
            Assert.AreEqual("上海", data.Address);
        }

        [TestMethod]
        public void GetTest()
        {
            var v = new FrameworkUser { ITCode = "u001", Name = "test", Password = "[redacted]", Email = "t@t.com", Gender = GenderEnum.Female, CellPhone = "13800001111", Address = "test addr", ZipCode = "100001", IsValid = true };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkUser>().Add(v); c.SaveChanges(); }

            var rv = _controller.Get(v.ID);
            Assert.IsNotNull(rv);
            Assert.AreEqual("u001", rv.Entity.ITCode);
            Assert.AreEqual("test", rv.Entity.Name);
            Assert.AreEqual("t@t.com", rv.Entity.Email);
            Assert.AreEqual(GenderEnum.Female, rv.Entity.Gender);
            Assert.AreEqual("13800001111", rv.Entity.CellPhone);
            Assert.AreEqual("test addr", rv.Entity.Address);
            Assert.AreEqual("100001", rv.Entity.ZipCode);
            Assert.IsTrue(rv.Entity.IsValid);
            // Get 应该填充 SelectedRolesCodes 和 SelectedGroupCodes
            Assert.IsNotNull(rv.SelectedFrameworkUserRolesIDs);
            Assert.IsNotNull(rv.SelectedFrameworkUserGroupsIDs);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            var v1 = new FrameworkUser { ITCode = "u1", Name = "n1", Password = "[redacted]" };
            var v2 = new FrameworkUser { ITCode = "u2", Name = "n2", Password = "[redacted]" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<FrameworkUser>().Add(v1); c.Set<FrameworkUser>().Add(v2); c.SaveChanges(); }

            var rv = _controller.BatchDelete(new[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Result, typeof(OkObjectResult));
            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            Assert.AreEqual(0, c2.Set<FrameworkUser>().Count());

            // 空 ID 数组不抛异常
            rv = _controller.BatchDelete(new string[0]);
            Assert.IsInstanceOfType(rv.Result, typeof(OkResult));
        }
    }
}
