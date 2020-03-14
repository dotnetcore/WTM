using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseVMTest
    {
        private BaseVM _vm;

        public BaseVMTest()
        {
            _vm = new BaseVM();
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(GlobalData))).Returns(new GlobalData());
            mockService.Setup(x => x.GetService(typeof(Configs))).Returns(new Configs());
            GlobalServices.SetServiceProvider(mockService.Object);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("1","1")]
        [DataRow("1,2,3","3")]
        [DataRow("1,2,3,4,5","5")]
        public void GetCurrentWindowId(string windowids, string expectedValue)
        {
            _vm.WindowIds = windowids;
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
            _vm.WindowIds = windowids;
            Assert.AreEqual(_vm.ParentWindowId, expectedValue);
        }

        [TestMethod]
        public void CopyContext()
        {
            BaseVM newvm = new BaseVM();
            newvm.DC = new DataContext("CopyContext", DBTypeEnum.Memory);
            newvm.FC = new Dictionary<string, object>();
            newvm.CurrentCS = "testcs";
            newvm.CreatorAssembly = "testassembly";
            newvm.MSD = new Mock<IModelStateService>().Object;
            newvm.Session = new Mock<ISessionService>().Object;
            newvm.ConfigInfo = new Configs();
            newvm.DataContextCI = newvm.DC.GetType().GetConstructors()[0];
            newvm.UIService = new Mock<IUIService>().Object;
            _vm.CopyContext(newvm);
            Assert.AreSame(_vm.DC, newvm.DC);
            Assert.AreSame(_vm.FC, newvm.FC);
            Assert.AreSame(_vm.CurrentCS, newvm.CurrentCS);
            Assert.AreSame(_vm.CreatorAssembly, newvm.CreatorAssembly);
            Assert.AreSame(_vm.MSD, newvm.MSD);
            Assert.AreSame(_vm.Session, newvm.Session);
            Assert.AreSame(_vm.ConfigInfo, newvm.ConfigInfo);
            Assert.AreSame(_vm.DataContextCI, newvm.DataContextCI);
            Assert.AreSame(_vm.UIService, newvm.UIService);
        }
    }
}
