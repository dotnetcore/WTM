using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Controllers;
using WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo;

namespace WalkingTec.Mvvm.Demo.Test
{
    [TestClass]
    public class WxReportDataApiTest
    {
        private WxReportDataApiController _controller;
        private string _seed;

        public WxReportDataApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<WxReportDataApiController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new WxReportDataApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            WxReportDataApiVM vm = _controller.Wtm.CreateVM<WxReportDataApiVM>();
            WxReportData v = new WxReportData();
            
            v.ToWxUser = "AuMSLD5";
            v.FrameworkUserId = AddFrameworkUser();
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Month;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
            v.TuFang1 = 15;
            v.KM1 = 86;
            v.TuFang2 = 16;
            v.KM2 = 96;
            v.TuFang3 = 79;
            v.KM3 = 32;
            v.TuFang4 = 6;
            v.KM4 = 22;
            v.TuFang5 = 76;
            v.KM5 = 45;
            v.TuFang6 = 44;
            v.KM6 = 83;
            v.YnNi1 = 27;
            v.YnNi1KM = 34;
            v.YnNi2 = 82;
            v.YnNi2KM = 38;
            v.YnNi3 = 61;
            v.YnNi3KM = 15;
            v.YnNi4 = 41;
            v.YnNi4KM = 44;
            v.JiaYou1 = 7;
            v.JiaYou2 = 73;
            v.CanFeiRen = 31;
            v.JieZhiMoney = 7;
            v.Extend01 = "J63";
            v.Extend02 = "FditGAiuD";
            v.Extend03 = "j3niMnylW";
            v.Extend04 = "IHa1qQJPp";
            v.Extend05 = "INuBZ";
            v.Extend07 = 85;
            v.Extend08 = 89;
            v.Extend09 = 21;
            v.Extend10 = 90;
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WxReportData>().Find(v.ID);
                
