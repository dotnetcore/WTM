using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo.Controllers;
using WalkingTec.Mvvm.ReactDemo.ViewModels.MajorVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.ReactDemo;

namespace WalkingTec.Mvvm.ReactDemo.Test
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
            
            v.MajorCode = "fsY4rlg";
            v.MajorName = "ZfzziuvGm";
            v.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
            v.Remark = "2gmb";
            v.SchoolId = AddSchool();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
                
                Assert.AreEqual(data.MajorCode, "fsY4rlg");
                Assert.AreEqual(data.MajorName, "ZfzziuvGm");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "2gmb");
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
       			
                v.MajorCode = "fsY4rlg";
                v.MajorName = "ZfzziuvGm";
                v.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
                v.Remark = "2gmb";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            MajorVM vm = _controller.Wtm.CreateVM<MajorVM>();
            var oldID = v.ID;
            v = new Major();
            v.ID = oldID;
       		
            v.MajorCode = "Wpp";
            v.MajorName = "VBvm";
            v.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
            v.Remark = "Xv9HtZp";
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
 				
                Assert.AreEqual(data.MajorCode, "Wpp");
                Assert.AreEqual(data.MajorName, "VBvm");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "Xv9HtZp");
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
        		
                v.MajorCode = "fsY4rlg";
                v.MajorName = "ZfzziuvGm";
                v.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
                v.Remark = "2gmb";
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
				
                v1.MajorCode = "fsY4rlg";
                v1.MajorName = "ZfzziuvGm";
                v1.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
                v1.Remark = "2gmb";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "Wpp";
                v2.MajorName = "VBvm";
                v2.MajorType = WalkingTec.Mvvm.ReactDemo.Models.MajorTypeEnum.Optional;
                v2.Remark = "Xv9HtZp";
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

                v.ID = 61;
                v.SchoolCode = "ilf3";
                v.SchoolName = "nfocBB";
                v.SchoolType = WalkingTec.Mvvm.ReactDemo.Models.SchoolTypeEnum.PRI;
                v.Remark = "I3QGJCnXU";
                v.Level = 35;
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
