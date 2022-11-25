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
    public class PatientApiTest
    {
        private PatientApiController _controller;
        private string _seed;

        public PatientApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<PatientApiController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new PatientApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            PatientApiVM vm = _controller.Wtm.CreateVM<PatientApiVM>();
            Patient v = new Patient();
            
            v.ID = 53;
            v.PatientName = "vMxfOAXiEXE";
            v.IdNumber = "8ultYw";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
            v.Birthday = DateTime.Parse("2021-09-08 10:11:05");
            v.LocationId = AddCity();
            v.HospitalId = AddHospital();
            v.PhotoId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 53);
                Assert.AreEqual(data.PatientName, "vMxfOAXiEXE");
                Assert.AreEqual(data.IdNumber, "8ultYw");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen);
                Assert.AreEqual(data.Birthday, DateTime.Parse("2021-09-08 10:11:05"));
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
       			
                v.ID = 53;
                v.PatientName = "vMxfOAXiEXE";
                v.IdNumber = "8ultYw";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
                v.Birthday = DateTime.Parse("2021-09-08 10:11:05");
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }

            PatientApiVM vm = _controller.Wtm.CreateVM<PatientApiVM>();
            var oldID = v.ID;
            v = new Patient();
            v.ID = oldID;
       		
            v.PatientName = "0HO1X13Aw5lIEVPoa";
            v.IdNumber = "Y";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi;
            v.Birthday = DateTime.Parse("2023-08-02 10:11:05");
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
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
 				
                Assert.AreEqual(data.PatientName, "0HO1X13Aw5lIEVPoa");
                Assert.AreEqual(data.IdNumber, "Y");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi);
                Assert.AreEqual(data.Birthday, DateTime.Parse("2023-08-02 10:11:05"));
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            Patient v = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ID = 53;
                v.PatientName = "vMxfOAXiEXE";
                v.IdNumber = "8ultYw";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
                v.Birthday = DateTime.Parse("2021-09-08 10:11:05");
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            Patient v1 = new Patient();
            Patient v2 = new Patient();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ID = 53;
                v1.PatientName = "vMxfOAXiEXE";
                v1.IdNumber = "8ultYw";
                v1.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v1.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
                v1.Birthday = DateTime.Parse("2021-09-08 10:11:05");
                v1.LocationId = AddCity();
                v1.HospitalId = AddHospital();
                v1.PhotoId = AddFileAttachment();
                v2.ID = 12;
                v2.PatientName = "0HO1X13Aw5lIEVPoa";
                v2.IdNumber = "Y";
                v2.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi;
                v2.Birthday = DateTime.Parse("2023-08-02 10:11:05");
                v2.LocationId = v1.LocationId; 
                v2.HospitalId = v1.HospitalId; 
                v2.PhotoId = v1.PhotoId; 
                context.Set<Patient>().Add(v1);
                context.Set<Patient>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<Patient>().Find(v1.ID);
                var data2 = context.Set<Patient>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Guid AddCity()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.Name = "G";
                v.Test = "3LO4xxeLOy9ThfmUTA";
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

                v.Name = "2m";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1;
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

                v.FileName = "FtnVXMQS";
                v.FileExt = "xz4";
                v.Path = "IXjCx";
                v.Length = 35;
                v.UploadTime = DateTime.Parse("2022-03-17 10:11:05");
                v.SaveMode = "CJrxTVjfYFNrQ";
                v.ExtraInfo = "fSL3Q6nf";
                v.HandlerInfo = "Dp9sVwLvy";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
