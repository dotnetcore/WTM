using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using System.Linq;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    /// <summary>
    /// layui多文件上传
    /// </summary>
    [HtmlTargetElement("wt:multiupload", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class MultiUploadTagHelper : BaseFieldTag
    {
        /// <summary>
        /// 限定上传文件大小，单位K
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// 上传文件类别
        /// </summary>
        public UploadTypeEnum UploadType { get; set; }

        /// <summary>
        /// 当上传文件类别为ImageFile时，指定缩小的宽度，框架会使用缩小后的图片保存
        /// </summary>
        public int? ThumbWidth { get; set; }

        /// <summary>
        /// 当上传文件类别为ImageFile时，指定缩小的高度，框架会使用缩小后的图片保存
        /// </summary>
        public int? ThumbHeight { get; set; }

        /// <summary>
        /// 是否使用缩略图预览，当上传文件类别为ImageFile时，默认为true
        /// </summary>
        public bool? ShowPreview { get; set; }

        /// <summary>
        /// 如果使用缩略图预览，指定缩略图宽度，默认64
        /// </summary>
        public int? PreviewWidth { get; set; }

        /// <summary>
        /// 如果使用缩略图预览，指定缩略图高度，默认64
        /// </summary>
        public int? PreviewHeight { get; set; }

        public string CustomType { get; set; }
        /// <summary>
        /// 同时上传的文件数（0不限制）
        /// </summary>
        public int NumFileOnce { get; set; }
        public string ConnectionString { get; set; }
        public string ExtraQuery { get; set; }
        public string UploadGroupName { get; set; }
        public string UploadSubdir { get; set; }
        public string UploadMode { get; set;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string idstring = "";
            if (Field.Model is IEnumerable<ISubFile> subfiles)
            {
                idstring = subfiles.Select(x => x.FileId.ToString()).ToSepratedString(seperator: "|");
            }
            //else
            //{
            //    output.TagName = "div";
            //    output.TagMode = TagMode.StartTagAndEndTag;
            //    output.Content.SetContent("Field must be set to a List<ISubFile>");
            //    return;
            //}
            output.TagName = "button";
            output.Attributes.Add("id", Id + "button");
            output.Attributes.Add("name", Id + "button");
            output.Attributes.Add("class", "layui-btn layui-btn-sm");
            output.Attributes.Add("type", "button");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(WalkingTec.Mvvm.TagHelpers.LayUI.THProgram._localizer["Sys.Select"]);
            if (Disabled == true)
            {
                output.Attributes.Add("style", "display:none");
            }
            string ext = "";
            if (string.IsNullOrEmpty(CustomType))
            {
                switch (UploadType)
                {
                    case UploadTypeEnum.AllFiles:
                        ext = "";
                        break;
                    case UploadTypeEnum.ImageFile:
                        ext = "jpg|jpeg|gif|bmp|png|tif";
                        break;
                    case UploadTypeEnum.ZipFile:
                        ext = "zip";
                        break;
                    case UploadTypeEnum.ExcelFile:
                        ext = "xls|xlsx";
                        break;
                    case UploadTypeEnum.PDFFile:
                        ext = "pdf";
                        break;
                    case UploadTypeEnum.WordFile:
                        ext = "doc|docx";
                        break;
                    case UploadTypeEnum.TextFile:
                        ext = "txt";
                        break;
                }
            }
            else
            {
                ext = CustomType;
            }
            var vm = context.Items["model"] as BaseVM;
            var url = "/_Framework/Upload?1=1";
            if (UploadType == UploadTypeEnum.ImageFile)
            {
                if (ShowPreview == null)
                {
                    ShowPreview = true;
                }
                url = "/_Framework/UploadImage?1=1";
                if (ThumbWidth != null)
                {
                    url += "&width=" + ThumbWidth;
                }
                if (ThumbHeight != null)
                {
                    url += "&height=" + ThumbHeight;
                }
            }
            if (string.IsNullOrEmpty(ConnectionString) == true)
            {
                if (vm != null)
                {
                    url = url.AppendQuery($"_DONOT_USE_CS={vm.CurrentCS}");
                }
            }
            else
            {
                url = url.AppendQuery($"_DONOT_USE_CS={ConnectionString}");
            }
            if (string.IsNullOrEmpty(UploadGroupName) == false)
            {
                url = url.AppendQuery($"groupName={UploadGroupName}");
            }
            if (string.IsNullOrEmpty(UploadSubdir) == false)
            {
                url = url.AppendQuery($"subdir={UploadSubdir}");
            }
            if (string.IsNullOrEmpty(UploadMode) == false)
            {
                url = url.AppendQuery($"sm={UploadMode}");
            }
            url = url.AppendQuery(ExtraQuery);
            output.PreElement.SetHtmlContent($@"
<div id='{Id}label'></div>
");
            string initselected = "[";
            foreach (var fileId in idstring.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                initselected += $"\"{fileId}\",";
            }
            if (initselected.EndsWith(","))
            {
                initselected = initselected.Substring(0, initselected.Length - 1);
            }
            initselected += "]";
            var requiredtext = "";
            if (Field.Metadata.IsRequired)
            {
                requiredtext = $" lay-verify=\"required\" lay-reqText=\"{THProgram._localizer["Validate.{0}required", Field?.Metadata?.DisplayName ?? Field?.Metadata?.Name]}\"";
            }
            output.PostElement.SetHtmlContent($@"
<input type='hidden' id='{Id}'  {requiredtext} />
<script>
  var {Id}selected = {initselected};
{Id}SetValues();
  function {Id}SetValues(){{
    var f = $('#{Id}').parents('form');
    f.find(""[{Id}hidden='{Id}']"").remove();
    var count = 0;
    for(count=0; count< {Id}selected.length;count++){{
        var name = ""{Field.Name}["" + count + ""].FileId"";
        f.append($('<input {Id}hidden=""{Id}"" type=""hidden"" name=""' + name + '"" value=""' + {Id}selected[count] + '"">'));
    }}
    if(count > 0){{
        f.find(""#{Id}"").val(""1"");
    }}
    else{{
        f.find(""#{Id}"").val("""");
    }}
  }}
  function {Id}DoDelete(fileid){{
    $.ajax({{
            type: 'get',
            url: '/api/_file/DeletedFile/' + fileid,
            success: function () {{
                $('#label'+fileid).remove();
                {Id}selected = {Id}selected.filter(function(item) {{
                    return item != fileid;
                 }});
                {Id}SetValues();
            }},
            error: function () {{
                console.log('failed');
            }}
        }});
}}
function {Id}DoPreview(){{
		layui.layer.photos({{
			photos: '#{Id}label',
			anim: 5
		}});
}}
  var index = 0;
  var {Id}preview;

layui.use(['upload'],function(){{
  /* 普通图片上传 */
  var uploadInst = layui.upload.render({{
    elem: '#{Id}button'
    ,url: '{url}'
    ,size: {FileSize}
    ,accept: 'file'
    ,multiple:true
    ,number:{NumFileOnce}
    {(ext == "" ? "" : $", exts: '{ext}'")}
    ,before: function(obj){{
        index = layui.layer.load(2);
        {Id}preview = obj;
    }}
    ,done: function(res){{
      layui.layer.close(index);
      if(res.Data.Id == ''){{
          layui.layer.msg('{WalkingTec.Mvvm.TagHelpers.LayUI.THProgram._localizer["Sys.UploadFailed"]}');
      }}
      else{{
        {Id}selected.push(res.Data.Id);
        {Id}SetValues();
        {(ShowPreview == true ? $@"
            $('#{Id}label').append('<label id=""label'+res.Data.Id+'""><img alt=""'+res.Data.Name+'"" layer-src=""/_Framework/GetFile?id='+ res.Data.Id+'&_DONOT_USE_CS={(vm != null ? vm.CurrentCS : "")}"" src=""/_Framework/GetFile?id='+ res.Data.Id+'&stream=true&width={PreviewWidth ?? 64}&height={PreviewHeight ?? 64}&_DONOT_USE_CS={(vm!=null?vm.CurrentCS:"")}""  class=""layui-upload-img"" width={PreviewWidth ?? 64} height={PreviewHeight ?? 64} id=""preview'+res.Data.Id+'"" style=""cursor:pointer;margin-bottom:5px"" /><i class=""layui-icon layui-icon-close"" style=""font-size: 20px;position:relative;left:-10px;top:-27px;color: #ff0000;cursor: pointer;"" id=""del'+res.Data.Id+'""></i></label> ');
            $('#preview'+res.Data.Id).on('click',function(){{
              {Id}DoPreview(res.Data.Id);
            }});
            $('#del'+res.Data.Id).on('click',function(){{
              {Id}DoDelete(res.Data.Id);
            }});
      " : $@"
           $('#{Id}label').append(""<label id='label""+res.Data.Id+""'><button class='layui-btn layui-btn-sm layui-btn-danger' type='button' id='del""+res.Data.Id+""' style='color:white;margin-left:0px;'>""+res.Data.Name +""  {WalkingTec.Mvvm.TagHelpers.LayUI.THProgram._localizer["Sys.Delete"]}</button><br/></label>"");
           $('#del'+res.Data.Id).on('click',function(){{
              {Id}DoDelete(res.Data.Id);
          }});
      "
      )}
      }}
    }}
    ,error: function(){{
        layui.layer.close(index);
    }}
  }});
}})
</script>
");
            if (string.IsNullOrEmpty(idstring) == false)
            {
                var allfileids = idstring.Split('|', StringSplitOptions.RemoveEmptyEntries);

                foreach (var fileId in allfileids)
                {
                    var geturl = $"/_Framework/GetFileName/{fileId}";
                    var downloadurl = $"/_Framework/GetFile/{fileId}";
                    var previewurl = $"/_Framework/ViewFile/{fileId}";
                    if (vm != null)
                    {
                        geturl += $"?_DONOT_USE_CS={vm.CurrentCS}";
                        downloadurl += $"?_DONOT_USE_CS={vm.CurrentCS}";
                        previewurl += $"?_DONOT_USE_CS={vm.CurrentCS}";
                    }
                    var picurl = $"/_Framework/GetFile?id={fileId}&stream=true&width={PreviewWidth ?? 64}&height={PreviewHeight ?? 64}";
                    if (vm != null)
                    {
                        picurl += $"&_DONOT_USE_CS={vm.CurrentCS}";
                    }
                    output.PostElement.AppendHtml($@"
<script>
$.ajax({{
  cache: false,
  type: 'GET',
  url: '{geturl}',
  async: true,
  success: function(data) {{
    {(ShowPreview == true ? $@" {(Disabled == true? $@"
      $('#{Id}label').append('<label id=""label{fileId}""><img layer-src=""{downloadurl}"" src=""{picurl}"" alt=""'+data+'""  class=""layui-upload-img"" width={PreviewWidth ?? 64} height={PreviewHeight ?? 64} id=""preview{fileId}"" style=""cursor:pointer;margin-bottom:5px""/></label> ');
            $('#preview{fileId}').on('click',function(){{
              {Id}DoPreview();
            }});
" :$@"
      $('#{Id}label').append('<label id=""label{fileId}""><img layer-src=""{downloadurl}"" src=""{picurl}"" alt=""'+data+'"" class=""layui-upload-img"" width={PreviewWidth ?? 64} height={PreviewHeight ?? 64} id=""preview{fileId}"" style=""cursor:pointer;margin-bottom:5px""/><i class=""layui-icon layui-icon-close"" style=""font-size: 20px;position:relative;left:-10px;top:-27px;color: #ff0000;cursor:pointer;margin-bottom:5px"" id=""del{fileId}""></i></label> ');
            $('#del{fileId}').on('click',function(){{
              {Id}DoDelete('{fileId}');
            }});
            $('#preview{fileId}').on('click',function(){{
              {Id}DoPreview();
            }});
")}
    " : $@"{(Disabled == true? $@"
        $('#{Id}label').append(""<label id='label{fileId}'><a class='layui-btn layui-btn-primary layui-btn-xs' style='margin:9px 0;width:unset width:300px;' href='{downloadurl}'>""+data+""</a></label>"");
" :$@"
        $('#{Id}label').append(""<label id='label{fileId}'><button class='layui-btn layui-btn-sm layui-btn-danger' type='button' id='del{fileId}' style='color:white'>""+data+""  {WalkingTec.Mvvm.TagHelpers.LayUI.THProgram._localizer["Sys.Delete"]}</button><br/></label>"");
        $('#del{fileId}').on('click',function(){{
          {Id}DoDelete('{fileId}');
        }});
")}
    ")}
  }}
}});
</script>
");
                }

                
            }
            base.Process(context, output);

        }
    }
}
