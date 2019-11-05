using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.Controllers;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Test
{
    [TestClass]
    public class FrameworkUserControllerTest
    {
        private FrameworkUserController _controller;
        private string _seed;

        public FrameworkUserControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<FrameworkUserController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search(rv.Model as FrameworkUserListVM);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as FrameworkUserListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }


        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserVM));

            FrameworkUserVM vm = rv.Model as FrameworkUserVM;
            FrameworkUserBase v = new FrameworkUserBase();

            v.ITCode = "itcode";
            v.Name = "name";
            v.Password = "password";
            vm.Entity = v;
            _controller.Create(vm);

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

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserVM));

            FrameworkUserVM vm = rv.Model as FrameworkUserVM;
            v = new FrameworkUserBase();
            v.ID = vm.Entity.ID;
            v.ITCode = "itcode1";
            v.Name = "name1";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
            vm.FC.Add("Entity.ITCode", "");
            vm.FC.Add("Entity.Name", "");
            _controller.Edit(vm);

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
        public void DeleteTest()
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

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID);
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserVM));

            FrameworkUserVM vm = rv.Model as FrameworkUserVM;
            v = new FrameworkUserBase();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID,null);

            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUserBase>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
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
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID);
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.ID);
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

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserBatchVM));
            (rv.Model as FrameworkUserBatchVM).ListVM.DoSearch();

            FrameworkUserBatchVM vm = rv.Model as FrameworkUserBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUserBase>().Count(), 0);
            }
        }

        private void AddXXX()
        {
            FrameworkAction v = new FrameworkAction();
            using (var context = new FrameworkContext(_seed, DBTypeEnum.Memory))
            {
                v.ActionName = "";
                v.Checked = true;
                context.Set<FrameworkAction>().Add(v);
                context.SaveChanges();
            }
        }
    }
}
