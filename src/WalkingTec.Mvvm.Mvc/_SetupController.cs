//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using WalkingTec.Mvvm.Core;

//namespace WalkingTec.Mvvm.Mvc
//{
//    [DebugOnly]
//    public class _SetupController : BaseController
//    {
//        public IActionResult Index()
//        {
//            var vm = CreateVM<SetupVM>();
//            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
//            var rv = vm.GetIndex();
//            return  Content(rv, "text/html", Encoding.UTF8);
//        }

//        [HttpPost]
//        public IActionResult Index(SetupVM vm)
//        {
//            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
//            vm.DoSetup();
//            string rv = $@"
//<html>
//<body>
//    {Program._localizer["SetupSuccess"]}
//</body>
//</html>";
//            return Content(rv, "text/html", Encoding.UTF8);
//        }
//    }
//}
