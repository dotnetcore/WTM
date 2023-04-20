using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class ListVMExtension
    {
        /// <summary>
        /// 获取Jason格式的列表数据
        /// </summary>
        /// <param name="self">是否需要对数据进行Json编码</param>
        /// <param name="returnColumnObject">不在后台进行ColumnFormatInfo的转化，而是直接输出ColumnFormatInfo的json结构到前端，由前端处理，默认False</param>
        /// <param name="enumToString"></param>
        /// <returns>Json格式的数据</returns>
        public static async Task<string> GetDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self, bool returnColumnObject = false, bool enumToString = true) where T : TopBasePoco, new()
        {
            var sb = new StringBuilder();
            self.GetHeaders();
            if (self.IsSearched == false)
            {
                await self.DoSearch();
            }
            var el = (await self.GetEntityList()).ToList();
            //如果列表主键都为0，则生成自增主键，避免主键重复
            if (el.All(x => {
                var id = x.GetID();
                if(id == null || (id is Guid gid && gid == Guid.Empty) || (id is int iid && iid==0) || (id is long lid && lid == 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } ))
            {
                el.ForEach(x => x.ID = Guid.NewGuid());
            }
            //循环生成列表数据
            for (int x = 0; x < el.Count; x++)
            {
                var sou = el[x];
                sb.Append(self.GetSingleDataJson(sou, returnColumnObject, x, enumToString));
                if (x < el.Count - 1)
                {
                    sb.Append(',');
                }
            }
            return $"[{sb}]";
        }

        private static string GetFormatResult(BaseVM vm, ColumnFormatInfo info)
        {
            string rv = "";
            switch (info.FormatType)
            {
                case ColumnFormatTypeEnum.Dialog:
                    rv = vm.UIService.MakeDialogButton(info.ButtonType, info.Url, info.Text, info.Width, info.Height, info.Title, info.ButtonID, info.ShowDialog, info.Resizable, info.Maxed, info.ButtonClass, info.Style).ToString();
                    break;
                case ColumnFormatTypeEnum.Button:
                    rv = vm.UIService.MakeButton(info.ButtonType, info.Url, info.Text, info.Width, info.Height, info.Title, info.ButtonID, info.Resizable, info.Maxed, vm.ViewDivId, info.ButtonClass, info.Style, info.RType).ToString();
                    break;
                case ColumnFormatTypeEnum.Download:
                    if (info.FileID == null)
                    {
                        rv = "";
                    }
                    else
                    {
                        rv = vm.UIService.MakeDownloadButton(info.ButtonType, info.FileID.Value, info.Text, vm.CurrentCS, info.ButtonClass, info.Style).ToString();
                    }
                    break;
                case ColumnFormatTypeEnum.ViewPic:
                    if (info.FileID == null)
                    {
                        rv = "";
                    }
                    else
                    {
                        rv = vm.UIService.MakeViewButton(info.ButtonType, info.FileID.Value, info.Text, info.Width, info.Height, info.Title, info.Resizable, vm.CurrentCS, info.Maxed, info.ButtonClass, info.Style).ToString();
                    }
                    break;
                case ColumnFormatTypeEnum.Script:
                    rv = vm.UIService.MakeScriptButton(info.ButtonType, info.Text, info.Script, info.ButtonID, info.Url, info.ButtonClass, info.Style).ToString();
                    break;
                case ColumnFormatTypeEnum.Html:
                    rv = info.Html;
                    break;
                default:
                    break;
            }
            return rv;
        }

        /// <summary>
        /// 生成单条数据的Json格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="obj">数据</param>
        /// <param name="returnColumnObject">不在后台进行ColumnFormatInfo的转化，而是直接输出ColumnFormatInfo的json结构到前端，由前端处理，默认False</param>
        /// <param name="index">index</param>
        /// <param name="enumToString"></param>
        /// <returns>Json格式的数据</returns>
        public static async Task<string> GetSingleDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self, object obj, bool returnColumnObject, int index = 0, bool enumToString = true) where T : TopBasePoco
        {
            bool inner = false;
            var sb = new StringBuilder();
            var RowBgColor = string.Empty;
            var RowColor = string.Empty;
            if (obj is not T sou)
            {
                sou = self.CreateEmptyEntity();
            }
            RowBgColor = self.SetFullRowBgColor(sou);
            RowColor = self.SetFullRowColor(sou);
            var isSelected = self.GetIsSelected(sou);
            //循环所有列
            sb.Append('{');
            bool containsID = false;
            bool addHiddenID = false;
            Dictionary<string, (string, string)> colorcolumns = new Dictionary<string, (string, string)>();
            foreach (var baseCol in await self.GetHeaders())
            {
                foreach (var col in baseCol.BottomChildren)
                {
                    inner = false;
                    if (col.ColumnType != GridColumnTypeEnum.Normal)
                    {
                        continue;
                    }
                    if (col.FieldName?.ToLower() == "id")
                    {
                        containsID = true;
                    }
                    var backColor = col.GetBackGroundColor(sou);
                    //获取ListVM中设定的单元格前景色
                    var foreColor = col.GetForeGroundColor(sou);

                    if (backColor == string.Empty)
                    {
                        backColor = RowBgColor;
                    }
                    if (foreColor == string.Empty)
                    {
                        foreColor = RowColor;
                    }
                    (string bgcolor, string forecolor) colors = (null, null);
                    if (backColor != string.Empty)
                    {
                        colors.bgcolor = backColor;
                    }
                    if (foreColor != string.Empty)
                    {
                        colors.forecolor = foreColor;
                    }
                    if (string.IsNullOrEmpty(colors.bgcolor) == false || string.IsNullOrEmpty(colors.forecolor) == false)
                    {
                        colorcolumns.Add(col.Field, colors);
                    }
                    //设定列名，如果是主键ID，则列名为id，如果不是主键列，则使用f0，f1,f2...这种方式命名，避免重复
                    var ptype = col.FieldType;
                    if (col.Field?.ToLower() == "children" && typeof(IEnumerable<T>).IsAssignableFrom(ptype))
                    {
                        var children = ((IEnumerable<T>)col.GetObject(obj))?.ToList();
                        if (children == null || children.Count == 0)
                        {
                            continue;
                        }
                    }
                    var html = string.Empty;

                    if (col.EditType == EditTypeEnum.Text || col.EditType == null)
                    {
                        if (typeof(IEnumerable<T>).IsAssignableFrom(ptype))
                        {
                            var children = ((IEnumerable<T>)col.GetObject(obj))?.ToList();
                            if (children != null)
                            {
                                html = "[";
                                for (int i = 0; i < children.Count; i++)
                                {
                                    var item = children[i];
                                    html += self.GetSingleDataJson(item, returnColumnObject,0,enumToString);
                                    if (i < children.Count - 1)
                                    {
                                        html += ",";
                                    }
                                }
                                html += "]";
                            }
                            else
                            {
                                //html = "[]";
                            }
                            inner = true;
                        }
                        else
                        {
                            if (returnColumnObject == true)
                            {
                                html = col.GetText(sou, false).ToString();
                            }
                            else
                            {
                                var info = col.GetText(sou);

                                if (info is ColumnFormatInfo)
                                {
                                    html = GetFormatResult(self as BaseVM, info as ColumnFormatInfo);
                                }
                                else if (info is List<ColumnFormatInfo>)
                                {
                                    var temp = string.Empty;
                                    foreach (var item in info as List<ColumnFormatInfo>)
                                    {
                                        temp += GetFormatResult(self as BaseVM, item);
                                        temp += "&nbsp;&nbsp;";
                                    }
                                    html = temp;
                                }
                                else
                                {
                                    html = info.ToString();
                                }
                            }

                            //如果列是布尔值，直接返回true或false，让前台生成CheckBox
                            if (ptype == typeof(bool) || ptype == typeof(bool?))
                            {
                                if(enumToString == false)
                                {
                                    html = html.ToLower();
                                    inner = true;
                                }
                                else if (returnColumnObject == false)
                                {
                                    if (html.ToLower() == "true")
                                    {
                                        html = (self as BaseVM).UIService.MakeCheckBox(true, isReadOnly: true);
                                    }
                                    if (html.ToLower() == "false" || html == string.Empty)
                                    {
                                        html = (self as BaseVM).UIService.MakeCheckBox(false, isReadOnly: true);
                                    }
                                }
                                else
                                {
                                    if (html != null && html != string.Empty)
                                    {
                                        html = html.ToLower();
                                    }
                                }
                            }
                            //如果列是枚举，直接使用枚举的文本作为多语言的Key查询多语言文字
                            else if (ptype.IsEnumOrNullableEnum())
                            {
                                if (enumToString == true)
                                {
                                    string enumdisplay = PropertyHelper.GetEnumDisplayName(ptype, html);
                                    if (string.IsNullOrEmpty(enumdisplay) == false)
                                    {
                                        html = enumdisplay;
                                    }
                                }
                            }
                            //If this column is a class or list, html will be set to a json string, sest inner to true to remove the "
                            if (returnColumnObject == true && ptype?.Namespace.Equals("System") == false && ptype?.IsEnumOrNullableEnum() == false)
                            {
                                inner = true;
                            }
                        }
                        if (enumToString == false && string.IsNullOrEmpty(html))
                        {
                            continue;
                        }

                    }
                    else
                    {
                        string val = col.GetText(sou).ToString();
                        string name = $"{self.DetailGridPrix}[{index}].{col.Field}";
                        switch (col.EditType)
                        {
                            case EditTypeEnum.TextBox:
                                html = (self as BaseVM).UIService.MakeTextBox(name, val,null,col.IsReadOnly);
                                break;
                            case EditTypeEnum.CheckBox:
                                _ = bool.TryParse(val, out bool nb);
                                html = (self as BaseVM).UIService.MakeCheckBox(nb, null, name, "true",col.IsReadOnly);
                                break;
                            case EditTypeEnum.ComboBox:
                                html = (self as BaseVM).UIService.MakeCombo(name, col.ListItems, val,null,col.IsReadOnly);
                                break;
                            case EditTypeEnum.Datetime:
                                html = (self as BaseVM).UIService.MakeDateTime(name, val,null, col.IsReadOnly,col.DateType);
                                break;
                            default:
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(self.DetailGridPrix) == false && addHiddenID == false)
                    {
                        html += $@"<input hidden name='{self.DetailGridPrix}[{index}].ID' value='{sou.GetID()}'/>";
                        addHiddenID = true;
                    }
                    if (inner == false)
                    {
                        html = "\"" + html.RemoveSpecialChar().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                    }
                    sb.Append($"\"{col.Field}\":");
                    sb.Append(html);
                    sb.Append(',');
                }
            }
            sb.Append($"\"TempIsSelected\":\"{ (isSelected == true ? "1" : "0") }\"");
            foreach (var cc in colorcolumns)
            {
                if (string.IsNullOrEmpty(cc.Value.Item1) == false)
                {
                    string bg = cc.Value.Item1;
                    if (bg.StartsWith("#") == false)
                    {
                        bg = "#" + bg;
                    }
                    sb.Append($",\"{cc.Key}__bgcolor\":\"{bg}\"");
                }
                if (string.IsNullOrEmpty(cc.Value.Item2) == false)
                {
                    string fore = cc.Value.Item2;
                    if (fore.StartsWith("#") == false)
                    {
                        fore = "#" + fore;
                    }
                    sb.Append($",\"{cc.Key}__forecolor\":\"{fore}\"");
                }
            }
            if (containsID == false)
            {
                sb.Append($",\"ID\":\"{(sou as dynamic).ID}\"");
            }
            // 标识当前行数据是否被选中
            sb.Append($@",""LAY_CHECKED"":{sou.Checked.ToString().ToLower()}");
            sb.Append(string.Empty);
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Get json format string of ListVM's search result
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="self">a listvm</param>
        /// <param name="PlainText">true to return plain text, false to return formated html, such as checkbox,buttons ...</param>
        /// <param name="enumToString">use enum display name</param>
        /// <param name="func">other key,value needed to be returned</param>
        /// <returns>json string</returns>
        public static string GetJson<T>(this IBasePagedListVM<T, BaseSearcher> self, bool PlainText = true, bool enumToString = true, Func<Dictionary<string, object>> func = null) where T : TopBasePoco, new()
        {
            if(self.Searcher.IsPlainText != null)
            {
                PlainText = self.Searcher.IsPlainText.Value;
            }
            if (self.Searcher.IsEnumToString != null)
            {
                enumToString = self.Searcher.IsEnumToString.Value;
            }
            if (!self.IsSearched) self.DoSearch();

            StringBuilder builder = new("{", capacity: 1024);
            var dic = func?.Invoke();

            // 如果用户的附加字典不为空，则添加用户自定义的信息
            if (dic != null) foreach (var item in dic) builder.Append($"\"{item.Key}\":\"{item.Value}\",");

            // 设置wtm必要的数据
            builder
                .Append($"\"Code\":200,")
                .Append($"\"Count\":{self.Searcher.Count},")
                .Append($"\"Data\":{self.GetDataJson(PlainText, enumToString)},")
                .Append($"\"Msg\":\"success\",")
                .Append($"\"Page\":{self.Searcher.Page},")
                .Append($"\"PageCount\":{self.Searcher.PageCount}")
                .Append('}');
            return builder.ToString();
        }

        public static object GetJsonForApi<T>(this IBasePagedListVM<T, BaseSearcher> self, bool PlainText = true) where T : TopBasePoco, new()
        {
            return new { Data = self.GetEntityList(), Count = self.Searcher.Count, PageCount = self.Searcher.PageCount, Page = self.Searcher.Page, Msg = "success", Code = 200 };
        }


        public static string GetError<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
        {
            return $@"{{""Data"":{{}},""Count"":0,""Page"":0,""PageCount"":0,""Msg"":""{(self as BaseVM).MSD.GetFirstError()}"",""Code"":400}}";
        }


    }
}
