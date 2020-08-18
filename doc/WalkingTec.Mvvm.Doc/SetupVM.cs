using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WalkingTec.Mvvm.Core;
using System.Linq;
using System.Threading;

namespace WalkingTec.Mvvm.Doc
{
    public enum UIEnum { LayUI, React, Vue }
    public enum ProjectTypeEnum { Single, Multi }

    public enum DotnetVersionEnum { dotnet2_2, dotnet3_0}

    public class SetupVM : BaseVM
    {
        private string version = "";
        private string SwashbuckleVersion = "";
        private string EFDesignVersion = "";
        private string RazorPackage = "";
        public bool EnableLog { get; set; }

        public bool LogExceptionOnly { get; set; }

        public DBTypeEnum? DbType { get; set; }

        public string CS { get; set; }

        public string CookiePre { get; set; }

        public SaveFileModeEnum? FileMode { get; set; }

        public string UploadDir { get; set; }

        public int? Rpp { get; set; }

        public UIEnum? UI { get; set; }

        public DotnetVersionEnum? DotnetVersion { get; set; }

        public PageModeEnum? PageMode { get; set; }

        public ProjectTypeEnum? ProjectType { get; set; }

        [ValidateNever()]
        public string EntryDir { get; set; }

        public string ExtraDir { get; set; }
        public string ExtraNS { get; set; }
        public bool IsNew { get; set; }

        public int _port;
        public int Port
        {
            get
            {
                if (_port == 0)
                {
                    Random r = new Random();
                    _port = r.Next(5000, 9999);
                }
                return _port;
            }
        }

