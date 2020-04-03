using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.RelatedUserVMs
{
    public partial class RelatedUserBatchVM : BaseBatchVM<RelatedUser, RelatedUser_BatchEdit>
    {
        public RelatedUserBatchVM()
        {
            ListVM = new RelatedUserListVM();
            LinkedVM = new RelatedUser_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class RelatedUser_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
