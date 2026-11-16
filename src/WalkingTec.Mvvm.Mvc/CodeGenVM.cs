using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// API 鉴权方式枚举，用于代码生成时为 Controller 选择对应的授权特性
    /// </summary>
    public enum ApiAuthMode
    {
        [Display(Name = "Both Jwt and Cookie")]
        Both,
        [Display(Name = "Jwt")]
        Jwt,
        [Display(Name = "Cookie")]
        Cookie
    }

    /// <summary>
    /// 代码生成器核心 ViewModel，根据用户选择的模型和字段配置，
    /// 生成 Controller、各类 ViewModel、前端页面（LayUI、React、VUE、VUE3、Blazor）以及对应的测试代码
    /// </summary>
    [ReInit(ReInitModes.ALWAYS)]
    public class CodeGenVM : BaseVM
    {
        public CodeGenListVM FieldList { get; set; }

        public List<FieldInfo> FieldInfos { get; set; }

        public string PreviewFile { get; set; }

        public UIEnum UI { get; set; }

        [Display(Name = "Codegen.GenApi")]
        public bool IsApi { get; set; }

        [Display(Name = "Codegen.AuthMode")]
        public ApiAuthMode AuthMode { get; set; }

        public string ModelName
        {
            get
            {
                return SelectedModel?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            }
        }
        [Display(Name = "Codegen.ModelNS")]
        [ValidateNever()]
        public string ModelNS => SelectedModel?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSepratedString(seperator: ".");
        [Display(Name = "Codegen.ModuleName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string ModuleName { get; set; }
        [RegularExpression("^[A-Za-z_]+", ErrorMessage = "Codegen.EnglishOnly")]
        public string Area { get; set; }
        [ValidateNever()]
        [BindNever()]
        public List<ComboSelectListItem> AllModels { get; set; }
        [Required(ErrorMessage = "Validate.{0}required")]
        [Display(Name = "_Admin.SelectedModel")]
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

        public string _sharedir;
        [ValidateNever()]
        public string ShareDir
        {
            get
            {
                if (_sharedir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var sharedir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".shared")).FirstOrDefault();
                    if (string.IsNullOrEmpty(Area))
                    {
                        sharedir = Directory.CreateDirectory(sharedir.FullName + $"{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}{ModelName}");
                    }
                    else
                    {
                        sharedir = Directory.CreateDirectory(sharedir.FullName + $"{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}{ModelName}");
                    }

                    _sharedir = sharedir.FullName;
                }
                return _sharedir;
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
                    _mainNs = MainDir[(index + 1)..];
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
        [Display(Name = "Codegen.ControllerNs")]
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
        [Display(Name = "Codegen.TestNs")]
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
        [Display(Name = "Codegen.DataNs")]
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
        [Display(Name = "Codegen.VMNs")]
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
                            _vmNs = vmdir.FullName[(index + 1)..];
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

        /// <summary>
        /// 初始化 VM：若已选择模型，则遍历所有数据库连接，找到包含该模型 DbSet 的 DataContext 并赋值给 DC；同时初始化字段列表 VM
        /// </summary>
        protected override void InitVM()
        {
            if (string.IsNullOrEmpty(SelectedModel) == false)
            {
                foreach (var item in ConfigInfo.Connections)
                {
                    var dc = item.CreateDC();
                    Type t = typeof(DbSet<>).MakeGenericType(Type.GetType(SelectedModel));
                    var exist = dc.GetType().GetSingleProperty(x => x.PropertyType == t);
                    if (exist != null)
                    {
                        this.DC = dc;
                    }
                }

            }

            FieldList = new CodeGenListVM();
            FieldList.CopyContext(this);
        }
        /// <summary>
        /// 执行代码生成：将生成的 Controller、各类 ViewModel、Vue3页面与api文件写入磁盘，最后生成测试代码。
        /// 框架仅支持Vue3前端，其他UI类型（LayUI/React/Vue/Blazor）的生成分支已移除
        /// </summary>
        public void DoGen()
        {
            File.WriteAllText($"{ControllerDir}{Path.DirectorySeparatorChar}{ModelName}Controller.cs", GenerateController(), Encoding.UTF8);

            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}VM.cs", GenerateVM("CrudVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}ImportVM.cs", GenerateVM("ImportVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);

            // 生成 VUE3 的页面及 api 文件
            var pathvue3 = $"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}views{Path.DirectorySeparatorChar}{Area.ToLower()}{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}";
            if (Directory.Exists(pathvue3) == false)
            {
                Directory.CreateDirectory(pathvue3);
            }
            var pathapi = $"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}api{Path.DirectorySeparatorChar}{Area.ToLower()}{Path.DirectorySeparatorChar}{ModelName}{Path.DirectorySeparatorChar}";
            if (Directory.Exists(pathapi) == false)
            {
                Directory.CreateDirectory(pathapi);
            }
            File.WriteAllText($"{pathvue3}index.vue", GenerateVue3View("Index"), Encoding.UTF8);
            File.WriteAllText($"{pathvue3}create.vue", GenerateVue3View("Create"), Encoding.UTF8);
            File.WriteAllText($"{pathvue3}edit.vue", GenerateVue3View("Edit"), Encoding.UTF8);
            File.WriteAllText($"{pathvue3}details.vue", GenerateVue3View("Details"), Encoding.UTF8);
            File.WriteAllText($"{pathvue3}import.vue", GenerateVue3View("Import"), Encoding.UTF8);

            File.WriteAllText($"{pathapi}index.ts", GenerateVue3View("indexapi"), Encoding.UTF8);

            // 生成测试代码并写入测试项目
            var test = GenerateTest();
            if (test != "")
            {
                File.WriteAllText($"{TestDir}{Path.DirectorySeparatorChar}{ModelName}ApiTest.cs", test, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成 Controller 代码：根据 UI 类型和是否为 API 选择模板目录及授权特性，并替换命名空间、模型名等占位符
        /// </summary>
        /// <returns>生成的 Controller 代码文本</returns>
        public string GenerateController()
        {
            // SPA(Vue3) 模式使用 Spa 模板目录，并按鉴权方式设置授权特性
            string dir = "Spa";
            string jwt = "";
            switch (AuthMode)
            {
                case ApiAuthMode.Jwt:
                    jwt = "[AuthorizeJwt]";
                    break;
                case ApiAuthMode.Cookie:
                    jwt = "[AuthorizeCookie]";
                    break;
                default:
                    jwt = "[AuthorizeJwtWithCookie]";
                    break;
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
                        var subpro = subtype.GetSingleProperty(item.SubField);
                        var key = subtype.FullName + ":" + subpro.Name;
                        existSubPro.Add(key);
                        int count = existSubPro.Where(x => x == key).Count();
                        if (count == 1)
                        {

                            other.AppendLine($@"
        [HttpGet(""Get{subtype.Name}s"")]
        public ActionResult Get{subtype.Name}s()
        {{
            return Ok(DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, x => x.{item.SubField}));
        }}");
                        }
                    }
                }
                rv = rv.Replace("$other$", other.ToString());
                rv = GetRelatedNamespace(pros, rv);
            }
            return rv;
        }

        /// <summary>
        /// 生成各类 ViewModel 代码，根据 name 选择对应模板，并拼接字段属性、初始化逻辑、表格列、搜索条件等占位符内容
        /// </summary>
        /// <param name="name">VM 类型名称，可选值为 CrudVM、ListVM、BatchVM、ImportVM、Searcher</param>
        /// <returns>生成的 ViewModel 代码文本</returns>
        public string GenerateVM(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$modelnamespace$", ModelNS).Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName).Replace("$area$", $"{Area ?? ""}").Replace("$classname$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
            // Searcher 和 BatchVM 需要生成字段属性定义，以及关联表下拉数据的初始化代码
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
                    }

                    //生成普通字段定义
                    var proType = modelType.GetSingleProperty(pro.FieldName);
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
                    if ((typename == "DateTime" || typename == "DateTime?") && name == "Searcher")
                    {
                        typename = "DateRange";
                    }
                    prostring += $@"
        public {typename} {proname} {{ get; set; }}";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            // ListVM 需要生成表格列头、Select 投影、搜索条件、关联字段属性、格式化方法及标准按钮等代码
            if (name == "ListVM")
            {
                string headerstring = "";
                string selectstring = "";
                string wherestring = "";
                string subprostring = "";
                string formatstring = "";
                string actionstring = "";


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
                            var subpro = subtype.GetSingleProperty(pro.SubField);
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
                                if (typeof(IPersistPoco).IsAssignableFrom(Type.GetType(pro.RelatedField)))
                                {
                                    selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.Where(y=>y.{middlename}.IsValid==true).Select(y=>y.{middlename}.{pro.SubField}).ToSepratedString(null,"",""), ";
                                }
                                else
                                {
                                    selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.Select(y=>y.{middlename}.{pro.SubField}).ToSepratedString(null,"",""), ";
                                }
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
                    var proType = modelType.GetSingleProperty(pro.FieldName)?.PropertyType;

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
            // CrudVM 需要生成关联表下拉数据、SetInclude 语句，以及多对多中间表的新增和修改处理代码
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
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName});";

                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        Type modelType = Type.GetType(SelectedModel);
                        var protype = modelType.GetSingleProperty(pro.FieldName);
                        prostr += $@"
        [Display(Name = ""{protype.GetPropertyDisplayName()}"")]
        public List<string> Selected{pro.FieldName}IDs {{ get; set; }}";
                        initstr += $@"
            Selected{pro.FieldName}IDs = Entity.{pro.FieldName}?.Select(x => x.{pro.SubIdField}.ToString()).ToList();";
                        addstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if (Selected{pro.FieldName}IDs != null)
            {{
                foreach (var id in Selected{pro.FieldName}IDs)
                {{
                     {protype.PropertyType.GetGenericArguments()[0].Name} middle = new {protype.PropertyType.GetGenericArguments()[0].Name}();
                    middle.SetPropertyValue(""{pro.SubIdField}"", id);
                    Entity.{pro.FieldName}.Add(middle);
                }}
            }}
