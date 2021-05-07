using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.StudentVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
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
            
            v.ID = "APV9MZ6L";
            v.Password = "gkEJ2aJi";
            v.Email = "Tgp34";
            v.Name = "cs2yY";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "RQ3xuJ2vo";
            v.Address = "E1Y6Y";
            v.ZipCode = "3rw2";
            v.PhotoId = AddPhoto();
            v.IsValid = true;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
                
                Assert.AreEqual(data.ID, "APV9MZ6L");
                Assert.AreEqual(data.Password, "gkEJ2aJi");
                Assert.AreEqual(data.Email, "Tgp34");
                Assert.AreEqual(data.Name, "cs2yY");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "RQ3xuJ2vo");
                Assert.AreEqual(data.Address, "E1Y6Y");
                Assert.AreEqual(data.ZipCode, "3rw2");
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
       			
                v.ID = "APV9MZ6L";
                v.Password = "gkEJ2aJi";
                v.Email = "Tgp34";
                v.Name = "cs2yY";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "RQ3xuJ2vo";
                v.Address = "E1Y6Y";
                v.ZipCode = "3rw2";
                v.PhotoId = AddPhoto();
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            StudentVM vm = _controller.Wtm.CreateVM<StudentVM>();
            var oldID = v.ID;
            v = new Student();
            v.ID = oldID;
       		
            v.Password = "ROJcrAX";
            v.Email = "g0hYIfgw";
            v.Name = "Qyqz6";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "03S3Y";
            v.Address = "1ajFtn";
            v.ZipCode = "44Z";
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
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
 				
                Assert.AreEqual(data.Password, "ROJcrAX");
                Assert.AreEqual(data.Email, "g0hYIfgw");
                Assert.AreEqual(data.Name, "Qyqz6");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "03S3Y");
                Assert.AreEqual(data.Address, "1ajFtn");
                Assert.AreEqual(data.ZipCode, "44Z");
                Assert.AreEqual(data.IsValid, false);
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
        		
                v.ID = "APV9MZ6L";
                v.Password = "gkEJ2aJi";
                v.Email = "Tgp34";
                v.Name = "cs2yY";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "RQ3xuJ2vo";
                v.Address = "E1Y6Y";
                v.ZipCode = "3rw2";
                v.PhotoId = AddPhoto();
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
				
                v1.ID = "APV9MZ6L";
                v1.Password = "gkEJ2aJi";
                v1.Email = "Tgp34";
                v1.Name = "cs2yY";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v1.CellPhone = "RQ3xuJ2vo";
                v1.Address = "E1Y6Y";
                v1.ZipCode = "3rw2";
                v1.PhotoId = AddPhoto();
                v1.IsValid = true;
                v2.ID = "rBORobvBK";
                v2.Password = "ROJcrAX";
                v2.Email = "g0hYIfgw";
                v2.Name = "Qyqz6";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v2.CellPhone = "03S3Y";
                v2.Address = "1ajFtn";
                v2.ZipCode = "44Z";
                v2.PhotoId = v1.PhotoId; 
                v2.IsValid = false;
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

                v.FileName = "1InlyMG";
                v.FileExt = "4CkG51";
                v.Path = "wF6";
                v.Length = 93;
                v.SaveMode = "gI8o";
                v.ExtraInfo = "WFwpD2L";
                v.HandlerInfo = "oE22PWB";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
