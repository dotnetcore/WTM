using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
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

        [ActionDescription("PagedListVM")]
        public IActionResult List()
        {
            return PartialView();
        }

        [ActionDescription("ListVMAction")]
        public IActionResult ListAction()
        {
            return PartialView();
        }

        [ActionDescription("ListVMColumn")]
        public IActionResult ListColumn()
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
