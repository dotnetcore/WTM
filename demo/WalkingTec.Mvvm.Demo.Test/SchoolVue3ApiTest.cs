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
            
            v.SchoolCode = "B3GdSl9piEMoRWDZkA";
            v.SchoolName = "bF2O8VZnXITBEcXDhgn6oPFQKATmjXj30JD3gH";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
            v.Remark = "r9LGL4cVyAQgv6i3O";
            v.Level = 71;
            v.PlaceId = AddCity();
            v.IsSchool = false;
            v.PhotoId = AddFileAttachment();
            v.FileId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SchoolVue3>().Find(v.ID);
                
                Assert.AreEqual(data.SchoolCode, "B3GdSl9piEMoRWDZkA");
                Assert.AreEqual(data.SchoolName, "bF2O8VZnXITBEcXDhgn6oPFQKATmjXj30JD3gH");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "r9LGL4cVyAQgv6i3O");
                Assert.AreEqual(data.Level, 71);
                Assert.AreEqual(data.IsSchool, false);
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
       			
                v.SchoolCode = "B3GdSl9piEMoRWDZkA";
                v.SchoolName = "bF2O8VZnXITBEcXDhgn6oPFQKATmjXj30JD3gH";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "r9LGL4cVyAQgv6i3O";
                v.Level = 71;
                v.PlaceId = AddCity();
                v.IsSchool = false;
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
                context.Set<SchoolVue3>().Add(v);
                context.SaveChanges();
            }

            SchoolVue3VM vm = _controller.Wtm.CreateVM<SchoolVue3VM>();
            var oldID = v.ID;
            v = new SchoolVue3();
            v.ID = oldID;
       		
            v.SchoolCode = "3p3mNBAx9PlhPm";
            v.SchoolName = "QyhADkFKfvxqlgZ0oi3kuN0KN";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
            v.Remark = "xct92M8PY";
            v.Level = 42;
            v.IsSchool = false;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.SchoolCode", "");
            vm.FC.Add("Entity.SchoolName", "");
            vm.FC.Add("Entity.SchoolType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.Level", "");
            vm.FC.Add("Entity.PlaceId", "");
            vm.FC.Add("Entity.IsSchool", "");
            vm.FC.Add("Entity.PhotoId", "");
            vm.FC.Add("Entity.FileId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SchoolVue3>().Find(v.ID);
 				
                Assert.AreEqual(data.SchoolCode, "3p3mNBAx9PlhPm");
                Assert.AreEqual(data.SchoolName, "QyhADkFKfvxqlgZ0oi3kuN0KN");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "xct92M8PY");
                Assert.AreEqual(data.Level, 42);
                Assert.AreEqual(data.IsSchool, false);
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
        		
                v.SchoolCode = "B3GdSl9piEMoRWDZkA";
                v.SchoolName = "bF2O8VZnXITBEcXDhgn6oPFQKATmjXj30JD3gH";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "r9LGL4cVyAQgv6i3O";
                v.Level = 71;
                v.PlaceId = AddCity();
                v.IsSchool = false;
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
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
				
                v1.SchoolCode = "B3GdSl9piEMoRWDZkA";
                v1.SchoolName = "bF2O8VZnXITBEcXDhgn6oPFQKATmjXj30JD3gH";
                v1.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v1.Remark = "r9LGL4cVyAQgv6i3O";
                v1.Level = 71;
                v1.PlaceId = AddCity();
                v1.IsSchool = false;
                v1.PhotoId = AddFileAttachment();
                v1.FileId = AddFileAttachment();
                v2.SchoolCode = "3p3mNBAx9PlhPm";
                v2.SchoolName = "QyhADkFKfvxqlgZ0oi3kuN0KN";
                v2.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v2.Remark = "xct92M8PY";
                v2.Level = 42;
                v2.PlaceId = v1.PlaceId; 
                v2.IsSchool = false;
                v2.PhotoId = v1.PhotoId; 
                v2.FileId = v1.FileId; 
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

                v.Name = "ZfA";
                v.Level = 62;
                context.Set<City>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.FileName = "L8xaKFDJuHrRATz";
                v.FileExt = "r";
                v.Path = "RNia";
                v.Length = 82;
                v.UploadTime = DateTime.Parse("2023-10-24 22:31:48");
                v.SaveMode = "ku7O5";
                v.ExtraInfo = "OE2JTaMkfrWjv9oV";
                v.HandlerInfo = "lnr8JY7IQKVf8QdDO";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
