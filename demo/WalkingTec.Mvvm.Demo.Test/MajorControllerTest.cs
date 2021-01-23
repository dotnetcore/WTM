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
    public class MajorControllerTest
    {
        private MajorController _controller;
        private string _seed;

        public MajorControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<MajorController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as MajorListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(MajorVM));

            MajorVM vm = rv.Model as MajorVM;
            Major v = new Major();
			
            v.MajorCode = "7oW";
            v.MajorName = "31Kk00X8S";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
            v.Remark = "OQDEf8";
            v.SchoolId = AddSchool();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
				
                Assert.AreEqual(data.MajorCode, "7oW");
                Assert.AreEqual(data.MajorName, "31Kk00X8S");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "OQDEf8");
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
       			
                v.MajorCode = "7oW";
                v.MajorName = "31Kk00X8S";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "OQDEf8";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(MajorVM));

            MajorVM vm = rv.Model as MajorVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new Major();
            v.ID = vm.Entity.ID;
       		
            v.MajorCode = "1gVy";
            v.MajorName = "dq9GN";
            v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
            v.Remark = "jxT";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.MajorCode", "");
            vm.FC.Add("Entity.MajorName", "");
            vm.FC.Add("Entity.MajorType", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.SchoolId", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
 				
                Assert.AreEqual(data.MajorCode, "1gVy");
                Assert.AreEqual(data.MajorName, "dq9GN");
                Assert.AreEqual(data.MajorType, WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional);
                Assert.AreEqual(data.Remark, "jxT");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            Major v = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.MajorCode = "7oW";
                v.MajorName = "31Kk00X8S";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "OQDEf8";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(MajorVM));

            MajorVM vm = rv.Model as MajorVM;
            v = new Major();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Major>().Find(v.ID);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            Major v = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.MajorCode = "7oW";
                v.MajorName = "31Kk00X8S";
                v.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v.Remark = "OQDEf8";
                v.SchoolId = AddSchool();
                context.Set<Major>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            Major v1 = new Major();
            Major v2 = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.MajorCode = "7oW";
                v1.MajorName = "31Kk00X8S";
                v1.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v1.Remark = "OQDEf8";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "1gVy";
                v2.MajorName = "dq9GN";
                v2.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v2.Remark = "jxT";
                v2.SchoolId = v1.SchoolId; 
                context.Set<Major>().Add(v1);
                context.Set<Major>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(MajorBatchVM));

            MajorBatchVM vm = rv.Model as MajorBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Remark = "n57";
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Remark", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Major>().Find(v1.ID);
                var data2 = context.Set<Major>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Remark, "n57");
                Assert.AreEqual(data2.Remark, "n57");
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            Major v1 = new Major();
            Major v2 = new Major();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.MajorCode = "7oW";
                v1.MajorName = "31Kk00X8S";
                v1.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v1.Remark = "OQDEf8";
                v1.SchoolId = AddSchool();
                v2.MajorCode = "1gVy";
                v2.MajorName = "dq9GN";
                v2.MajorType = WalkingTec.Mvvm.Demo.Models.MajorTypeEnum.Optional;
                v2.Remark = "jxT";
                v2.SchoolId = v1.SchoolId; 
                context.Set<Major>().Add(v1);
                context.Set<Major>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(MajorBatchVM));

            MajorBatchVM vm = rv.Model as MajorBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Major>().Find(v1.ID);
                var data2 = context.Set<Major>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as MajorListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Int32 AddSchool()
        {
            School v = new School();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.ID = 96;
                v.SchoolCode = "3ejO";
                v.SchoolName = "1sAsCzx";
                v.SchoolType = WalkingTec.Mvvm.Demo.Models.SchoolTypeEnum.PRI;
                v.Remark = "C4rs1Lr33";
                v.Level = 65;
                context.Set<School>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
