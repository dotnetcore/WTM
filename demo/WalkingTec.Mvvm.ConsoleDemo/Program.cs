using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Demo;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs;

namespace WalkingTec.Mvvm.ConsoleDemo
{
    class Program
    {
        public static ServiceProvider Provider { get; set; }

        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            IConfigurationRoot ConfigRoot = configBuilder.WTMConfig(null).Build();
            var services = new ServiceCollection();
            services.Configure<Configs>(ConfigRoot);
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(ConfigRoot.GetSection("Logging")).AddConsole()
                       .AddDebug()
                       .AddWTMLogger();
            });
            services.AddScoped<WTMContext>();
            Provider = services.BuildServiceProvider();

            AddSchool();

            var logger = Provider.GetRequiredService<ILogger<ActionLog>>();
            logger.LogInformation("School Added");
        }

        static T CreateVM<T>() where T : IBaseVM, new()
        {
            T rv = new T();
            rv.WtmContext = Provider.GetRequiredService<WTMContext>();
            return rv;
        }

        static void AddSchool()
        {
            SchoolVM vm = CreateVM<SchoolVM>();

            WtmFileProvider fp = new WtmFileProvider(vm.WtmContext.ConfigInfo);
            var fh = fp.CreateFileHandler();
            var fs = File.OpenRead("C:\\Users\\michael\\Pictures\\vue1.png");
            var file = fh.Upload("vue1.png", fs.Length, fs);

            vm.Entity = new Demo.Models.School
            {
                SchoolCode = "111",
                SchoolName = "222",
                SchoolType = Demo.Models.SchoolTypeEnum.PRI,
                Remark = "abc",
                Photos = new System.Collections.Generic.List<Demo.Models.SchoolPhoto>
                {
                    new Demo.Models.SchoolPhoto
                    {
                         FileId = Guid.Parse(file.GetID()),
                         SchoolId = 0
                    }
                },
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
                Console.WriteLine($"添加成功");
            }
            else
            {
                Console.WriteLine($"验证错误:{vm.MSD.GetFirstError()}");
            }

        }
    }
}
