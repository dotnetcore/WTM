using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WtmBlazorUtils
{
    public abstract class BasePage : ComponentBase
    {
        [Inject]
        public WtmBlazorContext WtmBlazor { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public List<string> DeletedFileIds { get; set; }

        [CascadingParameter]
        public LoginUserInfo UserInfo
        {
            get;
            set;
        }

        public object _userinfo;

        [CascadingParameter(Name = "BodyContext")]
        public object UserInfoForDialog
        {
            get
            {
                return _userinfo;
            }
            set
            {
                _userinfo = value;
                UserInfo = value as LoginUserInfo;
            }
        }


        [Parameter]
        public Action<DialogResult> OnCloseDialog { get; set; }

        protected void CloseDialog(DialogResult result = DialogResult.Close)
        {
            OnCloseDialog?.Invoke(result);
        }


        public async Task<DialogResult> OpenDialog<T>(string Title, Expression<Func<T, object>> Values = null, Size size = Size.ExtraExtraLarge, LoginUserInfo userinfo = null, bool isMax = false)
        {
            return await WtmBlazor.OpenDialog(Title, Values, size, userinfo??this.UserInfo, isMax);
        }

        public async Task<bool> PostsData(object data, string url, Func<string, string> Msg = null, Action<ErrorObj> ErrorHandler = null, HttpMethodEnum method = HttpMethodEnum.POST)
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
                if (rv.Errors == null)
                {
                    await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.StatusCode.ToString());
                }
                else
                {
                    var err = rv.Errors.GetFirstError();
                    if (string.IsNullOrEmpty(err) == false)
                    {
                        await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], err);
                    }
                    else
                    {
                        await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.ErrorMsg);
                    }
                    ErrorHandler?.Invoke(rv.Errors);
                }
                return false;
            }

        }

        public async Task<bool> PostsForm(ValidateForm form, string url, Func<string, string> Msg = null, Action<ErrorObj> ErrorHandler = null, HttpMethodEnum method = HttpMethodEnum.POST)
        {
            if(form.Model is BaseVM bv)
            {
                bv.DeletedFileIds = this.DeletedFileIds;
            }
            var rv = await WtmBlazor.Api.CallAPI(url, method, form.Model);
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
                if (rv.Errors == null)
                {
                    await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], rv.StatusCode.ToString());
                }
                else
                {
                    SetError(form, rv.Errors);
                    ErrorHandler?.Invoke(rv.Errors);
                }
                return false;
            }
        }

        public async Task<QueryData<T>> StartSearch<T>(string url, BaseSearcher searcher, QueryPageOptions options) where T : class, new()
        {
            if (searcher != null)
            {
                searcher.IsEnumToString = false;
            }
            var rv = await WtmBlazor.Api.CallSearchApi<T>(url, searcher, options);
            QueryData<T> data = new QueryData<T>();
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                data.Items = rv.Data?.Data;
                data.TotalCount = rv.Data?.Count ?? 0;
            }
            if (rv.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], WtmBlazor.Localizer["Sys.NoPrivilege"]);
            }
            return data;
        }

        public async Task<QueryData<T>> StartSearchTree<T>(string url, BaseSearcher searcher, QueryPageOptions options) where T : class, new()
        {
            if (searcher != null)
            {
                searcher.IsEnumToString = false;
            }
            var rv = await WtmBlazor.Api.CallSearchApi<T>(url, searcher, options);
            QueryData<T> data = new QueryData<T>();
            if (rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var idpro = typeof(T).GetSingleProperty("ID");
                if (rv.Data?.Data != null)
                {
                    foreach (var item in rv.Data.Data)
                    {
                        string pid = idpro.GetValue(item)?.ToString();
                        item.SetPropertyValue("Children", new List<T>(rv.Data.Data.AsQueryable().CheckParentID(pid)));
                    }
                }
                data.Items = rv.Data?.Data.AsQueryable().CheckParentID(null);
                data.TotalCount = rv.Data?.Count ?? 0;
            }
            if (rv.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], WtmBlazor.Localizer["Sys.NoPrivilege"]);
            }
            return data;
        }


        public async void SetError(ValidateForm form, ErrorObj errors)
        {
            if (errors != null)
            {
                foreach (var item in errors.Form)
                {
                    Regex r = new Regex("(.*?)\\[(\\-?\\d?)\\]\\.(.*?)$");
                    var match = r.Match(item.Key);
                    if (match.Success)
                    {
                        int index = 0;
                        int.TryParse(match.Groups[2].Value, out index);
                        await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], $"{index+1}:{item.Value}" );
                    }
                    else
                    {
                        form.SetError(item.Key, item.Value);
                    }
                }
                if (errors.Message != null && errors.Message.Count > 0)
                {
                    await WtmBlazor.Toast.Error(WtmBlazor.Localizer["Sys.Error"], errors.Message[0]);
                }
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
            return await GetLocalStorage<string>("wtmtoken");
        }

        public async Task<string> GetRefreshToken()
        {
            return await GetLocalStorage<string>("wtmrefreshtoken");
        }

        public async Task SetToken(string token, string refreshtoken)
        {
            await SetLocalStorage("wtmtoken", token);
            await SetLocalStorage("wtmrefreshtoken", refreshtoken);
        }

        public async Task DeleteToken()
        {
            await DeleteLocalStorage("wtmtoken");
            await DeleteLocalStorage("wtmrefreshtoken");
        }

        public async Task SetUserInfo(LoginUserInfo userinfo)
        {
            await JSRuntime.InvokeVoidAsync("localStorageFuncs.set", "wtmuser", JsonSerializer.Serialize(userinfo));
        }

        public async Task<LoginUserInfo> GetUserInfo()
        {
            var user = await GetLocalStorage<LoginUserInfo>("wtmuser");
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            user.Attributes["Actions"] = JsonSerializer.Deserialize<string[]>(user.Attributes["Actions"].ToString(), options).Where(x => x != null).ToArray();
            user.Attributes["Menus"] = JsonSerializer.Deserialize<SimpleMenuApi[]>(user.Attributes["Menus"].ToString(), options);
            return user;
        }

        public async Task<T> GetLocalStorage<T>(string key) where T : class
        {
            string rv = "";
            while (true)
            {
                string part = await JSRuntime.InvokeAsync<string>("localStorageFuncs.get", System.Threading.CancellationToken.None, key, rv.Length);
                if(part == null)
                {
                    return null;
                }
                rv += part;
                if (part.Length < 20000)
                {
                    break;
                }
            }
            if(typeof(T) == typeof(string))
            {
                return rv as T;
            }
            var obj = JsonSerializer.Deserialize<T>(rv);
            return obj;
        }

        public async Task SetLocalStorage<T>(string key, T data) where T : class
        {
            if (typeof(T) == typeof(string))
            {
                await JSRuntime.InvokeVoidAsync("localStorageFuncs.set", key, data);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("localStorageFuncs.set", key, JsonSerializer.Serialize(data));
            }
        }
        public async Task DeleteLocalStorage(string key)
        {
            await JSRuntime.InvokeAsync<string>("localStorageFuncs.remove", key);
        }


        public bool IsAccessable(string url)
        {
            if (WtmBlazor.ConfigInfo.IsQuickDebug == true)
            {
                return true;
            }
            if (UserInfo != null)
            {
                var actions = UserInfo.Attributes["Actions"] as string[];
                url = url.ToLower();
                return actions.Any(x => x.ToLower() == url.ToLower());
            }
            else if (WtmBlazor.PublicPages.Contains(url?.ToLower()))
            {
                return true;
            }
            
            return false;
        }

        public async Task Redirect(string path)
        {
            await JSRuntime.InvokeVoidAsync("urlFuncs.redirect", path);
        }

        public async Task Download(string url, object data, HttpMethodEnum method = HttpMethodEnum.POST)
        {
            url = WtmBlazor.GetServerUrl() + url;
            await JSRuntime.InvokeVoidAsync("urlFuncs.download", url, JsonSerializer.Serialize(data, CoreProgram.DefaultPostJsonOption), method.ToString());
        }

        public string GetLanguageStr()
        {
            if (CultureInfo.CurrentUICulture.Name.Contains("zh"))
            {
                return "zh-CN";
            }
            else return CultureInfo.CurrentUICulture.Name;
        }
    }

    public class WtmBlazorContext
    {
        public IStringLocalizer Localizer { get; set; }
        public GlobalItems GlobalSelectItems { get; set; }
        public ApiClient Api { get; set; }
        public DialogService Dialog { get; set; }
        public ToastService Toast { get; set; }
        public IHttpClientFactory ClientFactory { get; set; }
        private Configs _configInfo;
        public Configs ConfigInfo { get => _configInfo; }

        public List<string> _publicPages = null;

        public WtmBlazorContext(IStringLocalizer local, GlobalItems gi, ApiClient api, DialogService dialog, ToastService toast, IHttpClientFactory cf, Configs _config)
        {
            _configInfo = _config;
            this.Localizer = local;
            this.GlobalSelectItems = gi;
            this.Api = api;
            this.Dialog = dialog;
            this.Toast = toast;
            this.ClientFactory = cf;
        }

        public List<FrameworkMenu> GetAllPages()
        {
            var pages = Assembly.GetCallingAssembly().GetTypes().Where(x => typeof(BasePage).IsAssignableFrom(x)).ToList();
            var menus = new List<FrameworkMenu>();
            foreach (var item in pages)
            {
                var actdes = item.GetCustomAttribute<ActionDescriptionAttribute>();
                if (actdes != null)
                {
                    var route = item.GetCustomAttribute<RouteAttribute>();
                    var parts = route.Template.Split("/").Where(x => x != "").ToArray();
                    var area = Localizer["Sys.DefaultArea"].Value;
                    if (parts.Length > 1)
                    {
                        area = parts[0];
                    }
                    var areamenu = menus.Where(x => x.PageName == area).FirstOrDefault();
                    if (areamenu == null)
                    {
                        areamenu = new FrameworkMenu
                        {
                            PageName = area,
                            Icon = "fa fa-fw fa-folder",
                            Children = new List<FrameworkMenu>()
                        };
                        menus.Add(areamenu);
                    }
                    areamenu.Children.Add(new FrameworkMenu
                    {
                        PageName = Localizer[actdes.Description],
                        Icon = "fa fa-fw fa-file",
                        Url = route.Template,
                        ClassName = actdes.ClassFullName
                    });
                }
            }
            return menus;
        }

        public List<string> PublicPages
        {
            get {
                if(_publicPages == null)
                {
                    var pages = Assembly.GetCallingAssembly().GetTypes().Where(x => typeof(BasePage).IsAssignableFrom(x)).ToList();
                     _publicPages = new List<string>();
                    foreach (var item in pages)
                    {
                        var route = item.GetCustomAttribute<RouteAttribute>();
                        var ispublic = item.GetCustomAttribute<PublicAttribute>();
                        if (ispublic != null)
                        {
                            var url = route.Template.ToLower();
                            if (url.StartsWith("/"))
                            {
                                url = url[1..];
                            }
                            url = Regex.Replace(url, "{.*?}", ".*?");
                            url = "^" + url + "$";
                            _publicPages.Add(url);
                        }
                    }
                }
                return _publicPages;
            }
        }

        public async Task<string> GetBase64Image(string fileid, int? width = null, int? height = null)
        {
            if (string.IsNullOrEmpty(fileid) == false)
            {
                var rv = await Api.CallAPI<byte[]>($"/api/_file/GetFile/{fileid}", HttpMethodEnum.GET, new Dictionary<string, string> {
                    {"width", width?.ToString() },
                    {"height", height?.ToString() }
                });
                if (rv.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(rv.Data)}";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public async Task<DialogResult> OpenDialog<T>(string Title, Expression<Func<T, object>> Values = null, Size? size = null, LoginUserInfo userinfo = null, bool isMax = false)
        {
            TaskCompletionSource<DialogResult> ReturnTask = new TaskCompletionSource<DialogResult>();
            SetValuesParser p = new SetValuesParser();
            if(size != null)
            {
                size = Size.ExtraLarge;
            }
            DialogOption option = new DialogOption
            {
                ShowCloseButton = false,
                ShowFooter = false,
                IsDraggable = true,
                ShowMaximizeButton = !isMax,
                FullScreenSize = isMax==true?FullScreenSize.Always:FullScreenSize.Medium,
                Size =  size.Value,
                IsScrolling = true,
                BodyContext = userinfo,
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
                        option.CloseDialogAsync();
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
                await option.CloseDialogAsync();
                ReturnTask.TrySetResult(DialogResult.Close);
            };
            await Dialog.Show(option);
            var rv = await ReturnTask.Task;
            return rv;
        }
        public string GetServerUrl()
        {
            var server = ConfigInfo.Domains.Where(x => x.Key.ToLower() == "serverpub").Select(x => x.Value).FirstOrDefault();
            if (server == null)
            {
                server = ConfigInfo.Domains.Where(x => x.Key.ToLower() == "server").Select(x => x.Value).FirstOrDefault();
            }
            if (server != null)
            {
                return server.Address.TrimEnd('/');
            }
            else
            {
                return "";
            }
        }

        public Task<IEnumerable<TableTreeNode<T>>> DefaultTreeConverter<T>(IEnumerable<T> data) where T:TreePoco<T>
        {
            List<TableTreeNode<T>> rv = new List<TableTreeNode<T>>();
            if(data == null || data.Count() == 0)
            {
                return Task.FromResult(rv.AsEnumerable());
            }
            foreach (var item in data)
            {
                TableTreeNode<T> tt = new TableTreeNode<T>(item);
                tt.HasChildren = item.HasChildren;
                tt.Items = DefaultTreeConverter<T>(item.Children).Result;
                rv.Add(tt);
            }
            return Task.FromResult(rv.AsEnumerable());
        }
    }
}
