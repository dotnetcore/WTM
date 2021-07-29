using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class LinkTestControllerTest
    {
        private LinkTestController _controller;
        private string _seed;

        public LinkTestControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<LinkTestController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as LinkTestListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTestVM));

            LinkTestVM vm = rv.Model as LinkTestVM;
            LinkTest v = new LinkTest();
			
            v.name = "2Waa";
            v.StudentId = AddStudent();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest>().Find(v.ID);
				
                Assert.AreEqual(data.name, "2Waa");
            }

        }

        [TestMethod]
        public void EditTest()
        {
            LinkTest v = new LinkTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.name = "2Waa";
                v.StudentId = AddStudent();
                context.Set<LinkTest>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTestVM));

            LinkTestVM vm = rv.Model as LinkTestVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new LinkTest();
            v.ID = vm.Entity.ID;
       		
            v.name = "F0attW9";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.name", "");
            vm.FC.Add("Entity.StudentId", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest>().Find(v.ID);
 				
                Assert.AreEqual(data.name, "F0attW9");
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            LinkTest v = new LinkTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.name = "2Waa";
                v.StudentId = AddStudent();
                context.Set<LinkTest>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTestVM));

            LinkTestVM vm = rv.Model as LinkTestVM;
            v = new LinkTest();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest>().Find(v.ID);
                Assert.AreEqual(data, null);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            LinkTest v = new LinkTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.name = "2Waa";
                v.StudentId = AddStudent();
                context.Set<LinkTest>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            LinkTest v1 = new LinkTest();
            LinkTest v2 = new LinkTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.name = "2Waa";
                v1.StudentId = AddStudent();
                v2.name = "F0attW9";
                v2.StudentId = v1.StudentId; 
                context.Set<LinkTest>().Add(v1);
                context.Set<LinkTest>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTestBatchVM));

            LinkTestBatchVM vm = rv.Model as LinkTestBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<LinkTest>().Find(v1.ID);
                var data2 = context.Set<LinkTest>().Find(v2.ID);
 				
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            LinkTest v1 = new LinkTest();
            LinkTest v2 = new LinkTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.name = "2Waa";
                v1.StudentId = AddStudent();
                v2.name = "F0attW9";
                v2.StudentId = v1.StudentId; 
                context.Set<LinkTest>().Add(v1);
                context.Set<LinkTest>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTestBatchVM));

            LinkTestBatchVM vm = rv.Model as LinkTestBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<LinkTest>().Find(v1.ID);
                var data2 = context.Set<LinkTest>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.FileName = "GMib1";
                v.FileExt = "GLf";
                v.Path = "3Gk";
                v.Length = 50;
                v.SaveMode = "2utBUY";
                v.ExtraInfo = "uB7SeA9R";
                v.HandlerInfo = "UF9OLK";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }

        private String AddStudent()
        {
            Student v = new Student();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.ID = "pnZlBo96u";
                v.Password = "phfbKplb";
                v.Email = "dQiwMnj";
                v.Name = "Rj9PDVQ";
                v.Sex = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "FgomcUOU";
                v.Address = "LxWYi";
                v.ZipCode = "IyQ25";
                v.PhotoId = AddFileAttachment();
                v.IsValid = false;
                context.Set<Student>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
