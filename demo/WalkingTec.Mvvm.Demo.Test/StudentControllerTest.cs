using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.StudentVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class StudentControllerTest
    {
        private StudentController _controller;
        private string _seed;

        public StudentControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<StudentController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as StudentListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(StudentVM));

            StudentVM vm = rv.Model as StudentVM;
            Student v = new Student();
			
            v.ID = "Kn3sTt5dn";
            v.Password = "BUeKyVtmQ";
            v.Name = "s2L";
            v.IsValid = true;
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().FirstOrDefault();
				
                Assert.AreEqual(data.ID, "Kn3sTt5dn");
                Assert.AreEqual(data.Password, "BUeKyVtmQ");
                Assert.AreEqual(data.Name, "s2L");
                Assert.AreEqual(data.IsValid, true);
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ID = "Kn3sTt5dn";
                v.Password = "BUeKyVtmQ";
                v.Name = "s2L";
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(StudentVM));

            StudentVM vm = rv.Model as StudentVM;
            v = new Student();
            v.ID = vm.Entity.ID;
       		
            v.Password = "upCfHJ";
            v.Name = "JT5";
            v.IsValid = true;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.Password", "");
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.IsValid", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().FirstOrDefault();
 				
                Assert.AreEqual(data.Password, "upCfHJ");
                Assert.AreEqual(data.Name, "JT5");
                Assert.AreEqual(data.IsValid, true);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = "Kn3sTt5dn";
                v.Password = "BUeKyVtmQ";
                v.Name = "s2L";
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(StudentVM));

            StudentVM vm = rv.Model as StudentVM;
            v = new Student();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<Student>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ID = "Kn3sTt5dn";
                v.Password = "BUeKyVtmQ";
                v.Name = "s2L";
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            Student v1 = new Student();
            Student v2 = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = "Kn3sTt5dn";
                v1.Password = "BUeKyVtmQ";
                v1.Name = "s2L";
                v1.IsValid = true;
                v2.ID = "KYQz3t";
                v2.Password = "upCfHJ";
                v2.Name = "JT5";
                v2.IsValid = true;
                context.Set<Student>().Add(v1);
                context.Set<Student>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(StudentBatchVM));

            StudentBatchVM vm = rv.Model as StudentBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Password = "5bYPJb";
            vm.LinkedVM.Name = "imQZR";
            vm.LinkedVM.IsValid = true;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Password", "");
            vm.FC.Add("LinkedVM.Name", "");
            vm.FC.Add("LinkedVM.IsValid", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Student>().Find(v1.ID);
                var data2 = context.Set<Student>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Password, "5bYPJb");
                Assert.AreEqual(data2.Password, "5bYPJb");
                Assert.AreEqual(data1.Name, "imQZR");
                Assert.AreEqual(data2.Name, "imQZR");
                Assert.AreEqual(data1.IsValid, true);
                Assert.AreEqual(data2.IsValid, true);
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            Student v1 = new Student();
            Student v2 = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = "Kn3sTt5dn";
                v1.Password = "BUeKyVtmQ";
                v1.Name = "s2L";
                v1.IsValid = true;
                v2.ID = "KYQz3t";
                v2.Password = "upCfHJ";
                v2.Name = "JT5";
                v2.IsValid = true;
                context.Set<Student>().Add(v1);
                context.Set<Student>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(StudentBatchVM));

            StudentBatchVM vm = rv.Model as StudentBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<Student>().Count(), 0);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as StudentListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }


    }
}
