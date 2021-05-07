using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.MajorVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
{
    [TestClass]
    public class MajorApiTest
    {
        private MajorController _controller;
        private string _seed;

        public MajorApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<MajorController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new MajorSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            MajorVM vm = _controller.Wtm.CreateVM<MajorVM>();
            Major v = new Major();
            
            v.MajorCode = "RUkUgqsa6";
            v.MajorName = "H7GHS";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
            v.Remark = "asc9my9G";
            v.SchoolId = AddSchool();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
                
                Assert.AreEqual(data.MajorCode, "RUkUgqsa6");
                Assert.AreEqual(data.MajorName, "H7GHS");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "asc9my9G");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            Major v = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.MajorCode = "RUkUgqsa6";
                v.MajorName = "H7GHS";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "asc9my9G";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            MajorVM vm = _controller.Wtm.CreateVM<MajorVM>();
            var oldID = v.ID;
            v = new Major();
            v.ID = oldID;
       		
            v.MajorCode = "4XLGZmw";
            v.MajorName = "YoByxXR";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
            v.Remark = "o3Q0f";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.MajorCode", "");
            vm.FC.Add("Entity.MajorName", "");
            vm.FC.Add("Entity.MajorType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.SchoolId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
 				
                Assert.AreEqual(data.MajorCode, "4XLGZmw");
                Assert.AreEqual(data.MajorName, "YoByxXR");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "o3Q0f");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            Major v = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.MajorCode = "RUkUgqsa6";
                v.MajorName = "H7GHS";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "asc9my9G";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            Major v1 = new Major();
            Major v2 = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.MajorCode = "RUkUgqsa6";
                v1.MajorName = "H7GHS";
                v1.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v1.Remark = "asc9my9G";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "4XLGZmw";
                v2.MajorName = "YoByxXR";
                v2.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v2.Remark = "o3Q0f";
                v2.SchoolId = v1.SchoolId; 
                context.Set<Major>().Add(v1);
                context.Set<Major>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Major>().Find(v1.ID);
                var data2 = context.Set<Major>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "416W2";
                v.FileExt = "XdiB";
                v.Path = "MXaemt";
                v.Length = 39;
                v.SaveMode = "WZV";
                v.ExtraInfo = "Sc4X5";
                v.HandlerInfo = "P01FKePP";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

        private Int32 AddSchool()
        {
            School v = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.ID = 86;
                v.SchoolCode = "6fCGHLj";
                v.SchoolName = "3Hit";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "ZCf14v6y";
                v.PhotoId = AddPhoto();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
