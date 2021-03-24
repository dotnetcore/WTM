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

        private TaskCompletionSource<DialogResult> ReturnTask { get; } = new TaskCompletionSource<DialogResult>();

        public async Task<DialogResult> OpenDialog<T>(string Title, Expression<Func<T, object>> Values = null)
        {
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
                        ReturnTask.TrySetResult(r);
                        option.Dialog!.Close();
                    }));
                }
                catch { };
                builder.SetKey(Guid.NewGuid());
                builder.CloseComponent();
            };
            await WtmBlazor.Dialog.Show(option);
            var rv = await ReturnTask.Task;
            return rv;
        }

        public async Task SetError(ValidateForm form, ErrorObj errors)
        {
            foreach (var item in errors.Form)
            {
                //var exp = PropertyHelper.GetPropertyExpression(form.Model.GetType(), item.Key);
                //form.SetError(exp, item.Value);
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
    }

    public class WtmBlazorContext
    {
        public IStringLocalizer Localizer { get; set; }
        public GlobalItems GlobalSelectItems { get; set; }
        public ApiClient Api { get; set; }
        public DialogService Dialog { get; set; }

        public WtmBlazorContext(IStringLocalizerFactory factory, GlobalItems gi, ApiClient api, DialogService dialog)
        {
            this.Localizer = factory.Create(typeof(Program)); ;
            this.GlobalSelectItems = gi;
            this.Api = api;
            this.Dialog = dialog;
        }
    }
}
