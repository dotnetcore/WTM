using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    public enum ApiAuthMode
    {
        [Display(Name = "Both Jwt and Cookie")]
        Both,
        [Display(Name = "Jwt")]
        Jwt,
        [Display(Name = "Cookie")]
        Cookie
    }

    [ReInit(ReInitModes.ALWAYS)]
    public class CodeGenVM : BaseVM
    {
        public CodeGenListVM FieldList { get; set; }

        public List<FieldInfo> FieldInfos { get; set; }

        public string PreviewFile { get; set; }

        public UIEnum UI { get; set; }

        [Display(Name = "GenApi")]
        public bool IsApi { get; set; }

        [Display(Name = "AuthMode")]
        public ApiAuthMode AuthMode { get; set; }

        public string ModelName
        {
            get
            {
                return SelectedModel?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            }
        }
        [Display(Name = "ModelNS")]
        [ValidateNever()]
        public string ModelNS => SelectedModel?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSpratedString(seperator: ".");
        [Display(Name = "ModuleName")]
        [Required(ErrorMessage = "{0}required")]
        public string ModuleName { get; set; }
        [RegularExpression("^[A-Za-z_]+", ErrorMessage = "EnglishOnly")]
        public string Area { get; set; }
        [ValidateNever()]
        [BindNever()]
        public List<ComboSelectListItem> AllModels { get; set; }
        [Required(ErrorMessage = "{0}required")]
        [Display(Name = "SelectedModel")]
        public string SelectedModel { get; set; }
        [ValidateNever()]
        public string EntryDir { get; set; }


        public string _mainDir;
        [ValidateNever()]
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    int? index = EntryDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}");
                    if (index == null || index < 0)
                    {
                        index = EntryDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}") ?? 0;
                    }

                    _mainDir = EntryDir?.Substring(0, index.Value);
                }
                return _mainDir;
            }
            set
            {
                _mainDir = value;
            }
        }

        public string _vmdir;
        [ValidateNever()]
        public string VmDir
        {
            get
            {
                if (_vmdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }

                    }
                    _vmdir = vmdir.FullName;
                }
                return _vmdir;
            }
        }

        public string _testdir;
        [ValidateNever()]
        public string TestDir
        {
            get
            {
                if (_testdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var testdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".test")).FirstOrDefault();
                    _testdir = testdir?.FullName;
                }
                return _testdir;
            }
        }


        public string _controllerdir;
        [ValidateNever()]
        public string ControllerDir
        {
            get
            {
                if (_controllerdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                    else
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                }
                return _controllerdir;
            }
        }

        public string _viewdir;
        [ValidateNever()]
        public string ViewDir
        {
            get
            {
                if (_viewdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}{ModelName}").FullName;
                    }
                    else
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}{ModelName}").FullName;
                    }
                }
                return _viewdir;
            }
        }


        private string _mainNs;
        public string MainNS
        {
            get
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
                return _mainNs;
            }
            set
            {
                _mainNs = value;
            }

        }

        private string _controllerNs;
        [Display(Name = "ControllerNs")]
        [ValidateNever()]
        public string ControllerNs
        {
            get
            {
                if (_controllerNs == null)
                {
                    _controllerNs = MainNS + ".Controllers";
                }
                return _controllerNs;
            }
            set
            {
                _controllerNs = value;
            }
        }

        private string _testNs;
        [Display(Name = "TestNs")]
        [ValidateNever()]
        public string TestNs
        {
            get
            {
                if (_testNs == null)
                {
                    _testNs = MainNS + ".Test";
                }
                return _testNs;
            }
            set
            {
                _testNs = value;
            }
        }

        private string _dataNs;
        [Display(Name = "DataNs")]
        [ValidateNever()]
        public string DataNs
        {
            get
            {
                if (_dataNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".dataaccess")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        _dataNs = MainNS;
                    }
                    else
                    {
                        _dataNs = MainNS + ".DataAccess"; ;
                    }
                }
                return _dataNs;
            }
            set
            {
                _dataNs = value;
            }
        }


        private string _vmNs;
        [Display(Name = "VMNs")]
        [ValidateNever()]
        public string VMNs
        {
            get
            {
                if (_vmNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs = MainNS + $".ViewModels.{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs = MainNS + $".{Area}.ViewModels.{ModelName}VMs";
                        }
                    }
                    else
                    {
                        int index = vmdir.FullName.LastIndexOf(Path.DirectorySeparatorChar);
                        if (index > 0)
                        {
                            _vmNs = vmdir.FullName.Substring(index + 1);
                        }
                        else
                        {
                            _vmNs = vmdir.FullName;
                        }
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs += $".{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs += $".{Area}.{ModelName}VMs";
                        }
                    }
                }
                return _vmNs;
            }
            set
            {
                _vmNs = value;
            }
        }

        protected override void InitVM()
        {
            if (string.IsNullOrEmpty(SelectedModel) == false)
            {
                foreach (var item in ConfigInfo.ConnectionStrings)
                {
                    var dc = item.CreateDC();
                    Type t = typeof(DbSet<>).MakeGenericType(Type.GetType(SelectedModel));
                    var exist = dc.GetType().GetProperties().Where(x => x.PropertyType == t).FirstOrDefault();
                    if (exist != null)
                    {
                        this.DC = dc;
                    }
                }

            }

            FieldList = new CodeGenListVM();
            FieldList.CopyContext(this);
        }
        public void DoGen()
        {
            File.WriteAllText($"{ControllerDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}Controller.cs", GenerateController(), Encoding.UTF8);

            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}VM.cs", GenerateVM("CrudVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}ImportVM.cs", GenerateVM("ImportVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);

            if (IsApi == false)
            {
                if (UI == UIEnum.LayUI)
                {
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Index.cshtml", GenerateView("ListView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Create.cshtml", GenerateView("CreateView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Edit.cshtml", GenerateView("EditView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Delete.cshtml", GenerateView("DeleteView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Details.cshtml", GenerateView("DetailsView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Import.cshtml", GenerateView("ImportView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}BatchEdit.cshtml", GenerateView("BatchEditView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}BatchDelete.cshtml", GenerateView("BatchDeleteView"), Encoding.UTF8);
                }
                if (UI == UIEnum.React || UI == UIEnum.VUE)
                {
                    if (Directory.Exists($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}") == false)
                    {
                        Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}");
                    }
                    if (Directory.Exists($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views") == false)
                    {
                        Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views");
                    }
                    if (Directory.Exists($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}store") == false)
                    {
                        Directory.CreateDirectory($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}store");
                    }
                    if (UI == UIEnum.React)
                    {
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}action.tsx", GenerateReactView("action"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}forms.tsx", GenerateReactView("forms"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}models.tsx", GenerateReactView("models"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}other.tsx", GenerateReactView("other"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}search.tsx", GenerateReactView("search"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}table.tsx", GenerateReactView("table"), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}store{Path.DirectorySeparatorChar}index.ts", GetResource("index.txt", "Spa.React.store").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}index.tsx", GetResource("index.txt", "Spa.React").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}style.less", GetResource("style.txt", "Spa.React").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                    }
                    if (UI == UIEnum.VUE)
                    {
                        List<string> apipneeded = new List<string>();
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}index.vue", GenerateVUEView("index", apipneeded), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}config.ts", GenerateVUEView("config", apipneeded), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}dialog-form.vue", GenerateVUEView("views.dialog-form", apipneeded), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}store{Path.DirectorySeparatorChar}index.ts", GetResource("index.txt", "Spa.Vue.store").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}store{Path.DirectorySeparatorChar}api.ts", GenerateVUEView("store.api", apipneeded), Encoding.UTF8);
                    }
                    var index = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}index.ts");
                    if (index.Contains($"path: '/{ModelName.ToLower()}'") == false)
                    {
                        if (UI == UIEnum.React)
                        {
                            index = index.Replace("/**WTM**/", $@"
, {ModelName.ToLower()}: {{
        name: '{ModuleName.ToLower()}',
        path: '/{ModelName.ToLower()}',
        controller: '{ControllerNs},{ModelName}',
        component: React.lazy(() => import('./{ModelName.ToLower()}'))
    }}
/**WTM**/
 ");
                        }
                        if (UI == UIEnum.VUE)
                        {
                            index = index.Replace("/**WTM**/", $@"
, {ModelName.ToLower()}: {{
    name: '{ModuleName.ToLower()}',
    path: '/{ModelName.ToLower()}',
    controller: '{ControllerNs},{ModelName}'
    }}
/**WTM**/
 ");

                        }
                        File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}index.ts", index, Encoding.UTF8);
                    }
                    string menu = "";
                    if (UI == UIEnum.React)
                    {
                        menu = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}public{Path.DirectorySeparatorChar}subMenu.json");
                        if (menu.Contains($@"""Url"": ""/{ModelName.ToLower()}""") == false)
                        {
                            var i = menu.LastIndexOf("}");
                            menu = menu.Insert(i + 1, $@"
,{{
    ""Id"": ""{Guid.NewGuid().ToString()}"",
    ""ParentId"": null,
    ""Text"": ""{ModuleName.ToLower()}"",
    ""Url"": ""/{ModelName.ToLower()}""
    }}
");
                            File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}public{Path.DirectorySeparatorChar}subMenu.json", menu, Encoding.UTF8);

                        }
                    }
                    if (UI == UIEnum.VUE)
                    {
                        menu = File.ReadAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}subMenu.json");
                        if (menu.Contains($@"""Url"": ""/{ModelName.ToLower()}""") == false)
                        {
                            var i = menu.LastIndexOf("}");
                            menu = menu.Insert(i + 1, $@"
,{{
    ""Id"": ""{Guid.NewGuid().ToString()}"",
    ""ParentId"": null,
    ""Text"": ""{ModuleName.ToLower()}"",
    ""Url"": ""/{ModelName.ToLower()}""
    }}
");
                            File.WriteAllText($"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}subMenu.json", menu, Encoding.UTF8);

                        }
                    }
                }
            }
            var test = GenerateTest();
            if (test != "")
            {
                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    File.WriteAllText($"{TestDir}{Path.DirectorySeparatorChar}{ModelName}ControllerTest.cs", test, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText($"{TestDir}{Path.DirectorySeparatorChar}{ModelName}ApiTest.cs", test, Encoding.UTF8);
                }
            }
        }

        public string GenerateController()
        {
            string dir = "";
            string jwt = "";
            if (UI == UIEnum.LayUI && IsApi == false)
            {
                dir = "Mvc";
            }
            else
            {
                dir = "Spa";
                switch (AuthMode)
                {
                    case ApiAuthMode.Both:
                        jwt = "[AuthorizeJwtWithCookie]";
                        break;
                    case ApiAuthMode.Jwt:
                        jwt = "[AuthorizeJwt]";
                        break;
                    case ApiAuthMode.Cookie:
                        jwt = "[AuthorizeCookie]";
                        break;
                    default:
                        break;
                }
            }
            var rv = GetResource("Controller.txt", dir).Replace("$jwt$", jwt).Replace("$vmnamespace$", VMNs).Replace("$namespace$", ControllerNs).Replace("$des$", ModuleName).Replace("$modelname$", ModelName).Replace("$modelnamespace$", ModelNS).Replace("$controllername$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
            if (string.IsNullOrEmpty(Area))
            {
                rv = rv.Replace("$area$", "");
            }
            else
            {
                rv = rv.Replace("$area$", $"[Area(\"{Area}\")]");
            }
            //生成api中获取下拉菜单数据的api
            //如果一个一对多关联其他类的字段是搜索条件或者表单字段，则生成对应的获取关联表数据的api
            if (UI != UIEnum.LayUI || IsApi == true)
            {
                StringBuilder other = new StringBuilder();
                List<FieldInfo> pros = FieldInfos.Where(x => x.IsSearcherField == true || x.IsFormField == true).ToList();
                List<string> existSubPro = new List<string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    if ((item.InfoType == FieldInfoType.One2Many || item.InfoType == FieldInfoType.Many2Many) && item.SubField != "`file")
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                        var key = subtype.FullName + ":" + subpro.Name;
                        existSubPro.Add(key);
                        int count = existSubPro.Where(x => x == key).Count();
                        if (count == 1)
                        {

                            other.AppendLine($@"
        [HttpGet(""Get{subtype.Name}s"")]
        public ActionResult Get{subtype.Name}s()
        {{
            return Ok(DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.{item.SubField}));
        }}");
                        }
                    }
                }
                rv = rv.Replace("$other$", other.ToString());
            }
            return rv;
        }

        public string GenerateVM(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$modelnamespace$", ModelNS).Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName).Replace("$area$", $"{Area ?? ""}").Replace("$classname$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
            if (name == "Searcher" || name == "BatchVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = null;
                if (name == "Searcher")
                {
                    pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                }
                if (name == "BatchVM")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                }
                foreach (var pro in pros)
                {
                    //对于一对一或者一对多的搜索和批量修改字段，需要在vm中生成对应的变量来获取关联表的数据
                    if (pro.InfoType != FieldInfoType.Normal)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        if (UI == UIEnum.LayUI && IsApi == false)
                        {
                            var fname = "All" + pro.FieldName + "s";
                            prostring += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                            initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.{pro.SubField});";
                        }
                    }

                    //生成普通字段定义
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    string typename = proType.PropertyType.Name;
                    string proname = pro.GetField(DC, modelType);

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType.PropertyType.IsNullable())
                            {
                                typename = proType.PropertyType.GetGenericArguments()[0].Name + "?";
                            }
                            else if (proType.PropertyType != typeof(string))
                            {
                                typename = proType.PropertyType.Name + "?";
                            }
                            break;
                        case FieldInfoType.One2Many:
                            typename = pro.GetFKType(DC, modelType);
                            if (typename != "string")
                            {
                                typename += "?";
                            }
                            break;
                        case FieldInfoType.Many2Many:
                            proname = $@"Selected{pro.FieldName}IDs";
                            typename = $"List<{pro.GetFKType(DC, modelType)}>";
                            break;
                        default:
                            break;
                    }
                    if (typename == "DateTime" || typename == "DateTime?")
                    {
                        typename = "DateRange";
                    }
                    prostring += $@"
        public {typename} {proname} {{ get; set; }}";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ListVM")
            {
                string headerstring = "";
                string selectstring = "";
                string wherestring = "";
                string subprostring = "";
                string formatstring = "";
                string actionstring = "";

                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    actionstring = $@"
        protected override List<GridAction> InitGridAction()
        {{
            return new List<GridAction>
            {{
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Create, Localizer[""Create""],""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Edit, Localizer[""Edit""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Delete, Localizer[""Delete""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Details, Localizer[""Details""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.BatchEdit, Localizer[""BatchEdit""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.BatchDelete, Localizer[""BatchDelete""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Import, Localizer[""Import""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.ExportExcel, Localizer[""Export""], ""{Area ?? ""}""),
            }};
        }}
";
                }

                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Normal)
                    {
                        headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}),";
                        if (pro.FieldName.ToLower() != "id")
                        {
                            selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
                        }
                    }
                    else
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (subtype == typeof(FileAttachment))
                        {
                            var filefk = DC.GetFKName2(modelType, pro.FieldName);
                            headerstring += $@"
                this.MakeGridHeader(x => x.{filefk}).SetFormat({filefk}Format),";
                            selectstring += $@"
                    {filefk} = x.{filefk},";
                            formatstring += GetResource("HeaderFormat.txt").Replace("$modelname$", ModelName).Replace("$field$", filefk).Replace("$classname$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                        }
                        else
                        {
                            var subpro = subtype.GetProperties().Where(x => x.Name == pro.SubField).FirstOrDefault();
                            existSubPro.Add(subpro);
                            string prefix = "";
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            string subtypename = subpro.PropertyType.Name;
                            if (subpro.PropertyType.IsNullable())
                            {
                                subtypename = subpro.PropertyType.GetGenericArguments()[0].Name + "?";
                            }

                            var subdisplay = subpro.GetCustomAttribute<DisplayAttribute>();
                            headerstring += $@"
                this.MakeGridHeader(x => x.{pro.SubField + "_view" + prefix}),";
                            if (pro.InfoType == FieldInfoType.One2Many)
                            {
                                selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.{pro.SubField},";
                            }
                            else
                            {
                                var middleType = modelType.GetSingleProperty(pro.FieldName).PropertyType.GenericTypeArguments[0];
                                var middlename = DC.GetPropertyNameByFk(middleType, pro.SubIdField);
                                selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.Select(y=>y.{middlename}.{pro.SubField}).ToSpratedString(null,"",""), ";
                            }
                            if (subdisplay?.Name != null)
                            {
                                subprostring += $@"
        [Display(Name = ""{subdisplay.Name}"")]";
                            }
                            subprostring += $@"
        public {subtypename} {pro.SubField + "_view" + prefix} {{ get; set; }}";
                        }
                    }

                }
                var wherepros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                foreach (var pro in wherepros)
                {
                    if (pro.SubField == "`file")
                    {
                        continue;
                    }
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).Select(x => x.PropertyType).FirstOrDefault();

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType == typeof(string))
                            {
                                wherestring += $@"
                .CheckContain(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            else if (proType == typeof(DateTime) || proType == typeof(DateTime?))
                            {
                                wherestring += $@"
                .CheckBetween(Searcher.{pro.FieldName}?.GetStartTime(), Searcher.{pro.FieldName}?.GetEndTime(), x => x.{pro.FieldName}, includeMax: false)";
                            }
                            else
                            {
                                wherestring += $@"
                .CheckEqual(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            break;
                        case FieldInfoType.One2Many:
                            var fk = DC.GetFKName2(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckEqual(Searcher.{fk}, x=>x.{fk})";
                            break;
                        case FieldInfoType.Many2Many:
                            var subtype = Type.GetType(pro.RelatedField);
                            var fk2 = DC.GetFKName(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckWhere(Searcher.Selected{pro.FieldName}IDs,x=>DC.Set<{proType.GetGenericArguments()[0].Name}>().Where(y=>Searcher.Selected{pro.FieldName}IDs.Contains(y.{pro.SubIdField})).Select(z=>z.{fk2}).Contains(x.ID))";
                            break;
                        default:
                            break;
                    }
                }
                rv = rv.Replace("$headers$", headerstring).Replace("$where$", wherestring).Replace("$select$", selectstring).Replace("$subpros$", subprostring).Replace("$format$", formatstring).Replace("$actions$", actionstring);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "CrudVM")
            {
                string prostr = "";
                string initstr = "";
                string includestr = "";
                string addstr = "";
                string editstr = "";
                var pros = FieldInfos.Where(x => x.IsFormField == true && string.IsNullOrEmpty(x.RelatedField) == false).ToList();
                foreach (var pro in pros)
                {
                    var subtype = Type.GetType(pro.RelatedField);
                    if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                    {
                        continue;
                    }
                    var fname = "All" + pro.FieldName + "s";
                    prostr += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                    initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.{pro.SubField});";
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName});";

                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        Type modelType = Type.GetType(SelectedModel);
                        var protype = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                        prostr += $@"
        [Display(Name = ""{protype.GetPropertyDisplayName()}"")]
        public List<{pro.GetFKType(DC, modelType)}> Selected{pro.FieldName}IDs {{ get; set; }}";
                        initstr += $@"
            Selected{pro.FieldName}IDs = Entity.{pro.FieldName}?.Select(x => x.{pro.SubIdField}).ToList();";
                        addstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if (Selected{pro.FieldName}IDs != null)
            {{
                foreach (var id in Selected{pro.FieldName}IDs)
                {{
                    Entity.{pro.FieldName}.Add(new {protype.PropertyType.GetGenericArguments()[0].Name} {{ {pro.SubIdField} = id }});
                }}
            }}
";
                        editstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if(Selected{pro.FieldName}IDs != null )
            {{
                Selected{pro.FieldName}IDs.ForEach(x => Entity.{pro.FieldName}.Add(new {protype.PropertyType.GetGenericArguments()[0].Name} {{ ID = Guid.NewGuid(), {pro.SubIdField} = x }}));
            }}
";
                    }
                }
                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    rv = rv.Replace("$pros$", prostr).Replace("$init$", initstr).Replace("$include$", includestr).Replace("$add$", addstr).Replace("$edit$", editstr);
                }
                else
                {
                    rv = rv.Replace("$pros$", "").Replace("$init$", "").Replace("$include$", includestr).Replace("$add$", "").Replace("$edit$", "");
                }
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ImportVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = FieldInfos.Where(x => x.IsImportField == true).ToList();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        initstr += $@"
            {pro.FieldName + "_Excel"}.DataType = ColumnDataType.ComboBox;
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).FirstOrDefault();
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    var filefk = DC.GetFKName2(modelType, pro.FieldName);
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{filefk});";
                    }
                    else
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{pro.FieldName});";
                    }
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);

            }
            return rv;
        }

        public string GenerateView(string name)
        {
            var rv = GetResource($"{name}.txt", "Mvc").Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName);
            if (name == "CreateView" || name == "EditView" || name == "DeleteView" || name == "DetailsView" || name == "BatchEditView")
            {
                StringBuilder fieldstr = new StringBuilder();
                string pre = "";
                List<FieldInfo> pros = null;
                if (name == "BatchEditView")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                    pre = "LinkedVM";
                }
                else
                {
                    pros = FieldInfos.Where(x => x.IsFormField == true).ToList();
                    pre = "Entity";
                }
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Two"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (name == "DeleteView" || name == "DetailsView")
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            if (string.IsNullOrEmpty(item.RelatedField) == false && item.SubField != "`file")
                            {
                                fieldstr.Append($@"<wt:display field=""{pre}.{item.FieldName}.{item.SubField}"" />");
                            }
                            else
                            {
                                string idname = item.FieldName;
                                if (string.IsNullOrEmpty(item.RelatedField) == false && item.SubField == "`file")
                                {
                                    var filefk = DC.GetFKName2(modelType, item.FieldName);
                                    idname = filefk;
                                }
                                fieldstr.Append($@"<wt:display field=""{pre}.{idname}"" />");
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var filefk = DC.GetFKName2(modelType, item.FieldName);
                            if (item.SubField == "`file")
                            {
                                if (name != "BatchEditView")
                                {
                                    fieldstr.Append($@"<wt:upload field=""{pre}.{filefk}"" />");
                                }
                            }
                            else
                            {
                                var fname = "All" + item.FieldName + "s";
                                if (name == "BatchEditView")
                                {
                                    fname = "LinkedVM." + fname;
                                }
                                if (string.IsNullOrEmpty(item.SubIdField))
                                {
                                    fieldstr.Append($@"<wt:combobox field=""{pre}.{filefk}"" items=""{fname}""/>");
                                }
                                else
                                {
                                    if (name == "BatchEditView")
                                    {
                                        fieldstr.Append($@"<wt:checkbox field=""LinkedVM.Selected{item.FieldName}IDs"" items=""{fname}""/>");
                                    }
                                    else
                                    {
                                        fieldstr.Append($@"<wt:checkbox field=""Selected{item.FieldName}IDs"" items=""{fname}""/>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if(checktype == typeof(bool) && proType.IsNullable() == false)
                            {
                                fieldstr.Append($@"<wt:switch field=""{pre}.{item.FieldName}"" />");

                            }
                            else if (checktype == typeof(bool) || checktype.IsEnum())
                            {
                                fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype.IsPrimitive || checktype == typeof(string) || checktype == typeof(decimal))
                            {
                                fieldstr.Append($@"<wt:textbox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype == typeof(DateTime))
                            {
                                fieldstr.Append($@"<wt:datetime field=""{pre}.{item.FieldName}"" />");
                            }
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append("</wt:row>");
                rv = rv.Replace("$fields$", fieldstr.ToString());
            }
            if (name == "ListView")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Three"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (item.SubField == "`file")
                        {
                            continue;
                        }
                        var fname = "All" + item.FieldName + "s";
                        var fk = "";
                        if (string.IsNullOrEmpty(item.SubIdField))
                        {
                            fk = DC.GetFKName2(modelType, item.FieldName); ;
                        }
                        else
                        {
                            fk = $@"Selected{item.FieldName}IDs";
                        }
                        fieldstr.Append($@"<wt:combobox field=""Searcher.{fk}"" items=""Searcher.{fname}"" empty-text=""@Localizer[""All""]"" />");
                    }
                    else
                    {
                        var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if ((checktype.IsPrimitive && checktype != typeof(bool)) || checktype == typeof(string) || checktype == typeof(decimal))
                        {
                            fieldstr.Append($@"<wt:textbox field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype == typeof(DateTime))
                        {
                            fieldstr.Append($@"<wt:datetime field=""Searcher.{item.FieldName}"" range=""true"" />");
                        }
                        if (checktype.IsEnum() || checktype.IsBool())
                        {
                            fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" empty-text=""@Localizer[""All""]"" />");
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append($@"</wt:row>");
                string url = "";
                if (string.IsNullOrEmpty(Area))
                {
                    url = $"/{ModelName}/Search";
                }
                else
                {
                    url = $"/{Area}/{ModelName}/Search";
                }
                rv = rv.Replace("$fields$", fieldstr.ToString()).Replace("$searchurl$", url);
            }
            return rv;
        }

        public string GenerateTest()
        {
            var rv = "";
            if (TestDir != null)
            {
                Type modelType = Type.GetType(SelectedModel);
                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    if (modelType.IsSubclassOf(typeof(BasePoco)))
                    {
                        rv = GetResource($"ControllerTest.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs);
                    }
                    else
                    {
                        rv = GetResource($"ControllerTestTopPoco.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs);
                    }
                }
                else
                {
                    if (modelType.IsSubclassOf(typeof(BasePoco)))
                    {
                        rv = GetResource($"ApiTest.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                    else
                    {
                        rv = GetResource($"ApiTestTopPoco.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                }
                var modelprops = modelType.GetRandomValues();
                string cpros = "";
                string epros = "";
                string pros = "";
                string mpros = "";
                string assert = "";
                string eassert = "";
                string fc = "";
                string add = "";

                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        var fktype = modelType.GetProperties().Where(x => x.Name == pro.Key.Substring(0, pro.Key.Length - 2)).Select(x => x.PropertyType).FirstOrDefault();
                        add += GenerateAddFKModel(pro.Key.Substring(0, pro.Key.Length - 2), fktype);
                    }
                }

                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        cpros += $@"
            v.{pro.Key} = Add{pro.Key.Substring(0, pro.Key.Length - 2)}();";
                        pros += $@"
                v.{pro.Key} = Add{pro.Key.Substring(0, pro.Key.Length - 2)}();";
                        mpros += $@"
                v1.{pro.Key} = Add{pro.Key.Substring(0, pro.Key.Length - 2)}();";
                        fc += $@"
            vm.FC.Add(""Entity.{pro.Key}"", """");";

                    }
                    else
                    {
                        cpros += $@"
            v.{pro.Key} = {pro.Value};";
                        pros += $@"
                v.{pro.Key} = {pro.Value};";
                        mpros += $@"
                v1.{pro.Key} = {pro.Value};";
                        assert += $@"
                Assert.AreEqual(data.{pro.Key}, {pro.Value});";
                        fc += $@"
            vm.FC.Add(""Entity.{pro.Key}"", """");";
                    }
                }

                var modelpros2 = modelType.GetRandomValues();
                foreach (var pro in modelpros2)
                {
                    if (pro.Key.ToLower() == "id")
                    {
                        continue;
                    }

                    if (pro.Value == "$fk$")
                    {
                        mpros += $@"
                v2.{ pro.Key} = v1.{pro.Key}; ";

                    }
                    else
                    {
                        epros += $@"
            v.{pro.Key} = {pro.Value};";
                        mpros += $@"
                v2.{pro.Key} = {pro.Value};";
                        eassert += $@"
                Assert.AreEqual(data.{pro.Key}, {pro.Value});";
                    }
                }

                string del = $"Assert.AreEqual(context.Set<{ModelName}>().Count(), 0);";
                string mdel = $"Assert.AreEqual(context.Set<{ModelName}>().Count(), 0);";
                if (modelType.IsSubclassOf(typeof(PersistPoco)))
                {
                    del = $"Assert.AreEqual(context.Set<{ModelName}>().Count(), 1);";
                    mdel = $"Assert.AreEqual(context.Set<{ModelName}>().Count(), 2);";
                }

                rv = rv.Replace("$cpros$", cpros).Replace("$epros$", epros).Replace("$pros$", pros).Replace("$mpros$", mpros).Replace("$assert$", assert).Replace("$eassert$", eassert).Replace("$fc$", fc).Replace("$add$", add).Replace("$del$", del).Replace("$mdel$", mdel);
            }
            return rv;
        }

        private string GenerateAddFKModel(string keyname, Type t)
        {
            var modelprops = t.GetRandomValues();
            var mname = t.Name?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            string cpros = "";
            string rv = "";
            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    var fktype = t.GetProperties().Where(x => x.Name == pro.Key.Substring(0, pro.Key.Length - 2)).Select(x => x.PropertyType).FirstOrDefault();
                    rv += GenerateAddFKModel(pro.Key.Substring(0, pro.Key.Length - 2), fktype);
                }
            }


            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    cpros += $@"
                v.{pro.Key} = Add{pro.Key.Substring(0, pro.Key.Length - 2)}();";
                }
                else
                {
                    cpros += $@"
                v.{pro.Key} = {pro.Value};";
                }
            }
            var idpro = t.GetProperties().Where(x => x.Name.ToLower() == "id").Select(x => x.PropertyType).FirstOrDefault();
            rv += $@"
        private {idpro.Name} Add{keyname}()
        {{
            {mname} v = new {mname}();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {{
{cpros}
                context.Set<{mname}>().Add(v);
                context.SaveChanges();
            }}
            return v.ID;
        }}
";
            return rv;
        }

        public string GenerateReactView(string name)
        {
            var rv = GetResource($"{name}.txt", "Spa.React.views")
                .Replace("$modelname$", ModelName.ToLower());
            Type modelType = Type.GetType(SelectedModel);
            if (name == "table")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                int rowheight = 30;
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var mpro = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                    string label = mpro.GetPropertyDisplayName();
                    string render = "";
                    string newname = item.FieldName;
                    if (mpro.PropertyType.IsBoolOrNullableBool())
                    {
                        render = "columnsRenderBoolean";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";
                        if (subtype == typeof(FileAttachment))
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                            {
                                render = "columnsRenderImg";
                                rowheight = 110;
                            }
                            else
                            {
                                render = "columnsRenderDownload";
                            }
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            newname = fk;
                        }
                        else
                        {
                            var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                            existSubPro.Add(subpro);
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            newname = item.SubField + "_view" + prefix;
                        }
                    }
                    fieldstr.Append($@"
    {{
        field: ""{newname}"",
        headerName: ""{label}""");

                    if (render != "")
                    {
                        fieldstr.Append($@",
        cellRenderer: ""{render}"" ");
                    }
                    fieldstr.Append($@"
    }}");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(",");
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                return rv.Replace("$columns$", fieldstr.ToString()).Replace("$rowheight$", rowheight.ToString());
            }
            if (name == "models")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr2 = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();
                var pros2 = FieldInfos.Where(x => x.IsSearcherField == true).ToList();

                //生成表单model
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var property = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                    string label = property.GetPropertyDisplayName();
                    bool isrequired = property.IsPropertyRequired();
                    var fktest = DC.GetFKName2(modelType, item.FieldName);
                    if (string.IsNullOrEmpty(fktest) == false)
                    {
                        isrequired = modelType.GetProperties().Where(x => x.Name == fktest).FirstOrDefault().IsPropertyRequired();
                    }
                    string rules = "rules: []";
                    if (isrequired == true)
                    {
                        rules = $@"rules: [{{ ""required"": true, ""message"": <FormattedMessage id='tips.error.required' values={{{{ txt: getLocalesValue('{label}') }}}} /> }}]";
                    }
                    fieldstr.AppendLine($@"            /** {label} */");
                    if (string.IsNullOrEmpty(item.RelatedField) == false && string.IsNullOrEmpty(item.SubIdField) == true)
                    {
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        fieldstr.AppendLine($@"            ""Entity.{fk}"":{{");
                    }
                    else
                    {
                        fieldstr.AppendLine($@"            ""Entity.{item.FieldName}"":{{");
                    }
                    fieldstr.AppendLine($@"                label: ""{label}"",");
                    fieldstr.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            fieldstr.AppendLine($@"                formItem: <WtmUploadImg />");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                fieldstr.AppendLine($@"                formItem: <WtmSelect placeholder=""{label}""
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");
                            }
                            else
                            {
                                fieldstr.AppendLine($@"                formItem: <WtmTransfer
                    listStyle={{undefined}}
                    dataSource={{Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                    mapKey=""{item.SubIdField}""
                /> ");

                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            fieldstr.AppendLine($@"                formItem: <Switch checkedChildren={{<Icon type=""check"" />}} unCheckedChildren={{<Icon type=""close"" />}} />");
                        }
                        else if (checktype.IsEnum())
                        {
                            var es = checktype.ToListItems();
                            fieldstr.AppendLine($@"                formItem: <WtmSelect placeholder=""{label}"" dataSource={{[  ");
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                fieldstr.Append($@"                    {{ Text: ""{e.Text}"", Value: {e.Value} }}");
                                if (a < es.Count - 1)
                                {
                                    fieldstr.Append(",");
                                }
                                fieldstr.AppendLine();
                            }
                            fieldstr.AppendLine($@"                ]}}/>");
                        }
                        else if (checktype.IsNumber())
                        {
                            fieldstr.AppendLine($@"                formItem: <InputNumber placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                        else if (checktype == typeof(string))
                        {
                            fieldstr.AppendLine($@"                formItem: <Input placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr.AppendLine($@"                formItem: <WtmDatePicker placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                    }
                    fieldstr.Append("            }");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(",");
                    }
                    fieldstr.Append(Environment.NewLine);
                }

                //生成searchmodel
                for (int i = 0; i < pros2.Count; i++)
                {
                    var item = pros2[i];
                    if (item.SubField == "`file")
                    {
                        continue;
                    }
                    var property = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                    string label = property.GetPropertyDisplayName();
                    string rules = "rules: []";

                    fieldstr2.AppendLine($@"            /** {label} */");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            fieldstr2.AppendLine($@"            ""{fk}"":{{");
                        }
                        else
                        {
                            fieldstr2.AppendLine($@"            ""Selected{item.FieldName}IDs"":{{");
                        }
                    }
                    else
                    {
                        fieldstr2.AppendLine($@"            ""{item.FieldName}"":{{");
                    }
                    fieldstr2.AppendLine($@"                label: ""{label}"",");
                    fieldstr2.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");
                        }
                        else
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}  multiple
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");

                        }
                    }
                    else
                    {
                        var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}  dataSource={{[
                    {{ Text: <FormattedMessage id='tips.bool.true' />, Value: true }},{{ Text: <FormattedMessage id='tips.bool.false' />, Value: false }}
                ]}}/>");
                        }
                        else if (checktype.IsEnum())
                        {
                            var es = checktype.ToListItems();
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}} dataSource={{[  ");
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                fieldstr2.Append($@"                    {{ Text: ""{e.Text}"", Value: {e.Value} }}");
                                if (a < es.Count - 1)
                                {
                                    fieldstr2.Append(",");
                                }
                                fieldstr2.AppendLine();
                            }
                            fieldstr2.AppendLine($@"                ]}}/>");
                        }
                        else if (checktype.IsNumber())
                        {
                            fieldstr2.AppendLine($@"                formItem: <InputNumber placeholder="""" />");
                        }
                        else if (checktype == typeof(string))
                        {
                            fieldstr2.AppendLine($@"                formItem: <Input placeholder="""" />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmRangePicker placeholder="""" />");
                        }
                    }
                    fieldstr2.Append("            }");
                    if (i < pros.Count - 1)
                    {
                        fieldstr2.Append(",");
                    }
                    fieldstr2.Append(Environment.NewLine);
                }

                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$fields2$", fieldstr2.ToString());
            }

            if (name == "search")
            {
                return rv;
            }
            if (name == "forms")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    if (string.IsNullOrEmpty(item.SubIdField))
                    {
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""Entity.{fk}"" />");
                        }
                        else
                        {
                            fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""Entity.{item.FieldName}"" />");
                        }
                    }
                    else
                    {
                        fieldstr.AppendLine($@"                <Col span={{24}}>
                    <FormItem {{...props}} fieId=""Entity.{item.FieldName}"" layout=""row"" />
                </Col>");
                    }
                }
                return rv.Replace("$fields$", fieldstr.ToString());
            }

            return rv;
        }


        public string GenerateVUEView(string name, List<string> apineeded)
        {
            var rv = GetResource($"{name}.txt", "Spa.Vue")
                .Replace("$modelname$", ModelName.ToLower());
            if (apineeded == null)
            {
                apineeded = new List<string>();
            }
            Type modelType = Type.GetType(SelectedModel);
            if (name == "config")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder enumstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsListField == true || x.IsSearcherField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                List<string> existEnum = new List<string>();
                int rowheight = 30;
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var mpro = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                    string label = mpro.GetPropertyDisplayName();
                    string render = "";
                    string newname = item.FieldName;
                    if (mpro.PropertyType.IsBoolOrNullableBool())
                    {
                        render = "columnsRenderBoolean";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";
                        if (subtype == typeof(FileAttachment))
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                            {
                                render = "columnsRenderImg";
                                rowheight = 110;
                            }
                            else
                            {
                                render = "columnsRenderDownload";
                            }
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            newname = fk;
                        }
                        else
                        {
                            var subpro = subtype.GetProperties().Where(x => x.Name == item.SubField).FirstOrDefault();
                            existSubPro.Add(subpro);
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            newname = item.SubField + "_view" + prefix;
                        }
                    }

                    else
                    {
                        var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype.IsEnum())
                        {
                            if (existEnum.Contains(checktype.Name) == false)
                            {
                                var es = checktype.ToListItems();
                                enumstr.AppendLine($@"export const {item.FieldName}Types: Array<any> = [");
                                for (int a = 0; a < es.Count; a++)
                                {
                                    var e = es[a];
                                    enumstr.Append($@"  {{ Text: ""{e.Text}"", Value: {e.Value} }}");
                                    if (a < es.Count - 1)
                                    {
                                        enumstr.Append(",");
                                    }
                                    enumstr.AppendLine();
                                }
                                enumstr.AppendLine($@"];");
                                existEnum.Add(checktype.Name);
                            }
                        }
                    }
                    fieldstr.Append($@"
    {{
        key: ""{newname}"",
        label: ""{label}""");

                    if (render != "")
                    {
                        fieldstr.Append($@",
        isSlot: true ");
                    }
                    fieldstr.Append($@"
    }}");
                    fieldstr.Append(",");
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$rowheight$", rowheight.ToString()).Replace("$enums$", enumstr.ToString());
            }
            if (name == "views.dialog-form")
            {
                StringBuilder fieldstr = new StringBuilder();
                List<string> actions = new List<string>();
                List<string> enums = new List<string>();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                //生成表单model
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var property = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                    string label = property.GetPropertyDisplayName();
                    bool isrequired = property.IsPropertyRequired();
                    var fktest = DC.GetFKName2(modelType, item.FieldName);
                    if (string.IsNullOrEmpty(fktest) == false)
                    {
                        isrequired = modelType.GetProperties().Where(x => x.Name == fktest).FirstOrDefault().IsPropertyRequired();
                    }
                    string rules = "rules: []";
                    if (isrequired == true)
                    {
                        rules = $@"rules: [{{ required: true, message: ""{label}""+this.$t(""form.notnull""),trigger: ""blur"" }}]";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false && string.IsNullOrEmpty(item.SubIdField) == true)
                    {
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        fieldstr.AppendLine($@"             ""Entity.{fk}"":{{");
                    }
                    else
                    {
                        fieldstr.AppendLine($@"             ""Entity.{item.FieldName}"":{{");
                    }
                    fieldstr.AppendLine($@"                 label: ""{label}"",");
                    fieldstr.AppendLine($@"                 {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                            {
                                fieldstr.AppendLine($@"                type: ""wtmUploadImg"",
                    props: {{
                        isHead: true,
                        imageStyle: {{ width: ""100px"", height: ""100px"" }}
                    }}
");
                            }
                            else
                            {
                                fieldstr.AppendLine($@"                type: ""upload""");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                fieldstr.AppendLine($@"                    type: ""select"",
                    children: this.get{subtype.Name}Data,
                    props: {{
                        clearable: true
                    }}");
                            }
                            else
                            {
                                fieldstr.AppendLine($@"                    type: ""transfer"",
                    mapKey: ""{item.SubIdField}"",
                    props: {{
                        data: this.get{subtype.Name}Data.map(item => ({{
                            key: item.Value,
                            label: item.Text
                        }})),
                        titles: [this.$t(""form.all""), this.$t(""form.selected"")],
                        filterable: true,
                        filterMethod: filterMethod
                    }},
                    span: 24,
                    defaultValue: []");

                            }
                            apineeded.Add($"get{subtype.Name}");
                            actions.Add($"get{subtype.Name}");
                        }
                    }
                    else
                    {
                        var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            fieldstr.AppendLine($@"                    type: ""switch""");
                        }
                        else if (checktype.IsEnum())
                        {
                            fieldstr.AppendLine($@"                    type: ""select"",
                    children: {item.FieldName}Types,
                    props: {{
                        clearable: true
                    }}");

                            enums.Add(item.FieldName + "Types");
                        }
                        else if (checktype.IsNumber())
                        {
                            fieldstr.AppendLine($@"                    type: ""input""");
                        }
                        else if (checktype == typeof(string))
                        {
                            fieldstr.AppendLine($@"                    type: ""input""");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr.AppendLine($@"                    type: ""datePicker""");
                        }
                    }
                    fieldstr.Append("            }");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(",");
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                string a1 = "";
                string a2 = "";
                foreach (var item in actions.Distinct())
                {
                    a1 += $@"    @Action
    {item};
    @State
    {item}Data;
";
                    a2 += $@"        this.{item}();
";
                }
                string import = "";
                if (enums.Count > 0)
                {
                    import = $@"import {{ {enums.Distinct().ToSpratedString()} }} from ""../config"";";
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$actions$", a1).Replace("$runactions$", a2).Replace("$import$", import);
            }

            if (name == "index")
            {
                StringBuilder fieldstr2 = new StringBuilder();
                StringBuilder actions = new StringBuilder();
                List<string> acts = new List<string>();
                List<string> enums = new List<string>();
                var pros2 = FieldInfos.Where(x => x.IsSearcherField == true || x.IsListField).ToList();
                int searchcount = 0;
                for (int i = 0; i < pros2.Count; i++)
                {

                    var item = pros2[i];
                    if (item.IsListField == true)
                    {
                        var mpro = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                        if (mpro.PropertyType.IsBoolOrNullableBool())
                        {
                            actions.AppendLine($@"      <template #{item.FieldName}=""rowData"">
        <el-switch :value=""rowData.row.{item.FieldName} === 'true' || rowData.row.{item.FieldName} === true"" disabled />
      </template>
");
                        }
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var subtype = Type.GetType(item.RelatedField);
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            if (subtype == typeof(FileAttachment))
                            {
                                if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                                {
                                    actions.AppendLine($@"      <template #{fk}=""rowData"">
        <el-image v-if=""!!rowData.row.{fk}"" style=""width: 100px; height: 100px"" :src=""'/api/_file/downloadFile/'+rowData.row.{fk}"" fit=""cover"" />
      </template>
");
                                }
                                else
                                {
                                    actions.AppendLine($@"      <template #{fk}=""rowData"">
        <el-link icon=""el-icon-edit"" v-if=""!!rowData.row.{fk}"" :href=""'/api/_file/downloadFile/'+rowData.row.{fk}"">{{{{ $t(""table.download"")}}}}</el-link>
      </template>
");
                                }
                            }
                        }
                    }
                    if (item.IsSearcherField == true)
                    {
                        if (item.SubField == "`file")
                        {
                            continue;
                        }
                        searchcount++;
                        var property = modelType.GetProperties().Where(x => x.Name == item.FieldName).FirstOrDefault();
                        string label = property.GetPropertyDisplayName();
                        string rules = "rules: []";

                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                var fk = DC.GetFKName2(modelType, item.FieldName);
                                fieldstr2.AppendLine($@"                ""{fk}"":{{");
                            }
                            else
                            {
                                fieldstr2.AppendLine($@"                ""Selected{item.FieldName}IDs"":{{");
                            }
                        }
                        else
                        {
                            fieldstr2.AppendLine($@"                ""{item.FieldName}"":{{");
                        }
                        fieldstr2.AppendLine($@"                    label: ""{label}"",");
                        fieldstr2.AppendLine($@"                    {rules},");
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var subtype = Type.GetType(item.RelatedField);
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                fieldstr2.AppendLine($@"                    type: ""select"",
                    children: this.get{subtype.Name}Data,
                    props: {{
                        clearable: true,
                        placeholder: '全部'
                    }}");

                            }
                            else
                            {
                                fieldstr2.AppendLine($@"                    type: ""select"",
                    children: this.get{subtype.Name}Data,
                    props: {{
                        clearable: true ,
                        multiple: true,
                        ""collapse-tags"": true
                    }}");

                            }
                            apineeded.Add($"get{subtype.Name}");
                            acts.Add($"get{subtype.Name}");
                        }
                        else
                        {
                            var proType = modelType.GetProperties().Where(x => x.Name == item.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if (checktype == typeof(bool))
                            {
                                fieldstr2.AppendLine($@"                    type: ""switch""");
                            }
                            else if (checktype.IsEnum())
                            {
                                fieldstr2.AppendLine($@"                    type: ""select"",
                    children: {item.FieldName}Types,
                    props: {{
                        clearable: true,
                        placeholder: this.$t(""form.all"")
                    }}");

                                enums.Add(item.FieldName + "Types");
                            }
                            else if (checktype.IsNumber())
                            {
                                fieldstr2.AppendLine($@"                    type: ""input""");
                            }
                            else if (checktype == typeof(string))
                            {
                                fieldstr2.AppendLine($@"                    type: ""input""");
                            }
                            else if (checktype == typeof(DateTime))
                            {
                                fieldstr2.AppendLine($@"                    type: ""datePicker"",
                    span: 12,
                    props: {{
                            type: ""datetimerange"",
                        ""value-format"": ""yyyy-MM-dd HH:mm:ss"",
                        ""range-separator"": ""-"",
                        ""start-placeholder"": this.$t(""table.startdate""),
                        ""end-placeholder"": this.$t(""table.enddate"")
                    }}");
                            }
                        }
                        if (searchcount > 2)
                        {
                            fieldstr2.AppendLine("                    ,isHidden: !this.isActive");
                        }
                        fieldstr2.Append("              },");
                        fieldstr2.Append(Environment.NewLine);
                    }
                }

                string a1 = "";
                string a2 = "";
                foreach (var item in acts.Distinct())
                {
                    a1 += $@"    @Action
    {item};
    @State
    {item}Data;
";
                    a2 += $@"        this.{item}();
";
                }


                return rv.Replace("$fields$", fieldstr2.ToString()).Replace("$actions$", actions.ToString()).Replace("$enums$", enums.Distinct().ToSpratedString())
                    .Replace("$acts$", a1).Replace("$runactions$", a2);

            }
            if (name == "store.api")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder efieldstr = new StringBuilder();

                var apis = apineeded.Distinct().ToList();
                for (int i = 0; i < apis.Count; i++)
                {
                    var item = apis[i];
                    fieldstr.AppendLine($@"const {item} = {{
  url: reqPath + ""{item}s"",
  method: ""get"",
  dataType: ""array""
}}; ");
                    efieldstr.AppendLine($"{item},");
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$efields$", efieldstr.ToString());
            }

            return rv;
        }


        public string GetResource(string fileName, string subdir = "")
        {
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();
            string loc = "";
            if (string.IsNullOrEmpty(subdir))
            {
                loc = $"WalkingTec.Mvvm.Mvc.GeneratorFiles.{fileName}";
            }
            else
            {
                loc = $"WalkingTec.Mvvm.Mvc.GeneratorFiles.{subdir}.{fileName}";
            }
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }

        private string GetRelatedNamespace(List<FieldInfo> pros, string s)
        {
            string otherns = @"";
            Type modelType = Type.GetType(SelectedModel);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.RelatedField))
                {
                    proType = modelType.GetProperties().Where(x => x.Name == pro.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                }
                else
                {
                    proType = Type.GetType(pro.RelatedField);
                }
                string prons = proType.Namespace;
                if (proType.IsNullable())
                {
                    prons = proType.GetGenericArguments()[0].Namespace;
                }
                if (s.Contains($"using {prons}") == false && otherns.Contains($"using {prons}") == false)
                {
                    otherns += $@"using {prons};
";
                }

            }

            return s.Replace("$othernamespace$", otherns);
        }

    }

    public enum FieldInfoType { Normal, One2Many, Many2Many }

    public class FieldInfo
    {
        public string FieldName { get; set; }
        public string RelatedField { get; set; }

        public bool IsSearcherField { get; set; }

        public bool IsListField { get; set; }

        public bool IsFormField { get; set; }

        public bool IsImportField { get; set; }
        public bool IsBatchField { get; set; }

        public FieldInfoType InfoType
        {
            get
            {
                if (string.IsNullOrEmpty(RelatedField))
                {
                    return FieldInfoType.Normal;
                }
                else
                {
                    if (string.IsNullOrEmpty(SubIdField))
                    {
                        return FieldInfoType.One2Many;
                    }
                    else
                    {
                        return FieldInfoType.Many2Many;
                    }
                }
            }
        }

        /// <summary>
        /// 字段关联的类名
        /// </summary>
        public string SubField { get; set; }
        /// <summary>
        /// 多对多关系时，记录中间表关联到主表的字段名称
        /// </summary>
        public string SubIdField { get; set; }

        public string GetField(IDataContext DC, Type modelType)
        {
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = DC.GetFKName2(modelType, this.FieldName);
                return fk;
            }
            else
            {
                return this.FieldName;
            }
        }

        public string GetFKType(IDataContext DC, Type modelType)
        {
            Type fktype = null;
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = this.GetField(DC, modelType);
                fktype = modelType.GetProperties().Where(x => x.Name == fk).Select(x => x.PropertyType).FirstOrDefault();
            }
            if (this.InfoType == FieldInfoType.Many2Many)
            {
                var middletype = modelType.GetProperties().Where(x => x.Name == this.FieldName).Select(x => x.PropertyType).FirstOrDefault();
                fktype = middletype.GetGenericArguments()[0].GetProperties().Where(x => x.Name == this.SubIdField).Select(x => x.PropertyType).FirstOrDefault();
            }
            var typename = "string";

            if (fktype == typeof(short) || fktype == typeof(short?))
            {
                typename = "short";
            }
            if (fktype == typeof(int) || fktype == typeof(int?))
            {
                typename = "int";
            }
            if (fktype == typeof(long) || fktype == typeof(long?))
            {
                typename = "long";
            }
            if (fktype == typeof(Guid) || fktype == typeof(Guid?))
            {
                typename = "Guid";
            }

            return typename;

        }
    }
}
