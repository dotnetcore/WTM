using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Vue3Demo.Controllers;
using WalkingTec.Mvvm.Vue3Demo.SchoolData.ViewModels.CityVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Vue3Demo;


namespace WalkingTec.Mvvm.Vue3Demo.Test
{
    [TestClass]
    public class CityApiTest
    {
        private CityController _controller;
        private string _seed;

        public CityApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<CityController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new CitySearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            CityVM vm = _controller.Wtm.CreateVM<CityVM>();
            City v = new City();
            
            v.Name = "mEJMSRD4";
            v.Level = 37;
            v.ParentId = AddCity();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
                
                Assert.AreEqual(data.Name, "mEJMSRD4");
                Assert.AreEqual(data.Level, 37);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Name = "mEJMSRD4";
                v.Level = 37;
                v.ParentId = AddCity();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }

            CityVM vm = _controller.Wtm.CreateVM<CityVM>();
            var oldID = v.ID;
            v = new City();
            v.ID = oldID;
       		
            v.Name = "EaIk";
            v.Level = 96;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Level", "");
            vm.FC.Add("Entity.ParentId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "EaIk");
                Assert.AreEqual(data.Level, 96);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Name = "mEJMSRD4";
                v.Level = 37;
                v.ParentId = AddCity();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            City v1 = new City();
            City v2 = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "mEJMSRD4";
                v1.Level = 37;
                v1.ParentId = AddCity();
                v2.Name = "EaIk";
                v2.Level = 96;
                v2.ParentId = v1.ParentId; 
                context.Set<City>().Add(v1);
                context.Set<City>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<City>().Find(v1.ID);
                var data2 = context.Set<City>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Guid AddCity()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.Name = "GtQGS2pD5d9SdmnafKK";
                v.Level = 42;
                context.Set<City>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
