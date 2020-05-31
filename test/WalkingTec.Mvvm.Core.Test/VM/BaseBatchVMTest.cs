using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BaseBatchVMTest
    {
        private BaseBatchVM<School, SchoolEdit> _schoolvm = new BaseBatchVM<School, SchoolEdit>();
        private BaseBatchVM<Major, MajorEdit> _majorvm = new BaseBatchVM<Major, MajorEdit>();
        private string _seed;

        public BaseBatchVMTest()
        {
            _seed = Guid.NewGuid().ToString();
            _schoolvm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            _majorvm.DC = new DataContext(_seed, DBTypeEnum.Memory);

            _schoolvm.Session = new MockSession();
            _majorvm.Session = new MockSession();

            _schoolvm.MSD = new MockMSD();
            _majorvm.MSD = new MockMSD();

            _schoolvm.LoginUserInfo = new LoginUserInfo { ITCode = "schooluser" };
            _majorvm.LoginUserInfo = new LoginUserInfo { ITCode = "majoruser" };

            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(GlobalData))).Returns(new GlobalData());
            mockService.Setup(x => x.GetService(typeof(Configs))).Returns(new Configs());
            GlobalServices.SetServiceProvider(mockService.Object);

        }

        [TestMethod]
        [Description("单表修改指定字段")]
        [DataTestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "newremark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "newremark2")]
        [DataRow("", "", null, "")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum? schooltype, string remark)
        {
            InitData();

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                SchoolEdit s2 = new SchoolEdit();
                s2.SchoolCode = code;
                s2.SchoolName = name;
                s2.SchoolType = schooltype;
                s2.Remark = remark;
                _schoolvm.DC = context;
                _schoolvm.LinkedVM = s2;
                _schoolvm.FC.Add("LinkedVM.SchoolCode", 0);
                _schoolvm.FC.Add("LinkedVM.SchoolName", 0);
                _schoolvm.FC.Add("LinkedVM.SchoolType", 0);
                _schoolvm.FC.Add("LinkedVM.Remark", 0);
                _schoolvm.Ids = new string[2] { "3E32E2A6-3B09-43CF-8E49-9DE26DCC30AE", "2BBE998F-D639-483E-AC0C-8FC7C18A77A3" };
                _schoolvm.DoBatchEdit();
            }

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var rv = context.Set<School>().ToList();
                Assert.AreEqual(2, rv.Count());
                Assert.AreEqual(code, rv[0].SchoolCode);
                Assert.AreEqual(name, rv[0].SchoolName);
                Assert.AreEqual(schooltype, rv[0].SchoolType);
                Assert.AreEqual(remark, rv[0].Remark);
                Assert.AreEqual(code, rv[1].SchoolCode);
                Assert.AreEqual(name, rv[1].SchoolName);
                Assert.AreEqual(schooltype, rv[1].SchoolType);
                Assert.AreEqual(remark, rv[1].Remark);
            }
        }

        public void InitData()
        {
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                School s2 = new School();
                context.Set<School>().Add(new School
                {
                    ID = new Guid("3E32E2A6-3B09-43CF-8E49-9DE26DCC30AE"),
                    SchoolCode = "001",
                    SchoolName = "school1",
                    SchoolType = null,
                    Remark = "remark1"
                });
                context.Set<School>().Add(new School
                {
                    ID = new Guid("2BBE998F-D639-483E-AC0C-8FC7C18A77A3"),
                    SchoolCode = "002",
                    SchoolName = "school2",
                    SchoolType = null,
                    Remark = "remark2"
                });
                context.SaveChanges();
            }

        }
    }

    public class SchoolEdit:BaseVM
    {
        public string SchoolCode { get; set; }

        public string SchoolName { get; set; }

        public SchoolTypeEnum? SchoolType { get; set; }

        public string Remark { get; set; }

    }
    public class MajorEdit : BaseVM
    {

    }

}
