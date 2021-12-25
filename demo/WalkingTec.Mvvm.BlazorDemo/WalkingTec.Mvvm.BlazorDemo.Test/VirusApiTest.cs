using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
{
    [TestClass]
    public class VirusApiTest
    {
        private VirusController _controller;
        private string _seed;

        public VirusApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<VirusController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new VirusSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            VirusVM vm = _controller.Wtm.CreateVM<VirusVM>();
            Virus v = new Virus();
            
            v.VirtusName = "Q4ZTQmmjeqr";
            v.VirtusCode = "XUN5";
            v.Remark = "WQJKRCxQG";
            v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Virus>().Find(v.ID);
                
                Assert.AreEqual(data.VirtusName, "Q4ZTQmmjeqr");
                Assert.AreEqual(data.VirtusCode, "XUN5");
                Assert.AreEqual(data.Remark, "WQJKRCxQG");
                Assert.AreEqual(data.VirtusType, WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            Virus v = new Virus();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.VirtusName = "Q4ZTQmmjeqr";
                v.VirtusCode = "XUN5";
                v.Remark = "WQJKRCxQG";
                v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                context.Set<Virus>().Add(v);
                context.SaveChanges();
            }

            VirusVM vm = _controller.Wtm.CreateVM<VirusVM>();
            var oldID = v.ID;
            v = new Virus();
            v.ID = oldID;
       		
            v.VirtusName = "0eAnXO6oKe22lhcHY1";
            v.VirtusCode = "pko";
            v.Remark = "bAvhotF8klwm10sejEg";
            v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.VirtusName", "");
            vm.FC.Add("Entity.VirtusCode", "");
            vm.FC.Add("Entity.Remark", "");
            vm.FC.Add("Entity.VirtusType", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Virus>().Find(v.ID);
 				
                Assert.AreEqual(data.VirtusName, "0eAnXO6oKe22lhcHY1");
                Assert.AreEqual(data.VirtusCode, "pko");
                Assert.AreEqual(data.Remark, "bAvhotF8klwm10sejEg");
                Assert.AreEqual(data.VirtusType, WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            Virus v = new Virus();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.VirtusName = "Q4ZTQmmjeqr";
                v.VirtusCode = "XUN5";
                v.Remark = "WQJKRCxQG";
                v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                context.Set<Virus>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            Virus v1 = new Virus();
            Virus v2 = new Virus();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.VirtusName = "Q4ZTQmmjeqr";
                v1.VirtusCode = "XUN5";
                v1.Remark = "WQJKRCxQG";
                v1.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                v2.VirtusName = "0eAnXO6oKe22lhcHY1";
                v2.VirtusCode = "pko";
                v2.Remark = "bAvhotF8klwm10sejEg";
                v2.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                context.Set<Virus>().Add(v1);
                context.Set<Virus>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Virus>().Find(v1.ID);
                var data2 = context.Set<Virus>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
