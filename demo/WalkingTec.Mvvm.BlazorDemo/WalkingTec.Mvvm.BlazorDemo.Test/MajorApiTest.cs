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
            
            v.MajorCode = "67Eezm";
            v.MajorName = "2GDXxRt";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
            v.Remark = "XSF";
            v.SchoolId = AddSchool();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
                
                Assert.AreEqual(data.MajorCode, "67Eezm");
                Assert.AreEqual(data.MajorName, "2GDXxRt");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required);
                Assert.AreEqual(data.Remark, "XSF");
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
       			
                v.MajorCode = "67Eezm";
                v.MajorName = "2GDXxRt";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
                v.Remark = "XSF";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            MajorVM vm = _controller.Wtm.CreateVM<MajorVM>();
            var oldID = v.ID;
            v = new Major();
            v.ID = oldID;
       		
            v.MajorCode = "meDumf";
            v.MajorName = "SydF";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
            v.Remark = "TY5bnEoRn";
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
 				
                Assert.AreEqual(data.MajorCode, "meDumf");
                Assert.AreEqual(data.MajorName, "SydF");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required);
                Assert.AreEqual(data.Remark, "TY5bnEoRn");
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
        		
                v.MajorCode = "67Eezm";
                v.MajorName = "2GDXxRt";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
                v.Remark = "XSF";
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
				
                v1.MajorCode = "67Eezm";
                v1.MajorName = "2GDXxRt";
                v1.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
                v1.Remark = "XSF";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "meDumf";
                v2.MajorName = "SydF";
                v2.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
                v2.Remark = "TY5bnEoRn";
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

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "6CTGjCH";
                v.FileExt = "D1Qs9fn5";
                v.Path = "B1ozU1P1";
                v.Length = 22;
                v.SaveMode = "Vtr";
                v.ExtraInfo = "pihq";
                v.HandlerInfo = "p8mQc";
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

                v.ID = 8;
                v.SchoolCode = "1KQFS";
                v.SchoolName = "XIbRGe2";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "cqFS";
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
