using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 配置文件相关的扩展函数，用于构建 WTM 框架的标准配置
    /// </summary>
    public static class ConfigExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configBuilder"></param>
        /// <param name="env"></param>
        /// <param name="jsonFileDir"></param>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        public static IConfigurationBuilder WTMConfig(this IConfigurationBuilder configBuilder, IHostEnvironment env, string jsonFileDir=null, string jsonFileName = null)
        {
            IConfigurationBuilder rv = configBuilder;
            if (string.IsNullOrEmpty(jsonFileDir))
            {
                rv = rv.WTM_SetCurrentDictionary();
            }
            else
            {
                rv = rv.SetBasePath(jsonFileDir);
            }
            if (string.IsNullOrEmpty(jsonFileName))
            {
                rv = rv.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            }
            else
            {
                rv = rv.AddJsonFile(jsonFileName, optional: true, reloadOnChange: true);
            }
            rv = rv.AddEnvironmentVariables();
            //将宿主根目录写入内存配置，供后续读取 HostRoot 配置项时使用
            if (env != null)
            {
                rv = rv.AddInMemoryCollection(new Dictionary<string, string> { { "HostRoot", env.ContentRootPath } });
            }
            else
            {
                rv = rv.AddInMemoryCollection(new Dictionary<string, string> { { "HostRoot", Directory.GetCurrentDirectory() } });
            }
            return rv;
        }


        /// <summary>
        /// 设置当前目录为配置文件所在目录，并以此为基准目录构建配置
        /// </summary>
        /// <param name="cb">配置构建器</param>
        /// <returns>设置基准目录后的配置构建器</returns>
        public static IConfigurationBuilder WTM_SetCurrentDictionary(this IConfigurationBuilder cb)
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();

            //如果当前目录下没有 appsettings.json，则尝试到程序集输出目录中寻找
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
            {
                var binLocation = Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrEmpty(binLocation))
                {
                    var binPath = new FileInfo(binLocation).Directory?.FullName;
                    if (File.Exists(Path.Combine(binPath, "appsettings.json")))
                    {
                        Directory.SetCurrentDirectory(binPath);
                        cb.SetBasePath(binPath);
                        //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        //.AddEnvironmentVariables();
                    }
                }
            }
            else
            {
                cb.SetBasePath(Directory.GetCurrentDirectory());
            }
            return cb;
        }

    }

    /// <summary>
    /// 解决IIS InProgress下CurrentDirectory获取错误的问题
    /// </summary>
    internal class CurrentDirectoryHelpers

    {

        internal const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]

        private static extern IntPtr GetModuleHandle(string lpModuleName);



        [System.Runtime.InteropServices.DllImport(AspNetCoreModuleDll)]

        private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);



        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]

        private struct IISConfigurationData

        {

            public IntPtr pNativeApplication;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzFullApplicationPath;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzVirtualApplicationPath;

            public bool fWindowsAuthEnabled;

            public bool fBasicAuthEnabled;

            public bool fAnonymousAuthEnable;

        }



        /// <summary>
        /// 修正 IIS InProcess 模式下当前目录指向错误的问题，将当前目录设置为站点的物理路径
        /// </summary>
        public static void SetCurrentDirectory()

        {

            try

            {

                // Check if physical path was provided by ANCM

                var sitePhysicalPath = Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH");

                if (string.IsNullOrEmpty(sitePhysicalPath))

                {

                    // Skip if not running ANCM InProcess

                    if (GetModuleHandle(AspNetCoreModuleDll) == IntPtr.Zero)

                    {

                        return;

                    }



                    IISConfigurationData configurationData = default(IISConfigurationData);

                    if (http_get_application_properties(ref configurationData) != 0)

                    {

                        return;

                    }



                    sitePhysicalPath = configurationData.pwzFullApplicationPath;

                }



                Environment.CurrentDirectory = sitePhysicalPath;

            }

            catch

            {

                // ignore

            }

        }

    }

}
