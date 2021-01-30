using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class CityApiTest
    {
        private CityApiController _controller;
        private string _seed;

        public CityApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<CityApiController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new CityApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            CityApiVM vm = _controller.Wtm.CreateVM<CityApiVM>();
            City v = new City();
            
            v.Name = "04W2";
            v.Test = "xdtsoZ68";
            v.ParentId = AddParent();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
                
                Assert.AreEqual(data.Name, "04W2");
                Assert.AreEqual(data.Test, "xdtsoZ68");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Name = "04W2";
                v.Test = "xdtsoZ68";
                v.ParentId = AddParent();
                context.Set<City>().Add(v);
                context.SaveChanges();
            }

            CityApiVM vm = _controller.Wtm.CreateVM<CityApiVM>();
            var oldID = v.ID;
            v = new City();
            v.ID = oldID;
       		
            v.Name = "ifE0uV";
            v.Test = "aDdwvFbFf";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Name", "");
            vm.FC.Add("Entity.Test", "");
            vm.FC.Add("Entity.ParentId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<City>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "ifE0uV");
                Assert.AreEqual(data.Test, "aDdwvFbFf");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Name = "04W2";
                v.Test = "xdtsoZ68";
                v.ParentId = AddParent();
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
				
                v1.Name = "04W2";
                v1.Test = "xdtsoZ68";
                v1.ParentId = AddParent();
                v2.Name = "ifE0uV";
                v2.Test = "aDdwvFbFf";
                v2.ParentId = v1.ParentId; 
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
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Guid AddParent()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Name = "O7c";
                v.Test = "TCwO2bmTp";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
