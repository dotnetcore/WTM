using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using WalkingTec.Mvvm.Core;
using WtmBlazorUtils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configs = builder.Configuration.Get<Configs>();
builder.RootComponents.Add<WalkingTec.Mvvm.BlazorDemo.Shared.App>("app");
builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
builder.Services.AddBootstrapBlazor(null, options => { options.ResourceManagerStringLocalizerType = typeof(WalkingTec.Mvvm.BlazorDemo.Shared.Program); });
builder.Services.AddWtmBlazor(builder.Configuration, builder.HostEnvironment.BaseAddress);
var host = builder.Build();
var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
var result = await jsInterop.InvokeAsync<string>("localStorageFuncs.get", "wtmculture");
CultureInfo culture = null;
if (result == null)
{
    culture = configs.SupportLanguages[0];
}
else
{
    culture = new CultureInfo(result);
}
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
await host.RunAsync();
