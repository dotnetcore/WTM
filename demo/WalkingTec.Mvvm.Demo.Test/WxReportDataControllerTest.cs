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
			
            v.ToWxUser = "mup91Qd";
            v.FrameworkUserId = AddFrameworkUser();
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
            v.TuFang1 = 15;
            v.KM1 = 87;
            v.TuFang2 = 33;
            v.KM2 = 8;
            v.TuFang3 = 21;
            v.KM3 = 8;
            v.TuFang4 = 28;
            v.KM4 = 38;
            v.TuFang5 = 38;
            v.KM5 = 72;
            v.TuFang6 = 30;
            v.KM6 = 92;
            v.YnNi1 = 17;
            v.YnNi1KM = 19;
            v.YnNi2 = 99;
            v.YnNi2KM = 52;
            v.YnNi3 = 85;
            v.YnNi3KM = 98;
            v.YnNi4 = 50;
            v.YnNi4KM = 36;
            v.JiaYou1 = 49;
            v.JiaYou2 = 18;
            v.CanFeiRen = 32;
            v.JieZhiMoney = 57;
            v.Extend01 = "agxCHEs";
            v.Extend02 = "X9UhbLpa";
            v.Extend03 = "NWjipu";
            v.Extend04 = "Afz6u";
            v.Extend05 = "2o4";
            v.Extend07 = 37;
            v.Extend08 = 29;
            v.Extend09 = 23;
            v.Extend10 = 41;
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<WxReportData>().Find(v.ID);
				
                Assert.AreEqual(data.ToWxUser, "mup91Qd");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal);
                Assert.AreEqual(data.TuFang1, 15);
                Assert.AreEqual(data.KM1, 87);
                Assert.AreEqual(data.TuFang2, 33);
                Assert.AreEqual(data.KM2, 8);
                Assert.AreEqual(data.TuFang3, 21);
                Assert.AreEqual(data.KM3, 8);
                Assert.AreEqual(data.TuFang4, 28);
                Assert.AreEqual(data.KM4, 38);
                Assert.AreEqual(data.TuFang5, 38);
                Assert.AreEqual(data.KM5, 72);
                Assert.AreEqual(data.TuFang6, 30);
                Assert.AreEqual(data.KM6, 92);
                Assert.AreEqual(data.YnNi1, 17);
                Assert.AreEqual(data.YnNi1KM, 19);
                Assert.AreEqual(data.YnNi2, 99);
                Assert.AreEqual(data.YnNi2KM, 52);
                Assert.AreEqual(data.YnNi3, 85);
                Assert.AreEqual(data.YnNi3KM, 98);
                Assert.AreEqual(data.YnNi4, 50);
                Assert.AreEqual(data.YnNi4KM, 36);
                Assert.AreEqual(data.JiaYou1, 49);
                Assert.AreEqual(data.JiaYou2, 18);
                Assert.AreEqual(data.CanFeiRen, 32);
                Assert.AreEqual(data.JieZhiMoney, 57);
                Assert.AreEqual(data.Extend01, "agxCHEs");
                Assert.AreEqual(data.Extend02, "X9UhbLpa");
                Assert.AreEqual(data.Extend03, "NWjipu");
                Assert.AreEqual(data.Extend04, "Afz6u");
                Assert.AreEqual(data.Extend05, "2o4");
                Assert.AreEqual(data.Extend07, 37);
                Assert.AreEqual(data.Extend08, 29);
                Assert.AreEqual(data.Extend09, 23);
                Assert.AreEqual(data.Extend10, 41);
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
       			
                v.ToWxUser = "mup91Qd";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v.TuFang1 = 15;
                v.KM1 = 87;
                v.TuFang2 = 33;
                v.KM2 = 8;
                v.TuFang3 = 21;
                v.KM3 = 8;
                v.TuFang4 = 28;
                v.KM4 = 38;
                v.TuFang5 = 38;
                v.KM5 = 72;
                v.TuFang6 = 30;
                v.KM6 = 92;
                v.YnNi1 = 17;
                v.YnNi1KM = 19;
                v.YnNi2 = 99;
                v.YnNi2KM = 52;
                v.YnNi3 = 85;
                v.YnNi3KM = 98;
                v.YnNi4 = 50;
                v.YnNi4KM = 36;
                v.JiaYou1 = 49;
                v.JiaYou2 = 18;
                v.CanFeiRen = 32;
                v.JieZhiMoney = 57;
                v.Extend01 = "agxCHEs";
                v.Extend02 = "X9UhbLpa";
                v.Extend03 = "NWjipu";
                v.Extend04 = "Afz6u";
                v.Extend05 = "2o4";
                v.Extend07 = 37;
                v.Extend08 = 29;
                v.Extend09 = 23;
                v.Extend10 = 41;
                context.Set<WxReportData>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataVM));

            WxReportDataVM vm = rv.Model as WxReportDataVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new WxReportData();
            v.ID = vm.Entity.ID;
       		
            v.ToWxUser = "88pH";
            v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
            v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
            v.TuFang1 = 20;
            v.KM1 = 99;
            v.TuFang2 = 55;
            v.KM2 = 6;
            v.TuFang3 = 55;
            v.KM3 = 39;
            v.TuFang4 = 18;
            v.KM4 = 91;
            v.TuFang5 = 43;
            v.KM5 = 8;
            v.TuFang6 = 66;
            v.KM6 = 88;
            v.YnNi1 = 96;
            v.YnNi1KM = 9;
            v.YnNi2 = 39;
            v.YnNi2KM = 74;
            v.YnNi3 = 48;
            v.YnNi3KM = 91;
            v.YnNi4 = 95;
            v.YnNi4KM = 38;
            v.JiaYou1 = 16;
            v.JiaYou2 = 16;
            v.CanFeiRen = 75;
            v.JieZhiMoney = 7;
            v.Extend01 = "2gljNui12";
            v.Extend02 = "iVlzPf";
            v.Extend03 = "iJG0DT";
            v.Extend04 = "HFB40";
            v.Extend05 = "ftP7uPIJk";
            v.Extend07 = 0;
            v.Extend08 = 43;
            v.Extend09 = 45;
            v.Extend10 = 13;
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
                var data = context.Set<WxReportData>().Find(v.ID);
 				
                Assert.AreEqual(data.ToWxUser, "88pH");
                Assert.AreEqual(data.Type, WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week);
                Assert.AreEqual(data.DataType, WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal);
                Assert.AreEqual(data.TuFang1, 20);
                Assert.AreEqual(data.KM1, 99);
                Assert.AreEqual(data.TuFang2, 55);
                Assert.AreEqual(data.KM2, 6);
                Assert.AreEqual(data.TuFang3, 55);
                Assert.AreEqual(data.KM3, 39);
                Assert.AreEqual(data.TuFang4, 18);
                Assert.AreEqual(data.KM4, 91);
                Assert.AreEqual(data.TuFang5, 43);
                Assert.AreEqual(data.KM5, 8);
                Assert.AreEqual(data.TuFang6, 66);
                Assert.AreEqual(data.KM6, 88);
                Assert.AreEqual(data.YnNi1, 96);
                Assert.AreEqual(data.YnNi1KM, 9);
                Assert.AreEqual(data.YnNi2, 39);
                Assert.AreEqual(data.YnNi2KM, 74);
                Assert.AreEqual(data.YnNi3, 48);
                Assert.AreEqual(data.YnNi3KM, 91);
                Assert.AreEqual(data.YnNi4, 95);
                Assert.AreEqual(data.YnNi4KM, 38);
                Assert.AreEqual(data.JiaYou1, 16);
                Assert.AreEqual(data.JiaYou2, 16);
                Assert.AreEqual(data.CanFeiRen, 75);
                Assert.AreEqual(data.JieZhiMoney, 7);
                Assert.AreEqual(data.Extend01, "2gljNui12");
                Assert.AreEqual(data.Extend02, "iVlzPf");
                Assert.AreEqual(data.Extend03, "iJG0DT");
                Assert.AreEqual(data.Extend04, "HFB40");
                Assert.AreEqual(data.Extend05, "ftP7uPIJk");
                Assert.AreEqual(data.Extend07, 0);
                Assert.AreEqual(data.Extend08, 43);
                Assert.AreEqual(data.Extend09, 45);
                Assert.AreEqual(data.Extend10, 13);
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
        		
                v.ToWxUser = "mup91Qd";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v.TuFang1 = 15;
                v.KM1 = 87;
                v.TuFang2 = 33;
                v.KM2 = 8;
                v.TuFang3 = 21;
                v.KM3 = 8;
                v.TuFang4 = 28;
                v.KM4 = 38;
                v.TuFang5 = 38;
                v.KM5 = 72;
                v.TuFang6 = 30;
                v.KM6 = 92;
                v.YnNi1 = 17;
                v.YnNi1KM = 19;
                v.YnNi2 = 99;
                v.YnNi2KM = 52;
                v.YnNi3 = 85;
                v.YnNi3KM = 98;
                v.YnNi4 = 50;
                v.YnNi4KM = 36;
                v.JiaYou1 = 49;
                v.JiaYou2 = 18;
                v.CanFeiRen = 32;
                v.JieZhiMoney = 57;
                v.Extend01 = "agxCHEs";
                v.Extend02 = "X9UhbLpa";
                v.Extend03 = "NWjipu";
                v.Extend04 = "Afz6u";
                v.Extend05 = "2o4";
                v.Extend07 = 37;
                v.Extend08 = 29;
                v.Extend09 = 23;
                v.Extend10 = 41;
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
                var data = context.Set<WxReportData>().Find(v.ID);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            WxReportData v = new WxReportData();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.ToWxUser = "mup91Qd";
                v.FrameworkUserId = AddFrameworkUser();
                v.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v.TuFang1 = 15;
                v.KM1 = 87;
                v.TuFang2 = 33;
                v.KM2 = 8;
                v.TuFang3 = 21;
                v.KM3 = 8;
                v.TuFang4 = 28;
                v.KM4 = 38;
                v.TuFang5 = 38;
                v.KM5 = 72;
                v.TuFang6 = 30;
                v.KM6 = 92;
                v.YnNi1 = 17;
                v.YnNi1KM = 19;
                v.YnNi2 = 99;
                v.YnNi2KM = 52;
                v.YnNi3 = 85;
                v.YnNi3KM = 98;
                v.YnNi4 = 50;
                v.YnNi4KM = 36;
                v.JiaYou1 = 49;
                v.JiaYou2 = 18;
                v.CanFeiRen = 32;
                v.JieZhiMoney = 57;
                v.Extend01 = "agxCHEs";
                v.Extend02 = "X9UhbLpa";
                v.Extend03 = "NWjipu";
                v.Extend04 = "Afz6u";
                v.Extend05 = "2o4";
                v.Extend07 = 37;
                v.Extend08 = 29;
                v.Extend09 = 23;
                v.Extend10 = 41;
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
				
                v1.ToWxUser = "mup91Qd";
                v1.FrameworkUserId = AddFrameworkUser();
                v1.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v1.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v1.TuFang1 = 15;
                v1.KM1 = 87;
                v1.TuFang2 = 33;
                v1.KM2 = 8;
                v1.TuFang3 = 21;
                v1.KM3 = 8;
                v1.TuFang4 = 28;
                v1.KM4 = 38;
                v1.TuFang5 = 38;
                v1.KM5 = 72;
                v1.TuFang6 = 30;
                v1.KM6 = 92;
                v1.YnNi1 = 17;
                v1.YnNi1KM = 19;
                v1.YnNi2 = 99;
                v1.YnNi2KM = 52;
                v1.YnNi3 = 85;
                v1.YnNi3KM = 98;
                v1.YnNi4 = 50;
                v1.YnNi4KM = 36;
                v1.JiaYou1 = 49;
                v1.JiaYou2 = 18;
                v1.CanFeiRen = 32;
                v1.JieZhiMoney = 57;
                v1.Extend01 = "agxCHEs";
                v1.Extend02 = "X9UhbLpa";
                v1.Extend03 = "NWjipu";
                v1.Extend04 = "Afz6u";
                v1.Extend05 = "2o4";
                v1.Extend07 = 37;
                v1.Extend08 = 29;
                v1.Extend09 = 23;
                v1.Extend10 = 41;
                v2.ToWxUser = "88pH";
                v2.FrameworkUserId = v1.FrameworkUserId; 
                v2.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v2.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v2.TuFang1 = 20;
                v2.KM1 = 99;
                v2.TuFang2 = 55;
                v2.KM2 = 6;
                v2.TuFang3 = 55;
                v2.KM3 = 39;
                v2.TuFang4 = 18;
                v2.KM4 = 91;
                v2.TuFang5 = 43;
                v2.KM5 = 8;
                v2.TuFang6 = 66;
                v2.KM6 = 88;
                v2.YnNi1 = 96;
                v2.YnNi1KM = 9;
                v2.YnNi2 = 39;
                v2.YnNi2KM = 74;
                v2.YnNi3 = 48;
                v2.YnNi3KM = 91;
                v2.YnNi4 = 95;
                v2.YnNi4KM = 38;
                v2.JiaYou1 = 16;
                v2.JiaYou2 = 16;
                v2.CanFeiRen = 75;
                v2.JieZhiMoney = 7;
                v2.Extend01 = "2gljNui12";
                v2.Extend02 = "iVlzPf";
                v2.Extend03 = "iJG0DT";
                v2.Extend04 = "HFB40";
                v2.Extend05 = "ftP7uPIJk";
                v2.Extend07 = 0;
                v2.Extend08 = 43;
                v2.Extend09 = 45;
                v2.Extend10 = 13;
                context.Set<WxReportData>().Add(v1);
                context.Set<WxReportData>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(WxReportDataBatchVM));

            WxReportDataBatchVM vm = rv.Model as WxReportDataBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Extend04 = "zIPY";
            vm.LinkedVM.Extend05 = "bMwEwtx";
            vm.LinkedVM.Extend07 = 2;
            vm.LinkedVM.Extend08 = 95;
            vm.LinkedVM.Extend09 = 58;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Extend04", "");
            vm.FC.Add("LinkedVM.Extend05", "");
            vm.FC.Add("LinkedVM.Extend07", "");
            vm.FC.Add("LinkedVM.Extend08", "");
            vm.FC.Add("LinkedVM.Extend09", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<WxReportData>().Find(v1.ID);
                var data2 = context.Set<WxReportData>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Extend04, "zIPY");
                Assert.AreEqual(data2.Extend04, "zIPY");
                Assert.AreEqual(data1.Extend05, "bMwEwtx");
                Assert.AreEqual(data2.Extend05, "bMwEwtx");
                Assert.AreEqual(data1.Extend07, 2);
                Assert.AreEqual(data2.Extend07, 2);
                Assert.AreEqual(data1.Extend08, 95);
                Assert.AreEqual(data2.Extend08, 95);
                Assert.AreEqual(data1.Extend09, 58);
                Assert.AreEqual(data2.Extend09, 58);
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
				
                v1.ToWxUser = "mup91Qd";
                v1.FrameworkUserId = AddFrameworkUser();
                v1.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v1.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v1.TuFang1 = 15;
                v1.KM1 = 87;
                v1.TuFang2 = 33;
                v1.KM2 = 8;
                v1.TuFang3 = 21;
                v1.KM3 = 8;
                v1.TuFang4 = 28;
                v1.KM4 = 38;
                v1.TuFang5 = 38;
                v1.KM5 = 72;
                v1.TuFang6 = 30;
                v1.KM6 = 92;
                v1.YnNi1 = 17;
                v1.YnNi1KM = 19;
                v1.YnNi2 = 99;
                v1.YnNi2KM = 52;
                v1.YnNi3 = 85;
                v1.YnNi3KM = 98;
                v1.YnNi4 = 50;
                v1.YnNi4KM = 36;
                v1.JiaYou1 = 49;
                v1.JiaYou2 = 18;
                v1.CanFeiRen = 32;
                v1.JieZhiMoney = 57;
                v1.Extend01 = "agxCHEs";
                v1.Extend02 = "X9UhbLpa";
                v1.Extend03 = "NWjipu";
                v1.Extend04 = "Afz6u";
                v1.Extend05 = "2o4";
                v1.Extend07 = 37;
                v1.Extend08 = 29;
                v1.Extend09 = 23;
                v1.Extend10 = 41;
                v2.ToWxUser = "88pH";
                v2.FrameworkUserId = v1.FrameworkUserId; 
                v2.Type = WalkingTec.Mvvm.Demo.Models.WxMsgReport.Week;
                v2.DataType = WalkingTec.Mvvm.Demo.Models.ReportDataType.ToTal;
                v2.TuFang1 = 20;
                v2.KM1 = 99;
                v2.TuFang2 = 55;
                v2.KM2 = 6;
                v2.TuFang3 = 55;
                v2.KM3 = 39;
                v2.TuFang4 = 18;
                v2.KM4 = 91;
                v2.TuFang5 = 43;
                v2.KM5 = 8;
                v2.TuFang6 = 66;
                v2.KM6 = 88;
                v2.YnNi1 = 96;
                v2.YnNi1KM = 9;
                v2.YnNi2 = 39;
                v2.YnNi2KM = 74;
                v2.YnNi3 = 48;
                v2.YnNi3KM = 91;
                v2.YnNi4 = 95;
                v2.YnNi4KM = 38;
                v2.JiaYou1 = 16;
                v2.JiaYou2 = 16;
                v2.CanFeiRen = 75;
                v2.JieZhiMoney = 7;
                v2.Extend01 = "2gljNui12";
                v2.Extend02 = "iVlzPf";
                v2.Extend03 = "iJG0DT";
                v2.Extend04 = "HFB40";
                v2.Extend05 = "ftP7uPIJk";
                v2.Extend07 = 0;
                v2.Extend08 = 43;
                v2.Extend09 = 45;
                v2.Extend10 = 13;
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
                var data1 = context.Set<WxReportData>().Find(v1.ID);
                var data2 = context.Set<WxReportData>().Find(v2.ID);
                Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);
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

                v.FileName = "gCMo";
                v.FileExt = "nBihIRflK";
                v.Path = "3nXwit";
                v.Length = 15;
                v.SaveMode = "mWy";
                v.ExtraInfo = "huG";
                v.HandlerInfo = "AV36S";
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

                v.Email = "msuZKexN";
                v.Gender = WalkingTec.Mvvm.Core.GenderEnum.Male;
                v.CellPhone = "6wR";
                v.HomePhone = "Op30Rg";
                v.Address = "ut1ZUxfD";
                v.ZipCode = "Mj3bn";
                v.ITCode = "2aRRn0fH";
                v.Password = "13aiv";
                v.Name = "eL8Rc8l7";
                v.IsValid = false;
                v.PhotoId = AddPhoto();
                v.TenantCode = "d56B";
                context.Set<FrameworkUser>().Add(v);
                context.SaveChanges();
            }
            return v.ID;
        }


    }
}
