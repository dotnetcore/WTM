using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    /// <summary>
    /// BaseCRUDVM 娴嬭瘯 鈥?SoftKey/SoftFK 妯″瀷鍙樹綋锛堥潪 Guid FK锛岄€氳繃瀛楃�涓茶蒋閿�叧鑱旓級
    /// </summary>
    [TestClass]
    [TestCategory("CRUD")]
    public class BaseCRUDVMTestNoFk : CrudTestBase
    {
        private BaseCRUDVM<SchoolNoFK> _schoolvm;
        private BaseCRUDVM<MajorNoFK> _majorvm;
        private BaseCRUDVM<StudentNoFK> _studentvm;

        public BaseCRUDVMTestNoFk()
        {
            InitSeed();
            _schoolvm = new BaseCRUDVM<SchoolNoFK> { Wtm = NewWtmContext("schooluser") };
            _majorvm = new BaseCRUDVM<MajorNoFK> { Wtm = NewWtmContext("majoruser") };
            _studentvm = new BaseCRUDVM<StudentNoFK> { Wtm = NewWtmContext("studentuser") };
        }

        #region 鍗曡〃鎿嶄綔

        [TestMethod]
        [DataRow("111", "test1", SchoolTypeEnum.PRI, "remark1")]
        [DataRow("222", "test2", SchoolTypeEnum.PUB, "remark2")]
        public void SingleTableDoAdd(string code, string name, SchoolTypeEnum schooltype, string remark)
        {
            var s = new SchoolNoFK { SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            var rv = ctx.Set<SchoolNoFK>().Single();
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
            var s = new SchoolNoFK { SchoolCode = "000", SchoolName = "default", SchoolType = null, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolNoFK { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.DoEdit(true);
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolNoFK>().Single();
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
            var s = new SchoolNoFK { SchoolCode = "000", SchoolName = "default", SchoolType = null, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolNoFK { ID = s.ID, SchoolCode = code, SchoolName = name, SchoolType = schooltype, Remark = remark };
                _schoolvm.FC.Add("Entity.SchoolName", name);
                _schoolvm.DoEdit();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<SchoolNoFK>().Single();
            Assert.AreEqual("000", rv.SchoolCode);
            Assert.AreEqual(name, rv.SchoolName);
            Assert.IsNull(rv.SchoolType);
            Assert.AreEqual("default", rv.Remark);
            AssertUpdatedAudit(rv, "schooluser");
        }

        [TestMethod]
        public void SingleTableDelete()
        {
            var s = new SchoolNoFK { SchoolCode = "000", SchoolName = "default", SchoolType = null, Remark = "default" };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<SchoolNoFK>(), 1);
                _schoolvm.DC = ctx;
                _schoolvm.Entity = new SchoolNoFK { ID = s.ID };
                _schoolvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolNoFK>(), 0);
        }

        #endregion

        #region Persist 杞�垹闄?
        [TestMethod]
        public void SingleTablePersistDelete()
        {
            var s = new StudentNoFK { LoginName = "a", Password = "p", Name = "ab" };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                AssertCount(ctx.Set<StudentNoFK>(), 1);
                _studentvm.DC = ctx;
                _studentvm.Entity = ctx.Set<StudentNoFK>().First(x => x.ID == s.ID);
                _studentvm.DoDelete();
            }

            using var verifyCtx = NewContext();
            var rv = verifyCtx.Set<StudentNoFK>().IgnoreQueryFilters().Single();
            Assert.IsFalse(rv.IsValid);
            AssertUpdatedAudit(rv, "studentuser");
        }

        [TestMethod]
        public void SingleTablePersistRealDelete()
        {
            var s = new StudentNoFK { LoginName = "a", Password = "p", Name = "ab" };
            _studentvm.Entity = s;
            _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<StudentNoFK>(), 1);

            _studentvm.SetEntityById(s.ID);
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentNoFK>(), 0);
        }

        #endregion

        #region 涓€瀵瑰�鎿嶄綔

        [TestMethod]
        public void One2ManyDoAdd()
        {
            var s = new SchoolNoFK
            {
                SchoolCode = "000", SchoolName = "school", SchoolType = SchoolTypeEnum.PRI,
                Majors = new List<MajorNoFK>
                {
                    new MajorNoFK { MajorCode = "111", MajorName = "major1", MajorType = MajorTypeEnum.Optional },
                    new MajorNoFK { MajorCode = "222", MajorName = "major2", MajorType = MajorTypeEnum.Required }
                }
            };
            _schoolvm.Entity = s;
            _schoolvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<SchoolNoFK>(), 1);
            AssertCount(ctx.Set<MajorNoFK>(), 2);
            var majors = ctx.Set<MajorNoFK>().ToList();
            Assert.AreEqual("111", majors[0].MajorCode);
            Assert.AreEqual("major1", majors[0].MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, majors[0].MajorType);
            Assert.AreEqual("222", majors[1].MajorCode);
            Assert.AreEqual("major2", majors[1].MajorName);
            Assert.AreEqual(MajorTypeEnum.Required, majors[1].MajorType);

            AssertCreatedAudit(ctx.Set<SchoolNoFK>().First(), "schooluser");
            AssertCreatedAudit(majors[0], "schooluser");
            AssertCreatedAudit(majors[1], "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEdit()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var old = ctx.Set<SchoolNoFK>().AsNoTracking().First();
            var majorId = ctx.Set<MajorNoFK>().Select(x => x.ID).First();
            var s = new SchoolNoFK { ID = old.ID, SchoolCode = old.SchoolCode };
            s.Majors = new List<MajorNoFK>
            {
                new MajorNoFK { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorNoFK { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.DoEdit(true);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolNoFK>(), 1);
            AssertCount(verifyCtx.Set<MajorNoFK>().Where(x => x.SchoolId == old.SchoolCode), 2);

            var updated = verifyCtx.Set<MajorNoFK>().Single(x => x.MajorCode == "111");
            Assert.AreEqual("111", updated.MajorCode);
            Assert.IsNull(updated.MajorName);

            var created = verifyCtx.Set<MajorNoFK>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);

            AssertUpdatedAudit(verifyCtx.Set<SchoolNoFK>().First(), "schooluser");
            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEditFields()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var old = ctx.Set<SchoolNoFK>().AsNoTracking().First();
            var majorId = ctx.Set<MajorNoFK>().Select(x => x.ID).First();
            var s = new SchoolNoFK { ID = old.ID, SchoolCode = old.SchoolCode };
            s.Majors = new List<MajorNoFK>
            {
                new MajorNoFK { MajorCode = "333", MajorName = "major3", MajorType = MajorTypeEnum.Optional },
                new MajorNoFK { ID = majorId, MajorCode = "111update" }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC = new Dictionary<string, object> { { "Entity.Majors[0].MajorCode", null } };
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolNoFK>(), 1);
            AssertCount(verifyCtx.Set<MajorNoFK>().Where(x => x.SchoolId == old.SchoolCode), 2);

            var updated = verifyCtx.Set<MajorNoFK>().Single(x => x.MajorCode == "111");
            Assert.AreEqual("111", updated.MajorCode);
            Assert.AreEqual("major1", updated.MajorName);  // 淇濈暀鍘熷€?
            var created = verifyCtx.Set<MajorNoFK>().Single(x => x.MajorCode == "333");
            Assert.AreEqual("333", created.MajorCode);
            Assert.AreEqual("major3", created.MajorName);

            AssertUpdatedAudit(verifyCtx.Set<SchoolNoFK>().First(), "schooluser");
            AssertUpdatedAudit(updated, "schooluser");
            AssertCreatedAudit(created, "schooluser");
        }

        [TestMethod]
        public void One2ManyDoEditSub()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var old = ctx.Set<SchoolNoFK>().AsNoTracking().First();
            var majorId = ctx.Set<MajorNoFK>().Select(x => x.ID).First();

            var major = new MajorNoFK { ID = majorId, MajorCode = "maaa", MajorName = "mbbb",
                School = new SchoolNoFK { ID = old.ID, SchoolName = "aaa", SchoolCode = "bbb", Remark = "ccc" } };

            _majorvm.Entity = major; _majorvm.DC = ctx;
            _majorvm.FC.Add("Entity.MajorCode", null);
            _majorvm.FC.Add("Entity.MajorName", null);

            _schoolvm.Entity = major.School; _schoolvm.DC = ctx;
            _schoolvm.FC.Add("Entity.SchoolCode", null);
            _schoolvm.FC.Add("Entity.SchoolName", null);

            _schoolvm.DoEdit(false);
            _majorvm.DoEdit(false);

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolNoFK>(), 1);
            AssertCount(verifyCtx.Set<MajorNoFK>().Where(x => x.SchoolId == old.SchoolCode), 2);
            Assert.IsNotNull(verifyCtx.Set<MajorNoFK>().SingleOrDefault(x => x.MajorName == "mbbb"));
            Assert.IsNotNull(verifyCtx.Set<SchoolNoFK>().SingleOrDefault(x => x.SchoolName == "aaa"));
        }

        [TestMethod]
        public void One2ManyTableDelete()
        {
            One2ManyDoAdd();
            _schoolvm.SetInclude(x => x.Majors);
            var id = GetFirstId<SchoolNoFK>();
            _schoolvm.SetEntityById(id);
            _schoolvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<SchoolNoFK>(), 0);
        }

        [TestMethod]
        public void One2ManySubDelete()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School);
            var id = GetFirstId<MajorNoFK>();
            _majorvm.SetEntityById(id);
            _majorvm.DoDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<MajorNoFK>(), 1);
        }

        #endregion

        #region 澶氬�澶氭搷浣?
        [TestMethod]
        public void Many2ManyDoAdd()
        {
            var m1 = AddMajorNoFK("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorNoFK("222", "major2", MajorTypeEnum.Required);

            var s1 = new StudentNoFK { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorNoFK> { new StudentMajorNoFK { MajorId = m1.MajorCode } } };
            var s2 = new StudentNoFK { LoginName = "s2", Password = "p", Name = "student2",
                StudentMajor = new List<StudentMajorNoFK> { new StudentMajorNoFK { MajorId = m2.MajorCode } } };

            _studentvm.Entity = s1; _studentvm.DoAdd();
            _studentvm.Entity = s2; _studentvm.DoAdd();

            using var ctx = NewContext();
            AssertCount(ctx.Set<MajorNoFK>(), 2);
            AssertCount(ctx.Set<StudentNoFK>(), 2);
            var sm = ctx.Set<StudentMajorNoFK>().ToList();
            AssertCount(sm, 2);
            Assert.AreEqual(s1.LoginName, sm[0].StudentId);
            Assert.AreEqual(m1.MajorCode, sm[0].MajorId);
            Assert.AreEqual(s2.LoginName, sm[1].StudentId);
            Assert.AreEqual(m2.MajorCode, sm[1].MajorId);
        }

        [TestMethod]
        public void Many2ManyDoEdit()
        {
            var m1 = AddMajorNoFK("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorNoFK("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorNoFK("333", "major3", MajorTypeEnum.Required);

            var s1 = new StudentNoFK { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorNoFK> { new StudentMajorNoFK { MajorId = m1.MajorCode }, new StudentMajorNoFK { MajorId = m2.MajorCode } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using var ctx = NewContext();
            s1.StudentMajor.RemoveAt(0);
            s1.StudentMajor.Add(new StudentMajorNoFK { MajorId = m3.MajorCode });
            _studentvm.DC = ctx;
            _studentvm.Entity = s1;
            _studentvm.DoEdit();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<MajorNoFK>(), 3);
            AssertCount(verifyCtx.Set<StudentNoFK>(), 1);
            AssertCount(verifyCtx.Set<StudentMajorNoFK>(), 2);
            Assert.IsNotNull(verifyCtx.Set<StudentMajorNoFK>().SingleOrDefault(x => x.MajorId == m2.MajorCode));
            Assert.IsNotNull(verifyCtx.Set<StudentMajorNoFK>().SingleOrDefault(x => x.MajorId == m3.MajorCode));
        }

        [TestMethod]
        public void Many2ManyDoEditClearRelation()
        {
            var m1 = AddMajorNoFK("111", "major1", MajorTypeEnum.Optional);
            var m2 = AddMajorNoFK("222", "major2", MajorTypeEnum.Required);
            var m3 = AddMajorNoFK("333", "major3", MajorTypeEnum.Required);

            var s1 = new StudentNoFK { LoginName = "s1", Password = "p", Name = "student1",
                StudentMajor = new List<StudentMajorNoFK> { new StudentMajorNoFK { MajorId = m1.MajorCode }, new StudentMajorNoFK { MajorId = m2.MajorCode }, new StudentMajorNoFK { MajorId = m3.MajorCode } } };
            _studentvm.Entity = s1; _studentvm.DoAdd();

            using (var ctx = NewContext())
            {
                s1.StudentMajor = null;
                _studentvm.DC = ctx; _studentvm.Entity = s1; _studentvm.DoEdit();
            }
            using (var ctx = NewContext())
                AssertCount(ctx.Set<StudentMajorNoFK>(), 3);

            using (var ctx = NewContext())
            {
                s1.StudentMajor = new List<StudentMajorNoFK>();
                _studentvm.DC = ctx; _studentvm.Entity = s1;
                _studentvm.FC = new Dictionary<string, object> { { "Entity.StudentMajor", null } };
                _studentvm.DoEdit();
            }
            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentMajorNoFK>(), 0);
        }

        [TestMethod]
        public void Many2ManyTableDelete()
        {
            Many2ManyDoAdd();
            _studentvm.SetInclude(x => x.StudentMajor, x => x.StudentMajor[0].Major);
            var id = GetFirstId<StudentNoFK>();
            _studentvm.SetEntityById(id);
            _studentvm.DoRealDelete();

            using var verifyCtx = NewContext();
            AssertCount(verifyCtx.Set<StudentNoFK>(), 1);
            // TODO: 楠岃瘉涓�棿琛?StudentMajor 璁板綍鏄�惁琚�骇鑱斿垹闄わ紙褰撳墠 DoRealDelete 琛屼负寰呯‘璁わ級
        }

        #endregion

        #region 璇诲彇涓庡敮涓€鎬?
        [TestMethod]
        public void GetById_One2Many()
        {
            One2ManyDoAdd();
            _majorvm.SetInclude(x => x.School, x => x.StudentMajors[0].Student, x => x.MajorType, x => x.School.SchoolCode);
            var id = GetFirstId<MajorNoFK>();
            _majorvm.SetEntityById(id);
            Assert.AreNotEqual(Guid.Empty, _majorvm.Entity.School.ID);
        }

        [TestMethod]
        public void GetById_Many2Many()
        {
            Many2ManyDoAdd();
            _majorvm.SetInclude(x => x.StudentMajors[0].Student);
            var id = GetFirstId<MajorNoFK>();
            _majorvm.SetEntityById(id);
            Assert.IsNotNull(_majorvm.Entity.StudentMajors[0].Student.Name);
        }

        [TestMethod]
        public void DuplicateTest_SingleField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorNoFK>().Add(new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM1 { Wtm = NewWtmContext(), Entity = new MajorNoFK { MajorCode = "111", MajorName = "not222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
        }

        [TestMethod]
        public void DuplicateTest_MultiField()
        {
            using (var ctx = NewContext())
            {
                ctx.Set<MajorNoFK>().Add(new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Optional });
                ctx.SaveChanges();
            }

            _majorvm = new MajorVM2 { Wtm = NewWtmContext(), Entity = new MajorNoFK { MajorCode = "111", MajorName = "222", MajorType = MajorTypeEnum.Required } };
            _majorvm.Validate();
            AssertModelError(_majorvm, "Entity.MajorCode");
            AssertModelError(_majorvm, "Entity.MajorName");
        }

        #endregion

        #region 边界值 & 不同行不同字段（SoftKey 模型）

        /// <summary>
        /// 验证 SoftKey 模型主子表编辑时 setnames 从所有行收集（与 Guid 模型同样的修复验证）
        /// </summary>
        [TestMethod]
        public void One2ManyDoEditFields_DifferentFieldsPerRow()
        {
            One2ManyDoAdd();

            using var ctx = NewContext();
            var old = ctx.Set<SchoolNoFK>().AsNoTracking().First();
            var majors = ctx.Set<MajorNoFK>().Where(x => x.SchoolId == old.SchoolCode).AsNoTracking().ToList();
            Assert.AreEqual(2, majors.Count);
            var id0 = majors[0].ID;
            var id1 = majors[1].ID;

            var s = new SchoolNoFK { ID = old.ID, SchoolCode = old.SchoolCode };
            s.Majors = new List<MajorNoFK>
            {
                // MajorCode 有 [CanNotEdit]，不能改。改 MajorName 和 MajorType
                new MajorNoFK { ID = id0, MajorCode = "111", MajorName = "name0changed", MajorType = MajorTypeEnum.Optional },
                new MajorNoFK { ID = id1, MajorCode = "222", MajorName = "name1changed", MajorType = MajorTypeEnum.Required }
            };
            _schoolvm.Entity = s;
            _schoolvm.DC = ctx;
            _schoolvm.FC = new Dictionary<string, object>
            {
                { "Entity.Majors[0].MajorName", null },
                { "Entity.Majors[1].MajorType", null }
            };
            _schoolvm.DoEdit();

            using var verifyCtx = NewContext();
            var updated = verifyCtx.Set<MajorNoFK>().Where(x => x.SchoolId == old.SchoolCode).ToList();
            var row0 = updated.First(x => x.ID == id0);
            var row1 = updated.First(x => x.ID == id1);

            // 行0：MajorName 在 FC 中 → 更新
            Assert.AreEqual("name0changed", row0.MajorName);
            Assert.AreEqual(MajorTypeEnum.Optional, row0.MajorType, "MajorType from entity value");

            // 行1：MajorType 在 FC 中 → 更新
            Assert.AreEqual("name1changed", row1.MajorName, "MajorName from entity value");
            Assert.AreEqual(MajorTypeEnum.Required, row1.MajorType);
        }

        #endregion

        #region Helpers

        private MajorNoFK AddMajorNoFK(string code, string name, MajorTypeEnum type)
        {
            var m = new MajorNoFK { MajorCode = code, MajorName = name, MajorType = type };
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

        #region 鍐呴儴 VM 瀛愮被

        class MajorVM1 : BaseCRUDVM<MajorNoFK>
        {
            public override DuplicatedInfo<MajorNoFK> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode));
        }
        class MajorVM2 : BaseCRUDVM<MajorNoFK>
        {
            public override DuplicatedInfo<MajorNoFK> SetDuplicatedCheck() => CreateFieldsInfo(SimpleField(x => x.MajorCode)).AddGroup(SimpleField(x => x.MajorName));
        }

        #endregion
    }
}
