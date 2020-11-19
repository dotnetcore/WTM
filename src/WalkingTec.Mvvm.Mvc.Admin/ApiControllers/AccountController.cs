using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using WalkingTec.Mvvm.Admin.ViewModels;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_[controller]")]
    [Route("api/_login")]
    [ActionDescription("Login")]
    [AllRights]
    public class AccountController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly ITokenService _authService;
        public AccountController(
            ILogger<AccountController> logger,
            ITokenService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm]string userid, [FromForm]string password, [FromForm]bool rememberLogin = false, [FromForm]bool cookie = true)
        {
            var user = DC.Set<FrameworkUserBase>()
                            .Include(x => x.UserRoles)
                            .Include(x => x.UserGroups)
                            .Where(x => x.ITCode.ToLower() == userid.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid)
                            .SingleOrDefault();

            //如果没有找到则输出错误
            if (user == null)
            {
                return BadRequest(Mvc.Admin.Program._localizer["LoginFailed"].Value);
            }
            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();
            //查找登录用户的数据权限
            var dpris = DC.Set<DataPrivilege>()
                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                .ToList();
            ProcessTreeDp(dpris);
            //生成并返回登录用户信息
            var rv = new LoginUserInfo();
            rv.Id = user.ID;
            rv.ITCode = user.ITCode;
            rv.Name = user.Name;
            rv.Roles = DC.Set<FrameworkRole>().Where(x => user.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToList();
            rv.Groups = DC.Set<FrameworkGroup>().Where(x => user.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToList();
            rv.DataPrivileges = dpris;
            //查找登录用户的页面权限
            var pris = DC.Set<FunctionPrivilege>()
                            .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                            .ToList();

            rv.FunctionPrivileges = pris;
            rv.PhotoId = user.PhotoId;
            LoginUserInfo = rv;

            if (cookie) // cookie auth
            {
                AuthenticationProperties properties = null;
                if (rememberLogin)
                {
                    properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var principal = LoginUserInfo.CreatePrincipal();
                // 在上面注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
                List<SimpleMenu> ms = new List<SimpleMenu>();
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.Id = LoginUserInfo.Id;
                forapi.ITCode = LoginUserInfo.ITCode;
                forapi.Name = LoginUserInfo.Name;
                forapi.Roles = LoginUserInfo.Roles;
                forapi.Groups = LoginUserInfo.Groups;
                forapi.PhotoId = LoginUserInfo.PhotoId;
                var menus = DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName == null)
                    .OrderBy(x=>x.DisplayOrder)
                    .Select(x => new SimpleMenu
                    {
                        Id = x.ID.ToString().ToLower(),
                        ParentId = x.ParentId.ToString().ToLower(),
                        Text = x.PageName,
                        Url = x.Url,
                        Icon = x.ICon
                    }).ToList();
                LocalizeMenu(menus);
                ms.AddRange(menus);

                List<string> urls = new List<string>();
                urls.AddRange(DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName != null)
                    .Select(x => x.Url)
                    );
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", menus);
                forapi.Attributes.Add("Actions", urls);

                return Ok(forapi);
            }
            else // jwt auth
            {
                var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;

                var token = await authService.IssueTokenAsync(LoginUserInfo);
                return Content(JsonConvert.SerializeObject(token), "application/json");
            }
        }


        private void LocalizeMenu(List<SimpleMenu> menus)
        {
            if (menus == null)
            {
                return;
            }
            //循环所有菜单项
            foreach (var menu in menus)
            {
                if (menu.Text?.StartsWith("MenuKey.") == true)
                {
                    if (Core.Program._Callerlocalizer[menu.Text].ResourceNotFound == true)
                    {
                        menu.Text = Core.Program._localizer[menu.Text];
                    }
                    else
                    {
                        menu.Text = Core.Program._Callerlocalizer[menu.Text];
                    }
                }
            }
        }


        [HttpPost("[action]")]
        [AllRights]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public async Task<Token> RefreshToken(string refreshToken)
        {
            return await _authService.RefreshTokenAsync(refreshToken);
        }

        [Public]
        [HttpGet("[action]/{id}")]
        public IActionResult CheckLogin(Guid? id)
        {
            if (LoginUserInfo == null || LoginUserInfo?.Id == Guid.Empty)
            {
                return BadRequest();
            }
            else
            {
                var forapi = new LoginUserInfo();
                forapi.Id = LoginUserInfo.Id;
                forapi.ITCode = LoginUserInfo.ITCode;
                forapi.Name = LoginUserInfo.Name;
                forapi.Roles = LoginUserInfo.Roles;
                forapi.Groups = LoginUserInfo.Groups;
                forapi.PhotoId = LoginUserInfo.PhotoId;

                var ms = new List<SimpleMenu>();
                var roleIDs = LoginUserInfo.Roles.Select(x => x.ID).ToList();
                var data = DC.Set<FrameworkMenu>().Where(x => x.MethodName == null).ToList();
                var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList(); var allowed = DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.UserId == LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                                .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                                .ToList();

                var allowedids = allowed.Select(x => x.ID).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID))
                    {
                        ms.Add(new SimpleMenu {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.ICon
                        });
                    }
                }

                LocalizeMenu(ms);

                List<string> urls = new List<string>();
                urls.AddRange(allowed.Select(x=>x.Url).Distinct());
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", ms);
                forapi.Attributes.Add("Actions", urls);
                return Ok(forapi);
            }
        }

        [Public]
        [HttpGet("[action]")]
        public IActionResult CheckUserInfo()
        {
            if (LoginUserInfo == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = new LoginUserInfo();
                forapi.Id = LoginUserInfo.Id;
                forapi.ITCode = LoginUserInfo.ITCode;
                forapi.Name = LoginUserInfo.Name;
                forapi.Roles = LoginUserInfo.Roles;
                forapi.Groups = LoginUserInfo.Groups;
                forapi.PhotoId = LoginUserInfo.PhotoId;

                var ms = new List<SimpleMenu>();
                var roleIDs = LoginUserInfo.Roles.Select(x => x.ID).ToList();
                var data = DC.Set<FrameworkMenu>().Where(x => x.MethodName == null).ToList();
                var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList();
                var allowed = DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.UserId == LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                                .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                                .ToList();

                var allowedids = allowed.Select(x => x.ID).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID))
                    {
                        ms.Add(new SimpleMenu
                        {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.ICon
                        });
                    }
                }

                LocalizeMenu(ms);

                List<string> urls = new List<string>();
                urls.AddRange(allowed.Select(x => x.Url).Distinct());
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", ms);
                forapi.Attributes.Add("Actions", urls);
                return Ok(forapi);
            }
        }


        [AllRights]
        [HttpPost("[action]")]
        public IActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoChange();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok();
                }
            }

        }

        [AllRights]
        [HttpGet("[action]/{id}")]
        public async Task Logout(Guid? id)
        {
            await LoginUserInfo.RemoveUserCache(LoginUserInfo.Id.ToString());
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }


        private void ProcessTreeDp(List<DataPrivilege> dps)
        {
            var dpsSetting = GlobalServices.GetService<Configs>().DataPrivilegeSettings;
            foreach (var ds in dpsSetting)
            {
                if (typeof(ITreeData).IsAssignableFrom(ds.ModelType))
                {
                    var ids = dps.Where(x => x.TableName == ds.ModelName).Select(x => x.RelateId).ToList();
                    if (ids.Count > 0 && ids.Contains(null) == false)
                    {
                        List<Guid> tempids = new List<Guid>();
                        foreach (var item in ids)
                        {
                            if (Guid.TryParse(item, out Guid g))
                            {
                                tempids.Add(g);
                            }
                        }
                        List<Guid> subids = new List<Guid>();
                        subids.AddRange(GetSubIds(tempids.ToList(), ds.ModelType));
                        subids = subids.Distinct().ToList();
                        subids.ForEach(x => dps.Add(new DataPrivilege
                        {
                            TableName = ds.ModelName,
                            RelateId = x.ToString()
                        }));
                    }
                }
            }
        }

        private IEnumerable<Guid> GetSubIds(List<Guid> p_id, Type modelType)
        {
            var basequery = DC.GetType().GetTypeInfo().GetMethod("Set").MakeGenericMethod(modelType).Invoke(DC, null) as IQueryable;
            var subids = basequery.Cast<ITreeData>().Where(x => p_id.Contains(x.ParentId.Value)).Select(x => x.ID).ToList();
            if (subids.Count > 0)
            {
                return subids.Concat(GetSubIds(subids, modelType));
            }
            else
            {
                return new List<Guid>();
            }
        }

    }

    public class SimpleMenu
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

    }
}
