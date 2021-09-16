using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.TreeTestVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class TreeTestControllerTest
    {
        private TreeTestController _controller;
        private string _seed;

        public TreeTestControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<TreeTestController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as TreeTestListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(TreeTestVM));

            TreeTestVM vm = rv.Model as TreeTestVM;
            TreeTest v = new TreeTest();
			
            v.ID = 54;
            v.ParentId = AddTreeTest();
            v.Name = "c";
            v.Test = "SBV";
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TreeTest>().Find(v.ID);
				
                Assert.AreEqual(data.ID, 54);
                Assert.AreEqual(data.Name, "c");
                Assert.AreEqual(data.Test, "SBV");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            TreeTest v = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ID = 54;
                v.ParentId = AddTreeTest();
                v.Name = "c";
                v.Test = "SBV";
                context.Set<TreeTest>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(TreeTestVM));

            TreeTestVM vm = rv.Model as TreeTestVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new TreeTest();
            v.ID = vm.Entity.ID;
       		
            v.Name = "S";
            v.Test = "hzBWhxRmPlOQDV";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.ParentId", "");
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Test", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TreeTest>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "S");
                Assert.AreEqual(data.Test, "hzBWhxRmPlOQDV");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            TreeTest v = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = 54;
                v.ParentId = AddTreeTest();
                v.Name = "c";
                v.Test = "SBV";
                context.Set<TreeTest>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(TreeTestVM));

            TreeTestVM vm = rv.Model as TreeTestVM;
            v = new TreeTest();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TreeTest>().Find(v.ID);
                Assert.AreEqual(data.IsValid, false);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            TreeTest v = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ID = 54;
                v.ParentId = AddTreeTest();
                v.Name = "c";
                v.Test = "SBV";
                context.Set<TreeTest>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            TreeTest v1 = new TreeTest();
            TreeTest v2 = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 54;
                v1.ParentId = AddTreeTest();
                v1.Name = "c";
                v1.Test = "SBV";
                v2.ID = 81;
                v2.ParentId = v1.ParentId; 
                v2.Name = "S";
                v2.Test = "hzBWhxRmPlOQDV";
                context.Set<TreeTest>().Add(v1);
                context.Set<TreeTest>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(TreeTestBatchVM));

            TreeTestBatchVM vm = rv.Model as TreeTestBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<TreeTest>().Find(v1.ID);
                var data2 = context.Set<TreeTest>().Find(v2.ID);
 				
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            TreeTest v1 = new TreeTest();
            TreeTest v2 = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 54;
                v1.ParentId = AddTreeTest();
                v1.Name = "c";
                v1.Test = "SBV";
                v2.ID = 81;
                v2.ParentId = v1.ParentId; 
                v2.Name = "S";
                v2.Test = "hzBWhxRmPlOQDV";
                context.Set<TreeTest>().Add(v1);
                context.Set<TreeTest>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(TreeTestBatchVM));

            TreeTestBatchVM vm = rv.Model as TreeTestBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<TreeTest>().Find(v1.ID);
                var data2 = context.Set<TreeTest>().Find(v2.ID);
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as TreeTestListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Int32 AddTreeTest()
        {
            TreeTest v = new TreeTest();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.ID = 28;
                v.Name = "DE";
                v.Test = "d4HdFsSzOPNWjP";
                context.Set<TreeTest>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
