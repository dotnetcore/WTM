using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseVMTest
    {
        private BaseVM _vm;

        public BaseVMTest()
        {
            _vm = new BaseVM();
            _vm.WtmContext = MockWtmContext.CreateWtmContext();
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("1","1")]
        [DataRow("1,2,3","3")]
        [DataRow("1,2,3,4,5","5")]
        public void GetCurrentWindowId(string windowids, string expectedValue)
        {
            Guid windowguid = Guid.NewGuid();
            (_vm.WtmContext.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.WtmContext.ConfigInfo?.CookiePre}windowguid", windowguid.ToString());
            (_vm.WtmContext.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.WtmContext.ConfigInfo?.CookiePre}{windowguid}windowids", windowids);
            Assert.AreEqual(_vm.CurrentWindowId, expectedValue);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("1", "")]
        [DataRow("1,2,3", "2")]
        [DataRow("1,2,3,4,5", "4")]
        public void GetParentWindowId(string windowids, string expectedValue)
        {
            Guid windowguid = Guid.NewGuid();
            (_vm.WtmContext.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.WtmContext.ConfigInfo?.CookiePre}windowguid", windowguid.ToString());
            (_vm.WtmContext.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.WtmContext.ConfigInfo?.CookiePre}{windowguid}windowids", windowids);
            Assert.AreEqual(_vm.ParentWindowId, expectedValue);
        }

        [TestMethod]
        public void CopyContext()
        {
            BaseVM newvm = new BaseVM();
            newvm.CopyContext(_vm);
            Assert.AreSame(_vm.DC, newvm.DC);
            Assert.AreSame(_vm.FC, newvm.FC);
            Assert.AreSame(_vm.CurrentCS, newvm.CurrentCS);
            Assert.AreSame(_vm.CreatorAssembly, newvm.CreatorAssembly);
            Assert.AreSame(_vm.MSD, newvm.MSD);
            Assert.AreSame(_vm.Session, newvm.Session);
            Assert.AreSame(_vm.ConfigInfo, newvm.ConfigInfo);
            Assert.AreSame(_vm.UIService, newvm.UIService);
        }
    }
}
