using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.TagHelpers.LayUI.Common;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public static class LayuiServiceCollectionExtensions
    {
        public static IServiceCollection AddLayui(this IServiceCollection services)
        {
            services.Remove(new ServiceDescriptor(typeof(IUIService), typeof(DefaultUIService), ServiceLifetime.Singleton));
            services.AddSingleton<IUIService, LayuiUIService>();

            GlobalServices.SetServiceProvider(services.BuildServiceProvider());
            return services;
        }
    }
}
