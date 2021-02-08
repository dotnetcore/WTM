using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo.Controllers;
using WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.ReactDemo;

namespace WalkingTec.Mvvm.ReactDemo.Test
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
            
            v.ID = 73;
            v.SchoolCode = "zIkWK";
            v.SchoolName = "aRgtrRcN";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
            v.Remark = "SxqAn97TF";
            v.Level = 9;
            v.PlaceId = AddPlace();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 73);
                Assert.AreEqual(data.SchoolCode, "zIkWK");
                Assert.AreEqual(data.SchoolName, "aRgtrRcN");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "SxqAn97TF");
                Assert.AreEqual(data.Level, 9);
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
       			
                v.ID = 73;
                v.SchoolCode = "zIkWK";
                v.SchoolName = "aRgtrRcN";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v.Remark = "SxqAn97TF";
                v.Level = 9;
                v.PlaceId = AddPlace();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }

            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            var oldID = v.ID;
            v = new School();
            v.ID = oldID;
       		
            v.SchoolCode = "n4RBR7FU";
            v.SchoolName = "kxP69d";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
            v.Remark = "M9G";
            v.Level = 8;
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
 				
                Assert.AreEqual(data.SchoolCode, "n4RBR7FU");
                Assert.AreEqual(data.SchoolName, "kxP69d");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "M9G");
                Assert.AreEqual(data.Level, 8);
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
        		
                v.ID = 73;
                v.SchoolCode = "zIkWK";
                v.SchoolName = "aRgtrRcN";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v.Remark = "SxqAn97TF";
                v.Level = 9;
                v.PlaceId = AddPlace();
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
				
                v1.ID = 73;
                v1.SchoolCode = "zIkWK";
                v1.SchoolName = "aRgtrRcN";
                v1.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v1.Remark = "SxqAn97TF";
                v1.Level = 9;
                v1.PlaceId = AddPlace();
                v2.ID = 55;
                v2.SchoolCode = "n4RBR7FU";
                v2.SchoolName = "kxP69d";
                v2.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v2.Remark = "M9G";
                v2.Level = 8;
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

        private Guid AddPlace()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Name = "HV4jUi";
                v.Level = 39;
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
