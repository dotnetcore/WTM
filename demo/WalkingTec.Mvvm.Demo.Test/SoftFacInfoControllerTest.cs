using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.SoftFacInfoVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class SoftFacInfoControllerTest
    {
        private SoftFacInfoController _controller;
        private string _seed;

        public SoftFacInfoControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<SoftFacInfoController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as SoftFacInfoListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(SoftFacInfoVM));

            SoftFacInfoVM vm = rv.Model as SoftFacInfoVM;
            SoftFacInfo v = new SoftFacInfo();
			
            v.IsoName = "1kQ4G";
            v.EXEVerSion = "PfemDb";
            v.Description = "xt9";
            v.EXEFileID = AddEXEFile();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SoftFacInfo>().Find(v.ID);
				
                Assert.AreEqual(data.IsoName, "1kQ4G");
                Assert.AreEqual(data.EXEVerSion, "PfemDb");
                Assert.AreEqual(data.Description, "xt9");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            SoftFacInfo v = new SoftFacInfo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.IsoName = "1kQ4G";
                v.EXEVerSion = "PfemDb";
                v.Description = "xt9";
                v.EXEFileID = AddEXEFile();
                context.Set<SoftFacInfo>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(SoftFacInfoVM));

            SoftFacInfoVM vm = rv.Model as SoftFacInfoVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new SoftFacInfo();
            v.ID = vm.Entity.ID;
       		
            v.IsoName = "NGprRptST";
            v.EXEVerSion = "2rYw";
            v.Description = "0uZHcGxN";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.IsoName", "");
            vm.FC.Add("Entity.EXEVerSion", "");
            vm.FC.Add("Entity.Description", "");
            vm.FC.Add("Entity.EXEFileID", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SoftFacInfo>().Find(v.ID);
 				
                Assert.AreEqual(data.IsoName, "NGprRptST");
                Assert.AreEqual(data.EXEVerSion, "2rYw");
                Assert.AreEqual(data.Description, "0uZHcGxN");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            SoftFacInfo v = new SoftFacInfo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.IsoName = "1kQ4G";
                v.EXEVerSion = "PfemDb";
                v.Description = "xt9";
                v.EXEFileID = AddEXEFile();
                context.Set<SoftFacInfo>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(SoftFacInfoVM));

            SoftFacInfoVM vm = rv.Model as SoftFacInfoVM;
            v = new SoftFacInfo();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<SoftFacInfo>().Find(v.ID);
                Assert.AreEqual(data, null);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            SoftFacInfo v = new SoftFacInfo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.IsoName = "1kQ4G";
                v.EXEVerSion = "PfemDb";
                v.Description = "xt9";
                v.EXEFileID = AddEXEFile();
                context.Set<SoftFacInfo>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            SoftFacInfo v1 = new SoftFacInfo();
            SoftFacInfo v2 = new SoftFacInfo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.IsoName = "1kQ4G";
                v1.EXEVerSion = "PfemDb";
                v1.Description = "xt9";
                v1.EXEFileID = AddEXEFile();
                v2.IsoName = "NGprRptST";
                v2.EXEVerSion = "2rYw";
                v2.Description = "0uZHcGxN";
                v2.EXEFileID = v1.EXEFileID; 
                context.Set<SoftFacInfo>().Add(v1);
                context.Set<SoftFacInfo>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(SoftFacInfoBatchVM));

            SoftFacInfoBatchVM vm = rv.Model as SoftFacInfoBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<SoftFacInfo>().Find(v1.ID);
                var data2 = context.Set<SoftFacInfo>().Find(v2.ID);
 				
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            SoftFacInfo v1 = new SoftFacInfo();
            SoftFacInfo v2 = new SoftFacInfo();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.IsoName = "1kQ4G";
                v1.EXEVerSion = "PfemDb";
                v1.Description = "xt9";
                v1.EXEFileID = AddEXEFile();
                v2.IsoName = "NGprRptST";
                v2.EXEVerSion = "2rYw";
                v2.Description = "0uZHcGxN";
                v2.EXEFileID = v1.EXEFileID; 
                context.Set<SoftFacInfo>().Add(v1);
                context.Set<SoftFacInfo>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(SoftFacInfoBatchVM));

            SoftFacInfoBatchVM vm = rv.Model as SoftFacInfoBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<SoftFacInfo>().Find(v1.ID);
                var data2 = context.Set<SoftFacInfo>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as SoftFacInfoListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Guid AddEXEFile()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "ZLCRj";
                v.FileExt = "731r";
                v.Path = "AYohgWi";
                v.Length = 29;
                v.SaveMode = "0ZkPXV";
                v.ExtraInfo = "ti4Wv9";
                v.HandlerInfo = "YwRIw";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
