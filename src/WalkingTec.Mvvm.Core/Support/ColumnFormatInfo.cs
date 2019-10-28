using System;

namespace WalkingTec.Mvvm.Core
{

    public class ColumnFormatInfo
    {
        public ColumnFormatTypeEnum FormatType { get; set; }

        public ButtonTypesEnum ButtonType { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }

        public string Script { get; set; }

        public string WindowID { get; set; }

        public string Url { get; set; }

        public bool ShowDialog { get; set; }

        public bool Resizable { get; set; }

        public string ButtonID { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public Guid? FileID { get; set; }

        public string Html { get; set; }

        public bool Maxed { get; set; }
        public static ColumnFormatInfo MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height,  string title = null, string buttonID = null, bool showDialog = true, bool resizable = true, bool maxed = false)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Dialog;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Width = width;
            rv.Height = height;
            rv.Text = buttonText;
            rv.Title = title;
            rv.ButtonID = buttonID;
            rv.ShowDialog = showDialog;
            rv.Resizable = resizable;
            rv.Maxed = maxed;
            return rv;
        }

        public static ColumnFormatInfo MakeScriptButton(ButtonTypesEnum buttonType, string url, string buttonText, string buttonID = null, string script = "")
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Script;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Text = buttonText;
            rv.ButtonID = buttonID;
            rv.Script = script;
            return rv;
        }

        public static ColumnFormatInfo MakeRedirectButton(ButtonTypesEnum buttonType, string url, string buttonText)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Redirect;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Text = buttonText;
            return rv;
        }

        public static ColumnFormatInfo MakeDownloadButton(ButtonTypesEnum buttonType, Guid? fileID, string buttonText = null)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Download;
            rv.ButtonType = buttonType;
            rv.FileID = fileID;
            rv.Text = buttonText?? Program._localizer["Download"];
            return rv;
        }

        public static ColumnFormatInfo MakeViewButton(ButtonTypesEnum buttonType, Guid? fileID, int? width = null, int? height = null, string title = null, string windowID = null, string buttonText = null, bool resizable = true, bool maxed = false)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.ViewPic;
            rv.ButtonType = buttonType;
            rv.FileID = fileID;
            rv.Width = width;
            rv.Height = height;
            rv.WindowID = windowID;
            rv.Text = buttonText ?? Program._localizer["Preview"];
            rv.Title = title ?? Program._localizer["Preview"];
            rv.Resizable = resizable;
            rv.Maxed = maxed;
            return rv;
        }

        public static ColumnFormatInfo MakeHtml(string html)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Html;
            rv.Html = html;
            if (string.IsNullOrEmpty(rv.Html) == false)
            {
                rv.Html += "<script></script>";
            }
            return rv;
        }
    }
}
