using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkUserApiTest
    {
        private FrameworkUserController _controller;
        private string _seed;

        public FrameworkUserApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<FrameworkUserController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            string rv = _controller.Search(new FrameworkUserSearcher());
            Assert.IsTrue(string.IsNullOrEmpty(rv)==false);
        }

        [TestMethod]
        public void CreateTest()
        {

            FrameworkUserVM vm = _controller.CreateVM<FrameworkUserVM>();
            FrameworkUserBase v = new FrameworkUserBase();
            
            v.ITCode = "itcode";
            v.Name = "name";
            v.Password = "password";
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkUserBase>().FirstOrDefault();
                Assert.AreEqual(data.ITCode, "itcode");
                Assert.AreEqual(data.Name, "name");
                Assert.AreEqual(data.Password, Utils.GetMD5String("password"));
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            FrameworkUserBase v = new FrameworkUserBase();
            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                v.ITCode = "itcode";
                v.Name = "name";
                v.Password = "password";
                context.Set<FrameworkUserBase>().Add(v);
                context.SaveChanges();
            }

            FrameworkUserVM vm = _controller.CreateVM<FrameworkUserVM>();
            var oldID = v.ID;
            v = new FrameworkUserBase();
            v.ID = oldID;
            v.ITCode = "itcode1";
            v.Name = "name1";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
            vm.FC.Add("Entity.ITCode", "");
            vm.FC.Add("Entity.Name", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkUserBase>().FirstOrDefault();
                Assert.AreEqual(data.ITCode, "itcode1");
                Assert.AreEqual(data.Name, "name1");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void GetTest()
        {
            FrameworkUserBase v = new FrameworkUserBase();
            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                v.ITCode = "itcode";
                v.Name = "name";
                v.Password = "password";
                context.Set<FrameworkUserBase>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID);
            Assert.IsNotNull(rv);
            Assert.AreEqual(rv.Entity.ITCode, "itcode");
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            FrameworkUserBase v1 = new FrameworkUserBase();
            FrameworkUserBase v2 = new FrameworkUserBase();
            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                v1.ITCode = "itcode";
                v1.Name = "name";
                v1.Password = "password";
                v2.ITCode = "itcode2";
                v2.Name = "name2";
                v2.Password = "password2";
                context.Set<FrameworkUserBase>().Add(v1);
                context.Set<FrameworkUserBase>().Add(v2);
                context.SaveChanges();
            }


            var rv = _controller.BatchDelete(new Guid[] { v1.ID, v2.ID });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUserBase>().Count(), 0);
            }

            rv = _controller.BatchDelete(new Guid[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));
        }

        [TestMethod]
        public void ExportTest()
        {
            var rv = _controller.ExportExcel(new FrameworkUserSearcher());
            Assert.IsInstanceOfType(rv, typeof(FileResult));

            rv = _controller.ExportExcelByIds(new Guid[] { });
            Assert.IsInstanceOfType(rv, typeof(FileResult));

            rv = _controller.GetExcelTemplate();
            Assert.IsInstanceOfType(rv, typeof(FileResult));

        }
    }
}
