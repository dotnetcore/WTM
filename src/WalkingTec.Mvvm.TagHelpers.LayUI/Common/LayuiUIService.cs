using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Common
{
    public class LayuiUIService : IUIService
    {
        public string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true)
        {
            if (buttonID == null)
            {
                buttonID = Guid.NewGuid().ToString();
            }
            var innerClick = "";
            string windowid = Guid.NewGuid().ToString();
            if (showDialog == true)
            {
                innerClick = $"ff.OpenDialog('{url}', '{windowid}', '{title ?? ""}',{width?.ToString() ?? "null"}, {height?.ToString() ?? "null"});";
            }
            else
            {
                innerClick = $"$.ajax({{cache: false,type: 'GET',url: '{url}',async: true,success: function(data, textStatus, request) {{eval(data);}} }});";
            }
            var click = $"<script>$('#{buttonID}').on('click',function(){{{innerClick};return false;}});</script>";
            string rv = "";
            if(buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a id='{buttonID}' style='color:blue;cursor:pointer'>{buttonText}</a>";
            }
            if(buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' class='layui-btn layui-btn-primary layui-btn-xs'>{buttonText}</a>";
            }
            rv += click;
            return rv;
        }

        public string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, string _DONOT_USE_CS = "default")
        {
            string rv = "";
            if (buttonType == ButtonTypesEnum.Link)
            {
                rv = $"<a style='color:blue;cursor:pointer' href='/_Framework/GetFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a class='layui-btn layui-btn-primary layui-btn-xs' href='/_Framework/GetFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}'>{buttonText}</a>";
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
            string rv = $"<select name='{name}' id='{(name == null? "": Utils.GetIdByName(name))}' class='layui-input' style='height:28px'   lay-ignore>";
            if(string.IsNullOrEmpty(emptyText) == false)
            {
                rv += $@"
<option value=''>{emptyText}</option>";
            }
            if (value != null)
            {
                foreach (var item in value)
                {
                    if (item.Value == selectedValue)
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
            return $@"<input class='layui-input' style='height:28px'  name='{name ?? ""}' id='{(name == null? "": Utils.GetIdByName(name))}' value='{value ?? ""}' {disable} />";

        }
        public string MakeRedirectButton(ButtonTypesEnum buttonType, string url, string buttonText)
        {
            return "";
        }

        public string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID,  string buttonText = null, int? width = null, int? height = null, string title = null,  bool resizable = true, string _DONOT_USE_CS = "default")
        {
            return MakeDialogButton(buttonType, $"/_Framework/ViewFile/{fileID}?_DONOT_USE_CS={_DONOT_USE_CS}", buttonText, width, height, title, null, true, resizable);
        }

        public string MakeScriptButton(ButtonTypesEnum buttonType, string buttonText, string script = "", string buttonID = null, string url = null)
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
                rv = $"<a id='{buttonID}' style='color:blue;cursor:pointer'>{buttonText}</a>";
            }
            if (buttonType == ButtonTypesEnum.Button)
            {
                rv = $"<a id='{buttonID}' class='layui-btn layui-btn-primary layui-btn-xs'>{buttonText}</a>";
            }
            rv += click;
            return rv;
        }
    }
}
