using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.SchoolVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
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
            
            v.ID = 22;
            v.SchoolCode = "0Ma";
            v.SchoolName = "LrZ";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
            v.Remark = "adz1SdJO";
            v.PhotoId = AddFileAttachment();
            v.FileId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 22);
                Assert.AreEqual(data.SchoolCode, "0Ma");
                Assert.AreEqual(data.SchoolName, "LrZ");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "adz1SdJO");
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
       			
                v.ID = 22;
                v.SchoolCode = "0Ma";
                v.SchoolName = "LrZ";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v.Remark = "adz1SdJO";
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }

            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            var oldID = v.ID;
            v = new School();
            v.ID = oldID;
       		
            v.SchoolCode = "Iz6jn";
            v.SchoolName = "cE9T";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
            v.Remark = "mfB";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.SchoolCode", "");
            vm.FC.Add("Entity.SchoolName", "");
            vm.FC.Add("Entity.SchoolType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.PhotoId", "");
            vm.FC.Add("Entity.FileId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
 				
                Assert.AreEqual(data.SchoolCode, "Iz6jn");
                Assert.AreEqual(data.SchoolName, "cE9T");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "mfB");
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
        		
                v.ID = 22;
                v.SchoolCode = "0Ma";
                v.SchoolName = "LrZ";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v.Remark = "adz1SdJO";
                v.PhotoId = AddFileAttachment();
                v.FileId = AddFileAttachment();
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
				
                v1.ID = 22;
                v1.SchoolCode = "0Ma";
                v1.SchoolName = "LrZ";
                v1.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v1.Remark = "adz1SdJO";
                v1.PhotoId = AddFileAttachment();
                v1.FileId = AddFileAttachment();
                v2.ID = 70;
                v2.SchoolCode = "Iz6jn";
                v2.SchoolName = "cE9T";
                v2.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v2.Remark = "mfB";
                v2.PhotoId = v1.PhotoId; 
                v2.FileId = v1.FileId; 
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

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "VKb7";
                v.FileExt = "mrgWw";
                v.Path = "ribSmzfVt";
                v.Length = 56;
                v.SaveMode = "AkZPwW3Ds";
                v.ExtraInfo = "McB6J";
                v.HandlerInfo = "NMk";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
