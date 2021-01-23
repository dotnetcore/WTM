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
    public class WxReportDataControllerTest
    {
        private WxReportDataController _controller;
        private string _seed;

        public WxReportDataControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<WxReportDataController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as WxReportDataListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataVM));

            WxReportDataVM vm = rv.Model as WxReportDataVM;
            WxReportData v = new WxReportData();
			
            v.ToWxUser = "dd2nxbM";
            v.FrameworkUserId = AddFrameworkUser();
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
            v.TuFang1 = 39;
            v.KM1 = 8;
            v.TuFang2 = 11;
            v.KM2 = 98;
            v.TuFang3 = 10;
            v.KM3 = 84;
            v.TuFang4 = 1;
            v.KM4 = 70;
            v.TuFang5 = 90;
            v.KM5 = 42;
            v.TuFang6 = 80;
            v.KM6 = 82;
            v.YnNi1 = 30;
            v.YnNi1KM = 45;
            v.YnNi2 = 58;
            v.YnNi2KM = 41;
            v.YnNi3 = 30;
            v.YnNi3KM = 72;
            v.YnNi4 = 6;
            v.YnNi4KM = 2;
            v.JiaYou1 = 40;
            v.JiaYou2 = 86;
            v.CanFeiRen = 3;
            v.JieZhiMoney = 36;
            v.Extend01 = "vuhEQo7Fj";
            v.Extend02 = "gOWx9Vz";
            v.Extend03 = "DhesrHFvi";
            v.Extend04 = "FrWUzWAW";
            v.Extend05 = "bG5PTZ";
            v.Extend07 = 3;
            v.Extend08 = 15;
            v.Extend09 = 89;
            v.Extend10 = 86;
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WxReportData>().FirstOrDefault();
				
                Assert.AreEqual(data.ToWxUser, "dd2nxbM");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed);
                Assert.AreEqual(data.TuFang1, 39);
                Assert.AreEqual(data.KM1, 8);
                Assert.AreEqual(data.TuFang2, 11);
                Assert.AreEqual(data.KM2, 98);
                Assert.AreEqual(data.TuFang3, 10);
                Assert.AreEqual(data.KM3, 84);
                Assert.AreEqual(data.TuFang4, 1);
                Assert.AreEqual(data.KM4, 70);
                Assert.AreEqual(data.TuFang5, 90);
                Assert.AreEqual(data.KM5, 42);
                Assert.AreEqual(data.TuFang6, 80);
                Assert.AreEqual(data.KM6, 82);
                Assert.AreEqual(data.YnNi1, 30);
                Assert.AreEqual(data.YnNi1KM, 45);
                Assert.AreEqual(data.YnNi2, 58);
                Assert.AreEqual(data.YnNi2KM, 41);
                Assert.AreEqual(data.YnNi3, 30);
                Assert.AreEqual(data.YnNi3KM, 72);
                Assert.AreEqual(data.YnNi4, 6);
                Assert.AreEqual(data.YnNi4KM, 2);
                Assert.AreEqual(data.JiaYou1, 40);
                Assert.AreEqual(data.JiaYou2, 86);
                Assert.AreEqual(data.CanFeiRen, 3);
                Assert.AreEqual(data.JieZhiMoney, 36);
                Assert.AreEqual(data.Extend01, "vuhEQo7Fj");
                Assert.AreEqual(data.Extend02, "gOWx9Vz");
                Assert.AreEqual(data.Extend03, "DhesrHFvi");
                Assert.AreEqual(data.Extend04, "FrWUzWAW");
                Assert.AreEqual(data.Extend05, "bG5PTZ");
                Assert.AreEqual(data.Extend07, 3);
                Assert.AreEqual(data.Extend08, 15);
                Assert.AreEqual(data.Extend09, 89);
                Assert.AreEqual(data.Extend10, 86);
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
       			
                v.ToWxUser = "dd2nxbM";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v.TuFang1 = 39;
                v.KM1 = 8;
                v.TuFang2 = 11;
                v.KM2 = 98;
                v.TuFang3 = 10;
                v.KM3 = 84;
                v.TuFang4 = 1;
                v.KM4 = 70;
                v.TuFang5 = 90;
                v.KM5 = 42;
                v.TuFang6 = 80;
                v.KM6 = 82;
                v.YnNi1 = 30;
                v.YnNi1KM = 45;
                v.YnNi2 = 58;
                v.YnNi2KM = 41;
                v.YnNi3 = 30;
                v.YnNi3KM = 72;
                v.YnNi4 = 6;
                v.YnNi4KM = 2;
                v.JiaYou1 = 40;
                v.JiaYou2 = 86;
                v.CanFeiRen = 3;
                v.JieZhiMoney = 36;
                v.Extend01 = "vuhEQo7Fj";
                v.Extend02 = "gOWx9Vz";
                v.Extend03 = "DhesrHFvi";
                v.Extend04 = "FrWUzWAW";
                v.Extend05 = "bG5PTZ";
                v.Extend07 = 3;
                v.Extend08 = 15;
                v.Extend09 = 89;
                v.Extend10 = 86;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataVM));

            WxReportDataVM vm = rv.Model as WxReportDataVM;
            v = new WxReportData();
            v.ID = vm.Entity.ID;
       		
            v.ToWxUser = "8Wy2";
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
            v.TuFang1 = 93;
            v.KM1 = 99;
            v.TuFang2 = 0;
            v.KM2 = 73;
            v.TuFang3 = 22;
            v.KM3 = 35;
            v.TuFang4 = 68;
            v.KM4 = 45;
            v.TuFang5 = 53;
            v.KM5 = 79;
            v.TuFang6 = 83;
            v.KM6 = 67;
            v.YnNi1 = 24;
            v.YnNi1KM = 13;
            v.YnNi2 = 50;
            v.YnNi2KM = 11;
            v.YnNi3 = 14;
            v.YnNi3KM = 82;
            v.YnNi4 = 15;
            v.YnNi4KM = 51;
            v.JiaYou1 = 87;
            v.JiaYou2 = 39;
            v.CanFeiRen = 75;
            v.JieZhiMoney = 12;
            v.Extend01 = "KsmfV8B";
            v.Extend02 = "SHvY";
            v.Extend03 = "iFLE5";
            v.Extend04 = "cQlG04";
            v.Extend05 = "WsMRwUV6";
            v.Extend07 = 82;
            v.Extend08 = 98;
            v.Extend09 = 48;
            v.Extend10 = 46;
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
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WxReportData>().FirstOrDefault();
 				
                Assert.AreEqual(data.ToWxUser, "8Wy2");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal);
                Assert.AreEqual(data.TuFang1, 93);
                Assert.AreEqual(data.KM1, 99);
                Assert.AreEqual(data.TuFang2, 0);
                Assert.AreEqual(data.KM2, 73);
                Assert.AreEqual(data.TuFang3, 22);
                Assert.AreEqual(data.KM3, 35);
                Assert.AreEqual(data.TuFang4, 68);
                Assert.AreEqual(data.KM4, 45);
                Assert.AreEqual(data.TuFang5, 53);
                Assert.AreEqual(data.KM5, 79);
                Assert.AreEqual(data.TuFang6, 83);
                Assert.AreEqual(data.KM6, 67);
                Assert.AreEqual(data.YnNi1, 24);
                Assert.AreEqual(data.YnNi1KM, 13);
                Assert.AreEqual(data.YnNi2, 50);
                Assert.AreEqual(data.YnNi2KM, 11);
                Assert.AreEqual(data.YnNi3, 14);
                Assert.AreEqual(data.YnNi3KM, 82);
                Assert.AreEqual(data.YnNi4, 15);
                Assert.AreEqual(data.YnNi4KM, 51);
                Assert.AreEqual(data.JiaYou1, 87);
                Assert.AreEqual(data.JiaYou2, 39);
                Assert.AreEqual(data.CanFeiRen, 75);
                Assert.AreEqual(data.JieZhiMoney, 12);
                Assert.AreEqual(data.Extend01, "KsmfV8B");
                Assert.AreEqual(data.Extend02, "SHvY");
                Assert.AreEqual(data.Extend03, "iFLE5");
                Assert.AreEqual(data.Extend04, "cQlG04");
                Assert.AreEqual(data.Extend05, "WsMRwUV6");
                Assert.AreEqual(data.Extend07, 82);
                Assert.AreEqual(data.Extend08, 98);
                Assert.AreEqual(data.Extend09, 48);
                Assert.AreEqual(data.Extend10, 46);
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            WxReportData v = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.ToWxUser = "dd2nxbM";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v.TuFang1 = 39;
                v.KM1 = 8;
                v.TuFang2 = 11;
                v.KM2 = 98;
                v.TuFang3 = 10;
                v.KM3 = 84;
                v.TuFang4 = 1;
                v.KM4 = 70;
                v.TuFang5 = 90;
                v.KM5 = 42;
                v.TuFang6 = 80;
                v.KM6 = 82;
                v.YnNi1 = 30;
                v.YnNi1KM = 45;
                v.YnNi2 = 58;
                v.YnNi2KM = 41;
                v.YnNi3 = 30;
                v.YnNi3KM = 72;
                v.YnNi4 = 6;
                v.YnNi4KM = 2;
                v.JiaYou1 = 40;
                v.JiaYou2 = 86;
                v.CanFeiRen = 3;
                v.JieZhiMoney = 36;
                v.Extend01 = "vuhEQo7Fj";
                v.Extend02 = "gOWx9Vz";
                v.Extend03 = "DhesrHFvi";
                v.Extend04 = "FrWUzWAW";
                v.Extend05 = "bG5PTZ";
                v.Extend07 = 3;
                v.Extend08 = 15;
                v.Extend09 = 89;
                v.Extend10 = 86;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataVM));

            WxReportDataVM vm = rv.Model as WxReportDataVM;
            v = new WxReportData();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<WxReportData>().Count(), 1);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            WxReportData v = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ToWxUser = "dd2nxbM";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v.TuFang1 = 39;
                v.KM1 = 8;
                v.TuFang2 = 11;
                v.KM2 = 98;
                v.TuFang3 = 10;
                v.KM3 = 84;
                v.TuFang4 = 1;
                v.KM4 = 70;
                v.TuFang5 = 90;
                v.KM5 = 42;
                v.TuFang6 = 80;
                v.KM6 = 82;
                v.YnNi1 = 30;
                v.YnNi1KM = 45;
                v.YnNi2 = 58;
                v.YnNi2KM = 41;
                v.YnNi3 = 30;
                v.YnNi3KM = 72;
                v.YnNi4 = 6;
                v.YnNi4KM = 2;
                v.JiaYou1 = 40;
                v.JiaYou2 = 86;
                v.CanFeiRen = 3;
                v.JieZhiMoney = 36;
                v.Extend01 = "vuhEQo7Fj";
                v.Extend02 = "gOWx9Vz";
                v.Extend03 = "DhesrHFvi";
                v.Extend04 = "FrWUzWAW";
                v.Extend05 = "bG5PTZ";
                v.Extend07 = 3;
                v.Extend08 = 15;
                v.Extend09 = 89;
                v.Extend10 = 86;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            WxReportData v1 = new WxReportData();
            WxReportData v2 = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ToWxUser = "dd2nxbM";
                v1.FrameworkUserId = AddFrameworkUser();
                v1.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v1.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v1.TuFang1 = 39;
                v1.KM1 = 8;
                v1.TuFang2 = 11;
                v1.KM2 = 98;
                v1.TuFang3 = 10;
                v1.KM3 = 84;
                v1.TuFang4 = 1;
                v1.KM4 = 70;
                v1.TuFang5 = 90;
                v1.KM5 = 42;
                v1.TuFang6 = 80;
                v1.KM6 = 82;
                v1.YnNi1 = 30;
                v1.YnNi1KM = 45;
                v1.YnNi2 = 58;
                v1.YnNi2KM = 41;
                v1.YnNi3 = 30;
                v1.YnNi3KM = 72;
                v1.YnNi4 = 6;
                v1.YnNi4KM = 2;
                v1.JiaYou1 = 40;
                v1.JiaYou2 = 86;
                v1.CanFeiRen = 3;
                v1.JieZhiMoney = 36;
                v1.Extend01 = "vuhEQo7Fj";
                v1.Extend02 = "gOWx9Vz";
                v1.Extend03 = "DhesrHFvi";
                v1.Extend04 = "FrWUzWAW";
                v1.Extend05 = "bG5PTZ";
                v1.Extend07 = 3;
                v1.Extend08 = 15;
                v1.Extend09 = 89;
                v1.Extend10 = 86;
                v2.ToWxUser = "8Wy2";
                v2.FrameworkUserId = v1.FrameworkUserId; 
                v2.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v2.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v2.TuFang1 = 93;
                v2.KM1 = 99;
                v2.TuFang2 = 0;
                v2.KM2 = 73;
                v2.TuFang3 = 22;
                v2.KM3 = 35;
                v2.TuFang4 = 68;
                v2.KM4 = 45;
                v2.TuFang5 = 53;
                v2.KM5 = 79;
                v2.TuFang6 = 83;
                v2.KM6 = 67;
                v2.YnNi1 = 24;
                v2.YnNi1KM = 13;
                v2.YnNi2 = 50;
                v2.YnNi2KM = 11;
                v2.YnNi3 = 14;
                v2.YnNi3KM = 82;
                v2.YnNi4 = 15;
                v2.YnNi4KM = 51;
                v2.JiaYou1 = 87;
                v2.JiaYou2 = 39;
                v2.CanFeiRen = 75;
                v2.JieZhiMoney = 12;
                v2.Extend01 = "KsmfV8B";
                v2.Extend02 = "SHvY";
                v2.Extend03 = "iFLE5";
                v2.Extend04 = "cQlG04";
                v2.Extend05 = "WsMRwUV6";
                v2.Extend07 = 82;
                v2.Extend08 = 98;
                v2.Extend09 = 48;
                v2.Extend10 = 46;
                context.Set<WxReportData>().Add(v1);
                context.Set<WxReportData>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataBatchVM));

            WxReportDataBatchVM vm = rv.Model as WxReportDataBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
            vm.LinkedVM.JiaYou1 = 81;
            vm.LinkedVM.Extend04 = "b4h7";
            vm.LinkedVM.Extend05 = "G5Vi4";
            vm.LinkedVM.Extend07 = 39;
            vm.LinkedVM.Extend08 = 98;
            vm.LinkedVM.Extend09 = 60;
            vm.LinkedVM.Extend10 = 88;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.DataType", "");
            vm.FC.Add("LinkedVM.JiaYou1", "");
            vm.FC.Add("LinkedVM.Extend04", "");
            vm.FC.Add("LinkedVM.Extend05", "");
            vm.FC.Add("LinkedVM.Extend07", "");
            vm.FC.Add("LinkedVM.Extend08", "");
            vm.FC.Add("LinkedVM.Extend09", "");
            vm.FC.Add("LinkedVM.Extend10", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<WxReportData>().Find(v1.ID);
                var data2 = context.Set<WxReportData>().Find(v2.ID);
 				
                Assert.AreEqual(data1.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal);
                Assert.AreEqual(data2.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal);
                Assert.AreEqual(data1.JiaYou1, 81);
                Assert.AreEqual(data2.JiaYou1, 81);
                Assert.AreEqual(data1.Extend04, "b4h7");
                Assert.AreEqual(data2.Extend04, "b4h7");
                Assert.AreEqual(data1.Extend05, "G5Vi4");
                Assert.AreEqual(data2.Extend05, "G5Vi4");
                Assert.AreEqual(data1.Extend07, 39);
                Assert.AreEqual(data2.Extend07, 39);
                Assert.AreEqual(data1.Extend08, 98);
                Assert.AreEqual(data2.Extend08, 98);
                Assert.AreEqual(data1.Extend09, 60);
                Assert.AreEqual(data2.Extend09, 60);
                Assert.AreEqual(data1.Extend10, 88);
                Assert.AreEqual(data2.Extend10, 88);
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            WxReportData v1 = new WxReportData();
            WxReportData v2 = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.ToWxUser = "dd2nxbM";
                v1.FrameworkUserId = AddFrameworkUser();
                v1.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v1.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.Detailed;
                v1.TuFang1 = 39;
                v1.KM1 = 8;
                v1.TuFang2 = 11;
                v1.KM2 = 98;
                v1.TuFang3 = 10;
                v1.KM3 = 84;
                v1.TuFang4 = 1;
                v1.KM4 = 70;
                v1.TuFang5 = 90;
                v1.KM5 = 42;
                v1.TuFang6 = 80;
                v1.KM6 = 82;
                v1.YnNi1 = 30;
                v1.YnNi1KM = 45;
                v1.YnNi2 = 58;
                v1.YnNi2KM = 41;
                v1.YnNi3 = 30;
                v1.YnNi3KM = 72;
                v1.YnNi4 = 6;
                v1.YnNi4KM = 2;
                v1.JiaYou1 = 40;
                v1.JiaYou2 = 86;
                v1.CanFeiRen = 3;
                v1.JieZhiMoney = 36;
                v1.Extend01 = "vuhEQo7Fj";
                v1.Extend02 = "gOWx9Vz";
                v1.Extend03 = "DhesrHFvi";
                v1.Extend04 = "FrWUzWAW";
                v1.Extend05 = "bG5PTZ";
                v1.Extend07 = 3;
                v1.Extend08 = 15;
                v1.Extend09 = 89;
                v1.Extend10 = 86;
                v2.ToWxUser = "8Wy2";
                v2.FrameworkUserId = v1.FrameworkUserId; 
                v2.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Day;
                v2.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v2.TuFang1 = 93;
                v2.KM1 = 99;
                v2.TuFang2 = 0;
                v2.KM2 = 73;
                v2.TuFang3 = 22;
                v2.KM3 = 35;
                v2.TuFang4 = 68;
                v2.KM4 = 45;
                v2.TuFang5 = 53;
                v2.KM5 = 79;
                v2.TuFang6 = 83;
                v2.KM6 = 67;
                v2.YnNi1 = 24;
                v2.YnNi1KM = 13;
                v2.YnNi2 = 50;
                v2.YnNi2KM = 11;
                v2.YnNi3 = 14;
                v2.YnNi3KM = 82;
                v2.YnNi4 = 15;
                v2.YnNi4KM = 51;
                v2.JiaYou1 = 87;
                v2.JiaYou2 = 39;
                v2.CanFeiRen = 75;
                v2.JieZhiMoney = 12;
                v2.Extend01 = "KsmfV8B";
                v2.Extend02 = "SHvY";
                v2.Extend03 = "iFLE5";
                v2.Extend04 = "cQlG04";
                v2.Extend05 = "WsMRwUV6";
                v2.Extend07 = 82;
                v2.Extend08 = 98;
                v2.Extend09 = 48;
                v2.Extend10 = 46;
                context.Set<WxReportData>().Add(v1);
                context.Set<WxReportData>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataBatchVM));

            WxReportDataBatchVM vm = rv.Model as WxReportDataBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<WxReportData>().Count(), 2);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as WxReportDataListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }

        private Guid AddPhoto()
        {
            FileAttachment v = new FileAttachment();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {

                v.FileName = "DRWFWJcx";
                v.FileExt = "NsQlwxEog";
                v.Path = "jDAuBH";
                v.Length = 35;
                v.SaveMode = "AesrR5";
                v.ExtraInfo = "JnGMFC4";
                v.HandlerInfo = "PMi6";
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

                v.Email = "plNsBI";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "l5dX";
                v.HomePhone = "WJmAK1DA";
                v.Address = "IC1uDc";
                v.ZipCode = "xkVMrJ6J";
                v.ITCode = "02k";
                v.Password = "I98w4j2";
                v.Name = "BJX";
                v.IsValid = true;
                v.PhotoId = AddPhoto();
                v.TenantCode = "dwXXt4WI";
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
