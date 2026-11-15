using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    [TestCategory("Core")]
    public class BaseVMTest
    {
        private BaseVM _vm;

        public BaseVMTest()
        {
            _vm = new BaseVM { Wtm = MockWtmContext.CreateWtmContext() };
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("1", "1")]
        [DataRow("1,2,3", "3")]
        [DataRow("1,2,3,4,5", "5")]
        public void GetCurrentWindowId(string windowids, string expectedValue)
        {
            var windowguid = Guid.NewGuid();
            (_vm.Wtm.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.Wtm.ConfigInfo?.CookiePre}windowguid", windowguid.ToString());
            (_vm.Wtm.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.Wtm.ConfigInfo?.CookiePre}{windowguid}windowids", windowids);
            Assert.AreEqual(expectedValue, _vm.CurrentWindowId);
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("1", "")]
        [DataRow("1,2,3", "2")]
        [DataRow("1,2,3,4,5", "4")]
        public void GetParentWindowId(string windowids, string expectedValue)
        {
            var windowguid = Guid.NewGuid();
            (_vm.Wtm.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.Wtm.ConfigInfo?.CookiePre}windowguid", windowguid.ToString());
            (_vm.Wtm.HttpContext.Request.Cookies as MockCookie).Add($"{_vm.Wtm.ConfigInfo?.CookiePre}{windowguid}windowids", windowids);
            Assert.AreEqual(expectedValue, _vm.ParentWindowId);
        }

        [TestMethod]
        public void CopyContext()
        {
            var newvm = new BaseVM();
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

        [TestMethod]
        public void UniqueId_IsNotEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(_vm.UniqueId));
            Assert.AreEqual(32, _vm.UniqueId.Length);
        }

        [TestMethod]
        public void UniqueId_IsStable()
        {
            var id1 = _vm.UniqueId;
            var id2 = _vm.UniqueId;
            Assert.AreEqual(id1, id2);
        }

        [TestMethod]
        public void DoInit_InvokesOnAfterInit()
        {
            var called = false;
            _vm.OnAfterInit += (vm) => called = true;
            _vm.DoInit();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void DoReInit_InvokesOnAfterReInit()
        {
            var called = false;
            _vm.OnAfterReInit += (vm) => called = true;
            _vm.DoReInit();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void ViewDivId_DefaultsToViewDivPlusUniqueId()
        {
            var id = _vm.ViewDivId;
            Assert.IsTrue(id.StartsWith("ViewDiv"));
            Assert.IsTrue(id.EndsWith(_vm.UniqueId));
        }

        [TestMethod]
        public void InitVM_Override_IsCalledDuringDoInit()
        {
            var vm = new BaseVMWithInitOverride { Wtm = MockWtmContext.CreateWtmContext() };
            vm.DoInit();
            Assert.IsTrue(((BaseVMWithInitOverride)vm).InitCalled, "DoInit 应调用 InitVM 覆写方法");
        }

        [TestMethod]
        public void ReInitVM_Override_IsCalledDuringDoReInit()
        {
            var vm = new BaseVMWithInitOverride { Wtm = MockWtmContext.CreateWtmContext() };
            vm.DoReInit();
            Assert.IsTrue(((BaseVMWithInitOverride)vm).ReInitCalled, "DoReInit 应调用 ReInitVM 覆写方法");
        }
    }

    /// <summary>
    /// 用于测试 InitVM/ReInitVM 覆写的 VM 子类
    /// </summary>
    public class BaseVMWithInitOverride : BaseVM
    {
        public bool InitCalled { get; set; }
        public bool ReInitCalled { get; set; }

        protected override void InitVM() { InitCalled = true; }
        protected override void ReInitVM() { ReInitCalled = true; }
    }
}
