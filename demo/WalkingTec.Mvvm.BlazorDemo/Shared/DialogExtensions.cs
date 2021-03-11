using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using WalkingTec.Mvvm.Core;

namespace WtmBlazorControls
{
    public static class DialogExtensions
    {
        public static async Task ShowDialog<T>(this DialogService self, string Title, Expression<Func<T, object>> Values = null)
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
                    builder.AddAttribute(3, "OnCloseDialog", EventCallback.Factory.Create(self, () =>
                    {
                        option.Dialog!.Close();
                    }));
                }
                catch { };
                builder.SetKey(Guid.NewGuid());
                builder.CloseComponent();
            };
            await self.Show(option);
        }
    }
}
