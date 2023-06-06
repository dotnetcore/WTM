using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Vue3Demo.Controllers;
using WalkingTec.Mvvm.Vue3Demo.SchoolData.ViewModels.SchoolVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Vue3Demo;


namespace WalkingTec.Mvvm.Vue3Demo.Test
{
    [TestClass]
    public class SchoolApiTest
    {
        private SchoolController _controller;
        private string _seed;

        public SchoolApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<SchoolController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new SchoolSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            School v = new School();
            
            v.ID = 67;
            v.SchoolCode = "AJnJCwG2";
            v.SchoolName = "8yYfaL";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
            v.Remark = "dqJpz640MQVe";
            v.Level = 6;
            v.PlaceId = AddCity();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 67);
                Assert.AreEqual(data.SchoolCode, "AJnJCwG2");
                Assert.AreEqual(data.SchoolName, "8yYfaL");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "dqJpz640MQVe");
                Assert.AreEqual(data.Level, 6);
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            School v = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ID = 67;
                v.SchoolCode = "AJnJCwG2";
                v.SchoolName = "8yYfaL";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "dqJpz640MQVe";
                v.Level = 6;
                v.PlaceId = AddCity();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }

            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            var oldID = v.ID;
            v = new School();
            v.ID = oldID;
       		
            v.SchoolCode = "SDDdXyA";
            v.SchoolName = "4eq2hEDVlTMiG1vBcMFTkztiR5P";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
            v.Remark = "gfCmDJmzOpO2vKYT33";
            v.Level = 10;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
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
                var data = context.Set<School>().Find(v.ID);
 				
                Assert.AreEqual(data.SchoolCode, "SDDdXyA");
                Assert.AreEqual(data.SchoolName, "4eq2hEDVlTMiG1vBcMFTkztiR5P");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "gfCmDJmzOpO2vKYT33");
                Assert.AreEqual(data.Level, 10);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            School v = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = 67;
                v.SchoolCode = "AJnJCwG2";
                v.SchoolName = "8yYfaL";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "dqJpz640MQVe";
                v.Level = 6;
                v.PlaceId = AddCity();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            School v1 = new School();
            School v2 = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 67;
                v1.SchoolCode = "AJnJCwG2";
                v1.SchoolName = "8yYfaL";
                v1.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v1.Remark = "dqJpz640MQVe";
                v1.Level = 6;
                v1.PlaceId = AddCity();
                v2.ID = 20;
                v2.SchoolCode = "SDDdXyA";
                v2.SchoolName = "4eq2hEDVlTMiG1vBcMFTkztiR5P";
                v2.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v2.Remark = "gfCmDJmzOpO2vKYT33";
                v2.Level = 10;
                v2.PlaceId = v1.PlaceId; 
                context.Set<School>().Add(v1);
                context.Set<School>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<School>().Find(v1.ID);
                var data2 = context.Set<School>().Find(v2.ID);
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

                v.Name = "vwP5TAXtnw0Rea4b5";
                v.Level = 5;
                context.Set<City>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
