using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum UploadTypeEnum { AllFiles, ImageFile, ZipFile, ExcelFile, WordFile, PDFFile, TextFile }

    [HtmlTargetElement("wt:upload", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class UploadTagHelper : BaseFieldTag
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
        /// 是否显示进度条，默认为false
        /// </summary>
        public bool? ShowProgress { get; set; }

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

        public string ConnectionString { get; set; }

        public string ButtonText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.Attributes.Add("id", Id + "button");
            output.Attributes.Add("name", Id + "button");
            output.Attributes.Add("class", "layui-btn layui-btn-sm");
            output.Attributes.Add("type", "button");
            output.TagMode = TagMode.StartTagAndEndTag;
            if (string.IsNullOrEmpty(ButtonText))
            {
                output.Content.SetHtmlContent(Program._localizer["Select"]);
            }
            else
            {
                output.Content.SetHtmlContent(ButtonText);
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

            output.PreElement.SetHtmlContent($@"
<label id='{Id}label'></label>
");
            var requiredtext = "";
            if (Field.Metadata.IsRequired)
            {
                requiredtext = $" lay-verify=\"required\" lay-reqText=\"{Program._localizer["{0}required", Field?.Metadata?.DisplayName ?? Field?.Metadata?.Name]}\"";
            }

            output.PostElement.SetHtmlContent($@"
<input type='hidden' id='{Id}' name='{Field.Name}' value='{Field.Model}' {requiredtext} />
");    
            if (ShowProgress != null)
            {
                if (ShowProgress == true)
                {
                    output.PostElement.AppendHtml($@"
<div class='layui-progress' lay-showpercent='true' lay-filter='demo'>
    <div class='layui-progress-bar layui-bg-red' lay-percent='0%'></div>
</div>
");
                }
            }
            output.PostElement.AppendHtml($@"
<script>
  function {Id}DoDelete(fileid){{
    $('#{Id}').parents('form').append(""<input type='hidden' id='DeletedFileIds' name='DeletedFileIds' value='""+fileid+""' />"");
    $('#{Id}label').html('');
    $('#{Id}').val('');
    $('.layui-progress .layui-progress-bar').css('width', '0%');
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
    {(ext == "" ? "" : $", exts: '{ext}'")}
    ,xhr: xhrOnProgress
    ,progress: function (value) {{
        $('.layui-progress .layui-progress-bar').css(
            'width',
            value + '%'
        );
    }}
    ,before: function(obj){{
        index = layui.layer.load(2);
        {Id}preview = obj;
    }}
    ,done: function(res){{
      layui.layer.close(index);
      if(res.Data.Id == ''){{
          $('#{Id}label').html('');
          layui.layer.msg('{Program._localizer["UploadFailed"]}');
      }}
      else{{
            $('#{Id}label').html('');
        $('#{Id}').val(res.Data.Id);
      {(ShowPreview == true ? $@"
       {Id}preview.preview(function(index, file, result){{
            $('#{Id}label').append('<img src=""'+ result +'"" alt=""'+ file.name +'"" class=""layui-upload-img"" width={PreviewWidth ?? 64} height={PreviewHeight ?? 64} />');
            $('#{Id}label').append('<i class=""layui-icon layui-icon-close"" style=""font-size: 20px;position:absolute;left:{(PreviewWidth ?? 64) - 10}px;top:-10px;color: #ff0000;"" id=""{Id}del""></i> ');
            $('#{Id}del').on('click',function(){{
              {Id}DoDelete(res.Data.Id);
            }});
          }});
      " : $@"
          $('#{Id}label').append(""<button class='layui-btn layui-btn-sm layui-btn-danger' type='button' id='{Id}del' style='color:white'>""+res.Data.Name +""  {Program._localizer["Delete"]}</button>"");
          $('#{Id}del').on('click',function(){{
            $('.layui-progress .layui-progress-bar').css('width', '0%');
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
    var xhrOnProgress = function (fun) {{
        xhrOnProgress.onprogress = fun; //绑定监听
        //使用闭包实现监听绑
        return function () {{
            var xhr = $.ajaxSettings.xhr();
            //判断监听函数是否为函数
            if (typeof xhrOnProgress.onprogress !== 'function')
                return xhr;
            if (xhrOnProgress.onprogress && xhr.upload) {{
                xhr.upload.onprogress = xhrOnProgress.onprogress;
            }}
            return xhr;
        }}
    }}
</script>
");
            if (Field.Model != null && Field.Model.ToString() != Guid.Empty.ToString())
            {
                var geturl = $"/_Framework/GetFileName/{Field.Model}";
                if (vm != null)
                {
                    geturl += $"?_DONOT_USE_CS={vm.CurrentCS}";
                }
                var picurl = $"/_Framework/GetFile?id={Field.Model}&stream=true&width={PreviewWidth ?? 64}&height={PreviewHeight ?? 64}";
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
  async: false,
  success: function(data) {{
    {(ShowPreview == true ? $@"
      $('#{Id}label').append('<img src=""{picurl}"" alt=""'+ data +'"" class=""layui-upload-img"" width={PreviewWidth ?? 64} height={PreviewHeight ?? 64} />');
      $('#{Id}label').append('<i class=""layui-icon layui-icon-close"" style=""font-size: 20px;position:absolute;left:{(PreviewWidth ?? 64) - 10}px;top:-10px;color: #ff0000;"" id=""{Id}del""></i> ');
      $('#{Id}del').on('click',function(){{
        {Id}DoDelete('{Field.Model}');
      }});
    " : $@"
        $('#{Id}label').append(""<button class='layui-btn layui-btn-sm layui-btn-danger' type='button' id='{Id}del' style='color:white'>""+data+""  {Program._localizer["Delete"]}</button>"");
        $('#{Id}del').on('click',function(){{
          {Id}DoDelete('{Field.Model}');
        }});
    ")}
  }}
}});
</script>
");
            }
            base.Process(context, output);

        }
    }
}
