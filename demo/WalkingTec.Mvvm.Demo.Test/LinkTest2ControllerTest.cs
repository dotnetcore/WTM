using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.LinkTest2VMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class LinkTest2ControllerTest
    {
        private LinkTest2Controller _controller;
        private string _seed;

        public LinkTest2ControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<LinkTest2Controller>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as LinkTest2ListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTest2VM));

            LinkTest2VM vm = rv.Model as LinkTest2VM;
            LinkTest2 v = new LinkTest2();
			
            v.name = "eE1L";
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest2>().Find(v.ID);
				
                Assert.AreEqual(data.name, "eE1L");
            }

        }

        [TestMethod]
        public void EditTest()
        {
            LinkTest2 v = new LinkTest2();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.name = "eE1L";
                context.Set<LinkTest2>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTest2VM));

            LinkTest2VM vm = rv.Model as LinkTest2VM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new LinkTest2();
            v.ID = vm.Entity.ID;
       		
            v.name = "cxZEl5gP";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.name", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest2>().Find(v.ID);
 				
                Assert.AreEqual(data.name, "cxZEl5gP");
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            LinkTest2 v = new LinkTest2();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.name = "eE1L";
                context.Set<LinkTest2>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTest2VM));

            LinkTest2VM vm = rv.Model as LinkTest2VM;
            v = new LinkTest2();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<LinkTest2>().Find(v.ID);
                Assert.AreEqual(data, null);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            LinkTest2 v = new LinkTest2();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.name = "eE1L";
                context.Set<LinkTest2>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            LinkTest2 v1 = new LinkTest2();
            LinkTest2 v2 = new LinkTest2();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.name = "eE1L";
                v2.name = "cxZEl5gP";
                context.Set<LinkTest2>().Add(v1);
                context.Set<LinkTest2>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTest2BatchVM));

            LinkTest2BatchVM vm = rv.Model as LinkTest2BatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<LinkTest2>().Find(v1.ID);
                var data2 = context.Set<LinkTest2>().Find(v2.ID);
 				
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            LinkTest2 v1 = new LinkTest2();
            LinkTest2 v2 = new LinkTest2();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.name = "eE1L";
                v2.name = "cxZEl5gP";
                context.Set<LinkTest2>().Add(v1);
                context.Set<LinkTest2>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(LinkTest2BatchVM));

            LinkTest2BatchVM vm = rv.Model as LinkTest2BatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<LinkTest2>().Find(v1.ID);
                var data2 = context.Set<LinkTest2>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }


    }
}