                Assert.AreEqual(data.ToWxUser, "AuMSLD5");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Month);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed);
                Assert.AreEqual(data.TuFang1, 15);
                Assert.AreEqual(data.KM1, 86);
                Assert.AreEqual(data.TuFang2, 16);
                Assert.AreEqual(data.KM2, 96);
                Assert.AreEqual(data.TuFang3, 79);
                Assert.AreEqual(data.KM3, 32);
                Assert.AreEqual(data.TuFang4, 6);
                Assert.AreEqual(data.KM4, 22);
                Assert.AreEqual(data.TuFang5, 76);
                Assert.AreEqual(data.KM5, 45);
                Assert.AreEqual(data.TuFang6, 44);
                Assert.AreEqual(data.KM6, 83);
                Assert.AreEqual(data.YnNi1, 27);
                Assert.AreEqual(data.YnNi1KM, 34);
                Assert.AreEqual(data.YnNi2, 82);
                Assert.AreEqual(data.YnNi2KM, 38);
                Assert.AreEqual(data.YnNi3, 61);
                Assert.AreEqual(data.YnNi3KM, 15);
                Assert.AreEqual(data.YnNi4, 41);
                Assert.AreEqual(data.YnNi4KM, 44);
                Assert.AreEqual(data.JiaYou1, 7);
                Assert.AreEqual(data.JiaYou2, 73);
                Assert.AreEqual(data.CanFeiRen, 31);
                Assert.AreEqual(data.JieZhiMoney, 7);
                Assert.AreEqual(data.Extend01, "J63");
                Assert.AreEqual(data.Extend02, "FditGAiuD");
                Assert.AreEqual(data.Extend03, "j3niMnylW");
                Assert.AreEqual(data.Extend04, "IHa1qQJPp");
                Assert.AreEqual(data.Extend05, "INuBZ");
                Assert.AreEqual(data.Extend07, 85);
                Assert.AreEqual(data.Extend08, 89);
                Assert.AreEqual(data.Extend09, 21);
                Assert.AreEqual(data.Extend10, 90);
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }
        }

        [TestMethod]
        public void EditTest()
        {
            WxReportData v = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.ToWxUser = "AuMSLD5";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Month;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v.TuFang1 = 15;
                v.KM1 = 86;
                v.TuFang2 = 16;
                v.KM2 = 96;
                v.TuFang3 = 79;
                v.KM3 = 32;
                v.TuFang4 = 6;
                v.KM4 = 22;
                v.TuFang5 = 76;
                v.KM5 = 45;
                v.TuFang6 = 44;
                v.KM6 = 83;
                v.YnNi1 = 27;
                v.YnNi1KM = 34;
                v.YnNi2 = 82;
                v.YnNi2KM = 38;
                v.YnNi3 = 61;
                v.YnNi3KM = 15;
                v.YnNi4 = 41;
                v.YnNi4KM = 44;
                v.JiaYou1 = 7;
                v.JiaYou2 = 73;
                v.CanFeiRen = 31;
                v.JieZhiMoney = 7;
                v.Extend01 = "J63";
                v.Extend02 = "FditGAiuD";
                v.Extend03 = "j3niMnylW";
                v.Extend04 = "IHa1qQJPp";
                v.Extend05 = "INuBZ";
                v.Extend07 = 85;
                v.Extend08 = 89;
                v.Extend09 = 21;
                v.Extend10 = 90;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }

            WxReportDataApiVM vm = _controller.Wtm.CreateVM<WxReportDataApiVM>();
            var oldID = v.ID;
            v = new WxReportData();
            v.ID = oldID;
       		
            v.ToWxUser = "lM8Ywl";
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
            v.TuFang1 = 15;
            v.KM1 = 98;
            v.TuFang2 = 79;
            v.KM2 = 2;
            v.TuFang3 = 33;
            v.KM3 = 32;
            v.TuFang4 = 69;
            v.KM4 = 19;
            v.TuFang5 = 19;
            v.KM5 = 37;
            v.TuFang6 = 47;
            v.KM6 = 55;
            v.YnNi1 = 8;
            v.YnNi1KM = 58;
            v.YnNi2 = 42;
            v.YnNi2KM = 35;
            v.YnNi3 = 11;
            v.YnNi3KM = 99;
            v.YnNi4 = 63;
            v.YnNi4KM = 55;
            v.JiaYou1 = 37;
            v.JiaYou2 = 38;
            v.CanFeiRen = 27;
            v.JieZhiMoney = 74;
            v.Extend01 = "0251Kh";
            v.Extend02 = "d91c7";
            v.Extend03 = "TaxYRsj";
            v.Extend04 = "Uml7";
            v.Extend05 = "6XcYTzvE";
            v.Extend07 = 83;
            v.Extend08 = 89;
            v.Extend09 = 19;
            v.Extend10 = 51;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.ToWxUser", "");
            vm.FC.Add("Entity.FrameworkUserId", "");
            vm.FC.Add("Entity.Type", "");
            vm.FC.Add("Entity.DataType", "");
            vm.FC.Add("Entity.TuFang1", "");
            vm.FC.Add("Entity.KM1", "");
            vm.FC.Add("Entity.TuFang2", "");
            vm.FC.Add("Entity.KM2", "");
            vm.FC.Add("Entity.TuFang3", "");
            vm.FC.Add("Entity.KM3", "");
            vm.FC.Add("Entity.TuFang4", "");
            vm.FC.Add("Entity.KM4", "");
            vm.FC.Add("Entity.TuFang5", "");
            vm.FC.Add("Entity.KM5", "");
            vm.FC.Add("Entity.TuFang6", "");
            vm.FC.Add("Entity.KM6", "");
            vm.FC.Add("Entity.YnNi1", "");
            vm.FC.Add("Entity.YnNi1KM", "");
            vm.FC.Add("Entity.YnNi2", "");
            vm.FC.Add("Entity.YnNi2KM", "");
            vm.FC.Add("Entity.YnNi3", "");
            vm.FC.Add("Entity.YnNi3KM", "");
            vm.FC.Add("Entity.YnNi4", "");
            vm.FC.Add("Entity.YnNi4KM", "");
            vm.FC.Add("Entity.JiaYou1", "");
            vm.FC.Add("Entity.JiaYou2", "");
            vm.FC.Add("Entity.CanFeiRen", "");
            vm.FC.Add("Entity.JieZhiMoney", "");
            vm.FC.Add("Entity.Extend01", "");
            vm.FC.Add("Entity.Extend02", "");
            vm.FC.Add("Entity.Extend03", "");
            vm.FC.Add("Entity.Extend04", "");
            vm.FC.Add("Entity.Extend05", "");
            vm.FC.Add("Entity.Extend07", "");
            vm.FC.Add("Entity.Extend08", "");
            vm.FC.Add("Entity.Extend09", "");
            vm.FC.Add("Entity.Extend10", "");
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WxReportData>().Find(v.ID);
 				
                Assert.AreEqual(data.ToWxUser, "lM8Ywl");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed);
                Assert.AreEqual(data.TuFang1, 15);
                Assert.AreEqual(data.KM1, 98);
                Assert.AreEqual(data.TuFang2, 79);
                Assert.AreEqual(data.KM2, 2);
                Assert.AreEqual(data.TuFang3, 33);
                Assert.AreEqual(data.KM3, 32);
                Assert.AreEqual(data.TuFang4, 69);
                Assert.AreEqual(data.KM4, 19);
                Assert.AreEqual(data.TuFang5, 19);
                Assert.AreEqual(data.KM5, 37);
                Assert.AreEqual(data.TuFang6, 47);
                Assert.AreEqual(data.KM6, 55);
                Assert.AreEqual(data.YnNi1, 8);
                Assert.AreEqual(data.YnNi1KM, 58);
                Assert.AreEqual(data.YnNi2, 42);
                Assert.AreEqual(data.YnNi2KM, 35);
                Assert.AreEqual(data.YnNi3, 11);
                Assert.AreEqual(data.YnNi3KM, 99);
                Assert.AreEqual(data.YnNi4, 63);
                Assert.AreEqual(data.YnNi4KM, 55);
                Assert.AreEqual(data.JiaYou1, 37);
                Assert.AreEqual(data.JiaYou2, 38);
                Assert.AreEqual(data.CanFeiRen, 27);
                Assert.AreEqual(data.JieZhiMoney, 74);
                Assert.AreEqual(data.Extend01, "0251Kh");
                Assert.AreEqual(data.Extend02, "d91c7");
                Assert.AreEqual(data.Extend03, "TaxYRsj");
                Assert.AreEqual(data.Extend04, "Uml7");
                Assert.AreEqual(data.Extend05, "6XcYTzvE");
                Assert.AreEqual(data.Extend07, 83);
                Assert.AreEqual(data.Extend08, 89);
                Assert.AreEqual(data.Extend09, 19);
                Assert.AreEqual(data.Extend10, 51);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            WxReportData v = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ToWxUser = "AuMSLD5";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Month;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v.TuFang1 = 15;
                v.KM1 = 86;
                v.TuFang2 = 16;
                v.KM2 = 96;
                v.TuFang3 = 79;
                v.KM3 = 32;
                v.TuFang4 = 6;
                v.KM4 = 22;
                v.TuFang5 = 76;
                v.KM5 = 45;
                v.TuFang6 = 44;
                v.KM6 = 83;
                v.YnNi1 = 27;
                v.YnNi1KM = 34;
                v.YnNi2 = 82;
                v.YnNi2KM = 38;
                v.YnNi3 = 61;
                v.YnNi3KM = 15;
                v.YnNi4 = 41;
                v.YnNi4KM = 44;
                v.JiaYou1 = 7;
                v.JiaYou2 = 73;
                v.CanFeiRen = 31;
                v.JieZhiMoney = 7;
                v.Extend01 = "J63";
                v.Extend02 = "FditGAiuD";
                v.Extend03 = "j3niMnylW";
                v.Extend04 = "IHa1qQJPp";
                v.Extend05 = "INuBZ";
                v.Extend07 = 85;
                v.Extend08 = 89;
                v.Extend09 = 21;
                v.Extend10 = 90;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            WxReportData v1 = new WxReportData();
            WxReportData v2 = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ToWxUser = "AuMSLD5";
                v1.FrameworkUserId = AddFrameworkUser();
                v1.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Month;
                v1.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v1.TuFang1 = 15;
                v1.KM1 = 86;
                v1.TuFang2 = 16;
                v1.KM2 = 96;
                v1.TuFang3 = 79;
                v1.KM3 = 32;
                v1.TuFang4 = 6;
                v1.KM4 = 22;
                v1.TuFang5 = 76;
                v1.KM5 = 45;
                v1.TuFang6 = 44;
                v1.KM6 = 83;
                v1.YnNi1 = 27;
                v1.YnNi1KM = 34;
                v1.YnNi2 = 82;
                v1.YnNi2KM = 38;
                v1.YnNi3 = 61;
                v1.YnNi3KM = 15;
                v1.YnNi4 = 41;
                v1.YnNi4KM = 44;
                v1.JiaYou1 = 7;
                v1.JiaYou2 = 73;
                v1.CanFeiRen = 31;
                v1.JieZhiMoney = 7;
                v1.Extend01 = "J63";
                v1.Extend02 = "FditGAiuD";
                v1.Extend03 = "j3niMnylW";
                v1.Extend04 = "IHa1qQJPp";
                v1.Extend05 = "INuBZ";
                v1.Extend07 = 85;
                v1.Extend08 = 89;
                v1.Extend09 = 21;
                v1.Extend10 = 90;
                v2.ToWxUser = "lM8Ywl";
                v2.FrameworkUserId = v1.FrameworkUserId; 
                v2.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v2.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v2.TuFang1 = 15;
                v2.KM1 = 98;
                v2.TuFang2 = 79;
                v2.KM2 = 2;
                v2.TuFang3 = 33;
                v2.KM3 = 32;
                v2.TuFang4 = 69;
                v2.KM4 = 19;
                v2.TuFang5 = 19;
                v2.KM5 = 37;
                v2.TuFang6 = 47;
                v2.KM6 = 55;
                v2.YnNi1 = 8;
                v2.YnNi1KM = 58;
                v2.YnNi2 = 42;
                v2.YnNi2KM = 35;
                v2.YnNi3 = 11;
                v2.YnNi3KM = 99;
                v2.YnNi4 = 63;
                v2.YnNi4KM = 55;
                v2.JiaYou1 = 37;
                v2.JiaYou2 = 38;
                v2.CanFeiRen = 27;
                v2.JieZhiMoney = 74;
                v2.Extend01 = "0251Kh";
                v2.Extend02 = "d91c7";
                v2.Extend03 = "TaxYRsj";
                v2.Extend04 = "Uml7";
                v2.Extend05 = "6XcYTzvE";
                v2.Extend07 = 83;
                v2.Extend08 = 89;
                v2.Extend09 = 19;
                v2.Extend10 = 51;
                context.Set<WxReportData>().Add(v1);
                context.Set<WxReportData>().Add(v2);
                context.SaveChanges();
            }

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<WxReportData>().Find(v1.ID);
                var data2 = context.Set<WxReportData>().Find(v2.ID);
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "WaHmxgy";
                v.FileExt = "rvf";
                v.Path = "6o6CaxLXf";
                v.Length = 22;
                v.SaveMode = "LFJ";
                v.ExtraInfo = "NogM25dYo";
                v.HandlerInfo = "4TT3Xg9";
                context.Set<FileAttachment>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }

        private Guid AddFrameworkUser()
        {
            FrameworkUser v = new FrameworkUser();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.Email = "8ZyI";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "dX21O8c";
                v.HomePhone = "vFp2";
                v.Address = "iPhDQt";
                v.ZipCode = "myRTaWOE3";
                v.ITCode = "6XoevT";
                v.Password = "TEnQ";
                v.Name = "QvC";
                v.IsValid = true;
                v.PhotoId = AddPhoto();
                v.TenantCode = "yDaV";
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
