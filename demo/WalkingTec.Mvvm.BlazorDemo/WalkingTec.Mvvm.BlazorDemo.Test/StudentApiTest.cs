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
            
            v.ID = "OeeYQDD4r";
            v.Password = "fAgazy5R";
            v.Email = "eLXQUxW";
            v.Name = "Wb1ibH";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.CellPhone = "orJ";
            v.Address = "jzFj";
            v.ZipCode = "pI6X25dv";
            v.PhotoId = AddFileAttachment();
            v.IsValid = true;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
                
                Assert.AreEqual(data.ID, "OeeYQDD4r");
                Assert.AreEqual(data.Password, "fAgazy5R");
                Assert.AreEqual(data.Email, "eLXQUxW");
                Assert.AreEqual(data.Name, "Wb1ibH");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.CellPhone, "orJ");
                Assert.AreEqual(data.Address, "jzFj");
                Assert.AreEqual(data.ZipCode, "pI6X25dv");
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
       			
                v.ID = "OeeYQDD4r";
                v.Password = "fAgazy5R";
                v.Email = "eLXQUxW";
                v.Name = "Wb1ibH";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.CellPhone = "orJ";
                v.Address = "jzFj";
                v.ZipCode = "pI6X25dv";
                v.PhotoId = AddFileAttachment();
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            StudentVM vm = _controller.Wtm.CreateVM<StudentVM>();
            var oldID = v.ID;
            v = new Student();
            v.ID = oldID;
       		
            v.Password = "M618xY";
            v.Email = "4rsZnVM";
            v.Name = "jpqDL";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "PDHo";
            v.Address = "0t14";
            v.ZipCode = "lFd";
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
 				
                Assert.AreEqual(data.Password, "M618xY");
                Assert.AreEqual(data.Email, "4rsZnVM");
                Assert.AreEqual(data.Name, "jpqDL");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "PDHo");
                Assert.AreEqual(data.Address, "0t14");
                Assert.AreEqual(data.ZipCode, "lFd");
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
        		
                v.ID = "OeeYQDD4r";
                v.Password = "fAgazy5R";
                v.Email = "eLXQUxW";
                v.Name = "Wb1ibH";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.CellPhone = "orJ";
                v.Address = "jzFj";
                v.ZipCode = "pI6X25dv";
                v.PhotoId = AddFileAttachment();
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
				
                v1.ID = "OeeYQDD4r";
                v1.Password = "fAgazy5R";
                v1.Email = "eLXQUxW";
                v1.Name = "Wb1ibH";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.CellPhone = "orJ";
                v1.Address = "jzFj";
                v1.ZipCode = "pI6X25dv";
                v1.PhotoId = AddFileAttachment();
                v1.IsValid = true;
                v2.ID = "DS5Hwe8i";
                v2.Password = "M618xY";
                v2.Email = "4rsZnVM";
                v2.Name = "jpqDL";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v2.CellPhone = "PDHo";
                v2.Address = "0t14";
                v2.ZipCode = "lFd";
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

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "JHVR";
                v.FileExt = "pKtzB";
                v.Path = "OuiCgwi";
                v.Length = 88;
                v.SaveMode = "6aOrWth1";
                v.ExtraInfo = "hn1GBF";
                v.HandlerInfo = "A5wLih";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
