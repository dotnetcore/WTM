using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class ListVMExtension
    {
        /// <summary>
        /// 获取Jason格式的列表数据
        /// </summary>
        /// <param name="self">是否需要对数据进行Json编码</param>
        /// <param name="returnColumnObject">不在后台进行ColumnFormatInfo的转化，而是直接输出ColumnFormatInfo的json结构到前端，由前端处理，默认False</param>
        /// <returns>Json格式的数据</returns>
        public static string GetDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self, bool returnColumnObject = false) where T : TopBasePoco, new()
        {
            var sb = new StringBuilder();
            self.GetHeaders();
            if (self.IsSearched == false)
            {
                self.DoSearch();
            }
            var el = self.GetEntityList().ToList();
            //如果列表主键都为0，则生成自增主键，避免主键重复
            if (el.All(x => x.ID == Guid.Empty))
            {
                el.ForEach(x => x.ID = Guid.NewGuid());
            }
            //循环生成列表数据
            for (int x = 0; x < el.Count; x++)
            {
                var sou = el[x];
                sb.Append(self.GetSingleDataJson(sou, returnColumnObject, x));
                if (x < el.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return $"[{sb.ToString()}]";
        }

        private static string GetFormatResult(BaseVM vm, ColumnFormatInfo info)
        {
            string rv = "";
            switch (info.FormatType)
            {
                case ColumnFormatTypeEnum.Dialog:
                    rv = vm.UIService.MakeDialogButton(info.ButtonType, info.Url, info.Text, info.Width, info.Height, info.Title, info.ButtonID, info.ShowDialog, info.Resizable).ToString();
                    break;
                case ColumnFormatTypeEnum.Redirect:
                    rv = vm.UIService.MakeRedirectButton(info.ButtonType, info.Url, info.Text).ToString();
                    break;
                case ColumnFormatTypeEnum.Download:
                    if (info.FileID == null)
                    {
                        rv = "";
                    }
                    else
                    {
                        rv = vm.UIService.MakeDownloadButton(info.ButtonType, info.FileID.Value, info.Text).ToString();
                    }
                    break;
                case ColumnFormatTypeEnum.ViewPic:
                    if (info.FileID == null)
                    {
                        rv = "";
                    }
                    else
                    {
                        rv = vm.UIService.MakeViewButton(info.ButtonType, info.FileID.Value, info.Text, info.Width, info.Height, info.Title, info.Resizable).ToString();
                    }
                    break;
                case ColumnFormatTypeEnum.Script:
                    rv = vm.UIService.MakeScriptButton(info.ButtonType, info.Text, info.Script, info.ButtonID, info.Url).ToString();
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
        /// <returns>Json格式的数据</returns>
        public static string GetSingleDataJson<T>(this IBasePagedListVM<T, BaseSearcher> self, object obj, bool returnColumnObject, int index = 0) where T : TopBasePoco
        {
            bool inner = false;
            var sb = new StringBuilder();
            var RowBgColor = string.Empty;
            var RowColor = string.Empty;
            if (!(obj is T sou))
            {
                sou = self.CreateEmptyEntity();
            }
            RowBgColor = self.SetFullRowBgColor(sou);
            RowColor = self.SetFullRowColor(sou);
            var isSelected = self.GetIsSelected(sou);
            //循环所有列
            sb.Append("{");
            bool containsID = false;
            bool addHiddenID = false;
            Dictionary<string, (string, string)> colorcolumns = new Dictionary<string, (string, string)>();
            foreach (var baseCol in self.GetHeaders())
            {
                foreach (var col in baseCol.BottomChildren)
                {
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
                    (string bgcolor, string forecolor) colors = (null,null);
                    if (backColor != string.Empty)
                    {
                        colors.bgcolor = backColor;
                    }
                    if (foreColor != string.Empty)
                    {
                        colors.forecolor = foreColor;
                    }
                    if( string.IsNullOrEmpty(colors.bgcolor) == false || string.IsNullOrEmpty(colors.forecolor) == false)
                    {
                        colorcolumns.Add(col.Field, colors);
                    }
                    //设定列名，如果是主键ID，则列名为id，如果不是主键列，则使用f0，f1,f2...这种方式命名，避免重复
                    var ptype = col.FieldType;
                    if (col.Field?.ToLower() == "children" && typeof(IEnumerable<T>).IsAssignableFrom(ptype))
                    {
                        var children = ((IEnumerable<T>)col.GetObject(obj))?.ToList();
                        if (children == null || children.Count() == 0)
                        {
                            continue;
                        }
                    }
                    sb.Append($"\"{col.Field}\":");
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
                                    html += self.GetSingleDataJson(item, returnColumnObject);
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
                                if (returnColumnObject == false)
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
                                if (int.TryParse(html, out int enumvalue))
                                {
                                    html = PropertyHelper.GetEnumDisplayName(ptype, enumvalue);
                                }
                            }
                        }
                    }
                    else
                    {
                        string val = col.GetText(sou).ToString();
                        string name = $"{self.DetailGridPrix}[{index}].{col.Field}";
                        switch (col.EditType)
                        {
                            case EditTypeEnum.TextBox:
                                html = (self as BaseVM).UIService.MakeTextBox(name, val);
                                break;
                            case EditTypeEnum.CheckBox:
                                bool.TryParse(val, out bool nb);
                                html = (self as BaseVM).UIService.MakeCheckBox(nb, null, name, "true");
                                break;
                            case EditTypeEnum.ComboBox:
                                html = (self as BaseVM).UIService.MakeCombo(name, col.ListItems, val);
                                break;
                            default:
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(self.DetailGridPrix) == false && addHiddenID == false)
                    {
                        html += $@"<input hidden name='{self.DetailGridPrix}[{index}].ID' value='{sou.ID}'/>";
                        addHiddenID = true;
                    }
                    if (inner == false)
                    {
                        html = "\"" + html.Replace(Environment.NewLine, "").Replace("\t", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                    }
                    sb.Append(html);
                    sb.Append(",");
                }
            }
            sb.Append($"\"TempIsSelected\":\"{ (isSelected == true ? "1" : "0") }\"");
            foreach (var cc in colorcolumns)
            {
                if(string.IsNullOrEmpty(cc.Value.Item1) == false)
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
                sb.Append($",\"ID\":\"{sou.ID}\"");
            }
            // 标识当前行数据是否被选中
            sb.Append($@",""LAY_CHECKED"":{sou.Checked.ToString().ToLower()}");
            sb.Append(string.Empty);
            sb.Append("}");
            return sb.ToString();
        }

        public static string GetJson<T>(this IBasePagedListVM<T, BaseSearcher> self) where T : TopBasePoco, new()
        {
            return $@"{{""Data"":{self.GetDataJson(true)},""Count"":{self.Searcher.Count},""Page"":{self.Searcher.Page},""PageCount"":{self.Searcher.PageCount}}}";
        }
    }
}
