using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WalkingTec.Mvvm.Admin.Api;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Test.Mock;

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
            _controller = MockController.CreateApi<FrameworkUserController>(new Demo.DataContext(_seed, DBTypeEnum.Memory), "user");
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

            FrameworkUserVM vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            FrameworkUser v = new FrameworkUser();

            v.ITCode = "itcode";
            v.Name = "name";
            v.Password = "password";
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv.Result, typeof(OkObjectResult));

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

            FrameworkUserVM vm = _controller.Wtm.CreateVM<FrameworkUserVM>();
            var oldID = v.ID;
            v = new FrameworkUser();
            v.ID = oldID;
            v.Name = "name1";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
            vm.FC.Add("Entity.Name", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv.Result, typeof(OkObjectResult));

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<FrameworkUser>().FirstOrDefault();
                Assert.AreEqual(data.ITCode, "itcode");
                Assert.AreEqual(data.Name, "name1");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void GetTest()
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
            var rv = _controller.Get(v.ID);
            Assert.IsNotNull(rv);
            Assert.AreEqual(rv.Entity.ITCode, "itcode");
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


            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Result, typeof(OkObjectResult));

            using (var context = new Demo.DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<FrameworkUser>().Count(), 0);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv.Result, typeof(OkResult));
        }

        [TestMethod]
        public void ExportTest()
        {
            var rv = _controller.ExportExcel(new FrameworkUserSearcher());
            Assert.IsInstanceOfType(rv, typeof(FileResult));

            rv = _controller.ExportExcelByIds(new string[] { });
            Assert.IsInstanceOfType(rv, typeof(FileResult));

            rv = _controller.GetExcelTemplate();
            Assert.IsInstanceOfType(rv, typeof(FileResult));

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
