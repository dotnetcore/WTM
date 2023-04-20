using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.Support.Quartz;
using WalkingTec.Mvvm.Demo;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs;

namespace WalkingTec.Mvvm.ConsoleDemo
{
    class Program
    {
        public static ServiceProvider Provider { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Start...");
            DateTime check = DateTime.Now;
            StartUpAsync();
            Console.WriteLine(DateTime.Now.Subtract(check).TotalSeconds);
            //var context = GetWtmContext();
            //var test = context.CallAPI("baidu","/").Result;
            List<int> ids = new List<int>();
            ids.Add( AddSchool("111","111","111"));
            ids.Add(AddSchool("222", "222", "222"));
            BatchEditSchool(ids);
            //Upload();
            Console.ReadLine();
        }

        static void StartUpAsync()
        {
            var services = new ServiceCollection();
            services.AddWtmContextForConsole();
            Provider = services.BuildServiceProvider();
            _= Provider.GetRequiredService<QuartzHostService>().StartAsync(new System.Threading.CancellationToken());
        }

        static WTMContext GetWtmContext()
        {
            var rv = Provider.GetRequiredService<WTMContext>();
            rv.SetServiceProvider(Provider);
            return rv;
        }

        static WtmFileProvider GetFileProvider()
        {
            var rv = Provider.GetRequiredService<WtmFileProvider>();
            return rv;
        }

        static int AddSchool(string name,string code,string remark)
        {
            SchoolVM vm = GetWtmContext().CreateVM<SchoolVM>();

            //WtmFileProvider fp = new WtmFileProvider(vm.Wtm.ConfigInfo);
            //var fh = fp.CreateFileHandler();
            //var fs = File.OpenRead("../../../");
            //var file = fh.Upload("vue1.png", fs.Length, fs);

            vm.Entity = new Demo.Models.School
            {
                SchoolCode = code,
                SchoolName = name,
                SchoolType = Demo.Models.SchoolTypeEnum.PRI,
                Remark = remark,
                //Photos = new System.Collections.Generic.List<Demo.Models.SchoolPhoto>
                //{
                //    new Demo.Models.SchoolPhoto
                //    {
                //         FileId = Guid.Parse(file.GetID()),
                //         SchoolId = 0
                //    }
                //},
                Majors = new System.Collections.Generic.List<Demo.Models.Major>
                {
                    new Demo.Models.Major
                    {
                        MajorName = "aaa",
                        MajorCode = "123",
                        MajorType = Demo.Models.MajorTypeEnum.Optional,
                        SchoolId = 0,
                    }
                }
            };
            vm.Validate();
            if(vm.MSD.IsValid == true)
            {
                await vm.DoAdd();
                vm.Wtm.DoLog("lalala");
                Console.WriteLine($"添加成功");
                return vm.Entity.ID;
            }
            else
            {
                Console.WriteLine($"验证错误:{vm.MSD.GetFirstError()}");
                return 0;
            }

        }

        static void BatchEditSchool(List<int> ids)
        {
            SchoolBatchVM vm = GetWtmContext().CreateVM<SchoolBatchVM>();
            vm.Ids = ids.Select(x=>x.ToString()).ToArray();
            vm.LinkedVM = new School_BatchEdit();
            vm.LinkedVM.SchoolType = SchoolTypeEnum.PUB;
            vm.Validate();
            if (vm.MSD.IsValid == true)
            {
                vm.DoBatchEdit();
                Console.WriteLine($"修改成功");
            }
            else
            {
                Console.WriteLine($"验证错误:{vm.MSD.GetFirstError()}");
            }
        }

        static void Upload()
        {
            WtmFileProvider fp = GetFileProvider();
            var fs = File.OpenRead("C:\\Users\\Michael\\Pictures\\QQ截图20201104025651.png");
            var file = fp.Upload("vue1.png", fs.Length, fs);
            fs.Close();
            Console.WriteLine("finish");
        }
    }
}
