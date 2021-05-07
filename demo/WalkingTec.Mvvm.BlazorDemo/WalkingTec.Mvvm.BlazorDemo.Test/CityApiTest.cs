using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;


namespace WalkingTec.Mvvm.BlazorDemo.Test
{
    [TestClass]
    public class CityApiTest
    {
        private CityController _controller;
        private string _seed;

        public CityApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<CityController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new CitySearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            CityVM vm = _controller.Wtm.CreateVM<CityVM>();
            City v = new City();
            
            v.ID = 71;
            v.ParentId = AddParent();
            v.Name = "G7E";
            v.Test = "BjECdd";
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 71);
                Assert.AreEqual(data.Name, "G7E");
                Assert.AreEqual(data.Test, "BjECdd");
            }
        }

        [TestMethod]
        public void EditTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ID = 71;
                v.ParentId = AddParent();
                v.Name = "G7E";
                v.Test = "BjECdd";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }

            CityVM vm = _controller.Wtm.CreateVM<CityVM>();
            var oldID = v.ID;
            v = new City();
            v.ID = oldID;
       		
            v.Name = "vJM47fTG";
            v.Test = "vHIP1";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.ParentId", "");
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Test", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "vJM47fTG");
                Assert.AreEqual(data.Test, "vHIP1");
            }

        }

		[TestMethod]
        public void GetTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = 71;
                v.ParentId = AddParent();
                v.Name = "G7E";
                v.Test = "BjECdd";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            City v1 = new City();
            City v2 = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 71;
                v1.ParentId = AddParent();
                v1.Name = "G7E";
                v1.Test = "BjECdd";
                v2.ID = 43;
                v2.ParentId = v1.ParentId; 
                v2.Name = "vJM47fTG";
                v2.Test = "vHIP1";
                context.Set<City>().Add(v1);
                context.Set<City>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<City>().Find(v1.ID);
                var data2 = context.Set<City>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Int32 AddParent()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.ID = 65;
                v.Name = "GVnV7l4";
                v.Test = "2CKTAwIv";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
