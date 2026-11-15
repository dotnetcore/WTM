using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    /// <summary>
    /// BaseCRUDVM 测试 — TopBasePoco 模型变体（无审计字段 CreateBy/UpdateBy/CreateTime/UpdateTime）
    /// </summary>
    [TestClass]
    [TestCategory("CRUD")]
    public class BaseCRUDVMTestTop : CrudTestBase
    {
        private BaseCRUDVM<SchoolTop> _schoolvm;
        private BaseCRUDVM<MajorTop> _majorvm;
        private BaseCRUDVM<StudentTop> _studentvm;

        public BaseCRUDVMTestTop()
        {
            InitSeed();
            _schoolvm = new BaseCRUDVM<SchoolTop> { Wtm = NewWtmContext("schooluser") };
            _majorvm = new BaseCRUDVM<MajorTop> { Wtm = NewWtmContext("majoruser") };
            _studentvm = new BaseCRUDVM<StudentTop> { Wtm = NewWtmContext("studentuser") };
        }

        #region 单表操作

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new SchoolTop { SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            var rv = ctx.Set<SchoolTop>().Single();
            Assert.AreEqual(code, rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(schooltype, rv.SchoolType);
            Assert.AreEqual(remark, rv.Remark);
            AssertNoModelError(_schoolvm);
        }

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEdit(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new SchoolTop { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolTop { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.DoEdit(true);
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolTop>().Single();
            Assert.AreEqual(code, rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(schooltype, rv.SchoolType);
            Assert.AreEqual(remark, rv.Remark);
        }

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new SchoolTop { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolTop { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolTop>().Single();
            Assert.AreEqual("000", rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(SchoolTypeEnum.PRI, rv.SchoolType);
            Assert.AreEqual("default", rv.Remark);
        }

        [TestMethod]
        public void SingleTableDelete()
        {
            var s = new SchoolTop { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PUB, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<SchoolTop>(), 1);
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolTop { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolTop>(), 0);
        }

        #endregion

        #region Persist 物理删除

        [TestMethod]
        public void SingleTablePersistRealDelete()
        {
            var s = new StudentTop { LoginName = "a", Password = "p", Name = "ab" };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<StudentTop>(), 1);
            _studentvm.DC = ctx;
            _studentvm.Entity = new StudentTop { ID = s.ID };
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentTop>(), 0);
        }

        #endregion

        #region 一对多操作

        [TestMethod]
        public void One2ManyDoAdd()
        {
            var s = new SchoolTop
            {
                SchoolCode = "000", SchoolName = "school", SchoolType = SchoolTypeEnum.PRI, Remark = "",
                Majors = new List<MajorTop>
                {
                    new MajorTop { MajorCode = "111", MajorName = "major1", MajorType = MajorTypeEnum.Optional },
                    new MajorTop { MajorCode = "222", MajorName = "major2", MajorType = MajorTypeEnum.Required }
                }
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<SchoolTop>(), 1);
            AssertCount(ctx.Set<MajorTop>(), 2);
            var majors = ctx.Set<MajorTop>().ToList();
            Assert.AreEqual("111", majors[0].MajorCode);
            Assert.AreEqual("major1", majors[0].MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, majors[0].MajorType);
            Assert.AreEqual("222", majors[1].MajorCode);
            Assert.AreEqual("major2", majors[1].MajorName);
            Assert.AreEqual(MajorTypeEnum.Required, majors[1].MajorType);
        }

        [TestMethod]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<SchoolTop>().Select(x => x.ID).First();
            var majorId = ctx.Set<MajorTop>().Select(x => x.ID).First();
            var s = new SchoolTop { ID = schoolId };
            s.Majors = new List<MajorTop>
            {
                new MajorTop { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorTop { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.DoEdit(true);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolTop>(), 1);
            AssertCount(verifyCtx.Set<MajorTop>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<MajorTop>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.IsNull(updated.MajorName);
            Assert.IsNull(updated.MajorType);

            var created = verifyCtx.Set<MajorTop>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);
        }

        [TestMethod]
        public void One2ManyDoEditFields()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<SchoolTop>().Select(x => x.ID).First();
            var majorId = ctx.Set<MajorTop>().Select(x => x.ID).First();
            var s = new SchoolTop { ID = schoolId };
            s.Majors = new List<MajorTop>
            {
                new MajorTop { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorTop { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC = new Dictionary<string, object> { { "Entity.Majors[0].MajorCode", null } };
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolTop>(), 1);
            AssertCount(verifyCtx.Set<MajorTop>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<MajorTop>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.AreEqual("major1", updated.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, updated.MajorType);

            var created = verifyCtx.Set<MajorTop>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);
        }

        [TestMethod]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<SchoolTop>().AsNoTracking().First().ID;
            _schoolvm.DC = ctx;
            _schoolvm.Entity = new SchoolTop { ID = id };
            _schoolvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolTop>(), 0);
        }

        #endregion

        #region 多对多操作

        [TestMethod]
        public void Many2ManyDoAdd()
        {
            var m1 = AddMajorTop("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorTop("222", "major2", MajorTypeEnum.Required);

            var s1 = new StudentTop { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorTop> { new StudentMajorTop { MajorId = m1.ID } } };
            var s2 = new StudentTop { LoginName = "s2", Password = "p", Name = "student2",
                StudentMajor = new List<StudentMajorTop> { new StudentMajorTop { MajorId = m2.ID } } };

            _studentvm.Entity = s1; _studentvm.DoAdd();
            _studentvm.Entity = s2; _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<MajorTop>(), 2);
            AssertCount(ctx.Set<StudentTop>(), 2);
            var sm = ctx.Set<StudentMajorTop>().ToList();
            AssertCount(sm, 2);
            Assert.AreEqual(s1.ID, sm[0].StudentId);
            Assert.AreEqual(m1.ID, sm[0].MajorId);
            Assert.AreEqual(s2.ID, sm[1].StudentId);
            Assert.AreEqual(m2.ID, sm[1].MajorId);
        }

        [TestMethod]
        public void Many2ManyDoEdit()
        {
            var m1 = AddMajorTop("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorTop("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorTop("333", "major3", MajorTypeEnum.Required);

            var s1 = new StudentTop { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorTop> { new StudentMajorTop { MajorId = m1.ID }, new StudentMajorTop { MajorId = m2.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using var ctx = NewContext();
            s1.StudentMajor.RemoveAt(0);
            s1.StudentMajor.Add(new StudentMajorTop { MajorId = m3.ID });
            _studentvm.DC = ctx;
            _studentvm.Entity = s1;
            _studentvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<MajorTop>(), 3);
            AssertCount(verifyCtx.Set<StudentTop>(), 1);
            AssertCount(verifyCtx.Set<StudentMajorTop>(), 2);
            Assert.IsNotNull(verifyCtx.Set<StudentMajorTop>().SingleOrDefault(x => x.MajorId == m2.ID));
            Assert.IsNotNull(verifyCtx.Set<StudentMajorTop>().SingleOrDefault(x => x.MajorId == m3.ID));
        }

        [TestMethod]
        public void Many2ManyDoEditClearRelation()
        {
            var m1 = AddMajorTop("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorTop("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorTop("333", "major3", MajorTypeEnum.Required);

            var s1 = new StudentTop { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorTop> { new StudentMajorTop { MajorId = m1.ID }, new StudentMajorTop { MajorId = m2.ID }, new StudentMajorTop { MajorId = m3.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                s1.StudentMajor = null;
                _studentvm.DC = ctx; _studentvm.Entity = s1; _studentvm.DoEdit();
            }
            using (var ctx = NewContext())
                AssertCount(ctx.Set<StudentMajorTop>(), 3);

            using (var ctx = NewContext())
            {
                s1.StudentMajor = new List<StudentMajorTop>();
                _studentvm.DC = ctx; _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object> { { "Entity.StudentMajor", null } };
                _studentvm.DoEdit();
            }
            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentMajorTop>(), 0);
        }

        [TestMethod]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<StudentTop>().AsNoTracking().First().ID;
            _studentvm.DC = ctx;
            _studentvm.Entity = new StudentTop { ID = id };
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentTop>(), 1);
            // TODO: 验证中间表 StudentMajor 记录是否被级联删除（当前 DoRealDelete 行为待确认）
        }

        #endregion

        #region 读取与唯一性

        [TestMethod]
        public void GetById()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School);
            var id = GetFirstId<MajorTop>();
            _majorvm.SetEntityById(id);
            Assert.AreNotEqual(Guid.Empty, _majorvm.Entity.School.ID);
        }

        [TestMethod]
        public void DuplicateTest_SingleField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorTop>().Add(new MajorTop { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM1 { Wtm = NewWtmContext(), Entity = new MajorTop { MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
        }

        [TestMethod]
        public void DuplicateTest_MultiField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorTop>().Add(new MajorTop { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM2 { Wtm = NewWtmContext(), Entity = new MajorTop { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
            AssertModelError(_majorvm, "Entity.MajorName");
        }

        [TestMethod]
        public void DuplicateTest_Composite()
        {
            var s = new SchoolTop { SchoolCode = "s1", SchoolName = "s1", Remark = "r", SchoolType = SchoolTypeEnum.PRI };
            using (var ctx = NewContext())
            {
                ctx.Set<SchoolTop>().Add(s); ctx.SaveChanges();
                ctx.Set<OptMajor>().Add(new OptMajor { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional, SchoolId = s.ID });
                ctx.SaveChanges();
            }

            var vm = new MajorVM3 { Wtm = NewWtmContext() };

            vm.Entity = new OptMajor { MajorCode = "111", MajorName = "not222", SchoolId = null, MajorType = MajorTypeEnum.Required };
            vm.Validate(); Assert.IsFalse(vm.MSD.Keys.Any());

            vm.MSD.Clear();
            vm.Entity = new OptMajor { MajorCode = "222", MajorName = "222", SchoolId = s.ID, MajorType = MajorTypeEnum.Required };
            vm.Validate(); Assert.IsFalse(vm.MSD.Keys.Any());

            vm.MSD.Clear();
            vm.Entity = new OptMajor { MajorCode = "111", MajorName = "222", SchoolId = s.ID, MajorType = MajorTypeEnum.Required };
            vm.Validate(); Assert.IsTrue(vm.MSD.Keys.Any());
        }

        #endregion

        #region Helpers

        private MajorTop AddMajorTop(string code, string name, MajorTypeEnum type)
        {
            var m = new MajorTop { MajorCode = code, MajorName = name, MajorType = type };
            _majorvm.Entity = m;
            _majorvm.DoAdd();
            return m;
        }

        private Guid GetFirstId<T>() where T : TopBasePoco
        {
            using var ctx = NewContext();
            return (Guid)ctx.Set<T>().Select(x => x.ID).First();
        }

        #endregion

        #region 内部 VM 子类

        class MajorVM1 : BaseCRUDVM<MajorTop>
        {
            public override DuplicatedInfo<MajorTop> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode));
        }
        class MajorVM2 : BaseCRUDVM<MajorTop>
        {
            public override DuplicatedInfo<MajorTop> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x => x.MajorName));
        }
        class MajorVM3 : BaseCRUDVM<OptMajor>
        {
            public override DuplicatedInfo<OptMajor> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode), SimpleField(x => x.SchoolId));
        }

        #endregion
    }
}
