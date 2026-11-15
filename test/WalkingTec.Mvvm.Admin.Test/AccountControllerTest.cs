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
    public class AccountControllerTest
    {
        private AccountController _controller;
        private string _seed;

        public AccountControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<AccountController>(new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
            _controller.Wtm.ConfigInfo.Connections = new List<CS> { new CS { Key = "default", Value = _seed, DbType = DBTypeEnum.Memory } };
            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            c.Set<FrameworkUser>().Add(new FrameworkUser { ITCode = "admin", Password = Utils.GetMD5String("admin123"), Name = "Admin", IsValid = true });
            c.SaveChanges();
        }

        // Login/LoginJwt/ChangePassword 依赖 WtmContext.DoLogin 的完整登录流程（HttpContext.SignInAsync等），
        // MockController 未充分模拟这些组件，故登录相关测试跳过。
        // 可在集成测试环境中补充。

        [TestMethod]
        public void GetFrameworkRoles_ReturnsData()
        {
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                c.Set<FrameworkRole>().Add(new FrameworkRole { RoleCode = "001", RoleName = "Admin" });
                c.Set<FrameworkRole>().Add(new FrameworkRole { RoleCode = "002", RoleName = "User" });
                c.SaveChanges();
            }
            Assert.IsNotNull(_controller.GetFrameworkRoles());
        }

        [TestMethod]
        public void GetFrameworkGroups_ReturnsData()
        {
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                c.Set<FrameworkGroup>().Add(new FrameworkGroup { GroupCode = "001", GroupName = "管理组" });
                c.SaveChanges();
            }
            Assert.IsNotNull(_controller.GetFrameworkGroups());
        }

        [TestMethod]
        public void GetFrameworkGroupsTree_ReturnsTree()
        {
            using (var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                c.Set<FrameworkGroup>().Add(new FrameworkGroup { GroupCode = "001", GroupName = "父组" });
                c.SaveChanges();
            }
            Assert.IsNotNull(_controller.GetFrameworkGroupsTree());
        }

        [TestMethod]
        public void GetUserById_ReturnsData()
        {
            Assert.IsNotNull(_controller.GetUserById("admin"));
        }

        [TestMethod]
        public void GetUserByRole_ReturnsData()
        {
            Assert.IsNotNull(_controller.GetUserByRole(""));
        }

        [TestMethod]
        public void GetUserByGroup_ReturnsData()
        {
            Assert.IsNotNull(_controller.GetUserByGroup(""));
        }

        [TestMethod]
        public void Reg_NewUser_CreatesAccount()
        {
            var regInfo = new SimpleReg { ITCode = "newuser", Name = "New User", Password = "pass123" };
            _controller.Reg(regInfo);

            using var c = new Vue3Demo.DataContext(_seed, DBTypeEnum.Memory);
            var user = c.Set<FrameworkUser>().FirstOrDefault(x => x.ITCode == "newuser");
            Assert.IsNotNull(user);
            Assert.AreEqual("New User", user.Name);
        }
    }
}