";
                        editstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if(Selected{pro.FieldName}IDs != null )
            {{
                 foreach (var item in Selected{pro.FieldName}IDs)
                {{
                    {protype.PropertyType.GetGenericArguments()[0].Name} middle = new {protype.PropertyType.GetGenericArguments()[0].Name}();
                    middle.SetPropertyValue(""{pro.SubIdField}"", item);
                    Entity.{pro.FieldName}.Add(middle);
                }}
            }}
";
                    }
                }
                rv = rv.Replace("$pros$", "").Replace("$init$", "").Replace("$include$", includestr).Replace("$add$", "").Replace("$edit$", "");
                rv = GetRelatedNamespace(pros, rv);
            }
            // ImportVM 需要生成 Excel 导入字段属性，以及关联表下拉数据的初始化代码
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
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetSingleProperty(pro.FieldName);
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

        /// <summary>
        /// 生成控制器测试代码：为模型属性生成随机值，拼接新增、修改、删除、批量修改等测试用例的属性赋值与断言代码
        /// </summary>
        /// <returns>生成的测试代码文本；若测试项目目录不存在则返回空字符串</returns>
        public string GenerateTest()
        {
            var rv = "";
            if (TestDir != null)
            {
                Type modelType = Type.GetType(SelectedModel);
                    if (typeof(IBasePoco).IsAssignableFrom(modelType) || typeof(IPersistPoco).IsAssignableFrom(modelType))
                    {
                        rv = GetResource($"ApiTest.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                    else
                    {
                        rv = GetResource($"ApiTestTopPoco.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                // 为模型属性生成随机值，依次拼接新增、修改、批量修改测试所需的属性赋值与断言代码
                var modelprops = modelType.GetRandomValues();
                var batchpros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                string cpros = "";
                string epros = "";
                string pros = "";
                string mpros = "";
                string assert = "";
                string eassert = "";
                string fc = "";
                string add = "";
                string linkedpros = "";
                string linkedfc = "";
                string meassert = "";
                List<Type> addexist = new List<Type>();
                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                        add += GenerateAddFKModel(pro.Key[0..^2], fktype, addexist);
                    }
                }

                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                        cpros += $@"
            v.{pro.Key} = Add{fktype.Name}();";
                        pros += $@"
                v.{pro.Key} = Add{fktype.Name}();";
                        mpros += $@"
                v1.{pro.Key} = Add{fktype.Name}();";
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
                    }
                    fc += $@"
            vm.FC.Add(""Entity.{pro.Key}"", """");";

                }

                var modelpros2 = modelType.GetRandomValues();
                foreach (var pro in modelpros2)
                {
                    if (pro.Value == "$fk$")
                    {
                        mpros += $@"
                v2.{pro.Key} = v1.{pro.Key}; ";

                    }
                    else
                    {
                        mpros += $@"
                v2.{pro.Key} = {pro.Value};";
                        if (pro.Key.ToLower() != "id")
                        {
                            epros += $@"
            v.{pro.Key} = {pro.Value};";
                            eassert += $@"
                Assert.AreEqual(data.{pro.Key}, {pro.Value});";
                        }
                    }
                }

                var modelpros3 = modelType.GetRandomValues();
                foreach (var pro in modelpros3)
                {
                    if (batchpros.Any(x => x.FieldName == pro.Key) && pro.Key.ToLower() != "id")
                    {
                        linkedpros += $@"
            vm.LinkedVM.{pro.Key} = {pro.Value};";
                        linkedfc += $@"
            vm.FC.Add(""LinkedVM.{pro.Key}"", """");";
                        meassert += $@"
                Assert.AreEqual(data1.{pro.Key}, {pro.Value});";
                        meassert += $@"
                Assert.AreEqual(data2.{pro.Key}, {pro.Value});";
                    }
                }


                string del = $"Assert.AreEqual(data, null);";
                string mdel = @"Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);";
                if (typeof(IPersistPoco).IsAssignableFrom(modelType))
                {
                    del = $"Assert.AreEqual(data, null);";
                    mdel = @"Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);";
                }

                rv = rv.Replace("$cpros$", cpros).Replace("$epros$", epros).Replace("$pros$", pros).Replace("$mpros$", mpros)
                    .Replace("$assert$", assert).Replace("$eassert$", eassert).Replace("$fc$", fc).Replace("$add$", add).Replace("$del$", del).Replace("$mdel$", mdel)
                    .Replace("$linkedpros$", linkedpros).Replace("$linkedfc$", linkedfc).Replace("$meassert$", meassert);

                rv = GetRelatedNamespace(FieldInfos.Where(x => string.IsNullOrEmpty(x.RelatedField) == false).ToList(), rv);
            }
            return rv;
        }

        /// <summary>
        /// 生成测试中为外键字段构造关联测试数据的辅助方法，并递归处理多级外键引用
        /// </summary>
        /// <param name="keyname">外键属性名称</param>
        /// <param name="t">外键关联的类型</param>
        /// <param name="exist">已处理过的类型列表，用于避免重复生成</param>
        /// <returns>生成的辅助方法代码文本；若类型已处理过则返回空字符串</returns>
        private string GenerateAddFKModel(string keyname, Type t, List<Type> exist)
        {
            if (exist == null)
            {
                exist = new List<Type>();
            }
            if (exist.Contains(t) == true)
            {
                return "";
            }
            exist.Add(t);
            var modelprops = t.GetRandomValues();
            var mname = t.Name?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            string cpros = "";
            string rv = "";
            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    var fktype = t.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                    if (fktype != t)
                    {
                        rv += GenerateAddFKModel(pro.Key[0..^2], fktype, exist);
                    }
                }
            }


            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    var fktype = t.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                    if (fktype != t)
                    {
                        cpros += $@"
                v.{pro.Key} = Add{fktype.Name}();";
                    }
                }
                else
                {
                    cpros += $@"
                v.{pro.Key} = {pro.Value};";
                }
            }
            var idpro = t.GetSingleProperty("ID");
            rv += $@"
        private {idpro.PropertyType.Name} Add{t.Name}()
        {{
            {mname} v = new {mname}();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {{
                try{{
{cpros}
                context.Set<{mname}>().Add(v);
                context.SaveChanges();
                }}
                catch{{}}
            }}
            return v.ID;
        }}
