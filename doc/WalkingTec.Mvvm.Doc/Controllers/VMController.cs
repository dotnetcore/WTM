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

        [ActionDescription("ListVM")]
        public IActionResult List()
        {
            return PartialView();
        }

        [ActionDescription("ListAction")]
        public IActionResult ListAction()
        {
            return PartialView();
        }

        [ActionDescription("ListColumn")]
        public IActionResult ListColumn()
        {
            return PartialView();
        }

        [ActionDescription("SearchMode")]
        public IActionResult SearchMode()
        {
            return PartialView();
        }

        [ActionDescription("Export")]
        public IActionResult Export()
        {
            return PartialView();
        }

        [ActionDescription("Transaction")]
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
