using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo._Admin.ViewModels.FrameworkTenantVMs;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class FrameworkTenantControllerTest
    {
        private FrameworkTenantController _controller;
        private string _seed;

        public FrameworkTenantControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<FrameworkTenantController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as FrameworkTenantListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkTenantVM));

            FrameworkTenantVM vm = rv.Model as FrameworkTenantVM;
            FrameworkTenant v = new FrameworkTenant();
			
            v.TCode = "S7uYdYp";
            v.TName = "RATQQP8t0Qa";
            v.TDb = "L6KQ3ldXaURav9Q";
            v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
            v.DbContext = "35wj0SbDBsU2";
            v.TDomain = "x6h2fEwrNp5PoXPN3Bs";
            v.TenantCode = "S1t8iEi1Trjzmnqd0";
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkTenant>().Find(v.ID);
				
                Assert.AreEqual(data.TCode, "S7uYdYp");
                Assert.AreEqual(data.TName, "RATQQP8t0Qa");
                Assert.AreEqual(data.TDb, "L6KQ3ldXaURav9Q");
                Assert.AreEqual(data.TDbType, WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer);
                Assert.AreEqual(data.DbContext, "35wj0SbDBsU2");
                Assert.AreEqual(data.TDomain, "x6h2fEwrNp5PoXPN3Bs");
                Assert.AreEqual(data.TenantCode, "S1t8iEi1Trjzmnqd0");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            FrameworkTenant v = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.TCode = "S7uYdYp";
                v.TName = "RATQQP8t0Qa";
                v.TDb = "L6KQ3ldXaURav9Q";
                v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v.DbContext = "35wj0SbDBsU2";
                v.TDomain = "x6h2fEwrNp5PoXPN3Bs";
                v.TenantCode = "S1t8iEi1Trjzmnqd0";
                context.Set<FrameworkTenant>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkTenantVM));

            FrameworkTenantVM vm = rv.Model as FrameworkTenantVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new FrameworkTenant();
            v.ID = vm.Entity.ID;
       		
            v.TCode = "q21HLbPrxwPhtw8Rc35goEQQ3R8xJWFw";
            v.TName = "CGgsTQqbIXNDmNbwfqrrJq8yU";
            v.TDb = "HStXtWx8FPxsd";
            v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SQLite;
            v.DbContext = "9rX6o";
            v.TDomain = "M2NxLKRqippSwIeurB1AzmcL35I";
            v.TenantCode = "H";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.TCode", "");
            vm.FC.Add("Entity.TName", "");
            vm.FC.Add("Entity.TDb", "");
            vm.FC.Add("Entity.TDbType", "");
            vm.FC.Add("Entity.DbContext", "");
            vm.FC.Add("Entity.TDomain", "");
            vm.FC.Add("Entity.TenantCode", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkTenant>().Find(v.ID);
 				
                Assert.AreEqual(data.TCode, "q21HLbPrxwPhtw8Rc35goEQQ3R8xJWFw");
                Assert.AreEqual(data.TName, "CGgsTQqbIXNDmNbwfqrrJq8yU");
                Assert.AreEqual(data.TDb, "HStXtWx8FPxsd");
                Assert.AreEqual(data.TDbType, WalkingTec.Mvvm.Core.DBTypeEnum.SQLite);
                Assert.AreEqual(data.DbContext, "9rX6o");
                Assert.AreEqual(data.TDomain, "M2NxLKRqippSwIeurB1AzmcL35I");
                Assert.AreEqual(data.TenantCode, "H");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            FrameworkTenant v = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.TCode = "S7uYdYp";
                v.TName = "RATQQP8t0Qa";
                v.TDb = "L6KQ3ldXaURav9Q";
                v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v.DbContext = "35wj0SbDBsU2";
                v.TDomain = "x6h2fEwrNp5PoXPN3Bs";
                v.TenantCode = "S1t8iEi1Trjzmnqd0";
                context.Set<FrameworkTenant>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkTenantVM));

            FrameworkTenantVM vm = rv.Model as FrameworkTenantVM;
            v = new FrameworkTenant();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkTenant>().Find(v.ID);
                Assert.AreEqual(data, null);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            FrameworkTenant v = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.TCode = "S7uYdYp";
                v.TName = "RATQQP8t0Qa";
                v.TDb = "L6KQ3ldXaURav9Q";
                v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v.DbContext = "35wj0SbDBsU2";
                v.TDomain = "x6h2fEwrNp5PoXPN3Bs";
                v.TenantCode = "S1t8iEi1Trjzmnqd0";
                context.Set<FrameworkTenant>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            FrameworkTenant v1 = new FrameworkTenant();
            FrameworkTenant v2 = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.TCode = "S7uYdYp";
                v1.TName = "RATQQP8t0Qa";
                v1.TDb = "L6KQ3ldXaURav9Q";
                v1.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v1.DbContext = "35wj0SbDBsU2";
                v1.TDomain = "x6h2fEwrNp5PoXPN3Bs";
                v1.TenantCode = "S1t8iEi1Trjzmnqd0";
                v2.TCode = "q21HLbPrxwPhtw8Rc35goEQQ3R8xJWFw";
                v2.TName = "CGgsTQqbIXNDmNbwfqrrJq8yU";
                v2.TDb = "HStXtWx8FPxsd";
                v2.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SQLite;
                v2.DbContext = "9rX6o";
                v2.TDomain = "M2NxLKRqippSwIeurB1AzmcL35I";
                v2.TenantCode = "H";
                context.Set<FrameworkTenant>().Add(v1);
                context.Set<FrameworkTenant>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkTenantBatchVM));

            FrameworkTenantBatchVM vm = rv.Model as FrameworkTenantBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<FrameworkTenant>().Find(v1.ID);
                var data2 = context.Set<FrameworkTenant>().Find(v2.ID);
 				
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            FrameworkTenant v1 = new FrameworkTenant();
            FrameworkTenant v2 = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.TCode = "S7uYdYp";
                v1.TName = "RATQQP8t0Qa";
                v1.TDb = "L6KQ3ldXaURav9Q";
                v1.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v1.DbContext = "35wj0SbDBsU2";
                v1.TDomain = "x6h2fEwrNp5PoXPN3Bs";
                v1.TenantCode = "S1t8iEi1Trjzmnqd0";
                v2.TCode = "q21HLbPrxwPhtw8Rc35goEQQ3R8xJWFw";
                v2.TName = "CGgsTQqbIXNDmNbwfqrrJq8yU";
                v2.TDb = "HStXtWx8FPxsd";
                v2.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SQLite;
                v2.DbContext = "9rX6o";
                v2.TDomain = "M2NxLKRqippSwIeurB1AzmcL35I";
                v2.TenantCode = "H";
                context.Set<FrameworkTenant>().Add(v1);
                context.Set<FrameworkTenant>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkTenantBatchVM));

            FrameworkTenantBatchVM vm = rv.Model as FrameworkTenantBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<FrameworkTenant>().Find(v1.ID);
                var data2 = context.Set<FrameworkTenant>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as FrameworkTenantListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }


    }
}
