using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
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
            StartUp();
            Console.WriteLine(DateTime.Now.Subtract(check).TotalSeconds);
            //var context = GetWtmContext();
            //var test = context.CallAPI("baidu","/").Result;
            AddSchool();
            Upload();
            Console.ReadLine();
        }

        static void StartUp()
        {
            var services = new ServiceCollection();
            services.AddWtmContextForConsole();
            Provider = services.BuildServiceProvider();
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

        static void AddSchool()
        {
            SchoolVM vm = GetWtmContext().CreateVM<SchoolVM>();

            //WtmFileProvider fp = new WtmFileProvider(vm.Wtm.ConfigInfo);
            //var fh = fp.CreateFileHandler();
            //var fs = File.OpenRead("../../../");
            //var file = fh.Upload("vue1.png", fs.Length, fs);

            vm.Entity = new Demo.Models.School
            {
                SchoolCode = "111",
                SchoolName = "222",
                SchoolType = Demo.Models.SchoolTypeEnum.PRI,
                Remark = "abc",
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
                vm.DoAdd();
                vm.Wtm.DoLog("lalala");
                Console.WriteLine($"添加成功");
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
