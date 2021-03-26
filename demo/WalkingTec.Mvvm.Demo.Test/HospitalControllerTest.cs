using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.HospitalVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class HospitalControllerTest
    {
        private HospitalController _controller;
        private string _seed;

        public HospitalControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<HospitalController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as HospitalListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(HospitalVM));

            HospitalVM vm = rv.Model as HospitalVM;
            Hospital v = new Hospital();
			
            v.Name = "gDH";
            v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
            v.LocationId = AddLocation();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Hospital>().Find(v.ID);
				
                Assert.AreEqual(data.Name, "gDH");
                Assert.AreEqual(data.Level, WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            Hospital v = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Name = "gDH";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v.LocationId = AddLocation();
                context.Set<Hospital>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(HospitalVM));

            HospitalVM vm = rv.Model as HospitalVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new Hospital();
            v.ID = vm.Entity.ID;
       		
            v.Name = "FmG";
            v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Level", "");
            vm.FC.Add("Entity.LocationId", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Hospital>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "FmG");
                Assert.AreEqual(data.Level, WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            Hospital v = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Name = "gDH";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v.LocationId = AddLocation();
                context.Set<Hospital>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(HospitalVM));

            HospitalVM vm = rv.Model as HospitalVM;
            v = new Hospital();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Hospital>().Find(v.ID);
                Assert.AreEqual(data, null);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            Hospital v = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.Name = "gDH";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v.LocationId = AddLocation();
                context.Set<Hospital>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            Hospital v1 = new Hospital();
            Hospital v2 = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "gDH";
                v1.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v1.LocationId = AddLocation();
                v2.Name = "FmG";
                v2.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1;
                v2.LocationId = v1.LocationId; 
                context.Set<Hospital>().Add(v1);
                context.Set<Hospital>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(HospitalBatchVM));

            HospitalBatchVM vm = rv.Model as HospitalBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Hospital>().Find(v1.ID);
                var data2 = context.Set<Hospital>().Find(v2.ID);
 				
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            Hospital v1 = new Hospital();
            Hospital v2 = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "gDH";
                v1.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v1.LocationId = AddLocation();
                v2.Name = "FmG";
                v2.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1;
                v2.LocationId = v1.LocationId; 
                context.Set<Hospital>().Add(v1);
                context.Set<Hospital>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(HospitalBatchVM));

            HospitalBatchVM vm = rv.Model as HospitalBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Hospital>().Find(v1.ID);
                var data2 = context.Set<Hospital>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        private Guid AddLocation()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Name = "p7kpcq35";
                v.Test = "0iQgAi8se";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
