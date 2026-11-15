using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class DataPrivilegeControllerTest
    {
        private DataPrivilegeController _controller;
        private string _seed;

        public DataPrivilegeControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<DataPrivilegeController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
            _controller.Wtm.ConfigInfo.Connections = new List<CS> { new CS { Key = "default", Value = _seed, DbType = DBTypeEnum.Memory } };
        }

        [TestMethod] public void SearchTest() { var rv = _controller.Search(new DataPrivilegeSearcher()); Assert.IsTrue(string.IsNullOrEmpty((rv as ContentResult)?.Content) == false); }

        [TestMethod]
        public void Get_UserType_ReturnsVM()
        {
            var vm = _controller.Get("FrameworkUser", "user001", DpTypeEnum.User);
            Assert.IsNotNull(vm);
            Assert.AreEqual(DpTypeEnum.User, vm.DpType);
            Assert.AreEqual("FrameworkUser", vm.Entity.TableName);
            Assert.AreEqual("user001", vm.Entity.UserCode);
        }

        [TestMethod]
        public void Get_GroupType_ReturnsVM()
        {
            var vm = _controller.Get("FrameworkUser", "group001", DpTypeEnum.UserGroup);
            Assert.IsNotNull(vm);
            Assert.AreEqual(DpTypeEnum.UserGroup, vm.DpType);
            Assert.AreEqual("FrameworkUser", vm.Entity.TableName);
            Assert.AreEqual("group001", vm.Entity.GroupCode);
        }

        [TestMethod]
        public void CreateTest_SavesToDb()
        {
            var vm = _controller.Wtm.CreateVM<DataPrivilegeVM>();
            vm.Entity = new DataPrivilege { TableName = "School", UserCode = "u001" };
            vm.DpType = DpTypeEnum.User;
            vm.SelectedItemsID = new List<string>();

            var rv = _controller.Add(vm).Result;
            Assert.IsNotNull(rv);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            Assert.AreEqual(1, c.Set<DataPrivilege>().Count());
        }

        [TestMethod]
        public void DeleteTest_RemovesFromDb()
        {
            var dp = new DataPrivilege { TableName = "School", UserCode = "u001" };
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory)) { c.Set<DataPrivilege>().Add(dp); c.SaveChanges(); }

            var deleteModel = new SimpleDpModel { ModelName = "School", Id = "u001", Type = DpTypeEnum.User };
            var rv = _controller.Delete(deleteModel).Result;
            Assert.IsNotNull(rv);

            using var c2 = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            Assert.AreEqual(0, c2.Set<DataPrivilege>().Count());
        }
    }
}
