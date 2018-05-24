using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    public interface IUIService
    {
        string MakeRedirectButton(ButtonTypesEnum buttonType, string url, string buttonText);

        string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true);

        string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null);

        string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, int? width = null, int? height = null, string title = null, bool resizable = true);

        string MakeScriptButton(ButtonTypesEnum buttonType, string url, int? width, int? height, string windowID, string buttonText, string title = null, string buttonID = null, string script = "");

        string MakeCheckBox(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        string MakeRadio(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        string MakeCombo(string name = null, List<ComboSelectListItem> value = null, string selectedValue = null, string emptyText = null, bool isReadOnly = false);
    }
}
