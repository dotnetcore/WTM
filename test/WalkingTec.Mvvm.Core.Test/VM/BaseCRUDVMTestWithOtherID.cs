using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    /// <summary>
    /// BaseCRUDVM 测试 — 自定义主键类型变体（long ID，string ID）
    /// </summary>
    [TestClass]
    [TestCategory("CRUD")]
    public class BaseCRUDVMTestWithOtherID : CrudTestBase
    {
        private BaseCRUDVM<SchoolWithOtherID> _schoolvm;
        private BaseCRUDVM<MajorWithOtherID> _majorvm;
        private BaseCRUDVM<Student> _studentvm;

        public BaseCRUDVMTestWithOtherID()
        {
            InitSeed();
            _schoolvm = new BaseCRUDVM<SchoolWithOtherID> { Wtm = NewWtmContext("schooluser") };
            _majorvm = new BaseCRUDVM<MajorWithOtherID> { Wtm = NewWtmContext("majoruser") };
            _studentvm = new BaseCRUDVM<Student> { Wtm = NewWtmContext("studentuser") };
        }

        #region 单表操作

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new SchoolWithOtherID { SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            var rv = ctx.Set<SchoolWithOtherID>().Single();
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
            var s = new SchoolWithOtherID { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolWithOtherID { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.DoEdit(true);
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolWithOtherID>().Single();
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
            var s = new SchoolWithOtherID { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolWithOtherID { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolWithOtherID>().Single();
            Assert.AreEqual("000", rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(SchoolTypeEnum.PRI, rv.SchoolType);
            Assert.AreEqual("default", rv.Remark);
        }

        [TestMethod]
        public void SingleTableDelete()
        {
            var s = new SchoolWithOtherID { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PUB, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<SchoolWithOtherID>(), 1);
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolWithOtherID { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolWithOtherID>(), 0);
        }

        #endregion

        #region 一对多操作

        [TestMethod]
        public void One2ManyDoAdd()
        {
            var s = new SchoolWithOtherID
            {
                SchoolCode = "000", SchoolName = "school", SchoolType = SchoolTypeEnum.PRI, Remark = "",
                Majors = new List<MajorWithOtherID>
                {
                    new MajorWithOtherID { ID = "id1", MajorCode = "111", MajorName = "major1", MajorType = MajorTypeEnum.Optional },
                    new MajorWithOtherID { ID = "id2", MajorCode = "222", MajorName = "major2", MajorType = MajorTypeEnum.Required }
                }
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<SchoolWithOtherID>(), 1);
            AssertCount(ctx.Set<MajorWithOtherID>(), 2);
            var majors = ctx.Set<MajorWithOtherID>().ToList();
            Assert.AreEqual("id1", majors[0].ID);
            Assert.AreEqual("111", majors[0].MajorCode);
            Assert.AreEqual("major1", majors[0].MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, majors[0].MajorType);
            Assert.AreEqual("id2", majors[1].ID);
            Assert.AreEqual("222", majors[1].MajorCode);
            Assert.AreEqual("major2", majors[1].MajorName);
            Assert.AreEqual(MajorTypeEnum.Required, majors[1].MajorType);

            AssertCreatedAudit(majors[0], "schooluser");
            AssertCreatedAudit(majors[1], "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<SchoolWithOtherID>().Select(x => x.ID).First();
            var majorId = ctx.Set<MajorWithOtherID>().Select(x => x.ID).First();
            var s = new SchoolWithOtherID { ID = schoolId };
            s.Majors = new List<MajorWithOtherID>
            {
                new MajorWithOtherID { ID = "id3", MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorWithOtherID { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.DoEdit(true);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolWithOtherID>(), 1);
            AssertCount(verifyCtx.Set<MajorWithOtherID>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<MajorWithOtherID>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.IsNull(updated.MajorName);
            Assert.IsNull(updated.MajorType);

            var created = verifyCtx.Set<MajorWithOtherID>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);

            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEditFields()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<SchoolWithOtherID>().Select(x => x.ID).First();
            var majorId = ctx.Set<MajorWithOtherID>().Select(x => x.ID).First();
            var s = new SchoolWithOtherID { ID = schoolId };
            s.Majors = new List<MajorWithOtherID>
            {
                new MajorWithOtherID { ID = "id3", MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorWithOtherID { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC = new Dictionary<string, object> { { "Entity.Majors[0].MajorCode", null } };
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolWithOtherID>(), 1);
            AssertCount(verifyCtx.Set<MajorWithOtherID>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<MajorWithOtherID>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.AreEqual("major1", updated.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, updated.MajorType);

            var created = verifyCtx.Set<MajorWithOtherID>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);

            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        [TestMethod]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<SchoolWithOtherID>().AsNoTracking().First().ID;
            _schoolvm.DC = ctx;
            _schoolvm.Entity = new SchoolWithOtherID { ID = id };
            _schoolvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolWithOtherID>(), 0);
        }

        #endregion

        #region 多对多操作

        [TestMethod]
        public void Many2ManyDoAdd()
        {
            var m1 = AddMajorWithOtherID("id1", "111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorWithOtherID("id2", "222", "major2", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajorWithOtherID = new List<StudentMajorWithOtherID> { new StudentMajorWithOtherID { MajorId = m1.ID } } };
            var s2 = new Student { LoginName = "s2", Password = "p", Name = "student2",
                StudentMajorWithOtherID = new List<StudentMajorWithOtherID> { new StudentMajorWithOtherID { MajorId = m2.ID } } };

            _studentvm.Entity = s1; _studentvm.DoAdd();
            _studentvm.Entity = s2; _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<MajorWithOtherID>(), 2);
            AssertCount(ctx.Set<Student>(), 2);
            var sm = ctx.Set<StudentMajorWithOtherID>().ToList();
            AssertCount(sm, 2);
            Assert.AreEqual(s1.ID, sm[0].StudentId);
            Assert.AreEqual(m1.ID, sm[0].MajorId);
            Assert.AreEqual(s2.ID, sm[1].StudentId);
            Assert.AreEqual(m2.ID, sm[1].MajorId);
        }

        [TestMethod]
        public void Many2ManyDoEdit()
        {
            var m1 = AddMajorWithOtherID("id1", "111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorWithOtherID("id2", "222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorWithOtherID("id3", "333", "major3", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajorWithOtherID = new List<StudentMajorWithOtherID> { new StudentMajorWithOtherID { MajorId = m1.ID }, new StudentMajorWithOtherID { MajorId = m2.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using var ctx = NewContext();
            s1.StudentMajorWithOtherID.RemoveAt(0);
            s1.StudentMajorWithOtherID.Add(new StudentMajorWithOtherID { MajorId = m3.ID });
            _studentvm.DC = ctx;
            _studentvm.Entity = s1;
            _studentvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<MajorWithOtherID>(), 3);
            AssertCount(verifyCtx.Set<Student>(), 1);
            AssertCount(verifyCtx.Set<StudentMajorWithOtherID>(), 2);
            Assert.IsNotNull(verifyCtx.Set<StudentMajorWithOtherID>().SingleOrDefault(x => x.MajorId == m2.ID));
            Assert.IsNotNull(verifyCtx.Set<StudentMajorWithOtherID>().SingleOrDefault(x => x.MajorId == m3.ID));
        }

        [TestMethod]
        public void Many2ManyDoEditClearRelation()
        {
            var m1 = AddMajorWithOtherID("id1", "111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorWithOtherID("id2", "222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorWithOtherID("id3", "333", "major3", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajorWithOtherID = new List<StudentMajorWithOtherID> { new StudentMajorWithOtherID { MajorId = m1.ID }, new StudentMajorWithOtherID { MajorId = m2.ID }, new StudentMajorWithOtherID { MajorId = m3.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                s1.StudentMajorWithOtherID = null;
                _studentvm.DC = ctx; _studentvm.Entity = s1; _studentvm.DoEdit();
            }
            using (var ctx = NewContext())
                AssertCount(ctx.Set<StudentMajorWithOtherID>(), 3);

            using (var ctx = NewContext())
            {
                s1.StudentMajorWithOtherID = new List<StudentMajorWithOtherID>();
                _studentvm.DC = ctx; _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object> { { "Entity.StudentMajorWithOtherID", null } };
                _studentvm.DoEdit();
            }
            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentMajorWithOtherID>(), 0);
        }

        [TestMethod]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<Student>().AsNoTracking().First().ID;
            _studentvm.DC = ctx;
            _studentvm.Entity = new Student { ID = id };
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<Student>(), 1);
            // TODO: 验证中间表 StudentMajorWithOtherID 记录是否被级联删除（当前 DoRealDelete 行为待确认）
        }

        #endregion

        #region 读取与唯一性

        [TestMethod]
        public void GetById()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School);
            string id;
            using (var ctx = NewContext())
                id = ctx.Set<MajorWithOtherID>().Select(x => x.ID).First();
            _majorvm.SetEntityById(id);
            Assert.AreNotEqual(0L, _majorvm.Entity.School.ID);
        }

        [TestMethod]
        public void DuplicateTest_SingleField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorWithOtherID>().Add(new MajorWithOtherID { ID = "id1", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM1WithOtherID { Wtm = NewWtmContext(), Entity = new MajorWithOtherID { ID = "id2", MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
        }

        [TestMethod]
        public void DuplicateTest_MultiField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorWithOtherID>().Add(new MajorWithOtherID { ID = "id1", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM2WithOtherID { Wtm = NewWtmContext(), Entity = new MajorWithOtherID { ID = "id2", MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
            AssertModelError(_majorvm, "Entity.MajorName");
        }

        #endregion

        #region Helpers

        private MajorWithOtherID AddMajorWithOtherID(string id, string code, string name, MajorTypeEnum type)
        {
            var m = new MajorWithOtherID { ID = id, MajorCode = code, MajorName = name, MajorType = type };
            _majorvm.Entity = m;
            _majorvm.DoAdd();
            return m;
        }

        #endregion

        #region 内部 VM 子类

        class MajorVM1WithOtherID : BaseCRUDVM<MajorWithOtherID>
        {
            public override DuplicatedInfo<MajorWithOtherID> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode));
        }
        class MajorVM2WithOtherID : BaseCRUDVM<MajorWithOtherID>
        {
            public override DuplicatedInfo<MajorWithOtherID> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x => x.MajorName));
        }

        #endregion
    }
}
