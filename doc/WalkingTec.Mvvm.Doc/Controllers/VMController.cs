using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("视图模型层")]
    public class VMController : BaseController
    {

        [ActionDescription("BaseVM")]
        public IActionResult VM()
        {
            return PartialView();
        }


        [ActionDescription("BaseCRUDVM")]
        public IActionResult CRUD()
        {
            return PartialView();
        }

        [ActionDescription("列表介绍")]
        public IActionResult List()
        {
            return PartialView();
        }

        [ActionDescription("动作配置")]
        public IActionResult ListAction()
        {
            return PartialView();
        }

        [ActionDescription("列配置")]
        public IActionResult ListColumn()
        {
            return PartialView();
        }

        [ActionDescription("查询模式")]
        public IActionResult SearchMode()
        {
            return PartialView();
        }

        [ActionDescription("事务")]
        public IActionResult Transaction()
        {
            return PartialView();
        }

        [ActionDescription("ImportVM")]
        public IActionResult Import()
        {
            return PartialView();
        }

        [ActionDescription("BatchVM")]
        public IActionResult Batch()
        {
            return PartialView();
        }
    }
}