";
            return rv;
        }


        /// <summary>
        /// 生成 VUE3 前端页面代码：根据页面类型拼接表格列定义、搜索区控件、表单控件及下拉数据初始化等 vue/ts 代码
        /// </summary>
        /// <param name="name">页面名称，如 Index、Create、Edit、Details、Import、indexapi</param>
        /// <returns>生成的 VUE3 页面代码文本</returns>
        public string GenerateVue3View(string name)
        {
            string pagepath = string.IsNullOrEmpty(Area) ? $"/{ModelName}" : $"/{Area.ToLower()}/{ModelName}";
          
            var rv = GetResource($"{name}.txt", "Spa.Vue3")
                .Replace("$modelname$", ModelName)
                .Replace("$vmnamespace$", VMNs)
                .Replace("$des$", ModuleName)
                .Replace("$controllername$", $"{ControllerNs};{ModelName}")
                .Replace("$pagepath$", pagepath);
            Type modelType = Type.GetType(SelectedModel);
            // 生成列表页：表格列定义、搜索区控件、搜索实体初始化及下拉数据 api 的调用代码
            if (name == "Index")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr2 = new StringBuilder();
                StringBuilder searchentity = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                var pros2 = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                Dictionary<string, string> apis = new Dictionary<string, string>();
                Dictionary<string, string> multiapis = new Dictionary<string, string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var mpro = modelType.GetSingleProperty(item.FieldName);
                    string template = "text";
                    string newname = item.FieldName;
                    var property = modelType.GetSingleProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    if (mpro.PropertyType.IsBoolOrNullableBool())
                    {
                        template = "switch";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";
                        if (subtype == typeof(FileAttachment))
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                template = "image";
                            }
                            else
                            {

                            }
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            newname = fk;
                        }
                        else
                        {
                            var subpro = subtype.GetSingleProperty(item.SubField);
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
        {{title:'{label}',key: '{newname}',type: '{template}',isCheck: true}},");

                }
                for (int i = 0; i < pros2.Count; i++)
                {
                    string controltype = "input";
                    string sitems = "";
                    string bindfield = "";
                    string ph = "";
                    var item = pros2[i];
                    var property = modelType.GetSingleProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    if (item.SubField == "`file")
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                        }
                    }
                    else
                    {
                        bindfield = item.FieldName;
                    }

                    searchentity.AppendLine($@"			{bindfield}: null,");


                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var tempname = $"All{subtype.Name}s";
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            controltype = "select";
                            sitems += $@"
                       <el-option v-for=""item in state{ModelName}.{tempname}"" :key=""item.Value"" :value=""item.Value"" :label=""item.Text""></el-option>";
                        }
                        else
                        {
                            ph += " multiple";
                        }
                        ph += $" :data=\"state{ModelName}.{tempname}\"";
                        if (apis.ContainsKey(tempname) == false && multiapis.ContainsKey(tempname) == false)
                        {
                            if (controltype == "select")
                            {
                                apis.Add(tempname, $"/api/{ModelName}/Get{subtype.Name}s");
                            }
                            else
                            {
                                multiapis.Add(tempname, $"/api/{ModelName}/Get{subtype.Name}s");
                            }
                        }

                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            controltype = "select";
                            sitems = $@"
                <el-option :key=""1"" :value=true :label=""$t('message._system.common.vm.tips_bool_true')""></el-option>
                <el-option :key=""0"" :value=false :label=""$t('message._system.common.vm.tips_bool_false')""></el-option>
                ";
                        }
                        else if (checktype.IsEnum())
                        {
                            controltype = "select";
                            var es = checktype.ToListItems();
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                sitems += $@"
                <el-option key=""{e.Value}"" value=""{e.Value}"" label=""{e.Text}""></el-option>";
                            }

                        }
                        else if (checktype.IsNumber())
                        {
                            controltype = "input-number";
                        }
                        else if (checktype == typeof(string))
                        {
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            controltype = "date-picker";
                        }
                    }
                    if (controltype == "Select" || controltype == "MultiSelect")
                    {
                        ph = "PlaceHolder=\"@WtmBlazor.Localizer[\"Sys.All\"]\"";
                    }
                    //<{controltype} @bind-Value=""@SearchModel.{bindfield}"" {sitems} {ph}/>
                    fieldstr2.Append($@"
    <el-col :xs=""24"" :lg=""12"" class=""mb20"">
        <el-form-item ref=""{bindfield}_FormItem"" prop=""{bindfield}"" label=""{label}"">
            <el-{controltype} v-model=""searchData{ModelName}.{bindfield}""{ph} clearable>{sitems}</el-{controltype}>
        </el-form-item>
    </el-col>");
                }
                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
    other.getSelectList('{item.Value}',[],false).then(x=>{{state{ModelName}.{item.Key} = x}});
