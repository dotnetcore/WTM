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
			
            v.ID = "LmUA2Xk";
            v.Password = "Hme4r4";
            v.Email = "PeSV";
            v.Name = "gryLVdAj";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.CellPhone = "59Dfah";
            v.Address = "xUN4wj4M";
            v.ZipCode = "ySf1ozFZI";
            v.PhotoId = AddPhoto();
            v.IsValid = true;
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
				
                Assert.AreEqual(data.ID, "LmUA2Xk");
                Assert.AreEqual(data.Password, "Hme4r4");
                Assert.AreEqual(data.Email, "PeSV");
                Assert.AreEqual(data.Name, "gryLVdAj");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.CellPhone, "59Dfah");
                Assert.AreEqual(data.Address, "xUN4wj4M");
                Assert.AreEqual(data.ZipCode, "ySf1ozFZI");
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
       			
                v.ID = "LmUA2Xk";
                v.Password = "Hme4r4";
                v.Email = "PeSV";
                v.Name = "gryLVdAj";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.CellPhone = "59Dfah";
                v.Address = "xUN4wj4M";
                v.ZipCode = "ySf1ozFZI";
                v.PhotoId = AddPhoto();
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(StudentVM));

            StudentVM vm = rv.Model as StudentVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new Student();
            v.ID = vm.Entity.ID;
       		
            v.Password = "aFM";
            v.Email = "VJK1ew";
            v.Name = "RzQH8WpH";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "EF6xefMGq";
            v.Address = "ScO2k9r";
            v.ZipCode = "vSKRl8B6";
            v.IsValid = false;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.Password", "");
            vm.FC.Add("Entity.Email", "");
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Sex", "");
            vm.FC.Add("Entity.CellPhone", "");
            vm.FC.Add("Entity.Address", "");
            vm.FC.Add("Entity.ZipCode", "");
            vm.FC.Add("Entity.PhotoId", "");
            vm.FC.Add("Entity.IsValid", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
 				
                Assert.AreEqual(data.Password, "aFM");
                Assert.AreEqual(data.Email, "VJK1ew");
                Assert.AreEqual(data.Name, "RzQH8WpH");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "EF6xefMGq");
                Assert.AreEqual(data.Address, "ScO2k9r");
                Assert.AreEqual(data.ZipCode, "vSKRl8B6");
                Assert.AreEqual(data.IsValid, false);
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
        		
                v.ID = "LmUA2Xk";
                v.Password = "Hme4r4";
                v.Email = "PeSV";
                v.Name = "gryLVdAj";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.CellPhone = "59Dfah";
                v.Address = "xUN4wj4M";
                v.ZipCode = "ySf1ozFZI";
                v.PhotoId = AddPhoto();
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
                var data = context.Set<Student>().Find(v.ID);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ID = "LmUA2Xk";
                v.Password = "Hme4r4";
                v.Email = "PeSV";
                v.Name = "gryLVdAj";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.CellPhone = "59Dfah";
                v.Address = "xUN4wj4M";
                v.ZipCode = "ySf1ozFZI";
                v.PhotoId = AddPhoto();
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
				
                v1.ID = "LmUA2Xk";
                v1.Password = "Hme4r4";
                v1.Email = "PeSV";
                v1.Name = "gryLVdAj";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.CellPhone = "59Dfah";
                v1.Address = "xUN4wj4M";
                v1.ZipCode = "ySf1ozFZI";
                v1.PhotoId = AddPhoto();
                v1.IsValid = true;
                v2.ID = "tpHirI";
                v2.Password = "aFM";
                v2.Email = "VJK1ew";
                v2.Name = "RzQH8WpH";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v2.CellPhone = "EF6xefMGq";
                v2.Address = "ScO2k9r";
                v2.ZipCode = "vSKRl8B6";
                v2.PhotoId = v1.PhotoId; 
                v2.IsValid = false;
                context.Set<Student>().Add(v1);
                context.Set<Student>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(StudentBatchVM));

            StudentBatchVM vm = rv.Model as StudentBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Password = "MG8";
            vm.LinkedVM.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            vm.LinkedVM.ZipCode = "Ln1QKoZ";
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Password", "");
            vm.FC.Add("LinkedVM.Sex", "");
            vm.FC.Add("LinkedVM.ZipCode", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Student>().Find(v1.ID);
                var data2 = context.Set<Student>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Password, "MG8");
                Assert.AreEqual(data2.Password, "MG8");
                Assert.AreEqual(data1.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data2.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data1.ZipCode, "Ln1QKoZ");
                Assert.AreEqual(data2.ZipCode, "Ln1QKoZ");
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
				
                v1.ID = "LmUA2Xk";
                v1.Password = "Hme4r4";
                v1.Email = "PeSV";
                v1.Name = "gryLVdAj";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.CellPhone = "59Dfah";
                v1.Address = "xUN4wj4M";
                v1.ZipCode = "ySf1ozFZI";
                v1.PhotoId = AddPhoto();
                v1.IsValid = true;
                v2.ID = "tpHirI";
                v2.Password = "aFM";
                v2.Email = "VJK1ew";
                v2.Name = "RzQH8WpH";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v2.CellPhone = "EF6xefMGq";
                v2.Address = "ScO2k9r";
                v2.ZipCode = "vSKRl8B6";
                v2.PhotoId = v1.PhotoId; 
                v2.IsValid = false;
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
                var data1 = context.Set<Student>().Find(v1.ID);
                var data2 = context.Set<Student>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
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

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "v00ImvMXp";
                v.FileExt = "7Ih";
                v.Path = "acrFM";
                v.Length = 58;
                v.SaveMode = "JQkLP4";
                v.ExtraInfo = "5KO40o7V";
                v.HandlerInfo = "e79eU";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
