using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BasePaedListVMTest
    {
        private BasePagedListVM<Student,BaseSearcher> _studentListVM;
        private GoodsSpecificationTestListVM _goodsListVM ;
        private string _seed;

        public BasePaedListVMTest()
        {
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(GlobalData))).Returns(new GlobalData());
            mockService.Setup(x => x.GetService(typeof(Configs))).Returns(new Configs());
            GlobalServices.SetServiceProvider(mockService.Object);

            _seed = Guid.NewGuid().ToString();
            _studentListVM = new BasePagedListVM<Student, BaseSearcher>();
            _studentListVM.DC = new DataContext(_seed, DBTypeEnum.Memory);
            for (int i = 1; i <= 20; i++)
            {
                _studentListVM.DC.Set<Student>().Add(new Student
                {
                    LoginName = "s"+i.ToString("00"),
                    Password = "p",
                    Name = "n"+(int)(i/10),
                    IsValid = true
                });
            }
            _studentListVM.DC.SaveChanges();

            _goodsListVM = new GoodsSpecificationTestListVM();
            _goodsListVM.DC = new DataContext(_seed, DBTypeEnum.Memory);
            for (int i = 1; i <= 20; i++)
            {
                _goodsListVM.DC.Set<GoodsSpecification>().Add(new GoodsSpecification
                {
                    Name = "n" + (int)(i / 10),
                    IsValid = i <= 10 ? true : false
                });
            }
            _goodsListVM.DC.SaveChanges();

        }

        [TestMethod]
        public void SearchTest()
        {
            _studentListVM.Searcher.Limit = 10;
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 2);
            Assert.AreEqual(_studentListVM.EntityList.Count, 10);
        }

        [TestMethod]
        public void PersistSearchTest()
        {
            _goodsListVM.Searcher.Limit = 20;
            _goodsListVM.DoSearch();
            Assert.AreEqual(_goodsListVM.Searcher.Count, 10);
            Assert.AreEqual(_goodsListVM.Searcher.PageCount, 1);
            Assert.AreEqual(_goodsListVM.EntityList.Count, 10);            
        }

        [TestMethod]
        public void PersistSearchTest2()
        {
            _goodsListVM.Searcher.Limit = 20;
            _goodsListVM.SearcherMode = ListVMSearchModeEnum.Export;
            _goodsListVM.DoSearch();
            Assert.AreEqual(_goodsListVM.Searcher.Count, 0);
            Assert.AreEqual(_goodsListVM.Searcher.PageCount, 0);
            Assert.AreEqual(_goodsListVM.EntityList.Count, 0);
        }

        [TestMethod]
        public void SearchTest2()
        {
            _studentListVM.Searcher.Limit = 5;
            _studentListVM.Searcher.Page = 2;
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 4);
            Assert.AreEqual(_studentListVM.EntityList.Count, 5);
        }

        [TestMethod]
        [DataTestMethod()]
        [DataRow(-100)]
        [DataRow(-20)]
        [DataRow(0)]
        public void SearchTest3(int page)
        {
            _studentListVM.Searcher.Limit = 3;
            _studentListVM.Searcher.Page = page;
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 7);
            Assert.AreEqual(_studentListVM.EntityList.Count, 3);
            Assert.AreEqual(_studentListVM.Searcher.Page, 1);
        }


        [TestMethod]
        public void SearchWithoutPagingTest()
        {
            _studentListVM.NeedPage = false;
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 1);
            Assert.AreEqual(_studentListVM.EntityList.Count, 20);
        }

        [TestMethod]
        public void SortTest()
        {
            _studentListVM.Searcher.SortInfo = new SortInfo() { Direction = SortDir.Desc, Property = "LoginName" };
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.EntityList[0].LoginName, "s20");
        }
    }

    public class GoodsSpecificationTestListVM : BasePagedListVM<GoodsSpecification, BaseSearcher>
    {
        public override IOrderedQueryable<GoodsSpecification> GetSearchQuery()
        {
            var query = DC.Set<GoodsSpecification>().OrderBy(x => x.ID);
            return query;
        }

        public override IOrderedQueryable<GoodsSpecification> GetExportQuery()
        {
            var query = DC.Set<Major>().Select(x=>new GoodsSpecification { Name = x.MajorName }).OrderBy(x => x.ID);
            return query;
        }
    }
}
