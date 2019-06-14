using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    public class SetupController : BaseController
    {
        public IActionResult Index()
        {
            var vm = CreateVM<SetupVM>();
            var rv = vm.GetIndex1();
            return Content(rv, "text/html", Encoding.UTF8);
        }


        [HttpPost]
        public IActionResult Index(string ns)
        {
            var vm = CreateVM<SetupVM>();
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            Guid g = Guid.NewGuid();
            string newdir = vm.EntryDir + Path.DirectorySeparatorChar + g.ToString();
            vm.MainNs = ns;
            var rv = vm.GetIndex()
                .Replace("$extradir$", g.ToString())
                .Replace("$extrans$", ns)
                .Replace(@"<form method=""post"" class=""content"">", @"<form method=""post"" class=""content"" action=""/setup/index2"">")
                .Replace("该操作将重置框架相关的代码及配置，请提前做好备份，是否继续？","一个包含VS解决方案的zip文件将会生成，是否继续？");
            return Content(rv, "text/html", Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Index2(SetupVM vm)
        {
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            vm.ExtraDir = vm.EntryDir + Path.DirectorySeparatorChar + vm.ExtraDir;
            vm.MainNs = vm.ExtraNS;
            vm.MainDir = vm.ExtraDir + Path.DirectorySeparatorChar + vm.MainNs;
            string propertydir = vm.MainDir + Path.DirectorySeparatorChar +"Properties";
            if (Directory.Exists(vm.ExtraDir) == false)
            {
                Directory.CreateDirectory(vm.ExtraDir);
            }
            if (Directory.Exists(vm.MainDir) == false)
            {
                Directory.CreateDirectory(vm.MainDir);
            }
            if (Directory.Exists(propertydir) == false)
            {
                Directory.CreateDirectory(propertydir);
            }
            vm.WriteDefaultFiles();
            vm.DoSetup();

            var zipdir = vm.EntryDir + Path.DirectorySeparatorChar +"ZipFiles";
            if(Directory.Exists(zipdir) == false)
            {
                Directory.CreateDirectory(zipdir);
            }
            string g = Guid.NewGuid().ToString();
            var zipfile = zipdir + Path.DirectorySeparatorChar + g + ".zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(vm.ExtraDir, zipfile);

            byte[] rv = System.IO.File.ReadAllBytes(zipfile);
            System.IO.File.Delete(zipfile);
            System.IO.Directory.Delete(vm.ExtraDir, true);
            return File(rv, "application/zip", vm.ExtraNS + ".zip");
        }
    }
}
