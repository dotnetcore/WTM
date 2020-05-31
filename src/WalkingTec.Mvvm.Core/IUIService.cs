using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public interface IUIService
    {
        string MakeButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null,bool resizable = true, bool max = false, string currentdivid = "", string buttonClass = null, string style = null, RedirectTypesEnum rtype = RedirectTypesEnum.Layer);
        string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true, bool max = false,string buttonClass = null, string style = null);

        string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, string _DONOT_USE_CS = "default", string buttonClass = null, string style = null);

        string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, int? width = null, int? height = null, string title = null, bool resizable = true, string _DONOT_USE_CS = "default", bool maxed = false, string buttonClass = null, string style = null);

        string MakeScriptButton(ButtonTypesEnum buttonType, string buttonText, string script = "", string buttonID = null, string url = null, string buttonClass = null, string style = null);

        string MakeCheckBox(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        string MakeRadio(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        string MakeCombo(string name = null, List<ComboSelectListItem> value = null, string selectedValue = null, string emptyText = null, bool isReadOnly = false);

        string MakeTextBox(string name = null, string value = null, string emptyText = null, bool isReadOnly = false);

    }
}
