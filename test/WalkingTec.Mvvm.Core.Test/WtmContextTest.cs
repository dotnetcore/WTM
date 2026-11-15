using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test
{
    [TestClass]
    [TestCategory("Core")]
    public class WtmContextTest : CrudTestBase
    {
        private WTMContext _wtm;

        public WtmContextTest()
        {
            InitSeed();
            _wtm = NewWtmContext("admin");
            _wtm.ConfigInfo.Connections = new List<CS>
            {
                new CS { Key = "default", Value = Seed, DbType = DBTypeEnum.Memory, DbContext = nameof(DataContext) }
            };
        }

        #region CreateVM

        [TestMethod] public void CreateVM_PropagatesWtmContext() { var vm = _wtm.CreateVM<BaseCRUDVM<School>>(); Assert.IsNotNull(vm); Assert.AreSame(_wtm, vm.Wtm); }
        [TestMethod] public void CreateVM_PropagatesDC() { var vm = _wtm.CreateVM<BaseCRUDVM<School>>(); Assert.IsNotNull(vm.DC); }

        [TestMethod]
        public void CreateVM_WithId_SetsEntity()
        {
            var s = new School { SchoolCode = "111", SchoolName = "test", SchoolType = SchoolTypeEnum.PUB, Remark = "r" };
            _wtm.DC.Set<School>().Add(s);
            _wtm.DC.SaveChanges();

            var vm = _wtm.CreateVM<BaseCRUDVM<School>>(s.ID);
            Assert.IsNotNull(vm.Entity);
            Assert.AreEqual(s.ID, vm.Entity.ID);
        }

        [TestMethod]
        public void CreateVM_WithExpression_FiltersByValue()
        {
            var vm = _wtm.CreateVM<BaseCRUDVM<School>>(
                values: x => x.Entity.SchoolCode == "init001");
            Assert.AreEqual("init001", vm.Entity.SchoolCode);
        }

        [TestMethod]
        public void CreateVM_DifferentTypes()
        {
            Assert.IsNotNull(_wtm.CreateVM<BaseCRUDVM<School>>());
            Assert.IsNotNull(_wtm.CreateVM<BasePagedListVM<School, BaseSearcher>>());
            Assert.IsNotNull(_wtm.CreateVM<BaseImportVM<WtmTestImportTemplate, School>>());
        }

        [TestMethod]
        public void CreateVM_BaseVM_ByFullName()
        {
            var fullName = typeof(BaseCRUDVM<School>).AssemblyQualifiedName!;
            fullName = fullName.Substring(0, fullName.LastIndexOf(", Version="));
            var vm = _wtm.CreateVM(fullName);
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm, typeof(BaseCRUDVM<School>));
        }

        #endregion

        #region LoginUserInfo

        [TestMethod] public void LoginUserInfo_HasITCode() { Assert.AreEqual("admin", _wtm.LoginUserInfo.ITCode); }
        [TestMethod] public void LoginUserInfo_IsNotNull() { Assert.IsNotNull(_wtm.LoginUserInfo); }

        [TestMethod]
        public void LoginUserInfo_CanSetRoles()
        {
            _wtm.LoginUserInfo.Roles = new List<SimpleRole>
            {
                new SimpleRole { RoleCode = "r001", RoleName = "管理员" },
                new SimpleRole { RoleCode = "r002", RoleName = "普通用户" }
            };

            Assert.AreEqual(2, _wtm.LoginUserInfo.Roles.Count);
            Assert.AreEqual("r001", _wtm.LoginUserInfo.Roles[0].RoleCode);
            Assert.AreEqual("管理员", _wtm.LoginUserInfo.Roles[0].RoleName);
        }

        [TestMethod]
        public void LoginUserInfo_CanSetGroups()
        {
            _wtm.LoginUserInfo.Groups = new List<SimpleGroup>
            {
                new SimpleGroup { GroupCode = "g001", GroupName = "管理组" },
                new SimpleGroup { GroupCode = "g002", GroupName = "开发组" }
            };

            Assert.AreEqual(2, _wtm.LoginUserInfo.Groups.Count);
            Assert.AreEqual("g001", _wtm.LoginUserInfo.Groups[0].GroupCode);
        }

        [TestMethod]
        public void LoginUserInfo_CanSetDataPrivileges()
        {
            _wtm.LoginUserInfo.DataPrivileges = new List<SimpleDataPri>
            {
                new SimpleDataPri { TableName = "School" }
            };

            Assert.AreEqual(1, _wtm.LoginUserInfo.DataPrivileges.Count);
            Assert.AreEqual("School", _wtm.LoginUserInfo.DataPrivileges[0].TableName);
        }

        #endregion

        #region CreateDC

        [TestMethod] public void CreateDC_ReturnsValidDataContext() { var dc = _wtm.CreateDC(); Assert.IsNotNull(dc); }

        [TestMethod]
        public void CreateDC_WithCustomKey()
        {
            _wtm.ConfigInfo.Connections = new List<CS>
            {
                new CS { Key = "custom", Value = Seed + "_c", DbType = DBTypeEnum.Memory, DbContext = nameof(DataContext) }
            };
            Assert.IsNotNull(_wtm.CreateDC(cskey: "custom"));
        }

        [TestMethod] public void DC_Property_UsesDefaultConnection() { Assert.IsNotNull(_wtm.DC); }

        #endregion

        #region Session

        [TestMethod]
        public void Session_CanSetAndGet()
        {
            _wtm.Session.Set("testKey", "testValue");
            Assert.AreEqual("testValue", _wtm.Session.Get<string>("testKey"));
        }

        [TestMethod]
        public void Session_GetNonExistent_ReturnsNull()
        {
            Assert.IsNull(_wtm.Session.Get<string>("nonExistent_XYZ123"));
        }

        #endregion

        #region CurrentCS / CurrentDbType

        [TestMethod]
        public void CurrentCS_CanSet() { _wtm.CurrentCS = "custom_cs"; Assert.AreEqual("custom_cs", _wtm.CurrentCS); }
        [TestMethod]
        public void CurrentDbType_CanSet() { _wtm.CurrentDbType = DBTypeEnum.Memory; Assert.AreEqual(DBTypeEnum.Memory, _wtm.CurrentDbType); }

        #endregion

        #region Cache

        [TestMethod]
        public void Cache_CanStoreAndRetrieve()
        {
            var key = $"test_cache_{Guid.NewGuid()}";
            var value = new byte[] { 1, 2, 3, 4 };
            _wtm.Cache.Set(key, value, new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions());
            var retrieved = _wtm.Cache.Get(key);
            Assert.IsNotNull(retrieved);
            CollectionAssert.AreEqual(value, retrieved);
        }

        #endregion

        #region MSD

        [TestMethod]
        public void MSD_CanAddErrors()
        {
            _wtm.MSD.AddModelError("Entity.Name", "名称不能为空");
            Assert.AreEqual(1, _wtm.MSD.Count);
            Assert.AreEqual("名称不能为空", _wtm.MSD["Entity.Name"].First().ErrorMessage);
        }

        [TestMethod]
        public void MSD_IsWritable() { Assert.AreEqual(0, _wtm.MSD.Count); }

        #endregion

        #region _Import fixture

        public class WtmTestImportTemplate : BaseTemplateVM
        {
            [System.ComponentModel.DataAnnotations.Display(Name = "学校编码")]
            public ExcelPropety SchoolCode = new ExcelPropety { FieldName = nameof(School.SchoolCode), ColumnName = "学校编码", DataType = ColumnDataType.Text };
        }

        #endregion
    }
}
