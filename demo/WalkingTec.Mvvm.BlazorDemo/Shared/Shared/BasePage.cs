using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using WalkingTec.Mvvm.Core;
using System.Text.Json;

namespace WalkingTec.Mvvm.BlazorDemo.Shared.Shared
{
    public abstract class BasePage : ComponentBase
    {
        [Inject]
        public WtmBlazorContext WtmBlazor { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }

        [Parameter]
        public Action<DialogResult> OnCloseDialog { get; set; }

        protected void CloseDialog(DialogResult result= DialogResult.Close)
        {
            OnCloseDialog?.Invoke(result);
        }


        public async Task<DialogResult> OpenDialog<T>(string Title, Expression<Func<T, object>> Values = null)
        {
            TaskCompletionSource<DialogResult> ReturnTask = new TaskCompletionSource<DialogResult>();
            SetValuesParser p = new SetValuesParser();
            DialogOption option = new DialogOption
            {
                ShowCloseButton = false,
                ShowFooter = false,
                Title = Title
            };
            option.BodyTemplate = builder =>
            {
                builder.OpenComponent(0, typeof(T));
                builder.AddMultipleAttributes(2, p.Parse(Values));
                try
                {
                    builder.AddAttribute(3, "OnCloseDialog", new Action<DialogResult>((r) =>
                    {
                        option.OnCloseAsync = null;
                        ReturnTask.TrySetResult(r);
                        option.Dialog!.Close();
                    }));
                }
                catch { };
                //builder.SetKey(Guid.NewGuid());
                builder.AddMarkupContent(4, "<div style=\"height:10px\"></div>");
                builder.CloseComponent();
            };
            option.OnCloseAsync = async () =>
            {
                    option.OnCloseAsync = null;
                    await option.Dialog.Close();
                ReturnTask.TrySetResult( DialogResult.Close);
            };
            await WtmBlazor.Dialog.Show(option);
            var rv = await ReturnTask.Task;
            return rv;
        }

        public async Task<bool> PostsData(object data,string url, Func<string, string> Msg = null, Action<ErrorObj> ErrorHandler = null, HttpMethodEnum method = HttpMethodEnum.POST)
        {
            var rv = await WtmBlazor.Api.CallAPI(url, method, data);
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (Msg != null)
                {
                    var m = Msg.Invoke(rv.Data);
                    await WtmBlazor.Toast.Success(WtmBlazor.Localizer["Sys.Info"], WtmBlazor.Localizer[m]);
                }
                CloseDialog(DialogResult.Yes);
                return true;
            }
            else
            {
                ErrorHandler?.Invoke(rv.Errors);
                if (rv.Errors == null)
                {
                    await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.StatusCode.ToString());
                }
                else
                {
                    if (string.IsNullOrEmpty(rv.ErrorMsg))
                    {
                        if (rv.Errors.Form.Any())
                        {
                            await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.Errors.Form.First().Value);
                        }
                    }
                    else
                    {
                        await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.ErrorMsg);
                    }
                }
                return false;
            }

        }

        public async Task<bool> PostsForm(ValidateForm form, string url, Func<string,string> Msg = null, Action<ErrorObj> ErrorHandler=null, HttpMethodEnum method= HttpMethodEnum.POST)
        {
            var rv = await WtmBlazor.Api.CallAPI(url, method, form.Model);
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if(Msg != null)
                {
                    var m = Msg.Invoke(rv.Data);
                    await WtmBlazor.Toast.Success(WtmBlazor.Localizer["Sys.Info"],  WtmBlazor.Localizer[m]);
                }
                CloseDialog(DialogResult.Yes);
                return true;
            }
            else
            {
                ErrorHandler?.Invoke(rv.Errors);
                if (rv.Errors == null)
                {
                    await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.StatusCode.ToString());
                }
                else
                {
                    SetError(form, rv.Errors);
                }
                return false;
            }
        }
        public void SetError(ValidateForm form, ErrorObj errors)
        {
            if (errors != null)
            {
                foreach (var item in errors.Form)
                {
                    form.SetError(item.Key, item.Value);
                }
            }
        }

        public  async Task<string> GetBase64Image(string fileid, int? width=null, int? height=null)
        {
            var rv = await WtmBlazor.Api.CallAPI<byte[]>($"/api/_file/GetFile/{fileid}", HttpMethodEnum.GET, new Dictionary<string, string> {
                    {"width", width?.ToString() },
                    {"height", height?.ToString() }
                });
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return $"data:image/jpeg;base64,{Convert.ToBase64String(rv.Data)}";
            }
            else
            {
                return $"data:image/jpeg;base64,0";
            }
        }

        public async Task<string> GetFileUrl(string fileid, int? width = null, int? height = null)
        {
            var rv = await WtmBlazor.Api.CallAPI<byte[]>($"/api/_file/GetFile/{fileid}", HttpMethodEnum.GET, new Dictionary<string, string> {
                    {"width", width?.ToString() },
                    {"height", height?.ToString() }
                });
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return $"data:image/jpeg;base64,{Convert.ToBase64String(rv.Data)}";
            }
            else
            {
                return $"data:image/jpeg;base64,0";
            }
        }


        public async Task<string> GetToken()
        {
            return await JSRuntime.InvokeAsync<string>("localStorageFuncs.get","wtmtoken");
        }

        public async Task SetToken(string token)
        {
            await JSRuntime.InvokeVoidAsync("localStorageFuncs.set","wtmtoken",token);
        }

        public async Task Redirect(string path)
        {
            await JSRuntime.InvokeVoidAsync("urlFuncs.redirect", path);
        }

        public async Task Download(string url, object data, HttpMethodEnum method= HttpMethodEnum.POST)
        {
            await JSRuntime.InvokeVoidAsync("urlFuncs.download", url, JsonSerializer.Serialize(data), method.ToString());
        }

    }

    public class WtmBlazorContext
    {
        public IStringLocalizer Localizer { get; set; }
        public GlobalItems GlobalSelectItems { get; set; }
        public ApiClient Api { get; set; }
        public DialogService Dialog { get; set; }
        public ToastService Toast { get; set; }
        public WtmBlazorContext(IStringLocalizerFactory factory, GlobalItems gi, ApiClient api, DialogService dialog,ToastService toast)
        {
            this.Localizer = factory.Create(typeof(Program)); ;
            this.GlobalSelectItems = gi;
            this.Api = api;
            this.Dialog = dialog;
            this.Toast = toast;
        }
    }
}
