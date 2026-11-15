using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test
{
    [TestClass]
    public class DCExtensionTest : CrudTestBase
    {
        private WTMContext _wtm;

        public DCExtensionTest()
        {
            InitSeed();
            _wtm = NewWtmContext("admin");
            var dc = _wtm.DC;
            dc.Set<School>().Add(new School { SchoolCode = "001", SchoolName = "学校A", SchoolType = SchoolTypeEnum.PUB, Remark = "r1" });
            dc.Set<School>().Add(new School { SchoolCode = "002", SchoolName = "学校B", SchoolType = SchoolTypeEnum.PRI, Remark = "r2" });
            dc.Set<School>().Add(new School { SchoolCode = "003", SchoolName = "学校C", SchoolType = SchoolTypeEnum.PUB, Remark = "r3" });
            dc.Set<Major>().Add(new Major { MajorCode = "111", MajorName = "major1", MajorType = MajorTypeEnum.Required });
            dc.Set<Major>().Add(new Major { MajorCode = "222", MajorName = "major2", MajorType = MajorTypeEnum.Optional });
            dc.SaveChanges();
        }

        #region CheckID
        [TestMethod] public void CheckID_ValidGuid() { var id = _wtm.DC.Set<School>().First().ID; var r = _wtm.DC.Set<School>().CheckID(id).ToList(); Assert.AreEqual(1, r.Count); Assert.AreEqual(id, r[0].ID); }
        [TestMethod] public void CheckID_NonExistent() { Assert.AreEqual(0, _wtm.DC.Set<School>().CheckID(Guid.NewGuid()).ToList().Count); }
        [TestMethod] public void CheckID_String() { var s = _wtm.DC.Set<School>().First(); Assert.AreEqual(s.ID, _wtm.DC.Set<School>().CheckID(s.ID.ToString()).First().ID); }
        #endregion

        #region CheckIDs
        [TestMethod] public void CheckIDs_Multiple() { var ids = _wtm.DC.Set<School>().Take(2).Select(x => x.ID.ToString()).ToList(); Assert.AreEqual(2, _wtm.DC.Set<School>().CheckIDs(ids).Count()); }
        [TestMethod] public void CheckIDs_Empty() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckIDs(new List<string>()).Count()); }
        [TestMethod] public void CheckIDs_Null() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckIDs(null).Count()); }
        #endregion

        #region CheckEqual
        [TestMethod] public void CheckEqual_String() { Assert.AreEqual(1, _wtm.DC.Set<School>().CheckEqual("001", x => x.SchoolCode).Count()); }
        [TestMethod] public void CheckEqual_NoMatch() { Assert.AreEqual(0, _wtm.DC.Set<School>().CheckEqual("999", x => x.SchoolCode).Count()); }
        [TestMethod] public void CheckEqual_Null() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckEqual(null, x => x.SchoolCode).Count()); }
        [TestMethod] public void CheckEqual_Enum() { Assert.AreEqual(2, _wtm.DC.Set<School>().CheckEqual(SchoolTypeEnum.PUB, x => x.SchoolType).Count()); }
        #endregion

        #region CheckContain
        [TestMethod] public void CheckContain_Match() { Assert.AreEqual(1, _wtm.DC.Set<School>().CheckContain("学校A", x => x.SchoolName).Count()); }
        [TestMethod] public void CheckContain_NoMatch() { Assert.AreEqual(0, _wtm.DC.Set<School>().CheckContain("XYZ", x => x.SchoolName).Count()); }
        [TestMethod] public void CheckContain_Null() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckContain(null, x => x.SchoolName).Count()); }
        #endregion

        #region CheckWhere
        [TestMethod] public void CheckWhere_Value() { Assert.AreEqual(1, _wtm.DC.Set<School>().CheckWhere("001", x => x.SchoolCode == "001").Count()); }
        [TestMethod] public void CheckWhere_Null() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckWhere(null as string, x => false).Count()); }
        [TestMethod] public void CheckWhere_EmptyList() { Assert.AreEqual(3, _wtm.DC.Set<School>().CheckWhere(new List<string>(), x => false).Count()); }
        #endregion

        #region CheckNotNull/Null
        [TestMethod] public void CheckNotNull() { Assert.AreEqual(0, _wtm.DC.Set<Major>().CheckNotNull(x => x.Remark).Count()); }
        [TestMethod] public void CheckNull() { Assert.AreEqual(2, _wtm.DC.Set<Major>().CheckNull(x => x.Remark).Count()); }
        #endregion

        #region Sort
        [TestMethod] public void Sort_Asc() { var r = DCExtension.Sort(_wtm.DC.Set<School>(), null, new SortInfo { Property = "SchoolCode", Direction = SortDir.Asc }).ToList(); Assert.AreEqual("001", r[0].SchoolCode); }
        [TestMethod] public void Sort_Desc() { var r = DCExtension.Sort(_wtm.DC.Set<School>(), null, new SortInfo { Property = "SchoolCode", Direction = SortDir.Desc }).ToList(); Assert.AreEqual("003", r[0].SchoolCode); }
        [TestMethod] public void Sort_Default() { Assert.AreEqual(3, DCExtension.Sort(_wtm.DC.Set<School>(), null).Count()); }
        #endregion

        #region DPWhere

        [TestMethod]
        public void DPWhere_CreateBy_AsCreator_ReturnsAll()
        {
            var dps = new List<IDataPrivilege> { new DataPrivilegeInfo<School>("学校权限", m => m.SchoolName) };
            var w = MockWtmContext.CreateWtmContext(new DataContext(Seed + "_dp1", DBTypeEnum.Memory), "admin", dps);
            w.DC.Set<School>().Add(new School { SchoolCode = "001", SchoolName = "A", SchoolType = SchoolTypeEnum.PUB, Remark = "r", CreateBy = "admin" });
            w.DC.Set<School>().Add(new School { SchoolCode = "002", SchoolName = "B", SchoolType = SchoolTypeEnum.PRI, Remark = "r", CreateBy = "admin" });
            w.DC.SaveChanges();
            Assert.AreEqual(2, w.DC.Set<School>().DPWhere(w, x => x.ID).Count());
        }

        [TestMethod]
        public void DPWhere_CreateBy_OtherUser_ReturnsNone()
        {
            var dps = new List<IDataPrivilege> { new DataPrivilegeInfo<School>("学校权限", m => m.SchoolName) };
            var w = MockWtmContext.CreateWtmContext(new DataContext(Seed + "_dp2", DBTypeEnum.Memory), "other", dps);
            w.DC.Set<School>().Add(new School { SchoolCode = "001", SchoolName = "A", SchoolType = SchoolTypeEnum.PUB, Remark = "r" });
            w.DC.SaveChanges();
            Assert.AreEqual(0, w.DC.Set<School>().DPWhere(w, x => x.ID).Count(), "非创建者不应看到");
        }

        #endregion

        #region GetSelectListItems
        [TestMethod] public void GetSelectListItems_All() { var items = _wtm.DC.Set<School>().GetSelectListItems(_wtm, x => x.SchoolName); Assert.AreEqual(3, items.Count); }
        #endregion
    }
}