");
                    fieldinit.Append($@"
    {item.Key}: [] as any[],");

                }
                foreach (var item in multiapis)
                {
                    apiinit.Append(@$"
    other.getSelectList('{item.Value}',[],false).then(x=>{{state{ModelName}.{item.Key} = x}});
");
                    fieldinit.Append($@"
    {item.Key}: [] as any[],");
                }


                return rv.Replace("$columns$", fieldstr.ToString()).Replace("$searchentity$", searchentity.ToString()).Replace("$searchfields$", fieldstr2.ToString()).Replace("$init$", apiinit.ToString()).Replace("$fieldinit$", fieldinit.ToString());
            }

            // 生成新建/编辑/详情页面：根据字段类型和关联关系选择表单控件，详情页控件为禁用状态
            if (name == "Create" || name == "Edit" || name == "Details")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldentityinit = new StringBuilder();
                StringBuilder selectfieldinit = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                if (name != "Create")
                {
                    fieldentityinit.AppendLine($@"			ID: null,");
                }
                //生成表单model
                Dictionary<string, string> apis = new Dictionary<string, string>();
                Dictionary<string, string> multiapis = new Dictionary<string, string>();


                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    string controltype = "el-input";
                    string sitems = "";
                    string bindfield = "";
                    string ph = "";
                    var property = modelType.GetSingleProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    bool isrequired = property.IsPropertyRequired();
                    var fktest = DC.GetFKName2(modelType, item.FieldName);
                    if (string.IsNullOrEmpty(fktest) == false)
                    {
                        isrequired = modelType.GetSingleProperty(fktest).IsPropertyRequired();
                    }
                    string rules = "";
                    if (isrequired == true)
                    {
                        rules = $@" :rules=""[{{ required: true, message:'{label}为必填项',trigger:'blur'}}]""";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false && string.IsNullOrEmpty(item.SubIdField) == true)
                    {
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        fieldentityinit.AppendLine($@"			{fk}: null,");
                    }
                    else
                    {
                        fieldentityinit.AppendLine($@"			{item.FieldName}: null,");
                    }



                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = "Entity." + fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                            selectfieldinit.AppendLine($@"			{bindfield}: [],");
                        }
                    }
                    else
                    {
                        bindfield = "Entity." + item.FieldName;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                controltype = "wtm-upload-image";
                            }
                            else
                            {
                                controltype = "wtm-upload-file";
                            }
                        }
                        else
                        {
                            var tempname = $"All{subtype.Name}s";
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                controltype = "el-select";
                                sitems = $@"
                       <el-option v-for=""item in state{ModelName}.{tempname}"" :key=""item.Value"" :value=""item.Value"" :label=""item.Text""></el-option>";
                            }
                            else
                            {
                                controltype = "el-transfer";
                                ph += $@" :data=""state{ModelName}.{tempname}""";
                            }

                            //sitems = $"Items=\"@{tempname}\"";
                            if (apis.ContainsKey(tempname) == false && multiapis.ContainsKey(tempname) == false)
                            {
                                if (controltype == "el-select")
                                {
                                    apis.Add(tempname, $"/api/{ModelName}/Get{subtype.Name}s");
                                }
                                else
                                {
                                    multiapis.Add(tempname, $"/api/{ModelName}/Get{subtype.Name}s");
                                }
                            }

                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            controltype = "el-select";
                            sitems = $@"
                <el-option :key=""1"" :value=true :label=""$t('message._system.common.vm.tips_bool_true')""></el-option>
                <el-option :key=""0"" :value=false :label=""$t('message._system.common.vm.tips_bool_false')""></el-option>
                ";
                        }
                        else if (checktype.IsEnum())
                        {
                            controltype = "el-select";
                            var es = checktype.ToListItems();
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                sitems += $@"
                <el-option key=""{e.Value}"" value=""{e.Value}"" label=""{e.Text}""></el-option>";
                            }

                        }
                        else if (checktype.IsNumber())
                        {
                            controltype = "el-input-number";
                        }
                        else if (checktype == typeof(string))
                        {
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            controltype = "el-date-picker";
                        }
                    }


                    if (name == "Details")
                    {
                        ph += " disabled";
                    }
                    if (controltype == "el-transfer")
                    {
                        fieldstr.Append($@"
    <el-col :xs=""24"" :lg=""24"" class=""mb20"">
        <el-form-item ref=""{bindfield.Replace(".", "_")}_FormItem"" prop=""{bindfield}"" label=""{label}""{rules}>
            <{controltype} v-model=""state{ModelName}.vmModel.{bindfield}""{ph} clearable>{sitems}</{controltype}>
        </el-form-item>
    </el-col>");
                    }
                    else
                    {

                        fieldstr.Append($@"
    <el-col :xs=""24"" :lg=""12"" class=""mb20"">
        <el-form-item ref=""{bindfield.Replace(".", "_")}_FormItem"" prop=""{bindfield}"" label=""{label}""{rules}>
            <{controltype} v-model=""state{ModelName}.vmModel.{bindfield}""{ph} clearable>{sitems}</{controltype}>
        </el-form-item>
    </el-col>");
                    }
                }

                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
    other.getSelectList('{item.Value}',[],false).then(x=>{{state{ModelName}.{item.Key} = x}});
");
                    fieldinit.Append($@"
    {item.Key}: [] as any[],");

                }
                foreach (var item in multiapis)
                {
                    apiinit.Append(@$"
    other.getSelectList('{item.Value}',[],false).then(x=>{{state{ModelName}.{item.Key} = x}});
");
                    fieldinit.Append($@"
    {item.Key}: [] as any[],");
                }

                return rv.Replace("$formfields$", fieldstr.ToString()).Replace("$fieldinit$", fieldinit.ToString()).Replace("$selectfieldinit$", selectfieldinit.ToString())
                    .Replace("$fieldentityinit$", fieldentityinit.ToString()).Replace("$init$", apiinit.ToString());
            }

            if (name == "indexapi")
            {

                return rv;
            }

            return rv;
        }

        /// <summary>
        /// 从程序集内嵌资源中读取代码生成模板的原始文本
        /// </summary>
        /// <param name="fileName">模板文件名（不含路径）</param>
        /// <param name="subdir">模板所在的子目录名称，为空时表示位于 GeneratorFiles 根目录</param>
        /// <returns>模板文件的文本内容</returns>
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

        /// <summary>
        /// 收集字段及其关联类型所在的命名空间，并将代码文本中的占位符替换为所需的 using 语句
        /// </summary>
        /// <param name="pros">需要处理的字段信息列表</param>
        /// <param name="s">包含命名空间占位符的代码文本</param>
        /// <returns>替换了 using 语句之后的代码文本</returns>
        private string GetRelatedNamespace(List<FieldInfo> pros, string s)
        {
            string otherns = @"";
            Type modelType = Type.GetType(SelectedModel);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.RelatedField))
                {
                    proType = modelType.GetSingleProperty(pro.FieldName)?.PropertyType;
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
                if (s.Contains($"using {prons};") == false && otherns.Contains($"using {prons};") == false)
                {
                    otherns += $@"using {prons};
";
                }

            }

            return s.Replace("$othernamespace$", otherns);
        }

    }

    /// <summary>
    /// 字段关联类型枚举：普通字段、一对多关联、多对多关联
    /// </summary>
    public enum FieldInfoType { Normal, One2Many, Many2Many }

    /// <summary>
    /// 代码生成器中描述模型字段的信息类，记录字段名称、关联关系，
    /// 以及是否用于搜索、列表、表单、导入、批量修改等场景
    /// </summary>
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

        /// <summary>
        /// 获取字段在生成代码中使用的属性名：一对多关联时返回外键属性名，否则返回字段本身的名称
        /// </summary>
        /// <param name="DC">数据上下文，用于查询外键信息</param>
        /// <param name="modelType">字段所属的模型类型</param>
        /// <returns>生成代码中使用的属性名称</returns>
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

        /// <summary>
        /// 获取外键属性的基础类型名称（short、int、long、Guid、string），用于生成代码中的类型声明
        /// </summary>
        /// <param name="DC">数据上下文，用于查询外键信息</param>
        /// <param name="modelType">字段所属的模型类型</param>
        /// <returns>外键类型的名称字符串</returns>
        public string GetFKType(IDataContext DC, Type modelType)
        {
            Type fktype = null;
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = this.GetField(DC, modelType);
                fktype = modelType.GetSingleProperty(fk)?.PropertyType;
            }
            if (this.InfoType == FieldInfoType.Many2Many)
            {
                var middletype = modelType.GetSingleProperty(this.FieldName)?.PropertyType;
                fktype = middletype.GetGenericArguments()[0].GetSingleProperty(this.SubIdField)?.PropertyType;
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
