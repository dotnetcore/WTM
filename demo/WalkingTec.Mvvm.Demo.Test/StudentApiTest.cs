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
    public class StudentApiTest
    {
        private StudentApiController _controller;
        private string _seed;

        public StudentApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<StudentApiController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new StudentApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            StudentApiVM vm = _controller.Wtm.CreateVM<StudentApiVM>();
            Student v = new Student();
            
            v.ID = "SfXbbrifH";
            v.Password = "qDl";
            v.Name = "Iyr";
            v.IsValid = true;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().FirstOrDefault();
                
                Assert.AreEqual(data.ID, "SfXbbrifH");
                Assert.AreEqual(data.Password, "qDl");
                Assert.AreEqual(data.Name, "Iyr");
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
       			
                v.ID = "SfXbbrifH";
                v.Password = "qDl";
                v.Name = "Iyr";
                v.IsValid = true;
                context.Set<Student>().Add(v);
                context.SaveChanges();
            }

            StudentApiVM vm = _controller.Wtm.CreateVM<StudentApiVM>();
            var oldID = v.ID;
            v = new Student();
            v.ID = oldID;
       		
            v.Password = "Ph4";
            v.Name = "7MfJje";
            v.IsValid = true;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.Password", "");
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.IsValid", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Student>().FirstOrDefault();
 				
                Assert.AreEqual(data.Password, "Ph4");
                Assert.AreEqual(data.Name, "7MfJje");
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
        		
                v.ID = "SfXbbrifH";
                v.Password = "qDl";
                v.Name = "Iyr";
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
				
                v1.ID = "SfXbbrifH";
                v1.Password = "qDl";
                v1.Name = "Iyr";
                v1.IsValid = true;
                v2.ID = "b3e";
                v2.Password = "Ph4";
                v2.Name = "7MfJje";
                v2.IsValid = true;
                context.Set<Student>().Add(v1);
                context.Set<Student>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<Student>().Count(), 0);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
