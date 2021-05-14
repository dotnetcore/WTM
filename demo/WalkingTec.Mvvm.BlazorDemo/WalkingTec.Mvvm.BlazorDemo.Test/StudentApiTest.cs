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
            
            v.ID = "P2eM9";
            v.Password = "TyTX0V";
            v.Email = "ersxm7";
            v.Name = "Vs4";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.CellPhone = "87ZU";
            v.Address = "AJ5rL";
            v.ZipCode = "iutU5Rga";
            v.PhotoId = AddFileAttachment();
            v.IsValid = false;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().Find(v.ID);
                
                Assert.AreEqual(data.ID, "P2eM9");
                Assert.AreEqual(data.Password, "TyTX0V");
                Assert.AreEqual(data.Email, "ersxm7");
                Assert.AreEqual(data.Name, "Vs4");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.CellPhone, "87ZU");
                Assert.AreEqual(data.Address, "AJ5rL");
                Assert.AreEqual(data.ZipCode, "iutU5Rga");
                Assert.AreEqual(data.IsValid, false);
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
       			
                v.ID = "P2eM9";
                v.Password = "TyTX0V";
                v.Email = "ersxm7";
                v.Name = "Vs4";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "87ZU";
                v.Address = "AJ5rL";
                v.ZipCode = "iutU5Rga";
                v.PhotoId = AddFileAttachment();
                v.IsValid = false;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            StudentVM vm = _controller.Wtm.CreateVM<StudentVM>();
            var oldID = v.ID;
            v = new Student();
            v.ID = oldID;
       		
            v.Password = "tjs2nvvk";
            v.Email = "Qw15cTS1";
            v.Name = "R0p";
            v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.CellPhone = "atR";
            v.Address = "EwS";
            v.ZipCode = "2Rjk0aIR";
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
 				
                Assert.AreEqual(data.Password, "tjs2nvvk");
                Assert.AreEqual(data.Email, "Qw15cTS1");
                Assert.AreEqual(data.Name, "R0p");
                Assert.AreEqual(data.Sex, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.CellPhone, "atR");
                Assert.AreEqual(data.Address, "EwS");
                Assert.AreEqual(data.ZipCode, "2Rjk0aIR");
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
        		
                v.ID = "P2eM9";
                v.Password = "TyTX0V";
                v.Email = "ersxm7";
                v.Name = "Vs4";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "87ZU";
                v.Address = "AJ5rL";
                v.ZipCode = "iutU5Rga";
                v.PhotoId = AddFileAttachment();
                v.IsValid = false;
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
				
                v1.ID = "P2eM9";
                v1.Password = "TyTX0V";
                v1.Email = "ersxm7";
                v1.Name = "Vs4";
                v1.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v1.CellPhone = "87ZU";
                v1.Address = "AJ5rL";
                v1.ZipCode = "iutU5Rga";
                v1.PhotoId = AddFileAttachment();
                v1.IsValid = false;
                v2.ID = "IAjRO";
                v2.Password = "tjs2nvvk";
                v2.Email = "Qw15cTS1";
                v2.Name = "R0p";
                v2.Sex = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.CellPhone = "atR";
                v2.Address = "EwS";
                v2.ZipCode = "2Rjk0aIR";
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

                v.FileName = "YhtZfnWBt";
                v.FileExt = "6vmrChVJy";
                v.Path = "BJuyVm";
                v.Length = 79;
                v.SaveMode = "7FQ";
                v.ExtraInfo = "xScWsm6";
                v.HandlerInfo = "xTBqXEf8C";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
