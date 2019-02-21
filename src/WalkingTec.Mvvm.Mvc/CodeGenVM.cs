using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    [ReInit(ReInitModes.ALWAYS)]
    public class CodeGenVM : BaseVM
    {
        public CodeGenListVM FieldList { get; set; }

        public List<FieldInfo> FieldInfos { get; set; }

        public string PreviewFile { get; set; }

        public UIEnum UI { get; set; }

        public string ModelName
        {
            get
            {
                return SelectedModel?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            }
        }
        [Display(Name = "Model命名空间")]
        [ValidateNever()]
        public string ModelNS => SelectedModel?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSpratedString(seperator: ".");
        [Display(Name = "模块名称")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string ModuleName { get; set; }
        [RegularExpression("^[A-Za-z_]+", ErrorMessage = "{0}只能以英文字母或下划线开头")]
        public string Area { get; set; }
        [ValidateNever()]
        [BindNever()]
        public List<ComboSelectListItem> AllModels { get; set; }
        [Required(ErrorMessage = "{0}是必填项")]
        [Display(Name = "选择模型")]
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
                            vmdir = Directory.CreateDirectory(MainDir + $"\\ViewModels\\{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\ViewModels\\{ModelName}VMs");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"\\{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"\\{Area}\\{ModelName}VMs");
                        }

                    }
                    _vmdir = vmdir.FullName;
                }
                return _vmdir;
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
                        _controllerdir = Directory.CreateDirectory(MainDir + "\\Controllers").FullName;
                    }
                    else
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\Controllers").FullName;
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
                        _viewdir = Directory.CreateDirectory(MainDir + $"\\Views\\{ModelName}").FullName;
                    }
                    else
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\Views\\{ModelName}").FullName;
                    }
                }
                return _viewdir;
            }
        }


        private string _controllerNs;
        [Display(Name = "Controller命名空间")]
        [ValidateNever()]
        public string ControllerNs
        {
            get
            {
                if (_controllerNs == null)
                {
                    int index = MainDir.LastIndexOf("\\");
                    if (index > 0)
                    {
                        _controllerNs = MainDir.Substring(index + 1);
                    }
                    else
                    {
                        _controllerNs = MainDir;
                    }
                    _controllerNs += ".Controllers";
                }
                return _controllerNs;
            }
            set
            {
                _controllerNs = value;
            }
        }

        private string _vmNs;
        [Display(Name = "VM命名空间")]
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
                            _vmNs = ControllerNs.Replace(".Controllers", $".ViewModels.{ModelName}VMs");
                        }
                        else
                        {
                            _vmNs = ControllerNs.Replace(".Controllers", $".{Area}.ViewModels.{ModelName}VMs");
                        }
                    }
                    else
                    {
                        int index = vmdir.FullName.LastIndexOf("\\");
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
            FieldList = new CodeGenListVM();
            FieldList.CopyContext(this);
        }
        public void DoGen()
        {
            File.WriteAllText($"{ControllerDir}\\{ModelName}Controller.cs", GenerateController(), Encoding.UTF8);

            File.WriteAllText($"{VmDir}\\{ModelName}VM.cs", GenerateVM("CrudVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}ImportVM.cs", GenerateVM("ImportVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);

            if (UI == UIEnum.LayUI)
            {
                File.WriteAllText($"{ViewDir}\\Index.cshtml", GenerateView("ListView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\Create.cshtml", GenerateView("CreateView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\Edit.cshtml", GenerateView("EditView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\Delete.cshtml", GenerateView("DeleteView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\Details.cshtml", GenerateView("DetailsView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\Import.cshtml", GenerateView("ImportView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\BatchEdit.cshtml", GenerateView("BatchEditView"), Encoding.UTF8);
                File.WriteAllText($"{ViewDir}\\BatchDelete.cshtml", GenerateView("BatchDeleteView"), Encoding.UTF8);
            }
            if (UI == UIEnum.React)
            {
                if (Directory.Exists($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}") == false)
                {
                    Directory.CreateDirectory($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}");
                }
                if (Directory.Exists($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views") == false)
                {
                    Directory.CreateDirectory($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views");
                }
                if (Directory.Exists($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\store") == false)
                {
                    Directory.CreateDirectory($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\store");
                }
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\action.tsx", GenerateReactView("action"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\details.tsx", GenerateReactView("details"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\models.tsx", GenerateReactView("models"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\other.tsx", GenerateReactView("other"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\search.tsx", GenerateReactView("search"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\views\\table.tsx", GenerateReactView("table"), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\store\\index.ts", GetResource("index.txt", "Spa.React.store").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\index.tsx", GetResource("index.txt", "Spa.React").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);
                File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\{ModelName.ToLower()}\\style.less", GetResource("style.txt", "Spa.React").Replace("$modelname$", ModelName.ToLower()), Encoding.UTF8);

                var index = File.ReadAllText($"{MainDir}\\ClientApp\\src\\pages\\index.ts");
                if (index.Contains($"path: '/{ModelName.ToLower()}'") == false)
                {
                    index = index.Replace("/**WTM**/", $@"
, {ModelName.ToLower()}: {{
        name: '{ModuleName.ToLower()}',
        path: '/{ModelName.ToLower()}',
        component: () => import('./{ModelName.ToLower()}').then(x => x.default) 
    }}
/**WTM**/
 ");
                    File.WriteAllText($"{MainDir}\\ClientApp\\src\\pages\\index.ts", index, Encoding.UTF8);
                }

                var menu = File.ReadAllText($"{MainDir}\\ClientApp\\src\\subMenu.json");
                if (menu.Contains($@"""Path"": ""/{ModelName.ToLower()}""") == false)
                {
                    var i = menu.LastIndexOf("}");
                    menu = menu.Insert(i + 1, $@"
,{{
        ""Key"": ""{Guid.NewGuid().ToString()}"",
        ""Name"": ""{ModuleName.ToLower()}"",
        ""Icon"": ""menu-fold"",
        ""Path"": ""/{ModelName.ToLower()}"",
        ""Component"": ""{ModelName.ToLower()}"",
        ""Action"": [],
        ""Children"": []
    }}
");
                    File.WriteAllText($"{MainDir}\\ClientApp\\src\\subMenu.json", menu, Encoding.UTF8);

                }
            }
        }

        public string GenerateController()
        {
            string dir = "";
            if (UI == UIEnum.LayUI)
            {
                dir = "Mvc";
            }
            if (UI == UIEnum.React)
            {
                dir = "Spa";
            }
            var rv = GetResource("Controller.txt", dir).Replace("$vmnamespace$", VMNs).Replace("$namespace$", ControllerNs).Replace("$des$", ModuleName).Replace("$modelname$", ModelName).Replace("$modelnamespace$", ModelNS);
            if (string.IsNullOrEmpty(Area))
            {
                rv = rv.Replace("$area$", "");
            }
            else
            {
                rv = rv.Replace("$area$", $"[Area(\"{Area}\")]");
            }
            return rv;
        }

        public string GenerateVM(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$modelnamespace$", ModelNS).Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName).Replace("$area$", $"{Area ?? ""}");
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
                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        var fname = "All" + pro.FieldName + "s";
                        prostring += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                        initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    string typename = proType.PropertyType.Name;
                    if (proType.PropertyType.IsNullable())
                    {
                        typename = proType.PropertyType.GetGenericArguments()[0].Name + "?";
                    }
                    prostring += $@"
        public {typename} {proType.Name} {{ get; set; }}";
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
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                foreach (var pro in pros)
                {
                    if (string.IsNullOrEmpty(pro.RelatedField))
                    {
                        headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}),";
                        selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
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
                            formatstring += GetResource("HeaderFormat.txt").Replace("$modelname$", ModelName).Replace("$field$", filefk);
                        }
                        else
                        {
                            var subpro = subtype.GetProperty(pro.SubField);
                            string subtypename = subpro.PropertyType.Name;
                            if (subpro.PropertyType.IsNullable())
                            {
                                subtypename = subpro.PropertyType.GetGenericArguments()[0].Name + "?";
                            }

                            var subdisplay = subpro.GetCustomAttribute<DisplayAttribute>();
                            headerstring += $@"
                this.MakeGridHeader(x => x.{pro.SubField + "_view"}),";
                            if (string.IsNullOrEmpty(pro.SubIdField) == true)
                            {
                                selectstring += $@"
                    {pro.SubField + "_view"} = x.{pro.FieldName}.{pro.SubField},";
                            }
                            else
                            {
                                selectstring += $@"
                    {pro.SubField + "_view"} = DC.Set<{subtype.Name}>().Where(y => x.{pro.FieldName}.Select(z => z.{pro.SubIdField}).Contains(y.ID)).Select(y => y.{pro.SubField}).ToSpratedString(null,"",""),";
                            }
                            if (subdisplay?.Name != null)
                            {
                                subprostring += $@"
        [Display(Name = ""{subdisplay.Name}"")]";
                                subprostring += $@"
        public {subtypename} {pro.SubField + "_view"} {{ get; set; }}";
                            }
                        }
                    }

                }
                var wherepros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                foreach (var pro in wherepros)
                {
                    var proType = modelType.GetProperty(pro.FieldName).PropertyType;
                    if (proType == typeof(string))
                    {
                        wherestring += $@"
                .CheckContain(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                    }
                    else
                    {
                        wherestring += $@"
                .CheckEqual(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";

                    }
                }
                rv = rv.Replace("$headers$", headerstring).Replace("$where$", wherestring).Replace("$select$", selectstring).Replace("$subpros$", subprostring).Replace("$format$", formatstring);
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
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName});";

                    if(string.IsNullOrEmpty(pro.SubIdField) == false)
                    {
                        Type modelType = Type.GetType(SelectedModel);
                        var protype = modelType.GetProperty(pro.FieldName);
                        prostr += $@"
        [Display(Name = ""{protype.GetPropertyDisplayName()}"")]
        public List<Guid> Selected{pro.FieldName}IDs {{ get; set; }}";
                        initstr += $@"
            Selected{pro.FieldName}IDs = Entity.{pro.FieldName}.Select(x => x.{pro.SubIdField}).ToList();";
                        addstr += $@"
            if (Selected{pro.FieldName}IDs != null)
            {{
                foreach (var id in Selected{pro.FieldName}IDs)
                {{
                    Entity.{pro.FieldName}.Add(new {protype.Name} {{ {pro.SubIdField} = id }});
                }}
            }}
";
                        editstr += $@"
            if(Selected{pro.FieldName}IDs == null || Selected{pro.FieldName}IDs.Count == 0)
            {{
                FC.Add(""Entity.Selected{pro.FieldName}IDs.DONOTUSECLEAR"", ""true"");
            }}
            else
            {{
                Entity.{pro.FieldName} = new List<{protype.Name}>();
                Selected{pro.FieldName}IDs.ForEach(x => Entity.{pro.FieldName}.Add(new {protype.Name} {{ ID = Guid.NewGuid(), {pro.SubIdField} = x }}));
            }}
";
                    }
                }
                if (UI == UIEnum.LayUI)
                {
                    rv = rv.Replace("$pros$", prostr).Replace("$init$", initstr).Replace("$include$", includestr).Replace("$add$", addstr).Replace("$edit$", editstr);
                }
                if(UI == UIEnum.React)
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
                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        initstr += $@"
            {pro.FieldName + "_Excel"}.DataType = ColumnDataType.ComboBox;
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{pro.FieldName});";
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
                            if (item.SubField == "`file")
                            {
                                var filefk = DC.GetFKName2(modelType, item.FieldName);
                                fieldstr.Append($@"<wt:upload field=""{pre}.{filefk}"" />");
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
                                    fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" items=""{fname}""/>");
                                }
                                else
                                {
                                    fieldstr.Append($@"<wt:checkbox field=""Selected{item.FieldName}IDs"" items=""{fname}""/>");
                                }
                            }
                        }
                        else
                        {
                            var proType = modelType.GetProperty(item.FieldName).PropertyType;
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if (checktype == typeof(bool) || checktype.IsEnum())
                            {
                                fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype.IsPrimitive || checktype == typeof(string))
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
                        var fname = "All" + item.FieldName + "s";
                        fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" items=""Searcher.{fname}"" empty-text=""全部"" />");
                    }
                    else
                    {
                        var proType = modelType.GetProperty(item.FieldName).PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype.IsPrimitive || checktype == typeof(string))
                        {
                            fieldstr.Append($@"<wt:textbox field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype == typeof(DateTime))
                        {
                            fieldstr.Append($@"<wt:datetime field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype.IsEnum())
                        {
                            fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" empty-text=""全部"" />");
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append($@"</wt:row>");
                rv = rv.Replace("$fields$", fieldstr.ToString());
            }
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
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    string label = modelType.GetProperty(item.FieldName).GetPropertyDisplayName();
                    string render = "columnsRender";
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (subtype == typeof(FileAttachment))
                        {
                            render = "columnsRenderImg";
                        }
                    }
                    fieldstr.Append($@"
    {{
        dataIndex: ""{item.FieldName}"",
        title: ""{label}"",
        render: {render} 
    }}");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(",");
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                return rv.Replace("$columns$", fieldstr.ToString());
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
                    var property = modelType.GetProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    bool isrequired = property.IsPropertyRequired();
                    string rules = "rules: []";
                    if (isrequired == true)
                    {
                        rules = $@"rules: [{{ ""required"": true, ""message"": ""{label}不能为空"" }}]";
                    }
                    fieldstr.AppendLine($@"            {item.FieldName}:{{");
                    fieldstr.AppendLine($@"                label: ""{label}"",");
                    fieldstr.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            fieldstr.AppendLine($@"                formItem: <UploadImg />");
                        }
                        else
                        {

                            fieldstr.AppendLine($@"                formItem: <Select placeholder=""{label}"" showArrow allowClear></Select>");
                        }
                    }
                    else
                    {
                        var proType = modelType.GetProperty(item.FieldName).PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool) || checktype.IsEnum())
                        {
                            fieldstr.AppendLine($@"                formItem: <Switch checkedChildren={{<Icon type=""check"" />}} unCheckedChildren={{<Icon type=""close"" />}} />");
                        }
                        else if (checktype.IsPrimitive || checktype == typeof(string))
                        {
                            fieldstr.AppendLine($@"                formItem: <Input placeholder=""请输入 {label}"" />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr.AppendLine($@"                formItem: <Input placeholder=""请输入 {label}"" />");
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
                    var property = modelType.GetProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    string rules = "rules: []";

                    fieldstr2.AppendLine($@"            {item.FieldName}:{{");
                    fieldstr2.AppendLine($@"                label: ""{label}"",");
                    fieldstr2.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        fieldstr2.AppendLine($@"                formItem: <Select placeholder=""全部"" showArrow allowClear></Select>");
                    }
                    else
                    {
                        var proType = modelType.GetProperty(item.FieldName).PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool) || checktype.IsEnum())
                        {
                            fieldstr2.AppendLine($@"                formItem: <Switch checkedChildren={{<Icon type=""check"" />}} unCheckedChildren={{<Icon type=""close"" />}} />");
                        }
                        else if (checktype.IsPrimitive || checktype == typeof(string))
                        {
                            fieldstr2.AppendLine($@"                formItem: <Input placeholder="""" />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr2.AppendLine($@"                formItem: <Input placeholder="""" />");
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
            if (name == "details")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""{item.FieldName}"" />");
                }
                return rv.Replace("$fields$", fieldstr.ToString());
            }

            return rv;
        }

        private string GetResource(string fileName, string subdir = "")
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
                    proType = modelType.GetProperty(pro.FieldName).PropertyType;
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

    public class FieldInfo
    {
        public string FieldName { get; set; }
        public string RelatedField { get; set; }

        public bool IsSearcherField { get; set; }

        public bool IsListField { get; set; }

        public bool IsFormField { get; set; }

        public bool IsImportField { get; set; }
        public bool IsBatchField { get; set; }

        public string SubField { get; set; }
        public string SubIdField { get; set; }

    }
}
