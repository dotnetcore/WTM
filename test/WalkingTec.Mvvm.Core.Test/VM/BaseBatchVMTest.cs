using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    [TestCategory("Batch")]
    public class BaseBatchVMTest : CrudTestBase
    {
        private BaseBatchVM<School, SchoolEdit> _schoolvm;

        private static readonly Guid Id1 = Guid.Parse("3E32E2A6-3B09-43CF-8E49-9DE26DCC30AE");
        private static readonly Guid Id2 = Guid.Parse("2BBE998F-D639-483E-AC0C-8FC7C18A77A3");

        public BaseBatchVMTest()
        {
            InitSeed();
            _schoolvm = new BaseBatchVM<School, SchoolEdit> { Wtm = NewWtmContext("schooluser") };
        }

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "newremark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "newremark2")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum? schooltype, string remark)
        {
            InitData();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.LinkedVM = new SchoolEdit { SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.FC.Add("LinkedVM.SchoolCode", 0);
                _schoolvm.FC.Add("LinkedVM.SchoolName", 0);
                _schoolvm.FC.Add("LinkedVM.SchoolType", 0);
                _schoolvm.FC.Add("LinkedVM.Remark", 0);
                _schoolvm.Ids = new[] { Id1.ToString(), Id2.ToString() };
                _schoolvm.DoBatchEdit();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<School>().ToList();
            Assert.AreEqual(2, rv.Count);
            Assert.AreEqual(code, rv[0].SchoolCode);
            Assert.AreEqual(name, rv[0].SchoolName);
            Assert.AreEqual(schooltype, rv[0].SchoolType);
            Assert.AreEqual(remark, rv[0].Remark);
            Assert.AreEqual(code, rv[1].SchoolCode);
            Assert.AreEqual(name, rv[1].SchoolName);
            Assert.AreEqual(schooltype, rv[1].SchoolType);
            Assert.AreEqual(remark, rv[1].Remark);
        }

        [TestMethod]
        public void DoBatchDelete()
        {
            InitData();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<School>(), 2);
                _schoolvm.DC = ctx;
                _schoolvm.Ids = new[] { Id1.ToString(), Id2.ToString() };
                _schoolvm.DoBatchDelete();
            }

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 0);
        }

        [TestMethod]
        public void DoBatchDelete_EmptyIds_DoesNotCrash()
        {
            using var ctx = NewContext();
            _schoolvm.DC = ctx;
            _schoolvm.Ids = new string[0];
            _schoolvm.DoBatchDelete();
            // 空 ID 数组不应抛异常
        }

        [TestMethod]
        public void DoBatchEdit_EmptyIds_SkipsGracefully()
        {
            InitData();
            using var ctx = NewContext();
            _schoolvm.DC = ctx;
            _schoolvm.LinkedVM = new SchoolEdit { SchoolCode = "xxx" };
            _schoolvm.Ids = new string[0];
            _schoolvm.DoBatchEdit();
            // 空 ID 数组不应修改任何记录
            var schools = ctx.Set<School>().ToList();
            Assert.AreEqual(2, schools.Count);
            Assert.AreEqual("001", schools[0].SchoolCode); // unchanged
        }

        private void InitData()
        {
            using var ctx = NewContext();
            ctx.Set<School>().Add(new School { ID = Id1, SchoolCode = "001", SchoolName = "school1", SchoolType = SchoolTypeEnum.PRI, Remark = "remark1" });
            ctx.Set<School>().Add(new School { ID = Id2, SchoolCode = "002", SchoolName = "school2", SchoolType = SchoolTypeEnum.PRI, Remark = "remark2" });
            ctx.SaveChanges();
        }
    }

    public class SchoolEdit : BaseVM
    {
        public string SchoolCode { get; set; }
        public string SchoolName { get; set; }
        public SchoolTypeEnum? SchoolType { get; set; }
        public new string Remark { get; set; }
    }
}
