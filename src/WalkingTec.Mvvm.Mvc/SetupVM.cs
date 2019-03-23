using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WalkingTec.Mvvm.Core;
using System.Linq;
using System.Threading;

namespace WalkingTec.Mvvm.Mvc
{
    public enum UIEnum { LayUI, React }
    public enum ProjectTypeEnum { Single, Multi }

    public class SetupVM : BaseVM
    {
        private string version = "2.2.24";

        public bool EnableLog { get; set; }

        public bool LogExceptionOnly { get; set; }

        public DBTypeEnum? DbType { get; set; }

        public string CS { get; set; }

        public string CookiePre { get; set; }

        public SaveFileModeEnum? FileMode { get; set; }

        public string UploadDir { get; set; }

        public int? Rpp { get; set; }

        public UIEnum? UI { get; set; }

        public PageModeEnum? PageMode { get; set; }

        public ProjectTypeEnum? ProjectType { get; set; }

        [ValidateNever()]
        public string EntryDir { get; set; }

        public string ExtraDir { get; set; }
        public string ExtraNS { get; set; }
        public bool IsNew { get; set; }
        public string _mainDir;
        [ValidateNever()]
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    int index = EntryDir?.IndexOf("\\bin\\Debug\\") ?? 0;

                    _mainDir = EntryDir?.Substring(0, index);
                }
                return _mainDir;
            }
            set
            {
                _mainDir = value;
            }
        }

        private string _mainNs;
        [ValidateNever()]
        public string MainNs
        {
            get
            {
                if (_mainNs == null)
                {
                    int index = MainDir.LastIndexOf("\\");
                    if (index > 0)
                    {
                        _mainNs = MainDir.Substring(index + 1);
                    }
                    else
                    {
                        _mainNs = MainDir;
                    }
                }
                return _mainNs;
            }
            set
            {
                _mainNs = value;
            }
        }


        protected override void InitVM()
        {
            EnableLog = true;
            LogExceptionOnly = true;
            CS = "";
            UI = UIEnum.LayUI;
            DbType = DBTypeEnum.MySql;
            CookiePre = "WTM";
            FileMode = SaveFileModeEnum.Database;
            UploadDir = "";
            Rpp = 20;
        }

        public void DoSetup()
        {
            string vmdir = MainDir;
            string datadir = MainDir;
            string modeldir = MainDir;
            string vmns = MainNs + ".ViewModels";
            string datans = MainNs;
            string modelns = MainNs;
            if (ProjectType == ProjectTypeEnum.Single)
            {
                Directory.CreateDirectory($"{MainDir}\\Models");
                File.WriteAllText($"{MainDir}\\Models\\ReadMe.txt", "Put your models here");
                Directory.CreateDirectory($"{MainDir}\\ViewModels\\HomeVMs");
                vmdir = MainDir + "\\ViewModels";
            }
            else
            {
                Directory.CreateDirectory($"{MainDir}.ViewModel\\HomeVMs");
                Directory.CreateDirectory($"{MainDir}.Model");
                Directory.CreateDirectory($"{MainDir}.DataAccess");
                vmdir = MainDir + ".ViewModel";
                datadir = MainDir + ".DataAccess";
                modeldir = MainDir + ".Model";
                vmns = MainNs + ".ViewModel";
                datans = MainNs + ".DataAccess";
                modelns = MainNs + ".Model";
                File.WriteAllText($"{modeldir}\\{modelns}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
                File.WriteAllText($"{vmdir}\\{vmns}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
                File.WriteAllText($"{datadir}\\{datans}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
            }
            Directory.CreateDirectory($"{MainDir}\\Areas");
            Directory.CreateDirectory($"{MainDir}\\Controllers");
            if (UI == UIEnum.LayUI)
            {
                Directory.CreateDirectory($"{MainDir}\\Views\\Home");
                Directory.CreateDirectory($"{MainDir}\\Views\\Login");
                Directory.CreateDirectory($"{MainDir}\\Views\\Shared");
            }
            Directory.CreateDirectory($"{MainDir}\\wwwroot");

            var proj = File.ReadAllText($"{MainDir}\\{MainNs}.csproj");
            if (UI == UIEnum.LayUI)
            {
                proj = proj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.TagHelpers.LayUI"" Version=""{version}"" />
    <PackageReference Include=""WalkingTec.Mvvm.Mvc.Admin"" Version=""{version}"" />
 </ItemGroup>
</Project>
");
                proj = proj.Replace("</PropertyGroup>", $@"
    <CopyRefAssembliesToPublishDirectory>true</CopyRefAssembliesToPublishDirectory>
</PropertyGroup>
");

            }
            if (UI == UIEnum.React)
            {
                proj = proj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.TagHelpers.LayUI"" Version=""{version}"" />
    <PackageReference Include=""WalkingTec.Mvvm.Mvc.Admin"" Version=""{version}"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""4.0.1"" />
 </ItemGroup>
  <ItemGroup>
    <Content Remove=""$(SpaRoot)**"" />
    <None Include=""$(SpaRoot)**"" Exclude=""$(SpaRoot)node_modules\**;$(SpaRoot).awcache\**;$(SpaRoot).cache-loader\**"" />
  </ItemGroup>
  <Target Name=""DebugEnsureNodeEnv"" BeforeTargets=""Build"" Condition="" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)node_modules') "">
    <Exec Command=""node --version"" ContinueOnError=""true"">
      <Output TaskParameter=""ExitCode"" PropertyName=""ErrorCode"" />
    </Exec>
    <Error Condition=""'$(ErrorCode)' != '0'"" Text=""Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE."" />
    <Message Importance=""high"" Text=""Restoring dependencies using 'npm'. This may take several minutes..."" />
    <Exec WorkingDirectory=""$(SpaRoot)"" Command=""npm install"" />
  </Target>
  <Target Name=""PublishRunWebpack"" AfterTargets=""ComputeFilesToPublish"">
    <Exec WorkingDirectory=""$(SpaRoot)"" Command=""npm install"" />
    <Exec WorkingDirectory=""$(SpaRoot)"" Command=""npm run build"" />
    <ItemGroup>
      <DistFiles Include=""$(SpaRoot)build\**"" />
      <ResolvedFileToPublish Include=""@(DistFiles->'%(FullPath)')"" Exclude=""@(ResolvedFileToPublish)"">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>
</Project>
");
                proj = proj.Replace("</PropertyGroup>", $@"
    <TypeScriptCompileBlocked>true</TypeScriptCompileBlocked>
    <TypeScriptToolsVersion>3.2</TypeScriptToolsVersion>
    <IsPackable>false</IsPackable>
    <SpaRoot>ClientApp\</SpaRoot>
    <DefaultItemExcludes>$(DefaultItemExcludes);$(SpaRoot)node_modules\**</DefaultItemExcludes>
    <CopyRefAssembliesToPublishDirectory>true</CopyRefAssembliesToPublishDirectory>
</PropertyGroup>
");
            }

            if (ProjectType == ProjectTypeEnum.Multi)
            {
                proj = proj.Replace("</Project>", $@"
  <ItemGroup>
   <ProjectReference Include=""..\{modelns}\{modelns}.csproj"" />
    <ProjectReference Include=""..\{datans}\{datans}.csproj"" />
    <ProjectReference Include=""..\{vmns}\{vmns}.csproj"" />
 </ItemGroup >
</Project>
");
            }

            File.WriteAllText($"{MainDir}\\{MainNs}.csproj", proj, Encoding.UTF8);
            if (ProjectType == ProjectTypeEnum.Multi)
            {

                //修改modelproject
                var modelproj = File.ReadAllText($"{modeldir}\\{modelns}.csproj");
                if (modelproj.IndexOf("WalkingTec.Mvvm.Core") < 0)
                {
                    modelproj = modelproj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.Core"" Version=""{version}"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{modeldir}\\{modelns}.csproj", modelproj, Encoding.UTF8);
                }
                //修改dataproject
                var dataproj = File.ReadAllText($"{datadir}\\{datans}.csproj");
                if (dataproj.IndexOf($"{modelns}.csproj") < 0)
                {
                    dataproj = dataproj.Replace("</Project>", $@"
  <ItemGroup>
    <ProjectReference Include=""..\{modelns}\{modelns}.csproj"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{datadir}\\{datans}.csproj", dataproj, Encoding.UTF8);
                }
                //修改viewmodelproject
                var vmproj = File.ReadAllText($"{vmdir}\\{vmns}.csproj");
                if (vmproj.IndexOf($"{modelns}.csproj") < 0)
                {
                    vmproj = vmproj.Replace("</Project>", $@"
  <ItemGroup>
    <ProjectReference Include=""..\{modelns}\{modelns}.csproj"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{vmdir}\\{vmns}.csproj", vmproj, Encoding.UTF8);
                }
                var solution = File.ReadAllText($"{Directory.GetParent(MainDir)}\\{MainNs}.sln");
                if (solution.IndexOf($"{modelns}.csproj") < 0)
                {
                    Guid g1 = Guid.NewGuid();
                    Guid g2 = Guid.NewGuid();
                    Guid g3 = Guid.NewGuid();
                    solution = solution.Replace("EndProject", $@"EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{modelns}"", ""{modelns}\{modelns}.csproj"", ""{{{g1}}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{datans}"", ""{datans}\{datans}.csproj"", ""{{{g2}}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{vmns}"", ""{vmns}\{vmns}.csproj"", ""{{{g3}}}""
EndProject
");
                    solution = solution.Replace(".Release|Any CPU.Build.0 = Release|Any CPU", $@".Release|Any CPU.Build.0 = Release|Any CPU
		{{{g1}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{g1}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{g1}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{g1}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{g2}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{g2}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{g2}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{g2}}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{{g3}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{{g3}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{{g3}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{{g3}}}.Release|Any CPU.Build.0 = Release|Any CPU");
                    File.WriteAllText($"{Directory.GetParent(MainDir)}\\{MainNs}.sln", solution, Encoding.UTF8);
                }
            }


            File.WriteAllText($"{MainDir}\\appsettings.json", GetResource("Appsettings.txt")
    .Replace("$cs$", CS ?? "")
    .Replace("$dbtype$", DbType.ToString())
    .Replace("$pagemode$", PageMode.ToString())
    .Replace("$cookiepre$", CookiePre ?? "")
    .Replace("$enablelog$", EnableLog.ToString().ToLower())
    .Replace("$logexception$", LogExceptionOnly.ToString().ToLower())
    .Replace("$rpp$", Rpp == null ? "" : Rpp.ToString())
    .Replace("$filemode$", FileMode.ToString())
    .Replace("$uploaddir$", UploadDir ?? ""), Encoding.UTF8
    );
            File.WriteAllText($"{datadir}\\DataContext.cs", GetResource("DataContext.txt").Replace("$ns$", datans), Encoding.UTF8);
            if (UI == UIEnum.LayUI)
            {
                File.WriteAllText($"{MainDir}\\Controllers\\HomeController.cs", GetResource("HomeController.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Controllers\\LoginController.cs", GetResource("LoginController.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\_ViewStart.cshtml", GetResource("ViewStart.txt", "Mvc"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Home\\Index.cshtml", GetResource("home.Index.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Login\\ChangePassword.cshtml", GetResource("home.ChangePassword.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Home\\Header.cshtml", GetResource("home.Header.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Login\\Login.cshtml", GetResource("home.Login.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Home\\Menu.cshtml", GetResource("home.Menu.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Home\\PIndex.cshtml", GetResource("home.PIndex.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Home\\FrontPage.cshtml", GetResource("home.FrontPage.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}\\HomeVMs\\ChangePasswordVM.cs", GetResource("vms.ChangePasswordVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}\\HomeVMs\\IndexVM.cs", GetResource("vms.IndexVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}\\HomeVMs\\LoginVM.cs", GetResource("vms.LoginVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Shared\\_Layout.cshtml", GetResource("layui.Layout.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\Shared\\_PLayout.cshtml", GetResource("layui.PLayout.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Program.cs", GetResource("layui.Program.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Views\\_ViewImports.cshtml", GetResource("layui.ViewImports.txt", "Mvc"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\Areas\\_ViewImports.cshtml", GetResource("layui.ViewImports.txt", "Mvc"), Encoding.UTF8);
                UnZip("WalkingTec.Mvvm.Mvc.SetupFiles.Mvc.layui.layui.zip", $"{MainDir}\\wwwroot");
            }
            if (UI == UIEnum.React)
            {
                Directory.CreateDirectory($"{MainDir}\\ClientApp");
                UnZip("WalkingTec.Mvvm.Mvc.SetupFiles.Mvc.layui.layui.zip", $"{MainDir}\\wwwroot");
                UnZip("WalkingTec.Mvvm.Mvc.SetupFiles.Spa.React.ClientApp.zip", $"{MainDir}\\ClientApp");
                File.WriteAllText($"{MainDir}\\Program.cs", GetResource("Program.txt", "Spa").Replace("$ns$", MainNs), Encoding.UTF8);
                var config = File.ReadAllText($"{MainDir}\\ClientApp\\src\\global.config.tsx");
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\global.config.tsx", config.Replace("title: \"WalkingTec MVVM\",", $"title: \"{MainNs}\","), Encoding.UTF8);

            }

            if (File.Exists($"{MainDir}\\Startup.cs"))
            {
                File.Delete($"{MainDir}\\Startup.cs");
            }
        }

        public string GetIndex()
        {
            var rv = GetResource("SetupIndex.txt");
            string dbname = "";
            if (MainNs.Contains("."))
            {
                dbname = MainNs.Split('.').Last() + "_db";
            }
            else
            {
                dbname = MainNs + "_db";
            }
            rv = rv.Replace("{vm.CookiePre}", CookiePre).Replace("{vm.Rpp}", Rpp?.ToString()).Replace("$dbname$", dbname);
            return rv;
        }

        public string GetIndex1()
        {
            var rv = GetResource("SetupIndex1.txt");
            return rv;
        }

        private string GetResource(string fileName, string subdir = "")
        {
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();
            string loc = "";
            if (string.IsNullOrEmpty(subdir))
            {
                loc = $"WalkingTec.Mvvm.Mvc.SetupFiles.{fileName}";
            }
            else
            {
                loc = $"WalkingTec.Mvvm.Mvc.SetupFiles.{subdir}.{fileName}";
            }
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }

        private void UnZip(string fileName, string basedir)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var sr = assembly.GetManifestResourceStream(fileName);
            System.IO.Compression.ZipArchive zip = new System.IO.Compression.ZipArchive(sr);
            foreach (var entry in zip.Entries)
            {
                int index = entry.FullName.LastIndexOf("/");
                if (index >= 0)
                {
                    string dir = $"{basedir}\\{entry.FullName.Substring(0, index)}";
                    if (Directory.Exists(dir) == false)
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                if (entry.FullName.EndsWith("/") == false)
                {
                    var f = File.OpenWrite($"{basedir}\\{entry.FullName}");
                    var z = entry.Open();
                    z.CopyTo(f);
                    f.Flush();
                    f.Dispose();
                    z.Dispose();
                }
            }
            sr.Dispose();

        }


        public void WriteDefaultFiles()
        {
            File.WriteAllText($"{MainDir}\\{MainNs}.csproj", GetResource("DefaultProj.txt"), Encoding.UTF8);
            File.WriteAllText($"{ExtraDir}\\{MainNs}.sln", GetResource("DefaultSolution.txt").Replace("$ns$", MainNs).Replace("$guid$", Guid.NewGuid().ToString()), Encoding.UTF8);

        }
    }
}
