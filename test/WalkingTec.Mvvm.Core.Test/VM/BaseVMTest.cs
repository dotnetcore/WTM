using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseVMTest
    {
        private BaseVM _vm;

        public BaseVMTest()
        {
            _vm = new BaseVM();
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("test1", ActionLogTypesEnum.Debug)]
        [DataRow("test2", ActionLogTypesEnum.Normal)]
        [DataRow("test3", ActionLogTypesEnum.Exception)]
        public void DoLog(string msg, ActionLogTypesEnum logType)
        {
            _vm.Log = new ActionLog();
            _vm.DC = new DataContext("dologdb"+logType, DBTypeEnum.Memory);
            _vm.DoLog(msg, logType);

            using (var context = new DataContext("dologdb" + logType, DBTypeEnum.Memory))
            {
                var logs = context.Set<ActionLog>().ToList();
                Assert.AreEqual(1, logs.Count());
                Assert.AreEqual(msg, logs[0].Remark);
                Assert.AreEqual(logType, logs[0].LogType);
                Assert.IsTrue(DateTime.Now.Subtract(logs[0].ActionTime).Seconds < 10);
            }

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
