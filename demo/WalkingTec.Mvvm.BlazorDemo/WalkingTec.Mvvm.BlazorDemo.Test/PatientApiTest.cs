using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.BlazorDemo.Controllers;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.PatientVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.BlazorDemo.DataAccess;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.Test
{
    [TestClass]
    public class PatientApiTest
    {
        private PatientController _controller;
        private string _seed;

        public PatientApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<PatientController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new PatientSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            PatientVM vm = _controller.Wtm.CreateVM<PatientVM>();
            Patient v = new Patient();
            
            v.ID = 7;
            v.PatientName = "OIagR5Q";
            v.IdNumber = "jrVYOUdsn";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
            v.LocationId = AddCity();
            v.HospitalId = AddHospital();
            v.PhotoId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 7);
                Assert.AreEqual(data.PatientName, "OIagR5Q");
                Assert.AreEqual(data.IdNumber, "jrVYOUdsn");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen);
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
       			
                v.ID = 7;
                v.PatientName = "OIagR5Q";
                v.IdNumber = "jrVYOUdsn";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
                v.LocationId = AddCity();
                v.HospitalId = AddHospital();
                v.PhotoId = AddFileAttachment();
                context.Set<Patient>().Add(v);
                context.SaveChanges();
            }

            PatientVM vm = _controller.Wtm.CreateVM<PatientVM>();
            var oldID = v.ID;
            v = new Patient();
            v.ID = oldID;
       		
            v.PatientName = "Od5FFFT";
            v.IdNumber = "F6F7epAx";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ID", "");
            vm.FC.Add("Entity.PatientName", "");
            vm.FC.Add("Entity.IdNumber", "");
            vm.FC.Add("Entity.Gender", "");
            vm.FC.Add("Entity.Status", "");
            vm.FC.Add("Entity.LocationId", "");
            vm.FC.Add("Entity.HospitalId", "");
            vm.FC.Add("Entity.PhotoId", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
 				
                Assert.AreEqual(data.PatientName, "Od5FFFT");
                Assert.AreEqual(data.IdNumber, "F6F7epAx");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi);
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
        		
                v.ID = 7;
                v.PatientName = "OIagR5Q";
                v.IdNumber = "jrVYOUdsn";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
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
				
                v1.ID = 7;
                v1.PatientName = "OIagR5Q";
                v1.IdNumber = "jrVYOUdsn";
                v1.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v1.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.QueZhen;
                v1.LocationId = AddCity();
                v1.HospitalId = AddHospital();
                v1.PhotoId = AddFileAttachment();
                v2.ID = 80;
                v2.PatientName = "Od5FFFT";
                v2.IdNumber = "F6F7epAx";
                v2.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.YiSi;
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
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Int32 AddCity()
        {
            City v = new City();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                try{

                v.ID = 73;
                v.Name = "av7";
                v.Test = "dLh6GEPa";
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

                v.Name = "5qb";
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

                v.FileName = "JWooDRv6";
                v.FileExt = "4yCx0nS";
                v.Path = "XPJai";
                v.Length = 1;
                v.SaveMode = "HO9";
                v.ExtraInfo = "sAS";
                v.HandlerInfo = "68obg";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
                }
                catch{}
            }
            return v.ID;
        }


    }
}
