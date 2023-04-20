using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Test.Mock;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BasePaedListVMTest
    {
        private BasePagedListVM<Student,BaseSearcher> _studentListVM;
        private GoodsSpecificationTestListVM _goodsListVM ;
        private string _seed;

        public BasePaedListVMTest() {
            _ = Task.Run (Init);
        }

        public async Task Init () {
            _seed = Guid.NewGuid().ToString();
            _studentListVM = new BasePagedListVM<Student, BaseSearcher>();
            _studentListVM.Wtm = MockWtmContext.CreateWtmContext(new DataContext(_seed, DBTypeEnum.Memory));
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
            await _studentListVM.DC.SaveChangesAsync();

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
            await _goodsListVM.DC.SaveChangesAsync();

        }

        [TestMethod]
        public async Task SearchTest()
        {
            _studentListVM.Searcher.Limit = 10;
            await _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 2);
            Assert.AreEqual(_studentListVM.EntityList.Count, 10);
        }

        [TestMethod]
        public async Task PersistSearchTest()
        {
            _goodsListVM.Searcher.Limit = 20;
            await _goodsListVM.DoSearch();
            Assert.AreEqual(_goodsListVM.Searcher.Count, 10);
            Assert.AreEqual(_goodsListVM.Searcher.PageCount, 1);
            Assert.AreEqual(_goodsListVM.EntityList.Count, 10);            
        }

        [TestMethod]
        public async Task PersistSearchTest2()
        {
            _goodsListVM.Searcher.Limit = 20;
            _goodsListVM.SearcherMode = ListVMSearchModeEnum.Export;
            await _goodsListVM.DoSearch();
            Assert.AreEqual(_goodsListVM.Searcher.Count, 0);
            Assert.AreEqual(_goodsListVM.Searcher.PageCount, 0);
            Assert.AreEqual(_goodsListVM.EntityList.Count, 0);
        }

        [TestMethod]
        public async Task SearchTest2()
        {
            _studentListVM.Searcher.Limit = 5;
            _studentListVM.Searcher.Page = 2;
            await _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 4);
            Assert.AreEqual(_studentListVM.EntityList.Count, 5);
        }

        [TestMethod]
        [DataTestMethod()]
        [DataRow(-100)]
        [DataRow(-20)]
        [DataRow(0)]
        public async Task SearchTest3(int page)
        {
            _studentListVM.Searcher.Limit = 3;
            _studentListVM.Searcher.Page = page;
            await _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 7);
            Assert.AreEqual(_studentListVM.EntityList.Count, 3);
            Assert.AreEqual(_studentListVM.Searcher.Page, 1);
        }


        [TestMethod]
        public async Task SearchWithoutPagingTest()
        {
            _studentListVM.NeedPage = false;
            await _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 1);
            Assert.AreEqual(_studentListVM.EntityList.Count, 20);
        }

        [TestMethod]
        public async Task SortTest()
        {
            _studentListVM.Searcher.SortInfo = new SortInfo() { Direction = SortDir.Desc, Property = "LoginName" };
            await _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.EntityList[0].LoginName, "s20");
        }
    }

    public class GoodsSpecificationTestListVM : BasePagedListVM<GoodsSpecification, BaseSearcher>
    {
        public override Task<IOrderedQueryable<GoodsSpecification>> GetSearchQuery()
        {
            var query = DC.Set<GoodsSpecification>().OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

        public override Task<IOrderedQueryable<GoodsSpecification>> GetExportQuery()
        {
            var query = DC.Set<Major>().Select(x=>new GoodsSpecification { Name = x.MajorName }).OrderBy(x => x.ID);
            return Task.FromResult (query);
        }
    }
}
