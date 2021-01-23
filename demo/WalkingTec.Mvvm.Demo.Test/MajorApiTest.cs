using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.MajorVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class MajorApiTest
    {
        private MajorApiController _controller;
        private string _seed;

        public MajorApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<MajorApiController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new MajorApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            MajorApiVM vm = _controller.Wtm.CreateVM<MajorApiVM>();
            Major v = new Major();
            
            v.MajorCode = "O46MmQ";
            v.MajorName = "4y4YrB";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
            v.Remark = "t4HIXlO";
            v.SchoolId = AddSchool();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
                
                Assert.AreEqual(data.MajorCode, "O46MmQ");
                Assert.AreEqual(data.MajorName, "4y4YrB");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "t4HIXlO");
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
       			
                v.MajorCode = "O46MmQ";
                v.MajorName = "4y4YrB";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "t4HIXlO";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            MajorApiVM vm = _controller.Wtm.CreateVM<MajorApiVM>();
            var oldID = v.ID;
            v = new Major();
            v.ID = oldID;
       		
            v.MajorCode = "kXJO0Ox";
            v.MajorName = "Or1E87kc";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
            v.Remark = "CvwctbX";
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
 				
                Assert.AreEqual(data.MajorCode, "kXJO0Ox");
                Assert.AreEqual(data.MajorName, "Or1E87kc");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required);
                Assert.AreEqual(data.Remark, "CvwctbX");
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
        		
                v.MajorCode = "O46MmQ";
                v.MajorName = "4y4YrB";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "t4HIXlO";
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
				
                v1.MajorCode = "O46MmQ";
                v1.MajorName = "4y4YrB";
                v1.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v1.Remark = "t4HIXlO";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "kXJO0Ox";
                v2.MajorName = "Or1E87kc";
                v2.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Required;
                v2.Remark = "CvwctbX";
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

        private Int32 AddSchool()
        {
            School v = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.ID = 13;
                v.SchoolCode = "vSsUQd";
                v.SchoolName = "3th";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v.Remark = "9Vs";
                v.Level = 24;
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
