using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Vue3Demo.Controllers;
using WalkingTec.Mvvm.Vue3Demo.testvue.ViewModels.SchoolVue3VMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Vue3Demo;


namespace WalkingTec.Mvvm.Vue3Demo.Test
{
    [TestClass]
    public class SchoolVue3ApiTest
    {
        private SchoolVue3Controller _controller;
        private string _seed;

        public SchoolVue3ApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<SchoolVue3Controller>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new SchoolVue3Searcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            SchoolVue3VM vm = _controller.Wtm.CreateVM<SchoolVue3VM>();
            SchoolVue3 v = new SchoolVue3();
            
            v.SchoolCode = "Khs6C5TNVIhJWuSndeH";
            v.SchoolName = "DhGBTuhwRZJ0KXq52CvLhL4Pu06tFYrlAi6EMH";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
            v.Remark = "RTJO4kJrmuMS";
            v.Level = 20;
            v.PlaceId = AddCity();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SchoolVue3>().Find(v.ID);
                
                Assert.AreEqual(data.SchoolCode, "Khs6C5TNVIhJWuSndeH");
                Assert.AreEqual(data.SchoolName, "DhGBTuhwRZJ0KXq52CvLhL4Pu06tFYrlAi6EMH");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "RTJO4kJrmuMS");
                Assert.AreEqual(data.Level, 20);
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            SchoolVue3 v = new SchoolVue3();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.SchoolCode = "Khs6C5TNVIhJWuSndeH";
                v.SchoolName = "DhGBTuhwRZJ0KXq52CvLhL4Pu06tFYrlAi6EMH";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v.Remark = "RTJO4kJrmuMS";
                v.Level = 20;
                v.PlaceId = AddCity();
                context.Set<SchoolVue3>().Add(v);
                context.SaveChanges();
            }

            SchoolVue3VM vm = _controller.Wtm.CreateVM<SchoolVue3VM>();
            var oldID = v.ID;
            v = new SchoolVue3();
            v.ID = oldID;
       		
            v.SchoolCode = "v0ElgDjFgDehW7k0kWu";
            v.SchoolName = "senAHjyP4";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
            v.Remark = "Ara7LZY1LJVE";
            v.Level = 7;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.SchoolCode", "");
            vm.FC.Add("Entity.SchoolName", "");
            vm.FC.Add("Entity.SchoolType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.Level", "");
            vm.FC.Add("Entity.PlaceId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SchoolVue3>().Find(v.ID);
 				
                Assert.AreEqual(data.SchoolCode, "v0ElgDjFgDehW7k0kWu");
                Assert.AreEqual(data.SchoolName, "senAHjyP4");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "Ara7LZY1LJVE");
                Assert.AreEqual(data.Level, 7);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            SchoolVue3 v = new SchoolVue3();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.SchoolCode = "Khs6C5TNVIhJWuSndeH";
                v.SchoolName = "DhGBTuhwRZJ0KXq52CvLhL4Pu06tFYrlAi6EMH";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v.Remark = "RTJO4kJrmuMS";
                v.Level = 20;
                v.PlaceId = AddCity();
                context.Set<SchoolVue3>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            SchoolVue3 v1 = new SchoolVue3();
            SchoolVue3 v2 = new SchoolVue3();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.SchoolCode = "Khs6C5TNVIhJWuSndeH";
                v1.SchoolName = "DhGBTuhwRZJ0KXq52CvLhL4Pu06tFYrlAi6EMH";
                v1.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v1.Remark = "RTJO4kJrmuMS";
                v1.Level = 20;
                v1.PlaceId = AddCity();
                v2.SchoolCode = "v0ElgDjFgDehW7k0kWu";
                v2.SchoolName = "senAHjyP4";
                v2.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v2.Remark = "Ara7LZY1LJVE";
                v2.Level = 7;
                v2.PlaceId = v1.PlaceId; 
                context.Set<SchoolVue3>().Add(v1);
                context.Set<SchoolVue3>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<SchoolVue3>().Find(v1.ID);
                var data2 = context.Set<SchoolVue3>().Find(v2.ID);
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

                v.Name = "5v28Z1p6";
                v.Level = 54;
                context.Set<City>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
