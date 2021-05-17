using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.Controllers;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Test.Mock;

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
            _controller = MockController.CreateController<FrameworkUserController>(new Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as FrameworkUserListVM).Searcher);
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
            FrameworkUser v = new FrameworkUser();

            v.ITCode = "itcode";
            v.Name = "name";
            v.Password = "password";
            vm.Entity = v;
            _controller.Create(vm).Wait();

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkUser>().FirstOrDefault();
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
            FrameworkUser v = new FrameworkUser();
            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                v.ITCode = "itcode";
                v.Name = "name";
                v.Password = "password";
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserVM));

            FrameworkUserVM vm = rv.Model as FrameworkUserVM;
            v = new FrameworkUser();
            v.ID = vm.Entity.ID;
            v.Name = "name1";
            v.ITCode = "abc";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
            vm.FC.Add("Entity.Name", "");
            _controller.Edit(vm).Wait();

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkUser>().FirstOrDefault();
                Assert.AreEqual(data.Name, "name1");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            FrameworkUser v = new FrameworkUser();
            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                v.ITCode = "itcode";
                v.Name = "name";
                v.Password = "password";
                v.PhotoId = AddPhoto();
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID);
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserVM));

            FrameworkUserVM vm = rv.Model as FrameworkUserVM;
            v = new FrameworkUser();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID,null).Wait();

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUser>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            FrameworkUser v = new FrameworkUser();
            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                v.ITCode = "itcode";
                v.Name = "name";
                v.Password = "password";
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID);
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.ID);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            FrameworkUser v1 = new FrameworkUser();
            FrameworkUser v2 = new FrameworkUser();
            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                v1.ITCode = "itcode";
                v1.Name = "name";
                v1.Password = "password";
                v1.PhotoId = AddPhoto();
                v2.ITCode = "itcode2";
                v2.Name = "name2";
                v2.Password = "password2";
                v2.PhotoId = AddPhoto();
                context.Set<FrameworkUser>().Add(v1);
                context.Set<FrameworkUser>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(FrameworkUserBatchVM));
            (rv.Model as FrameworkUserBatchVM).ListVM.DoSearch();

            FrameworkUserBatchVM vm = rv.Model as FrameworkUserBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null).Wait();

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUser>().Count(), 0);
            }
        }

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "PEsnw";
                v.FileExt = "celfpE";
                v.Path = "egy";
                v.Length = 61;
                v.SaveMode = "uLfM37wt";
                v.ExtraInfo = "Od3aqjgP";
                v.HandlerInfo = "tbyzFF";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

    }
}
