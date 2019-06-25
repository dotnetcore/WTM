using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    public class BasePaedListVMTest
    {
        private BasePagedListVM<Student,BaseSearcher> _studentListVM;
        private string _seed;

        public BasePaedListVMTest()
        {
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
        [DataTestMethod()]
        [DataRow(100)]
        [DataRow(20)]
        [DataRow(7)]
        public void SearchTest4(int page)
        {
            _studentListVM.Searcher.Limit = 3;
            _studentListVM.Searcher.Page = page;
            _studentListVM.DoSearch();
            Assert.AreEqual(_studentListVM.Searcher.Count, 20);
            Assert.AreEqual(_studentListVM.Searcher.PageCount, 7);
            Assert.AreEqual(_studentListVM.EntityList.Count, 2);
            Assert.AreEqual(_studentListVM.Searcher.Page, 7);
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
}
