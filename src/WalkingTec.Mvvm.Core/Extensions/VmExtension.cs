using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Core.Extensions
{
    internal static class VmExtension
    {
        internal static void InitRigger(this BaseVM rv, bool passInit = false)
        {
            //如果ViewModel T继承自BaseCRUDVM<>且Id有值，那么自动调用ViewModel的GetById方法
            if (rv is IBaseCRUDVM<TopBasePoco> crudVm)
            {
                crudVm.SetEntityById(crudVm.Entity.ID);
            }

            //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
            if (rv is IBaseBatchVM<BaseVM> batchVm)
            {
                if (batchVm.ListVM != null)
                {
                    batchVm.ListVM.CopyContext(rv);
                    batchVm.ListVM.Ids = batchVm.Ids == null ? new List<string>() : batchVm.Ids.ToList();
                    batchVm.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
                    batchVm.ListVM.NeedPage = false;
                }
                if (batchVm.LinkedVM != null)
                {
                    batchVm.LinkedVM.CopyContext(rv);
                }
                if (batchVm.ListVM != null)
                {
                    //绑定ListVM的OnAfterInitList事件，当ListVM的InitList完成时，自动将操作列移除
                    batchVm.ListVM.OnAfterInitList += (self) =>
                    {
                        self.RemoveActionColumn();
                        self.RemoveAction();
                        if (batchVm.ErrorMessage.Count > 0)
                        {
                            self.AddErrorColumn();
                        }
                    };
                    batchVm.ListVM.DoInitListVM();
                    if (batchVm.ListVM.Searcher != null)
                    {
                        var searcher = batchVm.ListVM.Searcher;
                        searcher.CopyContext(rv);
                        if (passInit == false)
                        {
                            searcher.DoInit();
                        }
                    }
                }
                batchVm.LinkedVM?.DoInit();
                //temp.ListVM.DoSearch();
            }
            //如果ViewModel是ListVM，则初始化Searcher并调用Searcher的InitVM方法
            if (rv is IBasePagedListVM<TopBasePoco, ISearcher> listVm)
            {
                var searcher = listVm.Searcher;
                searcher.CopyContext(rv);
                if (passInit == false)
                {
                    searcher.DoInit();
                }
                listVm.DoInitListVM();

            }
            if (rv is IBaseImport<BaseTemplateVM> import)
            {
                var template = import.Template;
                template.CopyContext(rv);
                template.DoInit();
            }

            //自动调用ViewMode的InitVM方法
            if (passInit == false)
            {
                rv.DoInit();
            }
        }
    }
}
