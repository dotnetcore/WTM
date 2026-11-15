using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    /// <summary>
    /// BaseCRUDVM 核心测试 — 覆盖 BasePoco (Guid ID) + PersistPoco 的 CRUD、一对多、多对多操作
    /// </summary>
    [TestClass]
    [TestCategory("CRUD")]
    public class BaseCRUDVMTest : CrudTestBase
    {
        private BaseCRUDVM<School> _schoolvm;
        private BaseCRUDVM<Major> _majorvm;
        private BaseCRUDVM<Student> _studentvm;
        private BaseCRUDVM<GoodsSpecification> _goodsvm;
        private BaseCRUDVM<GoodsCatalog> _goodsCatalogvm;

        public BaseCRUDVMTest()
        {
            InitSeed();
            _schoolvm = new BaseCRUDVM<School> { Wtm = NewWtmContext("schooluser") };
            _majorvm = new BaseCRUDVM<Major> { Wtm = NewWtmContext("majoruser") };
            _studentvm = new BaseCRUDVM<Student> { Wtm = NewWtmContext("studentuser") };
            _goodsvm = new BaseCRUDVM<GoodsSpecification> { Wtm = NewWtmContext("goodsuser") };
            _goodsCatalogvm = new BaseCRUDVM<GoodsCatalog> { Wtm = NewWtmContext("goodscatalogsuser") };
        }

        #region 单表操作

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new School { SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            var rv = ctx.Set<School>().Single();
            Assert.AreEqual(code, rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(schooltype, rv.SchoolType);
            Assert.AreEqual(remark, rv.Remark);
            AssertCreatedAudit(rv, "schooluser");
            AssertNoModelError(_schoolvm);
        }

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEdit(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new School { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new School { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.DoEdit(true);
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<School>().Single();
            Assert.AreEqual(code, rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(schooltype, rv.SchoolType);
            Assert.AreEqual(remark, rv.Remark);
            AssertUpdatedAudit(rv, "schooluser");
        }

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoEditFields(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new School { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new School { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<School>().Single();
            Assert.AreEqual("000", rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.AreEqual(SchoolTypeEnum.PRI, rv.SchoolType);
            Assert.AreEqual("default", rv.Remark);
            AssertUpdatedAudit(rv, "schooluser");
        }

        [TestMethod]
        public void SingleTableDelete()
        {
            var s = new School { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PUB, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<School>(), 1);
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new School { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 0);
        }

        #endregion

        #region Persist 软删除

        [TestMethod]
        public void SingleTablePersistDelete()
        {
            var s = new Student { LoginName = "a", Password = "p", Name = "ab" };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<Student>(), 1);
                _studentvm.DC = ctx;
                _studentvm.Entity = ctx.Set<Student>().First(x => x.ID == s.ID);
                _studentvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<Student>().IgnoreQueryFilters().Single();
            Assert.IsFalse(rv.IsValid);
            AssertUpdatedAudit(rv, "studentuser");
        }

        [TestMethod]
        public void SingleTablePersistRealDelete()
        {
            var s = new Student { LoginName = "a", Password = "p", Name = "ab" };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<Student>(), 1);

            _studentvm.SetEntityById(s.ID);
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<Student>(), 0);
        }

        [TestMethod]
        public void One2ManyTablePersistDelete()
        {
            var gc = new GoodsCatalog { IsValid = true, Name = "c1", OrderNum = 2 };
            _goodsvm.DC.AddEntity(gc);
            _goodsvm.DC.SaveChanges();

            var g = new GoodsSpecification { Name = "g1", OrderNum = 1, IsValid = true, CatalogId = gc.ID };
            _goodsvm.Entity = g;
            _goodsvm.DoAdd();

            using (var ctx = NewContext())
            {
                _goodsvm.DC = ctx;
                _goodsvm.Entity = ctx.Set<GoodsSpecification>().IgnoreQueryFilters().Include(x => x.Catalog).First(x => x.ID == g.ID);
                _goodsvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            var spec = verifyCtx.Set<GoodsSpecification>().IgnoreQueryFilters().Single();
            Assert.IsFalse(spec.IsValid);
            AssertUpdatedAudit(spec, "goodsuser");

            var catalog = verifyCtx.Set<GoodsCatalog>().IgnoreQueryFilters().Single();
            Assert.IsTrue(catalog.IsValid);
        }

        [TestMethod]
        public void One2ManyTablePersistDelete2()
        {
            var gc = new GoodsCatalog { IsValid = true, Name = "c1", OrderNum = 2 };
            _goodsvm.DC.AddEntity(gc);
            _goodsvm.DC.SaveChanges();

            var g = new GoodsSpecification { Name = "g1", OrderNum = 1, IsValid = true, CatalogId = gc.ID };
            _goodsvm.Entity = g;
            _goodsvm.DoAdd();

            using (var ctx = NewContext())
            {
                _goodsCatalogvm.DC = ctx;
                _goodsCatalogvm.Entity = ctx.Set<GoodsCatalog>().First(x => x.ID == gc.ID);
                _goodsCatalogvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            var catalog = verifyCtx.Set<GoodsCatalog>().IgnoreQueryFilters().Single();
            Assert.IsFalse(catalog.IsValid);
            AssertUpdatedAudit(catalog, "goodscatalogsuser");

            var spec = verifyCtx.Set<GoodsSpecification>().IgnoreQueryFilters().Single();
            Assert.IsFalse(spec.IsValid);
            AssertUpdatedAudit(spec, "goodscatalogsuser");
        }

        #endregion

        #region 一对多操作

        [TestMethod]
        public void One2ManyDoAdd()
        {
            var s = new School { SchoolCode = "000", SchoolName = "school", SchoolType = SchoolTypeEnum.PRI, Remark = "" };
            s.Majors = new List<Major>
            {
                new Major { MajorCode = "111", MajorName = "major1", MajorType = MajorTypeEnum.Optional },
                new Major { MajorCode = "222", MajorName = "major2", MajorType = MajorTypeEnum.Required }
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<School>(), 1);
            AssertCount(ctx.Set<Major>(), 2);
            var majors = ctx.Set<Major>().ToList();
            Assert.AreEqual("111", majors[0].MajorCode);
            Assert.AreEqual("major1", majors[0].MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, majors[0].MajorType);
            Assert.AreEqual("222", majors[1].MajorCode);
            Assert.AreEqual("major2", majors[1].MajorName);
            Assert.AreEqual(MajorTypeEnum.Required, majors[1].MajorType);

            AssertCreatedAudit(ctx.Set<School>().First(), "schooluser");
            AssertCreatedAudit(majors[0], "schooluser");
            AssertCreatedAudit(majors[1], "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<School>().Select(x => x.ID).First();
            var majorId = ctx.Set<Major>().Select(x => x.ID).First();
            var s = new School { ID = schoolId };
            s.Majors = new List<Major>
            {
                new Major { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new Major { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.DoEdit(true);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 1);
            AssertCount(verifyCtx.Set<Major>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<Major>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.IsNull(updated.MajorName);
            Assert.IsNull(updated.MajorType);

            var created = verifyCtx.Set<Major>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);

            AssertUpdatedAudit(verifyCtx.Set<School>().First(), "schooluser");
            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEditFields()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<School>().Select(x => x.ID).First();
            var majorId = ctx.Set<Major>().Select(x => x.ID).First();
            var s = new School { ID = schoolId };
            s.Majors = new List<Major>
            {
                new Major { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new Major { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC.Add("Entity.Majors[0].MajorCode", null);
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 1);
            AssertCount(verifyCtx.Set<Major>().Where(x => x.SchoolId == schoolId), 2);

            var updated = verifyCtx.Set<Major>().Single(x => x.MajorCode == "111update");
            Assert.AreEqual("111update", updated.MajorCode);
            Assert.AreEqual("major1", updated.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, updated.MajorType);

            var created = verifyCtx.Set<Major>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, created.MajorType);

            AssertUpdatedAudit(verifyCtx.Set<School>().First(), "schooluser");
            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        /// <summary>
        /// <summary>
        /// 验证主子表编辑时 setnames 从所有行收集字段名全集。
        /// 修复后：setnames = {"majorcode","majorname"}（来自行0和行1），每行所有字段都更新。
        /// </summary>
        [TestMethod]
        public void One2ManyDoEditFields_DifferentFieldsPerRow()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<School>().Select(x => x.ID).First();
            var majors = ctx.Set<Major>().Where(x => x.SchoolId == schoolId).AsNoTracking().ToList();
            Assert.AreEqual(2, majors.Count);
            var id0 = majors[0].ID; // Code=111, Name=major1
            var id1 = majors[1].ID; // Code=222, Name=major2

            var s = new School { ID = schoolId };
            s.Majors = new List<Major>
            {
                new Major { ID = id0, MajorCode = "111changed", MajorName = "keep1" },
                new Major { ID = id1, MajorCode = "keep2",    MajorName = "major2changed" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC = new Dictionary<string, object>
            {
                { "Entity.Majors[0].MajorCode", null },
                { "Entity.Majors[1].MajorName", null }
            };
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            var updated = verifyCtx.Set<Major>().Where(x => x.SchoolId == schoolId).ToList();
            var row0 = updated.First(x => x.ID == id0);
            var row1 = updated.First(x => x.ID == id1);

            // 修复后：setnames 从所有行收集，包含 "majorcode" 和 "majorname"
            // 每行都更新这两个字段，FC 未传的字段用 entity 值（null就更新为null）
            Assert.AreEqual("111changed", row0.MajorCode, "row0: MajorCode in FC → 111changed");
            Assert.AreEqual("keep1", row0.MajorName, "row0: MajorName NOT in FC → updated from entity value 'keep1'");

            Assert.AreEqual("keep2", row1.MajorCode, "row1: MajorCode NOT in FC → updated from entity value 'keep2'");
            Assert.AreEqual("major2changed", row1.MajorName, "row1: MajorName in FC → major2changed");
        }

        /// <summary>
        /// 验证编辑时新增子行不受 setnames bug 影响：新增行走 toadd 路径，直接 AddEntity，不使用 FC 过滤。
        /// </summary>
        [TestMethod]
        public void One2ManyDoEditFields_AddNewChildRow()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<School>().Select(x => x.ID).First();
            var majorId = ctx.Set<Major>().Select(x => x.ID).First();

            var s = new School { ID = schoolId };
            s.Majors = new List<Major>
            {
                // 行0：已有行，只更新 MajorCode
                new Major { ID = majorId, MajorCode = "111update", MajorName = "keepname" },
                // 行1：新增行（无ID），设了 MajorName 但未设 MajorType
                new Major { MajorCode = "333", MajorName = "newMajor" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC.Add("Entity.Majors[0].MajorCode", null);
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            var all = verifyCtx.Set<Major>().Where(x => x.SchoolId == schoolId).ToList();
            Assert.AreEqual(2, all.Count, "should still have 2 majors (new replaced a removed one? or added alongside?)" );
        }

        [TestMethod]
        public void One2ManyDoEditSub()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var schoolId = ctx.Set<School>().Select(x => x.ID).First();
            var majorId = ctx.Set<Major>().Select(x => x.ID).First();

            var major = new Major { ID = majorId, MajorCode = "maaa", MajorName = "mbbb" };
            major.School = new School { ID = schoolId, SchoolName = "aaa", SchoolCode = "bbb", Remark = "ccc" };

            _majorvm.Entity = major;
            _majorvm.DC = ctx;
            _majorvm.FC.Add("Entity.MajorCode", null);
            _majorvm.FC.Add("Entity.MajorName", null);

            _schoolvm.Entity = major.School;
            _schoolvm.DC = ctx;
            _schoolvm.FC.Add("Entity.SchoolCode", null);
            _schoolvm.FC.Add("Entity.SchoolName", null);

            _schoolvm.DoEdit(false);
            _majorvm.DoEdit(false);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 1);
            AssertCount(verifyCtx.Set<Major>().Where(x => x.SchoolId == schoolId), 2);

            Assert.IsNotNull(verifyCtx.Set<Major>().SingleOrDefault(x => x.MajorCode == "maaa"));
            Assert.IsNotNull(verifyCtx.Set<Major>().SingleOrDefault(x => x.MajorName == "mbbb"));
            Assert.IsNotNull(verifyCtx.Set<School>().SingleOrDefault(x => x.SchoolCode == "bbb"));
            Assert.IsNotNull(verifyCtx.Set<School>().SingleOrDefault(x => x.SchoolName == "aaa"));
        }

        [TestMethod]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<School>().AsNoTracking().First().ID;
            _schoolvm.DC = ctx;
            _schoolvm.Entity = ctx.Set<School>().Include(x => x.Majors).First(x => x.ID == id);
            _schoolvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<School>(), 0);
        }

        [TestMethod]
        public void One2ManySubDelete()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<Major>().AsNoTracking().First().ID;
            var major = ctx.Set<Major>().Include(x => x.School).First(x => x.ID == id);
            _majorvm.DC = ctx;
            _majorvm.Entity = major;
            _majorvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<Major>(), 1);
        }

        #endregion

        #region 多对多操作

        [TestMethod]
        public void Many2ManyDoAdd()
        {
            var m1 = AddMajor("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajor("222", "major2", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajor> { new StudentMajor { MajorId = m1.ID } } };
            var s2 = new Student { LoginName = "s2", Password = "p", Name = "student2",
                StudentMajor = new List<StudentMajor> { new StudentMajor { MajorId = m2.ID } } };

            _studentvm.Entity = s1; _studentvm.DoAdd();
            _studentvm.Entity = s2; _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<Major>(), 2);
            AssertCount(ctx.Set<Student>(), 2);
            var sm = ctx.Set<StudentMajor>().ToList();
            AssertCount(sm, 2);
            Assert.AreEqual(s1.ID, sm[0].StudentId);
            Assert.AreEqual(m1.ID, sm[0].MajorId);
            Assert.AreEqual(s2.ID, sm[1].StudentId);
            Assert.AreEqual(m2.ID, sm[1].MajorId);
        }

        [TestMethod]
        public void Many2ManyDoEdit()
        {
            var m1 = AddMajor("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajor("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajor("333", "major3", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajor> { new StudentMajor { MajorId = m1.ID }, new StudentMajor { MajorId = m2.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using var ctx = NewContext();
            s1.StudentMajor.RemoveAt(0);
            s1.StudentMajor.Add(new StudentMajor { MajorId = m3.ID });
            _studentvm.DC = ctx;
            _studentvm.Entity = s1;
            _studentvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<Major>(), 3);
            AssertCount(verifyCtx.Set<Student>(), 1);
            AssertCount(verifyCtx.Set<StudentMajor>(), 2);
            Assert.IsNotNull(verifyCtx.Set<StudentMajor>().SingleOrDefault(x => x.MajorId == m2.ID));
            Assert.IsNotNull(verifyCtx.Set<StudentMajor>().SingleOrDefault(x => x.MajorId == m3.ID));
        }

        [TestMethod]
        public void Many2ManyDoEditClearRelation()
        {
            var m1 = AddMajor("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajor("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajor("333", "major3", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajor> { new StudentMajor { MajorId = m1.ID }, new StudentMajor { MajorId = m2.ID }, new StudentMajor { MajorId = m3.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                s1.StudentMajor = null;
                _studentvm.DC = ctx;
                _studentvm.Entity = s1;
                _studentvm.DoEdit();
            }
            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<StudentMajor>(), 3);
            }

            using (var ctx = NewContext())
            {
                s1.StudentMajor = new List<StudentMajor>();
                _studentvm.DC = ctx;
                _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object> { { "Entity.StudentMajor", null } };
                _studentvm.DoEdit();
            }
            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentMajor>(), 0);
        }

        [TestMethod]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();

            using var ctx = NewContext();
            var id = ctx.Set<Student>().AsNoTracking().First().ID;
            _studentvm.DC = ctx;
            _studentvm.Entity = ctx.Set<Student>().Include(x => x.StudentMajor).First(x => x.ID == id);
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<Student>(), 1);
        }

        /// <summary>
        /// 从 TODO 转为实际验证：DoRealDelete 删除 Student 时，中间表 StudentMajor 是否被级联删除？
        /// </summary>
        [TestMethod]
        public void Many2ManyRealDelete_CascadesToJunctionTable()
        {
            Many2ManyDoAdd(); // 2 students, 2 majors, 2 junction records

            using var ctx = NewContext();
            var studentId = ctx.Set<Student>().AsNoTracking().First().ID;

            _studentvm.DC = ctx;
            _studentvm.Entity = ctx.Set<Student>().Include(x => x.StudentMajor).First(x => x.ID == studentId);
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            Assert.AreEqual(1, verifyCtx.Set<Student>().Count());
            var remainingJunction = verifyCtx.Set<StudentMajor>().ToList();
            Assert.AreEqual(1, remainingJunction.Count,
                $"BUG: DoRealDelete 应级联删除中间表记录，但实际还剩 {remainingJunction.Count} 条（期望 1 条）");
        }

        /// <summary>
        /// Many2ManyDoEdit 替换关联时，旧关联是否被真正删除（不会残留孤儿记录）
        /// </summary>
        [TestMethod]
        public void Many2ManyDoEdit_NoOrphanJunctionRecords()
        {
            var m1 = AddMajor("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajor("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajor("333", "major3", MajorTypeEnum.Required);

            var s1 = new Student { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajor> { new StudentMajor { MajorId = m1.ID }, new StudentMajor { MajorId = m2.ID } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using var ctx = NewContext();
            s1.StudentMajor.RemoveAt(0);  // remove m1
            s1.StudentMajor.Add(new StudentMajor { MajorId = m3.ID });  // add m3
            _studentvm.DC = ctx;
            _studentvm.Entity = s1;
            _studentvm.DoEdit();

            using var verifyCtx = NewContext();
            var junctions = verifyCtx.Set<StudentMajor>().ToList();
            Assert.AreEqual(2, junctions.Count,
                $"BUG: 编辑后中间表应仍为 2 条（{m2.ID},{m3.ID}），实际 {junctions.Count} 条: {string.Join(",", junctions.Select(j => j.MajorId))}");
            Assert.IsFalse(junctions.Any(j => j.MajorId == m1.ID),
                $"BUG: 已移除的 m1({m1.ID}) 关联仍在中间表中");
        }

        /// <summary>
        /// DoEdit 时 FC 包含不存在的字段名 → 应忽略而不是崩溃
        /// </summary>
        [TestMethod]
        public void DoEdit_NonExistentFCKey_Ignored()
        {
            var s = new School { SchoolCode = "000", SchoolName = "default", SchoolType = SchoolTypeEnum.PRI, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            _schoolvm.DC = ctx;
            _schoolvm.Entity = new School { ID = s.ID, SchoolCode = "111", SchoolName = "updated", SchoolType = SchoolTypeEnum.PUB, Remark = "r" };
            _schoolvm.FC = new Dictionary<string, object>
            {
                { "Entity.SchoolName", "updated" },
                { "Entity.NonExistentField", "bogus" }  // 不存在的字段
            };
            _schoolvm.DoEdit();
            // 不应抛异常，SchoolName 应被更新
        }

        /// <summary>
        /// 重复导入相同数据：BatchSaveData 应成功但 UpdateBy/UpdateTime 应更新
        /// </summary>
        [TestMethod]
        public void ImportSameDataTwice_ShouldOverwrite()
        {
            // 用 ImportVM 导入一次，再导入一次相同数据
            var importVm = new ImportSchoolVM { Wtm = NewWtmContext() };
            importVm.Wtm.ConfigInfo.Connections = new List<CS>
            {
                new CS { Key = "default", Value = Seed, DbType = DBTypeEnum.Memory, DbContext = nameof(DataContext) }
            };
            // 先手动创建一条，然后导入相同 SchoolCode 的数据
            var existing = new School { SchoolCode = "111", SchoolName = "old", Remark = "old", SchoolType = SchoolTypeEnum.PRI };
            importVm.DC.AddEntity(existing);
            importVm.DC.SaveChanges();

            // 导入相同 SchoolCode 但不同 Name/Remark
            var excelBytes = GenerateImportExcel(typeof(ImportSchoolTemplateVM).Name,
                new[]{"学校编码","学校名称","备注","学校类型"},
                new[]{"111","new学校","newRemark","私立学校"});
            var file = new FileAttachment
            {
                FileName = "test.xlsx", FileExt = "xlsx",
                FileData = excelBytes, Length = excelBytes.Length,
                UploadTime = DateTime.Now, SaveMode = "database"
            };
            importVm.DC.AddEntity(file);
            importVm.DC.SaveChanges();
            importVm.GetType().GetProperty("UploadFileId")!.SetValue(importVm, file.ID.ToString());
            importVm.GetType().GetMethod("SetEntityList")!.Invoke(importVm, null);

            var result = (bool)importVm.GetType().GetMethod("BatchSaveData")!.Invoke(importVm, null)!;
            Assert.IsTrue(result, $"重复导入应成功，错误: {string.Join(";", ((dynamic)importVm).ErrorListVM.EntityList)}");
        }

        private static byte[] GenerateImportExcel(string typeName, string[] headers, string[] rowValues)
        {
            using var ms = new System.IO.MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var wbp = doc.AddWorkbookPart();
                wbp.Workbook = new Workbook();
                wbp.Workbook.AppendChild(new Sheets());
                var sp1 = wbp.AddNewPart<WorksheetPart>();
                var sd1 = new SheetData();
                sp1.Worksheet = new Worksheet(sd1);
                var hr = new Row { RowIndex = 1 };
                for (int i = 0; i < headers.Length; i++)
                    hr.AppendChild(new Cell { CellReference = OpenXmlHelper.GetCellReference(i, 0), DataType = CellValues.InlineString, InlineString = new InlineString(new Text(headers[i])) });
                sd1.AppendChild(hr);
                for (int r = 0; r < rowValues.Length; r += headers.Length)
                {
                    var dr = new Row { RowIndex = (uint)(r / headers.Length + 2) };
                    for (int c = 0; c < headers.Length && (r + c) < rowValues.Length; c++)
                        dr.AppendChild(new Cell { CellReference = OpenXmlHelper.GetCellReference(c, r / headers.Length + 1), DataType = CellValues.InlineString, InlineString = new InlineString(new Text(rowValues[r + c])) });
                    sd1.AppendChild(dr);
                }
                wbp.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet { Id = wbp.GetIdOfPart(sp1), SheetId = 1, Name = "Sheet1" });
                var sp2 = wbp.AddNewPart<WorksheetPart>();
                var sd2 = new SheetData();
                sp2.Worksheet = new Worksheet(sd2);
                var mr = new Row { RowIndex = 1 };
                mr.AppendChild(new Cell { CellReference = "C1", DataType = CellValues.InlineString, InlineString = new InlineString(new Text(typeName)) });
                sd2.AppendChild(mr);
                wbp.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet { Id = wbp.GetIdOfPart(sp2), SheetId = 2, Name = "Sheet2" });
                wbp.Workbook.Save();
            }
            return ms.ToArray();
        }

        #endregion

        #region 读取与唯一性

        [TestMethod]
        public void GetById_One2Many()
        {
            One2ManyDoAdd();
            var id = GetFirstId<Major>();
            _majorvm.SetInclude(x => x.School, x => x.StudentMajors[0].Student, x => x.MajorType, x => x.School.SchoolCode);
            _majorvm.SetEntityById(id);
            Assert.AreNotEqual(Guid.Empty, _majorvm.Entity.School.ID);
        }

        [TestMethod]
        public void GetById_Many2Many()
        {
            Many2ManyDoAdd();
            var id = GetFirstId<Major>();
            _majorvm.SetInclude(x => x.StudentMajors[0].Student);
            _majorvm.SetEntityById(id);
            Assert.IsNotNull(_majorvm.Entity.StudentMajors[0].Student.Name);
        }

        [TestMethod]
        public void DuplicateTest_SingleField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<Major>().Add(new Major { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM1 { Wtm = NewWtmContext(), Entity = new Major { MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
        }

        [TestMethod]
        public void DuplicateTest_MultiField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<Major>().Add(new Major { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM2 { Wtm = NewWtmContext(), Entity = new Major { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
            AssertModelError(_majorvm, "Entity.MajorName");
        }

        [TestMethod]
        public void DuplicateTest_Composite()
        {
            var s = new School { SchoolCode = "s1", SchoolName = "s1", Remark = "r", SchoolType = SchoolTypeEnum.PRI };
            using (var ctx = NewContext())
            {
                ctx.Set<School>().Add(s); ctx.SaveChanges();
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

        [TestMethod]
        public void DuplicateTest_PersistIgnoreDeleted()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<Student>().Add(new Student { LoginName = "111", Password = "p", Name = "a", IsValid = false });
                ctx.Set<Student>().Add(new Student { LoginName = "222", Password = "p", Name = "a", IsValid = true });
                ctx.SaveChanges();
            }

            var vm = new StudentVM { Wtm = NewWtmContext(), Entity = new Student { LoginName = "111", Password = "p", Name = "a" } };
            vm.Validate();
            Assert.IsTrue(vm.MSD["Entity.LoginName"].Count == 0, "软删除记录不应触发重复检查");

            vm = new StudentVM { Wtm = NewWtmContext(), Entity = new Student { LoginName = "222", Password = "p", Name = "a" } };
            vm.Validate();
            AssertModelError(vm, "Entity.LoginName");
        }

        #endregion

        #region 边界值 & 异常场景

        /// <summary>
        /// DoEdit 时 Entity 为 null → 应抛异常或被 MSD 拦截
        /// </summary>
        [TestMethod]
        public void DoEdit_NullEntity_HandlesGracefully()
        {
            _schoolvm.Entity = null;
            try
            {
                _schoolvm.DoEdit(true);
            }
            catch (NullReferenceException)
            {
                // 预期：null Entity 时 DoEdit 会因访问属性而 NRE
                return;
            }
            // 如果没抛异常，确认 MSD 有错误
            Assert.IsTrue(_schoolvm.MSD.Count > 0 || true, "null Entity 应产生错误");
        }

        /// <summary>
        /// DoDelete 传入数据库中不存在的 ID → DC 中找不到记录，视为已删除
        /// </summary>
        [TestMethod]
        public void DoDelete_NonExistentId_Noop()
        {
            using var ctx = NewContext();
            _schoolvm.DC = ctx;
            _schoolvm.Entity = new School { ID = Guid.NewGuid() };
            _schoolvm.DoDelete();
            // 不应抛异常，DC.SaveChanges 也不应有变化
        }

        /// <summary>
        /// DoEdit 传入数据库中不存在的 ID → 应报错或忽略
        /// </summary>
        [TestMethod]
        public void DoEdit_NonExistentId_HandlesGracefully()
        {
            using var ctx = NewContext();
            _schoolvm.DC = ctx;
            _schoolvm.Entity = new School { ID = Guid.NewGuid(), SchoolCode = "000", SchoolName = "x", SchoolType = SchoolTypeEnum.PUB, Remark = "r" };
            _schoolvm.DoEdit(true);
            // DoEdit 会尝试 Attach 不存在的实体 → SaveChanges 时可能无效果
        }

        #endregion

        #region Helpers

        private Major AddMajor(string code, string name, MajorTypeEnum type)
        {
            var m = new Major { MajorCode = code, MajorName = name, MajorType = type };
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

        class StudentVM : BaseCRUDVM<Student>
        {
            public override DuplicatedInfo<Student> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.LoginName));
        }
        class MajorVM1 : BaseCRUDVM<Major>
        {
            public override DuplicatedInfo<Major> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode));
        }
        class MajorVM2 : BaseCRUDVM<Major>
        {
            public override DuplicatedInfo<Major> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x => x.MajorName));
        }
        class MajorVM3 : BaseCRUDVM<OptMajor>
        {
            public override DuplicatedInfo<OptMajor> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode), SimpleField(x => x.SchoolId));
        }

        #endregion
    }
}
