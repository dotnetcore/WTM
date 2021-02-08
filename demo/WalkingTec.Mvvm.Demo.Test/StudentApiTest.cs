using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo.Controllers;
using WalkingTec.Mvvm.ReactDemo.ViewModels.StudentVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.ReactDemo;

namespace WalkingTec.Mvvm.ReactDemo.Test
{
    [TestClass]
    public class StudentApiTest
    {
        private StudentController _controller;
        private string _seed;

        public StudentApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<StudentController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new StudentSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            StudentVM vm = _controller.Wtm.CreateVM<StudentVM>();
            Student v = new Student();
            
            v.ID = "NonLj8";
            v.Password = "zxrLukxr";
            v.Email = "rexmv6lG8";
            v.Name = "7zG9j1pzs";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "sYQE";
            v.Address = "jOZigz";
            v.ZipCode = "KPx96nPIE";
            v.PhotoId = AddPhoto();
            v.FileId = AddFile();
            v.IsValid = true;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
                
                Assert.AreEqual(data.ID, "NonLj8");
                Assert.AreEqual(data.Password, "zxrLukxr");
                Assert.AreEqual(data.Email, "rexmv6lG8");
                Assert.AreEqual(data.Name, "7zG9j1pzs");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "sYQE");
                Assert.AreEqual(data.Address, "jOZigz");
                Assert.AreEqual(data.ZipCode, "KPx96nPIE");
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
       			
                v.ID = "NonLj8";
                v.Password = "zxrLukxr";
                v.Email = "rexmv6lG8";
                v.Name = "7zG9j1pzs";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "sYQE";
                v.Address = "jOZigz";
                v.ZipCode = "KPx96nPIE";
                v.PhotoId = AddPhoto();
                v.FileId = AddFile();
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            StudentVM vm = _controller.Wtm.CreateVM<StudentVM>();
            var oldID = v.ID;
            v = new Student();
            v.ID = oldID;
       		
            v.Password = "W4R2dh";
            v.Email = "Eid";
            v.Name = "nlVMKw";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.CellPhone = "Pb1g";
            v.Address = "WvUkBgHz";
            v.ZipCode = "BdiNo";
            v.IsValid = true;
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
            vm.FC.Add("Entity.FileId", "");
            vm.FC.Add("Entity.IsValid", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
 				
                Assert.AreEqual(data.Password, "W4R2dh");
                Assert.AreEqual(data.Email, "Eid");
                Assert.AreEqual(data.Name, "nlVMKw");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.CellPhone, "Pb1g");
                Assert.AreEqual(data.Address, "WvUkBgHz");
                Assert.AreEqual(data.ZipCode, "BdiNo");
                Assert.AreEqual(data.IsValid, true);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = "NonLj8";
                v.Password = "zxrLukxr";
                v.Email = "rexmv6lG8";
                v.Name = "7zG9j1pzs";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "sYQE";
                v.Address = "jOZigz";
                v.ZipCode = "KPx96nPIE";
                v.PhotoId = AddPhoto();
                v.FileId = AddFile();
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            Student v1 = new Student();
            Student v2 = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = "NonLj8";
                v1.Password = "zxrLukxr";
                v1.Email = "rexmv6lG8";
                v1.Name = "7zG9j1pzs";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v1.CellPhone = "sYQE";
                v1.Address = "jOZigz";
                v1.ZipCode = "KPx96nPIE";
                v1.PhotoId = AddPhoto();
                v1.FileId = AddFile();
                v1.IsValid = true;
                v2.ID = "mjGIdX";
                v2.Password = "W4R2dh";
                v2.Email = "Eid";
                v2.Name = "nlVMKw";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.CellPhone = "Pb1g";
                v2.Address = "WvUkBgHz";
                v2.ZipCode = "BdiNo";
                v2.PhotoId = v1.PhotoId; 
                v2.FileId = v1.FileId; 
                v2.IsValid = true;
                context.Set<Student>().Add(v1);
                context.Set<Student>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Student>().Find(v1.ID);
                var data2 = context.Set<Student>().Find(v2.ID);
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

                v.FileName = "hl15pLQ";
                v.FileExt = "sfh6A6k81";
                v.Path = "yxGeK0x";
                v.Length = 91;
                v.SaveMode = "k4mx7o";
                v.ExtraInfo = "W6bEnQs";
                v.HandlerInfo = "u7Hx";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

        private Guid AddFile()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "FsyjrAsnZ";
                v.FileExt = "aM8";
                v.Path = "RM5gwPm";
                v.Length = 96;
                v.SaveMode = "xfmc";
                v.ExtraInfo = "aGr";
                v.HandlerInfo = "lq8igA";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
