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
            
            v.ID = 92;
            v.PatientName = "QtzTKpN";
            v.IdNumber = "HKPW4QpoK";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
            v.LocationId = AddCity();
            v.HospitalId = AddHospital();
            v.PhotoId = AddFileAttachment();
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<Patient>().Find(v.ID);
                
                Assert.AreEqual(data.ID, 92);
                Assert.AreEqual(data.PatientName, "QtzTKpN");
                Assert.AreEqual(data.IdNumber, "HKPW4QpoK");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Male);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang);
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
       			
                v.ID = 92;
                v.PatientName = "QtzTKpN";
                v.IdNumber = "HKPW4QpoK";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
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
       		
            v.PatientName = "ljxnp";
            v.IdNumber = "7mV";
            v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
            v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu;
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
 				
                Assert.AreEqual(data.PatientName, "ljxnp");
                Assert.AreEqual(data.IdNumber, "7mV");
                Assert.AreEqual(data.Gender, WalkingTec.Mvvm.Core.GenderEnum.Female);
                Assert.AreEqual(data.Status, WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu);
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
        		
                v.ID = 92;
                v.PatientName = "QtzTKpN";
                v.IdNumber = "HKPW4QpoK";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
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
				
                v1.ID = 92;
                v1.PatientName = "QtzTKpN";
                v1.IdNumber = "HKPW4QpoK";
                v1.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v1.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.WuZhengZhuang;
                v1.LocationId = AddCity();
                v1.HospitalId = AddHospital();
                v1.PhotoId = AddFileAttachment();
                v2.ID = 26;
                v2.PatientName = "ljxnp";
                v2.IdNumber = "7mV";
                v2.Gender = WalkingTec.Mvvm.Core.GenderEnum.Female;
                v2.Status = WalkingTec.Mvvm.Demo.Models.Virus.PatientStatusEnum.ZhiYu;
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

                v.ID = 72;
                v.Name = "9vVpVuk";
                v.Test = "6XnA";
                context.Set<City>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

        private Guid AddHospital()
        {
            Hospital v = new Hospital();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Name = "Utb";
                v.Level = WalkingTec.Mvvm.Demo.Models.Virus.HospitalLevel.Class1;
                v.LocationId = AddCity();
                context.Set<Hospital>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

        private Guid AddFileAttachment()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "3aX";
                v.FileExt = "VxRAa";
                v.Path = "6FjF";
                v.Length = 29;
                v.SaveMode = "sONfcRL8D";
                v.ExtraInfo = "xa5viZd";
                v.HandlerInfo = "Y4hFty";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
