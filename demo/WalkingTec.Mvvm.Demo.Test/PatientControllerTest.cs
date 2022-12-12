using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.PatientVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class PatientControllerTest
    {
        private PatientController _controller;
        private string _seed;

        public PatientControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<PatientController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as PatientListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(PatientVM));

            PatientVM vm = rv.Model as PatientVM;
            Patient v = new Patient();
			
            v.ID = 35;
            v.PatientName = "pn";
            v.IdNumber = "xJU3MwxLEGFqaDfG";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
            v.Birthday = DateTime.Parse("2023-09-29 10:14:20");
            v.LocationId = AddCity();
            v.HospitalId = AddHospital();
            v.PhotoId = AddFileAttachment();
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
				
                Assert.AreEqual(data.ID, 35);
                Assert.AreEqual(data.PatientName, "pn");
                Assert.AreEqual(data.IdNumber, "xJU3MwxLEGFqaDfG");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang);
                Assert.AreEqual(data.Birthday, DateTime.Parse("2023-09-29 10:14:20"));
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            Patient v = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ID = 35;
                v.PatientName = "pn";
                v.IdNumber = "xJU3MwxLEGFqaDfG";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v.Birthday = DateTime.Parse("2023-09-29 10:14:20");
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(PatientVM));

            PatientVM vm = rv.Model as PatientVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new Patient();
            v.ID = vm.Entity.ID;
       		
            v.PatientName = "hKrpA";
            v.IdNumber = "wstyOrRXel6Kd9yS9d";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu;
            v.Birthday = DateTime.Parse("2021-11-18 10:14:20");
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.PatientName", "");
            vm.FC.Add("Entity.IdNumber", "");
            vm.FC.Add("Entity.Gender", "");
            vm.FC.Add("Entity.Status", "");
            vm.FC.Add("Entity.Birthday", "");
            vm.FC.Add("Entity.LocationId", "");
            vm.FC.Add("Entity.HospitalId", "");
            vm.FC.Add("Entity.PhotoId", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
 				
                Assert.AreEqual(data.PatientName, "hKrpA");
                Assert.AreEqual(data.IdNumber, "wstyOrRXel6Kd9yS9d");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu);
                Assert.AreEqual(data.Birthday, DateTime.Parse("2021-11-18 10:14:20"));
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            Patient v = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = 35;
                v.PatientName = "pn";
                v.IdNumber = "xJU3MwxLEGFqaDfG";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v.Birthday = DateTime.Parse("2023-09-29 10:14:20");
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(PatientVM));

            PatientVM vm = rv.Model as PatientVM;
            v = new Patient();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
                Assert.AreEqual(data, null);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            Patient v = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ID = 35;
                v.PatientName = "pn";
                v.IdNumber = "xJU3MwxLEGFqaDfG";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v.Birthday = DateTime.Parse("2023-09-29 10:14:20");
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            Patient v1 = new Patient();
            Patient v2 = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 35;
                v1.PatientName = "pn";
                v1.IdNumber = "xJU3MwxLEGFqaDfG";
                v1.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v1.Birthday = DateTime.Parse("2023-09-29 10:14:20");
                v1.LocationId = AddCity();
                v1.HospitalId = AddHospital();
                v1.PhotoId = AddFileAttachment();
                v2.ID = 52;
                v2.PatientName = "hKrpA";
                v2.IdNumber = "wstyOrRXel6Kd9yS9d";
                v2.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu;
                v2.Birthday = DateTime.Parse("2021-11-18 10:14:20");
                v2.LocationId = v1.LocationId; 
                v2.HospitalId = v1.HospitalId; 
                v2.PhotoId = v1.PhotoId; 
                context.Set<Patient>().Add(v1);
                context.Set<Patient>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(PatientBatchVM));

            PatientBatchVM vm = rv.Model as PatientBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Patient>().Find(v1.ID);
                var data2 = context.Set<Patient>().Find(v2.ID);
 				
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            Patient v1 = new Patient();
            Patient v2 = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 35;
                v1.PatientName = "pn";
                v1.IdNumber = "xJU3MwxLEGFqaDfG";
                v1.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v1.Birthday = DateTime.Parse("2023-09-29 10:14:20");
                v1.LocationId = AddCity();
                v1.HospitalId = AddHospital();
                v1.PhotoId = AddFileAttachment();
                v2.ID = 52;
                v2.PatientName = "hKrpA";
                v2.IdNumber = "wstyOrRXel6Kd9yS9d";
                v2.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu;
                v2.Birthday = DateTime.Parse("2021-11-18 10:14:20");
                v2.LocationId = v1.LocationId; 
                v2.HospitalId = v1.HospitalId; 
                v2.PhotoId = v1.PhotoId; 
                context.Set<Patient>().Add(v1);
                context.Set<Patient>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(PatientBatchVM));

            PatientBatchVM vm = rv.Model as PatientBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Patient>().Find(v1.ID);
                var data2 = context.Set<Patient>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as PatientListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Guid AddCity()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.Name = "o";
                v.Test = "WMRsTJO";
                context.Set<City>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }

        private Guid AddHospital()
        {
            Hospital v = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.Name = "C3";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class2;
                v.LocationId = AddCity();
                context.Set<Hospital>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.FileName = "qXEm12wb0y";
                v.FileExt = "iDD";
                v.Path = "bmCERxW42T";
                v.Length = 56;
                v.UploadTime = DateTime.Parse("2024-03-23 10:14:20");
                v.SaveMode = "IMbqvwWiZCf";
                v.ExtraInfo = "ns8Q9OKNg8zg7r";
                v.HandlerInfo = "0S9Hh";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
