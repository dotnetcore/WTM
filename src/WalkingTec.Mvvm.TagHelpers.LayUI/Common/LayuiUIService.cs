using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Common
{
    public class LayuiUIService : IUIService
    {
        public string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true, bool max = false, string buttonClass = null,string style=null)
        {
            if (buttonID == null)
            {
                buttonID = Guid.NewGuid().ToString();
            }
            var innerClick = "";
            string windowid = Guid.NewGuid().ToString();
            if (showDialog == true)
            {
                innerClick = $"ff.OpenDialog('{url}','{Guid.NewGuid().ToNoSplitString()}','{title ?? ""}',{width?.ToString() ?? "null"},{height?.ToString() ?? "null"},undefined,{max.ToString().ToLower()});";
            }
            else
            {
                innerClick = $"$.ajax({{cache: false,type: 'GET',url: '{url}',async: true,success: function(data, textStatus, request) {{eval(data);}} }});";
            }
            string funcname = $"x{buttonID.Replace("-", "")}click";
            var click = $"<script>function {funcname}(){{{innerClick};return false;}}</script>";
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}'>{buttonText}</a>";
            }
            rv += click;
            return rv;
        }

        public string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, string _DONOT_USE_CS = "default", string buttonClass = null, string style = null)
        {
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a  style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}' href='/_Framework/GetFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a  style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}' href='/_Framework/GetFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}'>{buttonText}</a>";
            }
            return rv;
        }

        public string MakeCheckBox(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false)
        {
            var disable = isReadOnly ? " disabled='' class='layui-disabled'" : " ";
            var selected = ischeck ? " checked" : " ";
            return $@"<input lay-skin='primary' type='checkbox' name='{name ?? ""}' id='{(name == null ? "" : Utils.GetIdByName(name))}' value='{value ?? ""}' title='{text ?? ""}' {selected} {disable}/>";
        }

        public string MakeRadio(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false)
        {
            var selected = ischeck ? " checked" : " ";
            var disable = isReadOnly ? " disabled='' class='layui-disabled'" : " ";
            return $@"<input lay-skin='primary' type='radio' name='{name ?? ""}' id='{(name == null ? "" : Utils.GetIdByName(name))}' value='{value ?? ""}' title='{text ?? ""}' {selected} {disable}/>";
        }

        public string MakeCombo(string name = null, List<ComboSelectListItem> value = null, string selectedValue = null, string emptyText = null, bool isReadOnly = false)
        {
            var disable = isReadOnly ? " disabled='' class='layui-disabled'" : " ";
            string rv = $"<select name='{name}' id='{(name == null ? "" : Utils.GetIdByName(name))}' class='layui-input' style='height:28px'   lay-ignore>";
            if (string.IsNullOrEmpty(emptyText) == false)
            {
                rv += $@"
<option value=''>{emptyText}</option>";
            }
            if (value != null)
            {
                foreach (var item in value)
                {
                    if (item.Value.ToString().ToLower() == selectedValue?.ToLower())
                    {
                        rv += $@"
<option value='{item.Value}' selected>{item.Text}</option>";

                    }
                    else
                    {
                        rv += $@"
<option value='{item.Value}'>{item.Text}</option>";
                    }
                }
            }
            rv += $@"
</select>
";
            return rv;
        }

        public string MakeTextBox(string name = null, string value = null, string emptyText = null, bool isReadOnly = false)
        {
            var disable = isReadOnly ? " disabled='' class='layui-disabled'" : " ";
            return $@"<input class='layui-input' style='height:28px'  name='{name ?? ""}' id='{(name == null ? "" : Utils.GetIdByName(name))}' value='{value ?? ""}' {disable} />";
        }

        public string MakeDateTime(string name = null, string value = null, string emptyText = null, bool isReadOnly = false)
        {
            var id = (name == null ? "" : Utils.GetIdByName(name));
            var disable = isReadOnly ? " disabled='' class='layui-disabled'" : " ";
            if (string.IsNullOrEmpty(value) == false)
            {
                DateTime p = DateTime.MinValue;
                DateTime.TryParse(value, out p);
                if(p == DateTime.MinValue)
                {
                    value = "";
                }
            }
            return $@"<input class='layui-input' style='height:28px'  name='{name ?? ""}' id='{id}' value='{value ?? ""}' {disable}  onclick='ff.SetGridCellDate(""{id}"")'/>";
        }


        public string MakeButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool resizable = true, bool max = false, string currentdivid = "", string buttonClass = null, string style = null, RedirectTypesEnum rtype= RedirectTypesEnum.Layer)
        {
            if (buttonID == null)
            {
                buttonID = Guid.NewGuid().ToString();
            }
            var innerClick = "";
            string windowid = Guid.NewGuid().ToString();

            switch (rtype)
            {
                case RedirectTypesEnum.Layer:
                    innerClick = $"ff.OpenDialog('{url}','{Guid.NewGuid().ToNoSplitString()}','{title ?? ""}',{width?.ToString() ?? "null"},{height?.ToString() ?? "null"},undefined,{max.ToString().ToLower()});";
                    break;
                case RedirectTypesEnum.Self:
                    innerClick = $"ff.BgRequest('{url}',undefined,'{currentdivid}');";
                    break;
                case RedirectTypesEnum.NewWindow:
                    innerClick = $"ff.LoadPage('{url}',true,'{title ?? ""}');";
                    break;
                case RedirectTypesEnum.NewTab:
                    innerClick = $"ff.LoadPage('{url}',false,'{title ?? ""}');";
                    break;
                default:
                    break;
            }
            string funcname = $"x{buttonID.Replace("-", "")}click";
            var click = $"<script>function {funcname}(){{{innerClick};return false;}}</script>";
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}'>{buttonText}</a>";
            }
            rv += click;
            return rv;
        }

        public string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, int? width = null, int? height = null, string title = null, bool resizable = true, string _DONOT_USE_CS = "default", bool maxed = false, string buttonClass = null, string style = null)
        {
            var  buttonID = Guid.NewGuid().ToString();
            var innerClick = "";
            string windowid = Guid.NewGuid().ToString();
            var url = $"/_Framework/GetFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}";
            innerClick = $"layui.layer.photos({{photos: {{data: [{{src: '{url}'}}]}},anim: 5}});";
            string funcname = $"x{buttonID.Replace("-", "")}click";
            var click = $"<script>function {funcname}(){{{innerClick};return false;}}</script>";
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}'>{buttonText}</a>";
            }
            switch (buttonType)
            {
                case ButtonTypesEnum.Button:
                    rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}'>{buttonText}</a>";
                    break;
                case ButtonTypesEnum.Link:
                    rv = $"<a id='{buttonID}' onclick='{funcname}()' style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'>{buttonText}</a>";
                    break;
                case ButtonTypesEnum.Img:
                    rv = $"<img src='{url}&width={width??50}&height={height??50}' id='{buttonID}' onclick='{funcname}()' style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'/>";
                    break;
                default:
                    break;
            }
            rv += click;
            return rv;
        }

        public string MakeScriptButton(ButtonTypesEnum buttonType, string buttonText, string script = "", string buttonID = null, string url = null, string buttonClass = null, string style=null)
        {
            if (buttonID == null)
            {
                buttonID = Guid.NewGuid().ToString();
            }
            var innerClick = script;
            var click = $"<script>$('#{buttonID}').on('click',function(){{{innerClick};return false;}});</script>";
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a id='{buttonID}'  style='{style ?? "color:blue;cursor:pointer"}' class='{buttonClass ?? ""}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' style='{style ?? ""}' class='layui-btn {(string.IsNullOrEmpty(buttonClass) ? "layui-btn-primary layui-btn-xs" : $"{buttonClass}")}'>{buttonText}</a>";
            }
            rv += click;
            return rv;
        }
    }
}
