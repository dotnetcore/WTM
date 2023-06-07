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
            
            v.SchoolCode = "Jn8sy";
            v.SchoolName = "7UOr6be3LAhtUnSql9SZ4zVOAguL";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
            v.Remark = "XBaLZOFxyhXMYhip9u";
            v.Level = 9;
            v.PlaceId = AddCity();
            v.IsSchool = true;
            v.PhotoId = AddFileAttachment();
            v.FileId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SchoolVue3>().Find(v.ID);
                
                Assert.AreEqual(data.SchoolCode, "Jn8sy");
                Assert.AreEqual(data.SchoolName, "7UOr6be3LAhtUnSql9SZ4zVOAguL");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "XBaLZOFxyhXMYhip9u");
                Assert.AreEqual(data.Level, 9);
                Assert.AreEqual(data.IsSchool, true);
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
       			
                v.SchoolCode = "Jn8sy";
                v.SchoolName = "7UOr6be3LAhtUnSql9SZ4zVOAguL";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "XBaLZOFxyhXMYhip9u";
                v.Level = 9;
                v.PlaceId = AddCity();
                v.IsSchool = true;
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
                context.Set<SchoolVue3>().Add(v);
                context.SaveChanges();
            }

            SchoolVue3VM vm = _controller.Wtm.CreateVM<SchoolVue3VM>();
            var oldID = v.ID;
            v = new SchoolVue3();
            v.ID = oldID;
       		
            v.SchoolCode = "fQfL535tt";
            v.SchoolName = "p6ExjfwiqCeV870OcUYJQcdZ";
            v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
            v.Remark = "7LwH0n";
            v.Level = 10;
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
 				
                Assert.AreEqual(data.SchoolCode, "fQfL535tt");
                Assert.AreEqual(data.SchoolName, "p6ExjfwiqCeV870OcUYJQcdZ");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "7LwH0n");
                Assert.AreEqual(data.Level, 10);
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
        		
                v.SchoolCode = "Jn8sy";
                v.SchoolName = "7UOr6be3LAhtUnSql9SZ4zVOAguL";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v.Remark = "XBaLZOFxyhXMYhip9u";
                v.Level = 9;
                v.PlaceId = AddCity();
                v.IsSchool = true;
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
				
                v1.SchoolCode = "Jn8sy";
                v1.SchoolName = "7UOr6be3LAhtUnSql9SZ4zVOAguL";
                v1.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PUB;
                v1.Remark = "XBaLZOFxyhXMYhip9u";
                v1.Level = 9;
                v1.PlaceId = AddCity();
                v1.IsSchool = true;
                v1.PhotoId = AddFileAttachment();
                v1.FileId = AddFileAttachment();
                v2.SchoolCode = "fQfL535tt";
                v2.SchoolName = "p6ExjfwiqCeV870OcUYJQcdZ";
                v2.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v2.Remark = "7LwH0n";
                v2.Level = 10;
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

                v.Name = "KJb9EEfba35qV61D7X";
                v.Level = 78;
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

                v.FileName = "VjVmYppy8KC9sQW5hhW";
                v.FileExt = "Ua5hJjF1U";
                v.Path = "Faq3wNeHFtcDWx5wuj";
                v.Length = 99;
                v.UploadTime = DateTime.Parse("2023-08-14 22:48:12");
                v.SaveMode = "V6y";
                v.ExtraInfo = "0y0HW4M2CnLQWXIF";
                v.HandlerInfo = "5wYdeDVnW7I7K1Q";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
