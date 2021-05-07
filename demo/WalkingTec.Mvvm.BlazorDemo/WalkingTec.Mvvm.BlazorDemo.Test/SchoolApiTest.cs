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
            
            v.ID = 65;
            v.SchoolCode = "iWWDf4m";
            v.SchoolName = "lq28aB";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
            v.Remark = "oitbPoM6";
            v.PhotoId = AddPhoto();
            v.FileId = AddFile();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 65);
                Assert.AreEqual(data.SchoolCode, "iWWDf4m");
                Assert.AreEqual(data.SchoolName, "lq28aB");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "oitbPoM6");
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
       			
                v.ID = 65;
                v.SchoolCode = "iWWDf4m";
                v.SchoolName = "lq28aB";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "oitbPoM6";
                v.PhotoId = AddPhoto();
                v.FileId = AddFile();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }

            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            var oldID = v.ID;
            v = new School();
            v.ID = oldID;
       		
            v.SchoolCode = "nzXz8AFqB";
            v.SchoolName = "8XTytqhmX";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
            v.Remark = "WNReYCb";
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
 				
                Assert.AreEqual(data.SchoolCode, "nzXz8AFqB");
                Assert.AreEqual(data.SchoolName, "8XTytqhmX");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI);
                Assert.AreEqual(data.Remark, "WNReYCb");
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
        		
                v.ID = 65;
                v.SchoolCode = "iWWDf4m";
                v.SchoolName = "lq28aB";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "oitbPoM6";
                v.PhotoId = AddPhoto();
                v.FileId = AddFile();
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
				
                v1.ID = 65;
                v1.SchoolCode = "iWWDf4m";
                v1.SchoolName = "lq28aB";
                v1.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v1.Remark = "oitbPoM6";
                v1.PhotoId = AddPhoto();
                v1.FileId = AddFile();
                v2.ID = 49;
                v2.SchoolCode = "nzXz8AFqB";
                v2.SchoolName = "8XTytqhmX";
                v2.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v2.Remark = "WNReYCb";
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

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "4hip";
                v.FileExt = "oVsGQkLjM";
                v.Path = "W30lRlkIr";
                v.Length = 97;
                v.SaveMode = "0brZA";
                v.ExtraInfo = "nXGeNGf";
                v.HandlerInfo = "mUtmg8fQ";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
