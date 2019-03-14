using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Implement
{
    public class DefaultUIService : IUIService
    {
        public string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true)
        {
            return "";
        }

        public string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, string _DONOT_USE_CS = "default")
        {
            return "";
        }

        public string MakeCheckBox(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false)
        {
            return "";
        }

        public string MakeRedirectButton(ButtonTypesEnum buttonType, string url, string buttonText)
        {
            return "";
        }

        public string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, int? width = null, int? height = null, string title = null, bool resizable = true, string _DONOT_USE_CS = "default")
        {
            return "";
        }

        public string MakeScriptButton(ButtonTypesEnum buttonType, string buttonText, string script = "", string buttonID = null, string url = null)
        {
            return "";
        }

        public string MakeRadio(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false)
        {
            return "";
        }

        public string MakeCombo(string name = null, List<ComboSelectListItem> value = null, string selectedValue = null, string emptyText = null, bool isReadOnly = false)
        {
            return "";
        }

        public string MakeTextBox(string name = null, string value = null, string emptyText = null, bool isReadOnly = false)
        {
            return "";
        }
    }
}
