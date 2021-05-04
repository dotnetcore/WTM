using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.SchoolVMs;
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
            
            v.ID = 58;
            v.SchoolCode = "K8nVHw6hz";
            v.SchoolName = "3JL";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
            v.Remark = "5716x";
            v.PhotoId = AddPhoto();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 58);
                Assert.AreEqual(data.SchoolCode, "K8nVHw6hz");
                Assert.AreEqual(data.SchoolName, "3JL");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "5716x");
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
       			
                v.ID = 58;
                v.SchoolCode = "K8nVHw6hz";
                v.SchoolName = "3JL";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "5716x";
                v.PhotoId = AddPhoto();
                context.Set<School>().Add(v);
                context.SaveChanges();
            }

            SchoolVM vm = _controller.Wtm.CreateVM<SchoolVM>();
            var oldID = v.ID;
            v = new School();
            v.ID = oldID;
       		
            v.SchoolCode = "UUKjMOaNb";
            v.SchoolName = "6kV4";
            v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
            v.Remark = "fjgZOc5";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.SchoolCode", "");
            vm.FC.Add("Entity.SchoolName", "");
            vm.FC.Add("Entity.SchoolType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.PhotoId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<School>().Find(v.ID);
 				
                Assert.AreEqual(data.SchoolCode, "UUKjMOaNb");
                Assert.AreEqual(data.SchoolName, "6kV4");
                Assert.AreEqual(data.SchoolType, WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB);
                Assert.AreEqual(data.Remark, "fjgZOc5");
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
        		
                v.ID = 58;
                v.SchoolCode = "K8nVHw6hz";
                v.SchoolName = "3JL";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v.Remark = "5716x";
                v.PhotoId = AddPhoto();
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
				
                v1.ID = 58;
                v1.SchoolCode = "K8nVHw6hz";
                v1.SchoolName = "3JL";
                v1.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v1.Remark = "5716x";
                v1.PhotoId = AddPhoto();
                v2.ID = 60;
                v2.SchoolCode = "UUKjMOaNb";
                v2.SchoolName = "6kV4";
                v2.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PUB;
                v2.Remark = "fjgZOc5";
                v2.PhotoId = v1.PhotoId; 
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

                v.FileName = "Jktt";
                v.FileExt = "HfjpA3YLO";
                v.Path = "tau4Z";
                v.Length = 42;
                v.SaveMode = "cJFx1Td";
                v.ExtraInfo = "SLG3nm0u";
                v.HandlerInfo = "13sqivfn2";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
