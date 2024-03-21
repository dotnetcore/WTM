using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.WorkFlowDemoVMs;
using WalkingTec.Mvvm.BlazorDemo.Model;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
{
    [TestClass]
    public class WorkFlowDemoApiTest
    {
        private WorkFlowDemoController _controller;
        private string _seed;

        public WorkFlowDemoApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<WorkFlowDemoController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new WorkFlowDemoSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            WorkFlowDemoVM vm = _controller.Wtm.CreateVM<WorkFlowDemoVM>();
            WorkFlowDemo v = new WorkFlowDemo();
            
            v.Content = "ItZjUkcEqd2Nj2E";
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WorkFlowDemo>().Find(v.ID);
                
                Assert.AreEqual(data.Content, "ItZjUkcEqd2Nj2E");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            WorkFlowDemo v = new WorkFlowDemo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Content = "ItZjUkcEqd2Nj2E";
                context.Set<WorkFlowDemo>().Add(v);
                context.SaveChanges();
            }

            WorkFlowDemoVM vm = _controller.Wtm.CreateVM<WorkFlowDemoVM>();
            var oldID = v.ID;
            v = new WorkFlowDemo();
            v.ID = oldID;
       		
            v.Content = "sE";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Content", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WorkFlowDemo>().Find(v.ID);
 				
                Assert.AreEqual(data.Content, "sE");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            WorkFlowDemo v = new WorkFlowDemo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Content = "ItZjUkcEqd2Nj2E";
                context.Set<WorkFlowDemo>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            WorkFlowDemo v1 = new WorkFlowDemo();
            WorkFlowDemo v2 = new WorkFlowDemo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Content = "ItZjUkcEqd2Nj2E";
                v2.Content = "sE";
                context.Set<WorkFlowDemo>().Add(v1);
                context.Set<WorkFlowDemo>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<WorkFlowDemo>().Find(v1.ID);
                var data2 = context.Set<WorkFlowDemo>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
