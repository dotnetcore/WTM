using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WalkingTec.Mvvm.Core.Extensions
{
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


        public static IConfigurationBuilder WTM_SetCurrentDictionary(this IConfigurationBuilder cb)
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();

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