        public string _mainDir;
        [ValidateNever()]
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    int index = EntryDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}") ?? 0;

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
                    int index = MainDir.LastIndexOf(Path.DirectorySeparatorChar);
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
            DbType = DBTypeEnum.SqlServer;
            CookiePre = "WTM";
            FileMode = SaveFileModeEnum.Database;
            UploadDir = "";
            Rpp = 20;
        }

        public SetupVM()
        {
        }

        public void DoSetup()
        {
            switch (DotnetVersion)
            {
                case DotnetVersionEnum.dotnet2_2:
                    SwashbuckleVersion = "4.0.1";
                    EFDesignVersion = "2.2.4";
                    version = Utils.GetNugetVersion("2.",false);
                    RazorPackage = "";
                    break;
                case DotnetVersionEnum.dotnet3_0:
                    SwashbuckleVersion = "5.0.0-rc4";
                    EFDesignVersion = "3.1.5";
                    version = Utils.GetNugetVersion("3.",true);
                    RazorPackage = "    <PackageReference Include=\"Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation\" Version=\"3.1.5\" />";
                    break;
                default:
                    break;
            }

            string vmdir = MainDir;
            string datadir = MainDir;
            string modeldir = MainDir;
            string resourcedir = MainDir + $"{Path.DirectorySeparatorChar}Resources";
            string testdir = MainDir + ".Test";
            string vmns = MainNs + ".ViewModels";
            string datans = MainNs;
            string modelns = MainNs;
            string testns = MainNs + ".Test";
            Directory.CreateDirectory($"{MainDir}.Test");
            Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Resources");
            if (ProjectType == ProjectTypeEnum.Single)
            {
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Models");
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Models{Path.DirectorySeparatorChar}ReadMe.txt", "Put your models here");
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ViewModels");
                if (UI == UIEnum.LayUI)
                {
                    Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}HomeVMs");
                }
                vmdir = MainDir + $"{Path.DirectorySeparatorChar}ViewModels";
            }
            else
            {
                Directory.CreateDirectory($"{MainDir}.ViewModel");
                if (UI == UIEnum.LayUI)
                {
                    Directory.CreateDirectory($"{MainDir}.ViewModel{Path.DirectorySeparatorChar}HomeVMs");
                }
                Directory.CreateDirectory($"{MainDir}.Model");
                Directory.CreateDirectory($"{MainDir}.DataAccess");
                vmdir = MainDir + ".ViewModel";
                datadir = MainDir + ".DataAccess";
                modeldir = MainDir + ".Model";
                vmns = MainNs + ".ViewModel";
                datans = MainNs + ".DataAccess";
                modelns = MainNs + ".Model";
                File.WriteAllText($"{modeldir}{Path.DirectorySeparatorChar}{modelns}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
                File.WriteAllText($"{vmdir}{Path.DirectorySeparatorChar}{vmns}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
                File.WriteAllText($"{datadir}{Path.DirectorySeparatorChar}{datans}.csproj", GetResource("Proj.txt"), Encoding.UTF8);
            }
            File.WriteAllText($"{testdir}{Path.DirectorySeparatorChar}{testns}.csproj", GetResource("TestProj.txt").Replace("$ns$", MainNs), Encoding.UTF8);
            File.WriteAllText($"{testdir}{Path.DirectorySeparatorChar}MockController.cs", GetResource("MockController.txt", "test").Replace("$ns$", testns).Replace("$datans$", datans), Encoding.UTF8);
            File.WriteAllText($"{testdir}{Path.DirectorySeparatorChar}MockHttpSession.cs", GetResource("MockHttpSession.txt", "test").Replace("$ns$", testns), Encoding.UTF8);
            Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Areas");
            Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Controllers");
            if (UI == UIEnum.LayUI)
            {
                File.WriteAllText($"{testdir}{Path.DirectorySeparatorChar}HomeControllerTest.cs", GetResource("HomeControllerTest.txt", "test").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{testdir}{Path.DirectorySeparatorChar}LoginControllerTest.cs", GetResource("LoginControllerTest.txt", "test").Replace("$ns$", MainNs).Replace("$vmns$", vmns).Replace("$datans$", datans), Encoding.UTF8);
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home");
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Login");
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Shared");
            }
            Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}wwwroot");

            var proj = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}{MainNs}.csproj");
            if (UI == UIEnum.LayUI)
            {
                proj = proj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.TagHelpers.LayUI"" Version=""{version}"" />
    <PackageReference Include=""WalkingTec.Mvvm.Mvc.Admin"" Version=""{version}"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""{SwashbuckleVersion}"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Tools"" Version=""{EFDesignVersion}"" />
    {RazorPackage}
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
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""{SwashbuckleVersion}"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Tools"" Version=""{EFDesignVersion}"" />
    {RazorPackage}
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

            if (UI == UIEnum.Vue)
            {
                proj = proj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.TagHelpers.LayUI"" Version=""{version}"" />
    <PackageReference Include=""WalkingTec.Mvvm.Mvc.Admin"" Version=""{version}"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""{SwashbuckleVersion}"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Tools"" Version=""{EFDesignVersion}"" />
    {RazorPackage}
  </ItemGroup>
  <ItemGroup>
    <Compile Remove=""ClientApp\dist\**"" />
    <Content Remove=""$(SpaRoot)**"" />
    <Content Remove=""ClientApp\dist\**"" />
    <None Include=""$(SpaRoot)**"" Exclude=""$(SpaRoot)node_modules\**;$(SpaRoot)dist\**;$(SpaRoot)dist\**;$(SpaRoot).awcache\**;$(SpaRoot).cache-loader\**"" />
    <EmbeddedResource Remove=""ClientApp\dist\**"" />
    <None Remove=""ClientApp\dist\**"" />
    <None Remove=""ClientApp\package-lock.json"" />
  </ItemGroup>
  <Target Name=""DebugEnsureNodeEnv"" BeforeTargets=""Build"" Condition="" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)node_modules') "">
    <Exec Command=""node --version"" ContinueOnError=""true"">
      <Output TaskParameter=""ExitCode"" PropertyName=""ErrorCode"" />
    </Exec>
    <Error Condition=""'$(ErrorCode)' != '0'"" Text=""Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE."" />
    <Message Importance=""high"" Text=""Restoring dependencies using 'npm'. This may take several minutes..."" />
    <Exec WorkingDirectory=""$(SpaRoot)"" Command=""npm install"" />
  </Target>

  <Target Name=""DebugRunWebpack"" BeforeTargets=""Build"" Condition="" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)dist') "">
    <!-- Ensure Node.js is installed -->
    <Exec Command=""node --version"" ContinueOnError=""true"">
      <Output TaskParameter=""ExitCode"" PropertyName=""ErrorCode"" />
    </Exec>
    <Error Condition=""'$(ErrorCode)' != '0'"" Text=""Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE."" />

    <!-- In development, the dist files won't exist on the first run or when cloning to
         a different machine, so rebuild them if not already present. -->
    <Message Importance=""high"" Text=""Performing first-run Webpack build..."" />
    <Exec WorkingDirectory=""$(SpaRoot)"" Command=""node node_modules/webpack/bin/webpack.js --config config/webpack.dev.js"" />
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

            File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}{MainNs}.csproj", proj, Encoding.UTF8);
            if (ProjectType == ProjectTypeEnum.Multi)
            {

                //修改modelproject
                var modelproj = File.ReadAllText($"{modeldir}{Path.DirectorySeparatorChar}{modelns}.csproj");
                if (modelproj.IndexOf("WalkingTec.Mvvm.Core") < 0)
                {
                    modelproj = modelproj.Replace("</Project>", $@"
  <ItemGroup>
    <PackageReference Include=""WalkingTec.Mvvm.Core"" Version=""{version}"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{modeldir}{Path.DirectorySeparatorChar}{modelns}.csproj", modelproj, Encoding.UTF8);
                }
                //修改dataproject
                var dataproj = File.ReadAllText($"{datadir}{Path.DirectorySeparatorChar}{datans}.csproj");
                if (dataproj.IndexOf($"{modelns}.csproj") < 0)
                {
                    dataproj = dataproj.Replace("</Project>", $@"
  <ItemGroup>
    <ProjectReference Include=""..\{modelns}\{modelns}.csproj"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{datadir}{Path.DirectorySeparatorChar}{datans}.csproj", dataproj, Encoding.UTF8);
                }
                //修改viewmodelproject
                var vmproj = File.ReadAllText($"{vmdir}{Path.DirectorySeparatorChar}{vmns}.csproj");
                if (vmproj.IndexOf($"{modelns}.csproj") < 0)
                {
                    vmproj = vmproj.Replace("</Project>", $@"
  <ItemGroup>
    <ProjectReference Include=""..\{modelns}\{modelns}.csproj"" />
  </ItemGroup >
</Project>
");
                    File.WriteAllText($"{vmdir}{Path.DirectorySeparatorChar}{vmns}.csproj", vmproj, Encoding.UTF8);
                }
                var solution = File.ReadAllText($"{Directory.GetParent(MainDir)}{Path.DirectorySeparatorChar}{MainNs}.sln");
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
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{testns}"", ""{testns}\{testns}.csproj"", ""{{{Guid.NewGuid()}}}""
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
                    File.WriteAllText($"{Directory.GetParent(MainDir)}{Path.DirectorySeparatorChar}{MainNs}.sln", solution, Encoding.UTF8);
                }
            }

            else
            {
                var s1 = File.ReadAllText($"{Directory.GetParent(MainDir)}{Path.DirectorySeparatorChar}{MainNs}.sln");
                s1 = s1.Replace("EndProject", $@"EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{testns}"", ""{testns}\{testns}.csproj"", ""{{{Guid.NewGuid()}}}""
EndProject
");
                File.WriteAllText($"{Directory.GetParent(MainDir)}{Path.DirectorySeparatorChar}{MainNs}.sln", s1, Encoding.UTF8);
            }

            File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}appsettings.json", GetResource("Appsettings.txt")
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
            File.WriteAllText($"{datadir}{Path.DirectorySeparatorChar}DataContext.cs", GetResource("DataContext.txt").Replace("$ns$", datans), Encoding.UTF8);

            File.WriteAllText($"{resourcedir}{Path.DirectorySeparatorChar}Program.zh.resx", GetResource("Resourcezh.txt"), Encoding.UTF8);
            File.WriteAllText($"{resourcedir}{Path.DirectorySeparatorChar}Program.en.resx", GetResource("Resourceen.txt"), Encoding.UTF8);
            if (UI == UIEnum.LayUI)
            {
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Controllers{Path.DirectorySeparatorChar}HomeController.cs", GetResource("HomeController.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Controllers{Path.DirectorySeparatorChar}LoginController.cs", GetResource("LoginController.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}_ViewStart.cshtml", GetResource("ViewStart.txt", "Mvc"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home{Path.DirectorySeparatorChar}Index.cshtml", GetResource("home.Index.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Login{Path.DirectorySeparatorChar}ChangePassword.cshtml", GetResource("home.ChangePassword.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home{Path.DirectorySeparatorChar}Layout.cshtml", GetResource("home.layout.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Login{Path.DirectorySeparatorChar}Login.cshtml", GetResource("home.Login.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Login{Path.DirectorySeparatorChar}Reg.cshtml", GetResource("home.Reg.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home{Path.DirectorySeparatorChar}PIndex.cshtml", GetResource("home.PIndex.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home{Path.DirectorySeparatorChar}FrontPage.cshtml", GetResource("home.FrontPage.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Home{Path.DirectorySeparatorChar}FrontPage.en.cshtml", GetResource("home.FrontPage2.txt", "Mvc").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}{Path.DirectorySeparatorChar}HomeVMs{Path.DirectorySeparatorChar}ChangePasswordVM.cs", GetResource("vms.ChangePasswordVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}{Path.DirectorySeparatorChar}HomeVMs{Path.DirectorySeparatorChar}LoginVM.cs", GetResource("vms.LoginVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{vmdir}{Path.DirectorySeparatorChar}HomeVMs{Path.DirectorySeparatorChar}RegVM.cs", GetResource("vms.RegVM.txt").Replace("$ns$", MainNs).Replace("$vmns$", vmns), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}Shared{Path.DirectorySeparatorChar}_Layout.cshtml", GetResource("layui.Layout.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Program.cs", GetResource("layui.Program.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}_ViewImports.cshtml", GetResource("layui.ViewImports.txt", "Mvc"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}_ViewImports.cshtml", GetResource("layui.ViewImports.txt", "Mvc"), Encoding.UTF8);
                File.WriteAllText($"{ExtraDir}{Path.DirectorySeparatorChar}alpine.Dockerfile", GetResource("alpine.Dockerfile.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                File.WriteAllText($"{ExtraDir}{Path.DirectorySeparatorChar}Dockerfile", GetResource("Dockerfile.txt", "Mvc").Replace("$ns$", MainNs), Encoding.UTF8);
                UnZip("WalkingTec.Mvvm.Doc.SetupFiles.Mvc.layui.layui.zip", $"{MainDir}{Path.DirectorySeparatorChar}wwwroot");
            }
            if (UI == UIEnum.React)
            {
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ClientApp");
                UnZip("WalkingTec.Mvvm.Doc.SetupFiles.Mvc.layui.layui.zip", $"{MainDir}{Path.DirectorySeparatorChar}wwwroot");
                UnZip("WalkingTec.Mvvm.Doc.SetupFiles.Spa.React.ClientApp.zip", $"{MainDir}{Path.DirectorySeparatorChar}ClientApp");
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Program.cs", GetResource("React.Program.txt", "Spa").Replace("$ns$", MainNs), Encoding.UTF8);
                var config = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}global.config.tsx");
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}global.config.tsx", config.Replace("title: \"WalkingTec MVVM\",", $"title: \"{MainNs}\","), Encoding.UTF8);
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}setupProxy.js", $@"
const proxy = require('http-proxy-middleware');

module.exports = (app) => {{
    app.use(proxy('/api', {{
        target: 'http://localhost:{Port}/',
        changeOrigin: true,
        logLevel: ""debug""
    }}));
}};
", Encoding.UTF8);

            }
            if (UI == UIEnum.Vue)
            {
                Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ClientApp");
                UnZip("WalkingTec.Mvvm.Doc.SetupFiles.Mvc.layui.layui.zip", $"{MainDir}{Path.DirectorySeparatorChar}wwwroot");
                UnZip("WalkingTec.Mvvm.Doc.SetupFiles.Spa.Vue.ClientApp.zip", $"{MainDir}{Path.DirectorySeparatorChar}ClientApp");
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Program.cs", GetResource("Vue.Program.txt", "Spa").Replace("$ns$", MainNs), Encoding.UTF8);
                var config = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}webpack.dev.js");
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}webpack.dev.js", config.Replace("target: \"http://localhost:7598/\",", $"target: \"http://localhost:{Port}/\","), Encoding.UTF8);
            }


            if (File.Exists($"{MainDir}{Path.DirectorySeparatorChar}Startup.cs"))
            {
                File.Delete($"{MainDir}{Path.DirectorySeparatorChar}Startup.cs");
            }
        }

        public string GetIndex(bool en=false)
        {
            string rv = "";
            if (en == false)
            {
                rv = GetResource("SetupIndex.txt");
            }
            else
            {
                rv = GetResource("SetupIndex_en.txt");
            }
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

        public string GetIndex1En()
        {
            var rv = GetResource("SetupIndex1_en.txt");
            return rv;
        }


        private string GetResource(string fileName, string subdir = "")
        {
            if(fileName == "Proj.txt" || fileName == "TestProj.txt" ||
                fileName == "React.Program.txt" || fileName == "Vue.Program.txt" || fileName == "layui.Program.txt"
                || fileName == "DefaultProj.txt" || fileName == "Launch.txt")
            {
                if(DotnetVersion == DotnetVersionEnum.dotnet3_0)
                {
                    var index = fileName.LastIndexOf('.');
                    fileName = fileName.Substring(0,index) + "3" + fileName.Substring(index);
                }
            }
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();
            string loc = "";
            if (string.IsNullOrEmpty(subdir))
            {
                loc = $"WalkingTec.Mvvm.Doc.SetupFiles.{fileName}";
            }
            else
            {
                loc = $"WalkingTec.Mvvm.Doc.SetupFiles.{subdir}.{fileName}";
            }
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();

            if (fileName.EndsWith("Dockerfile.txt"))
            {
                if (DotnetVersion == DotnetVersionEnum.dotnet3_0)
                {
                    content = content.Replace("2.2", "3.1");
                }
            }
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
                    string dir = $"{basedir}{Path.DirectorySeparatorChar}{entry.FullName.Substring(0, index)}";
                    if (Directory.Exists(dir) == false)
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                if (entry.FullName.EndsWith("/") == false)
                {
                    var f = File.OpenWrite($"{basedir}{Path.DirectorySeparatorChar}{entry.FullName}");
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
            File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}{MainNs}.csproj", GetResource("DefaultProj.txt"), Encoding.UTF8);
            File.WriteAllText($"{ExtraDir}{Path.DirectorySeparatorChar}{MainNs}.sln", GetResource("DefaultSolution.txt").Replace("$ns$", MainNs).Replace("$guid$", Guid.NewGuid().ToString()), Encoding.UTF8);
            if (UI == UIEnum.React || UI == UIEnum.Vue)
            {
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Properties{Path.DirectorySeparatorChar}launchSettings.json", GetResource("Launch.txt", "Spa").Replace("$ns$", MainNs).Replace("$port$", Port.ToString()), Encoding.UTF8);
            }
            else
            {
                File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}Properties{Path.DirectorySeparatorChar}launchSettings.json", GetResource("Launch.txt", "Mvc").Replace("$ns$", MainNs).Replace("$port$", Port.ToString()), Encoding.UTF8);
            }
        }
    }
}
