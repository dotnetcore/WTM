using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.ISOTypeVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class ISOTypeControllerTest
    {
        private ISOTypeController _controller;
        private string _seed;

        public ISOTypeControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<ISOTypeController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as ISOTypeListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(ISOTypeVM));

            ISOTypeVM vm = rv.Model as ISOTypeVM;
            ISOType v = new ISOType();
			
            v.IsoName = "S97HTxM1";
            v.ISOVerSion = "ktAfyjAt";
            v.Description = "5UYkPFp2";
            v.ISOFileID = AddISOFile();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ISOType>().Find(v.ID);
				
                Assert.AreEqual(data.IsoName, "S97HTxM1");
                Assert.AreEqual(data.ISOVerSion, "ktAfyjAt");
                Assert.AreEqual(data.Description, "5UYkPFp2");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            ISOType v = new ISOType();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.IsoName = "S97HTxM1";
                v.ISOVerSion = "ktAfyjAt";
                v.Description = "5UYkPFp2";
                v.ISOFileID = AddISOFile();
                context.Set<ISOType>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ISOTypeVM));

            ISOTypeVM vm = rv.Model as ISOTypeVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new ISOType();
            v.ID = vm.Entity.ID;
       		
            v.IsoName = "mF1tke";
            v.ISOVerSion = "fGam";
            v.Description = "5wwl";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.IsoName", "");
            vm.FC.Add("Entity.ISOVerSion", "");
            vm.FC.Add("Entity.Description", "");
            vm.FC.Add("Entity.ISOFileID", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ISOType>().Find(v.ID);
 				
                Assert.AreEqual(data.IsoName, "mF1tke");
                Assert.AreEqual(data.ISOVerSion, "fGam");
                Assert.AreEqual(data.Description, "5wwl");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            ISOType v = new ISOType();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.IsoName = "S97HTxM1";
                v.ISOVerSion = "ktAfyjAt";
                v.Description = "5UYkPFp2";
                v.ISOFileID = AddISOFile();
                context.Set<ISOType>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ISOTypeVM));

            ISOTypeVM vm = rv.Model as ISOTypeVM;
            v = new ISOType();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ISOType>().Find(v.ID);
                Assert.AreEqual(data, null);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            ISOType v = new ISOType();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.IsoName = "S97HTxM1";
                v.ISOVerSion = "ktAfyjAt";
                v.Description = "5UYkPFp2";
                v.ISOFileID = AddISOFile();
                context.Set<ISOType>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            ISOType v1 = new ISOType();
            ISOType v2 = new ISOType();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.IsoName = "S97HTxM1";
                v1.ISOVerSion = "ktAfyjAt";
                v1.Description = "5UYkPFp2";
                v1.ISOFileID = AddISOFile();
                v2.IsoName = "mF1tke";
                v2.ISOVerSion = "fGam";
                v2.Description = "5wwl";
                v2.ISOFileID = v1.ISOFileID; 
                context.Set<ISOType>().Add(v1);
                context.Set<ISOType>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(ISOTypeBatchVM));

            ISOTypeBatchVM vm = rv.Model as ISOTypeBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<ISOType>().Find(v1.ID);
                var data2 = context.Set<ISOType>().Find(v2.ID);
 				
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            ISOType v1 = new ISOType();
            ISOType v2 = new ISOType();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.IsoName = "S97HTxM1";
                v1.ISOVerSion = "ktAfyjAt";
                v1.Description = "5UYkPFp2";
                v1.ISOFileID = AddISOFile();
                v2.IsoName = "mF1tke";
                v2.ISOVerSion = "fGam";
                v2.Description = "5wwl";
                v2.ISOFileID = v1.ISOFileID; 
                context.Set<ISOType>().Add(v1);
                context.Set<ISOType>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(ISOTypeBatchVM));

            ISOTypeBatchVM vm = rv.Model as ISOTypeBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<ISOType>().Find(v1.ID);
                var data2 = context.Set<ISOType>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as ISOTypeListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Guid AddISOFile()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "aYU0T";
                v.FileExt = "bUoWCpap";
                v.Path = "dckcde";
                v.Length = 74;
                v.SaveMode = "yFBu3l";
                v.ExtraInfo = "iUjvTAL";
                v.HandlerInfo = "B1IgvsHpf";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
