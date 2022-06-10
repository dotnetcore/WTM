using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo.Controllers;
using WalkingTec.Mvvm.ReactDemo._Admin.ViewModels.FrameworkTenantVMs;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo;


namespace WalkingTec.Mvvm.ReactDemo.Test
{
    [TestClass]
    public class FrameworkTenantApiTest
    {
        private FrameworkTenantController _controller;
        private string _seed;

        public FrameworkTenantApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<FrameworkTenantController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new FrameworkTenantSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            FrameworkTenantVM vm = _controller.Wtm.CreateVM<FrameworkTenantVM>();
            FrameworkTenant v = new FrameworkTenant();
            
            v.TCode = "m5Gw276axzr5PTUMmqC7932NiYgHHrnYWFzm1";
            v.TName = "UYa6cQI2mzWSSA51DehIKXL6hzL";
            v.TDb = "3eZRq";
            v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.MySql;
            v.DbContext = "Jhl";
            v.TDomain = "iAGxBDuHx42VkT6gHuJp6qDrsritdHpGJjX5sBGhD688uCsa";
            v.EnableSub = false;
            v.Enabled = true;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkTenant>().Find(v.ID);
                
                Assert.AreEqual(data.TCode, "m5Gw276axzr5PTUMmqC7932NiYgHHrnYWFzm1");
                Assert.AreEqual(data.TName, "UYa6cQI2mzWSSA51DehIKXL6hzL");
                Assert.AreEqual(data.TDb, "3eZRq");
                Assert.AreEqual(data.TDbType, WalkingTec.Mvvm.Core.DBTypeEnum.MySql);
                Assert.AreEqual(data.DbContext, "Jhl");
                Assert.AreEqual(data.TDomain, "iAGxBDuHx42VkT6gHuJp6qDrsritdHpGJjX5sBGhD688uCsa");
                Assert.AreEqual(data.EnableSub, false);
                Assert.AreEqual(data.Enabled, true);
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            FrameworkTenant v = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.TCode = "m5Gw276axzr5PTUMmqC7932NiYgHHrnYWFzm1";
                v.TName = "UYa6cQI2mzWSSA51DehIKXL6hzL";
                v.TDb = "3eZRq";
                v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.MySql;
                v.DbContext = "Jhl";
                v.TDomain = "iAGxBDuHx42VkT6gHuJp6qDrsritdHpGJjX5sBGhD688uCsa";
                v.EnableSub = false;
                v.Enabled = true;
                context.Set<FrameworkTenant>().Add(v);
                context.SaveChanges();
            }

            FrameworkTenantVM vm = _controller.Wtm.CreateVM<FrameworkTenantVM>();
            var oldID = v.ID;
            v = new FrameworkTenant();
            v.ID = oldID;
       		
            v.TCode = "ogLPUPvUVx";
            v.TName = "sN7NmmZ1oZc2AY5Hud3N0AnrAWCZyH";
            v.TDb = "uZ1doz6A";
            v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
            v.DbContext = "jQVWG801";
            v.TDomain = "ClnXcQ8RjBxCn9URkfGRsoGYmBHLGsOVNJ33EtA";
            v.EnableSub = true;
            v.Enabled = false;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.TCode", "");
            vm.FC.Add("Entity.TName", "");
            vm.FC.Add("Entity.TDb", "");
            vm.FC.Add("Entity.TDbType", "");
            vm.FC.Add("Entity.DbContext", "");
            vm.FC.Add("Entity.TDomain", "");
            vm.FC.Add("Entity.EnableSub", "");
            vm.FC.Add("Entity.Enabled", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkTenant>().Find(v.ID);
 				
                Assert.AreEqual(data.TCode, "ogLPUPvUVx");
                Assert.AreEqual(data.TName, "sN7NmmZ1oZc2AY5Hud3N0AnrAWCZyH");
                Assert.AreEqual(data.TDb, "uZ1doz6A");
                Assert.AreEqual(data.TDbType, WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer);
                Assert.AreEqual(data.DbContext, "jQVWG801");
                Assert.AreEqual(data.TDomain, "ClnXcQ8RjBxCn9URkfGRsoGYmBHLGsOVNJ33EtA");
                Assert.AreEqual(data.EnableSub, true);
                Assert.AreEqual(data.Enabled, false);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            FrameworkTenant v = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.TCode = "m5Gw276axzr5PTUMmqC7932NiYgHHrnYWFzm1";
                v.TName = "UYa6cQI2mzWSSA51DehIKXL6hzL";
                v.TDb = "3eZRq";
                v.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.MySql;
                v.DbContext = "Jhl";
                v.TDomain = "iAGxBDuHx42VkT6gHuJp6qDrsritdHpGJjX5sBGhD688uCsa";
                v.EnableSub = false;
                v.Enabled = true;
                context.Set<FrameworkTenant>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            FrameworkTenant v1 = new FrameworkTenant();
            FrameworkTenant v2 = new FrameworkTenant();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.TCode = "m5Gw276axzr5PTUMmqC7932NiYgHHrnYWFzm1";
                v1.TName = "UYa6cQI2mzWSSA51DehIKXL6hzL";
                v1.TDb = "3eZRq";
                v1.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.MySql;
                v1.DbContext = "Jhl";
                v1.TDomain = "iAGxBDuHx42VkT6gHuJp6qDrsritdHpGJjX5sBGhD688uCsa";
                v1.EnableSub = false;
                v1.Enabled = true;
                v2.TCode = "ogLPUPvUVx";
                v2.TName = "sN7NmmZ1oZc2AY5Hud3N0AnrAWCZyH";
                v2.TDb = "uZ1doz6A";
                v2.TDbType = WalkingTec.Mvvm.Core.DBTypeEnum.SqlServer;
                v2.DbContext = "jQVWG801";
                v2.TDomain = "ClnXcQ8RjBxCn9URkfGRsoGYmBHLGsOVNJ33EtA";
                v2.EnableSub = true;
                v2.Enabled = false;
                context.Set<FrameworkTenant>().Add(v1);
                context.Set<FrameworkTenant>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<FrameworkTenant>().Find(v1.ID);
                var data2 = context.Set<FrameworkTenant>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
