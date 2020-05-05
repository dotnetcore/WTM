using System;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    public class SetupController : BaseController
    {
        public IActionResult Index()
        {
            var vm = CreateVM<SetupVM>();
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            string rv = "";
            if (requestCulture.RequestCulture.Culture.Name.ToLower().StartsWith("zh"))
            {
                rv = vm.GetIndex1();
            }
            else
            {
                rv = vm.GetIndex1En();
            }
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
            string rv = "";
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            if (requestCulture.RequestCulture.Culture.Name.ToLower().StartsWith("zh"))
            {
                rv = vm.GetIndex();
            }
            else
            {
                rv = vm.GetIndex(true);

            }
            rv = rv.Replace("$extradir$", g.ToString())
                .Replace("$extrans$", ns)
                .Replace(@"<form method=""post"" class=""content"">", @"<form method=""post"" class=""content"" action=""/setup/index2"">");
            return Content(rv, "text/html", Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Index2(SetupVM vm)
        {
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            vm.ExtraDir = vm.EntryDir + Path.DirectorySeparatorChar + vm.ExtraDir;
            vm.MainNs = vm.ExtraNS;
            vm.MainDir = vm.ExtraDir + Path.DirectorySeparatorChar + vm.MainNs;
            string propertydir = vm.MainDir + Path.DirectorySeparatorChar + "Properties";
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

            var zipdir = vm.EntryDir + Path.DirectorySeparatorChar + "ZipFiles";
            if (Directory.Exists(zipdir) == false)
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
