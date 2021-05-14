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
            
            v.VirtusName = "9nn";
            v.VirtusCode = "XDON";
            v.Remark = "8j5DwQK";
            v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Virus>().Find(v.ID);
                
                Assert.AreEqual(data.VirtusName, "9nn");
                Assert.AreEqual(data.VirtusCode, "XDON");
                Assert.AreEqual(data.Remark, "8j5DwQK");
                Assert.AreEqual(data.VirtusType, WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            Virus v = new Virus();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.VirtusName = "9nn";
                v.VirtusCode = "XDON";
                v.Remark = "8j5DwQK";
                v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                context.Set<Virus>().Add(v);
                context.SaveChanges();
            }

            VirusVM vm = _controller.Wtm.CreateVM<VirusVM>();
            var oldID = v.ID;
            v = new Virus();
            v.ID = oldID;
       		
            v.VirtusName = "N14lF";
            v.VirtusCode = "zZoFx";
            v.Remark = "RptU";
            v.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.DNA;
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
 				
                Assert.AreEqual(data.VirtusName, "N14lF");
                Assert.AreEqual(data.VirtusCode, "zZoFx");
                Assert.AreEqual(data.Remark, "RptU");
                Assert.AreEqual(data.VirtusType, WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.DNA);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            Virus v = new Virus();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.VirtusName = "9nn";
                v.VirtusCode = "XDON";
                v.Remark = "8j5DwQK";
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
				
                v1.VirtusName = "9nn";
                v1.VirtusCode = "XDON";
                v1.Remark = "8j5DwQK";
                v1.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.RNA;
                v2.VirtusName = "N14lF";
                v2.VirtusCode = "zZoFx";
                v2.Remark = "RptU";
                v2.VirtusType = WalkingTec.Mvvm.Demo.Models.Virus.VirtusTypeEnum.DNA;
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
