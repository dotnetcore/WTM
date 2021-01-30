using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class CityControllerTest
    {
        private CityController _controller;
        private string _seed;

        public CityControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<CityController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as CityListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(CityVM));

            CityVM vm = rv.Model as CityVM;
            City v = new City();
			
            v.Name = "NDS1wVfN";
            v.Test = "0cZlNfH";
            v.ParentId = AddParent();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
				
                Assert.AreEqual(data.Name, "NDS1wVfN");
                Assert.AreEqual(data.Test, "0cZlNfH");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Name = "NDS1wVfN";
                v.Test = "0cZlNfH";
                v.ParentId = AddParent();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(CityVM));

            CityVM vm = rv.Model as CityVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new City();
            v.ID = vm.Entity.ID;
       		
            v.Name = "oaARaowz";
            v.Test = "38928";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Test", "");
            vm.FC.Add("Entity.ParentId", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "oaARaowz");
                Assert.AreEqual(data.Test, "38928");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Name = "NDS1wVfN";
                v.Test = "0cZlNfH";
                v.ParentId = AddParent();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(CityVM));

            CityVM vm = rv.Model as CityVM;
            v = new City();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
                Assert.AreEqual(data.IsValid, false);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.Name = "NDS1wVfN";
                v.Test = "0cZlNfH";
                v.ParentId = AddParent();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            City v1 = new City();
            City v2 = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "NDS1wVfN";
                v1.Test = "0cZlNfH";
                v1.ParentId = AddParent();
                v2.Name = "oaARaowz";
                v2.Test = "38928";
                v2.ParentId = v1.ParentId; 
                context.Set<City>().Add(v1);
                context.Set<City>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(CityBatchVM));

            CityBatchVM vm = rv.Model as CityBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Name = "3NGL9";
            vm.LinkedVM.Test = "pvj8SCCPB";
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Name", "");
            vm.FC.Add("LinkedVM.Test", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<City>().Find(v1.ID);
                var data2 = context.Set<City>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Name, "3NGL9");
                Assert.AreEqual(data2.Name, "3NGL9");
                Assert.AreEqual(data1.Test, "pvj8SCCPB");
                Assert.AreEqual(data2.Test, "pvj8SCCPB");
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            City v1 = new City();
            City v2 = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "NDS1wVfN";
                v1.Test = "0cZlNfH";
                v1.ParentId = AddParent();
                v2.Name = "oaARaowz";
                v2.Test = "38928";
                v2.ParentId = v1.ParentId; 
                context.Set<City>().Add(v1);
                context.Set<City>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(CityBatchVM));

            CityBatchVM vm = rv.Model as CityBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<City>().Find(v1.ID);
                var data2 = context.Set<City>().Find(v2.ID);
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as CityListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Guid AddParent()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Name = "13ZhPNAb";
                v.Test = "5vBOx";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
