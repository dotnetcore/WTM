using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.Test.VM
{
    [TestClass]
    [TestCategory("List")]
    public class BasePagedListVMTest : CrudTestBase
    {
        private BasePagedListVM<Student, BaseSearcher> _studentListVM;
        private StudentNameListVM _studentNameListVM;
        private GoodsSpecificationTestListVM _goodsListVM;

        public BasePagedListVMTest()
        {
            InitSeed();
            _studentListVM = new BasePagedListVM<Student, BaseSearcher> { Wtm = NewWtmContext() };
            for (int i = 1; i <= 20; i++)
                _studentListVM.DC.Set<Student>().Add(new Student { LoginName = "s" + i.ToString("00"), Password = "[redacted]", Name = "n" + (i / 10), IsValid = true });
            _studentListVM.DC.SaveChanges();
            _studentNameListVM = new StudentNameListVM { DC = new DataContext(Guid.NewGuid().ToString(), DBTypeEnum.Memory), Wtm = NewWtmContext() };
            for (int i = 1; i <= 20; i++)
                _studentNameListVM.DC.Set<Student>().Add(new Student { LoginName = "s" + i.ToString("00"), Password = "[redacted]", Name = i <= 10 ? "张三" : "李四", IsValid = true });
            _studentNameListVM.DC.SaveChanges();
            _goodsListVM = new GoodsSpecificationTestListVM { DC = NewContext() };
            for (int i = 1; i <= 20; i++)
                _goodsListVM.DC.Set<GoodsSpecification>().Add(new GoodsSpecification { Name = "n" + (i / 10), IsValid = i <= 10 });
            _goodsListVM.DC.SaveChanges();
        }

        [TestMethod] public void Search_Page1() { _studentListVM.Searcher.Limit = 10; _studentListVM.DoSearch(); Assert.AreEqual(20, _studentListVM.Searcher.Count); Assert.AreEqual(2, _studentListVM.Searcher.PageCount); Assert.AreEqual(10, _studentListVM.EntityList.Count); }
        [TestMethod] public void Search_Page2() { _studentListVM.Searcher.Limit = 5; _studentListVM.Searcher.Page = 2; _studentListVM.DoSearch(); Assert.AreEqual(20, _studentListVM.Searcher.Count); Assert.AreEqual(4, _studentListVM.Searcher.PageCount); Assert.AreEqual(5, _studentListVM.EntityList.Count); }
        [TestMethod][DataRow(-100)][DataRow(-20)][DataRow(0)] public void Search_InvalidPageClamped(int page) { _studentListVM.Searcher.Limit = 3; _studentListVM.Searcher.Page = page; _studentListVM.DoSearch(); Assert.AreEqual(20, _studentListVM.Searcher.Count); Assert.AreEqual(7, _studentListVM.Searcher.PageCount); Assert.AreEqual(3, _studentListVM.EntityList.Count); Assert.AreEqual(1, _studentListVM.Searcher.Page); }
        [TestMethod] public void Search_WithoutPaging() { _studentListVM.NeedPage = false; _studentListVM.DoSearch(); Assert.AreEqual(20, _studentListVM.Searcher.Count); Assert.AreEqual(1, _studentListVM.Searcher.PageCount); Assert.AreEqual(20, _studentListVM.EntityList.Count); }
        [TestMethod] public void Search_SortDesc() { _studentListVM.Searcher.SortInfo = new SortInfo { Direction = SortDir.Desc, Property = "LoginName" }; _studentListVM.DoSearch(); Assert.AreEqual("s20", _studentListVM.EntityList[0].LoginName); }
        [TestMethod] public void Search_SortAsc() { _studentListVM.Searcher.SortInfo = new SortInfo { Direction = SortDir.Asc, Property = "LoginName" }; _studentListVM.DoSearch(); Assert.AreEqual("s01", _studentListVM.EntityList[0].LoginName); }
        [TestMethod] public void Search_CustomSearcher_FilterByName() { _studentNameListVM.Searcher = new StudentNameSearcher { Name = "张三" }; _studentNameListVM.DoSearch(); Assert.AreEqual(10, _studentNameListVM.Searcher.Count); }
        [TestMethod] public void Search_CustomSearcher_FilterWithSort() { _studentNameListVM.Searcher = new StudentNameSearcher { Name = "张三" }; _studentNameListVM.Searcher.SortInfo = new SortInfo { Direction = SortDir.Desc, Property = "LoginName" }; _studentNameListVM.DoSearch(); Assert.AreEqual(10, _studentNameListVM.Searcher.Count); Assert.AreEqual("s10", _studentNameListVM.EntityList[0].LoginName); }
        [TestMethod] public void Search_CustomSearcher_NoMatch() { _studentNameListVM.Searcher = new StudentNameSearcher { Name = "王五" }; _studentNameListVM.DoSearch(); Assert.AreEqual(0, _studentNameListVM.Searcher.Count); }
        [TestMethod] public void Search_ExportWithActualData() { var vm = new GoodsExportListVM { DC = NewContext() }; vm.DC.Set<Major>().Add(new Major { MajorCode = "001", MajorName = "major1", MajorType = MajorTypeEnum.Required }); vm.DC.Set<Major>().Add(new Major { MajorCode = "002", MajorName = "major2", MajorType = MajorTypeEnum.Optional }); vm.DC.SaveChanges(); vm.SearcherMode = ListVMSearchModeEnum.Export; vm.DoSearch(); Assert.AreEqual(2, vm.Searcher.Count); }
        [TestMethod] public void Search_PersistFiltersDeleted() { _goodsListVM.Searcher.Limit = 20; _goodsListVM.DoSearch(); Assert.AreEqual(10, _goodsListVM.Searcher.Count); }
        [TestMethod] public void Search_ExportModeReturnsCorrectCount() { _goodsListVM.Searcher.Limit = 20; _goodsListVM.SearcherMode = ListVMSearchModeEnum.Export; _goodsListVM.DoSearch(); Assert.AreEqual(0, _goodsListVM.Searcher.Count); }
        [TestMethod] public void DoSearch_NullSearcher_NoLongerNRE() { var vm = new BasePagedListVM<Student, BaseSearcher> { Wtm = NewWtmContext() }; vm.Searcher = null; vm.DoSearch(); Assert.IsNotNull(vm.Searcher); }
        [TestMethod] public void Sort_InvalidPropertyName_SkippedGracefully() { var q = _studentListVM.DC.Set<Student>().AsQueryable(); var r = DCExtension.Sort(q, "[{\"Property\":\"NonExistent\",\"Direction\":0}]"); Assert.IsNull(r, "所有排序属性无效时返回 null"); }
        [TestMethod] public void GetBatchQuery_InvalidSelectorValueField_NoCrash() { var vm = new InvalidSelectorListVM { DC = NewContext(), Wtm = NewWtmContext() }; vm.DC.Set<Student>().Add(new Student { LoginName = "s01", Password = "[redacted]", Name = "n", IsValid = true }); vm.DC.SaveChanges(); vm.SearcherMode = ListVMSearchModeEnum.Batch; vm.DoSearch(); Assert.IsNotNull(vm.EntityList); }
    }

    public class StudentNameSearcher : BaseSearcher { [Display(Name = "姓名")] public string Name { get; set; } }
    public class StudentNameListVM : BasePagedListVM<Student, StudentNameSearcher> { public override IOrderedQueryable<Student> GetSearchQuery() { var q = DC.Set<Student>().AsQueryable(); if (!string.IsNullOrEmpty(Searcher.Name)) q = q.Where(x => x.Name == Searcher.Name); return q.OrderBy(x => x.LoginName); } }
    public class GoodsSpecificationTestListVM : BasePagedListVM<GoodsSpecification, BaseSearcher> { public override IOrderedQueryable<GoodsSpecification> GetSearchQuery() => DC.Set<GoodsSpecification>().OrderBy(x => x.ID); public override IOrderedQueryable<GoodsSpecification> GetExportQuery() => DC.Set<Major>().Select(x => new GoodsSpecification { Name = x.MajorName }).OrderBy(x => x.ID); }
    public class GoodsExportListVM : BasePagedListVM<GoodsSpecification, BaseSearcher> { public override IOrderedQueryable<GoodsSpecification> GetSearchQuery() => DC.Set<GoodsSpecification>().OrderBy(x => x.ID); public override IOrderedQueryable<GoodsSpecification> GetExportQuery() => DC.Set<Major>().OrderBy(x => x.MajorCode).Select(x => new GoodsSpecification { Name = x.MajorName }).OrderBy(x => x.Name); }
    public class InvalidSelectorListVM : BasePagedListVM<Student, BaseSearcher> { public new string SelectorValueField => "NonExistentProperty"; public override IOrderedQueryable<Student> GetSearchQuery() => DC.Set<Student>().OrderBy(x => x.LoginName); }
}
