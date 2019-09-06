using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Admin.Api
{
    [ApiController]
    [Route("api/_login")]
    [ActionDescription("登陆")]
    [Public]
    public class _LoginController : BaseApiController
    {
        [HttpPost("login")]
        [ActionDescription("登录")]
        public IActionResult Login([FromForm] string userid, [FromForm]string password)
        {
            var user = DC.Set<FrameworkUserBase>()
    .Include(x => x.UserRoles).Include(x => x.UserGroups)
    .Where(x => x.ITCode.ToLower() == userid.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid)
    .SingleOrDefault();

            //如果没有找到则输出错误
            if (user == null)
            {
                ModelState.AddModelError(" login", "登录失败");
                return BadRequest(ModelState.GetErrorJson());
            }
            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();
            //查找登录用户的数据权限
            var dpris = DC.Set<DataPrivilege>()
                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                .ToList();
            //生成并返回登录用户信息
            LoginUserInfo rv = new LoginUserInfo();
            rv.Id = user.ID;
            rv.ITCode = user.ITCode;
            rv.Name = user.Name;
            rv.Roles = DC.Set<FrameworkRole>().Where(x => user.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToList();
            rv.Groups = DC.Set<FrameworkGroup>().Where(x => user.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToList();
            rv.DataPrivileges = dpris;
            rv.PhotoId = user.PhotoId;
            //查找登录用户的页面权限
            var pris = DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .ToList();
            rv.FunctionPrivileges = pris;
            LoginUserInfo = rv;

            LoginUserInfo forapi = new LoginUserInfo();
            forapi.Id = user.ID;
            forapi.ITCode = user.ITCode;
            forapi.Name = user.Name;
            forapi.Roles = rv.Roles;
            forapi.Groups = rv.Groups;
            forapi.PhotoId = rv.PhotoId;
            List<SimpleMenu> ms = new List<SimpleMenu>();

            var menus = DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .Select(x => x.MenuItem)
                .Where(x => x.MethodName == null)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SimpleMenu
                {
                    Id = x.ID.ToString().ToLower(),
                    ParentId = x.ParentId.ToString().ToLower(),
                    Text = x.PageName,
                    Url = x.Url,
                    Icon = x.ICon
                });
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

        [HttpGet("CheckLogin/{id}")]
        public IActionResult CheckLogin(Guid id)
        {
            if (LoginUserInfo?.Id != id)
            {
                return BadRequest();
            }
            else
            {
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.Id = LoginUserInfo.Id;
                forapi.ITCode = LoginUserInfo.ITCode;
                forapi.Name = LoginUserInfo.Name;
                forapi.Roles = LoginUserInfo.Roles;
                forapi.Groups = LoginUserInfo.Groups;
                forapi.PhotoId = LoginUserInfo.PhotoId;
                List<SimpleMenu> ms = new List<SimpleMenu>();
                var roleIDs = LoginUserInfo.Roles.Select(x => x.ID).ToList();

                var menus = DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName == null)
                  .OrderBy(x => x.DisplayOrder)
                  .Select(x => new SimpleMenu
                  {
                      Id = x.ID.ToString().ToLower(),
                      ParentId = x.ParentId.ToString().ToLower(),
                      Text = x.PageName,
                      Url = x.Url,
                      Icon = x.ICon
                  });
                ms.AddRange(menus);

                List<string> urls = new List<string>();
                urls.AddRange(DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
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
        }

        [HttpPost("ChangePassword")]
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

        [HttpGet("Logout/{id}")]
        public ActionResult Logout(Guid id)
        {
            if (LoginUserInfo?.Id == id)
            {
                LoginUserInfo = null;
                HttpContext.Session.Clear();
            }
            return Ok();
        }


        [HttpPost("get_token")]
        [ActionDescription("获取Token")]
        public IActionResult GetToken(SigninRequest request)
        {
            if (request.UserId == null)
                return BadRequest(nameof(request.UserId));
            if (request.Password == null)
                return BadRequest(nameof(request.Password));

            var user = DC.Set<Core.FrameworkUserBase>()
                .Include(x => x.UserRoles).Include(x => x.UserGroups)
                .Where(x => x.ITCode.ToLower() == request.UserId.ToLower() && x.Password == Utils.GetMD5String(request.Password) &&
                            x.IsValid).SingleOrDefault();
            if (user == null)
                return NotFound();

            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();

            //查找登录用户的角色
            var roles = DC.Set<FrameworkRole>().Where(x => roleIDs.Contains(x.ID)).ToList();

            //查找登录用户的组
            var groups = DC.Set<FrameworkGroup>().Where(x => groupIDs.Contains(x.ID)).ToList();

            //查找登录用户的数据权限
            var dataPrivileges = DC.Set<DataPrivilege>().Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value))).ToList();

            //查找登录用户的页面权限
            var functionPrivileges = DC.Set<FunctionPrivilege>().Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value))).ToList();

            //生成并返回登录用户信息
            var userInfo = new LoginUserInfo
            {
                Id = user.ID,
                ITCode = user.ITCode,
                Name = user.Name,
                Roles = roles,
                Groups = groups,
                DataPrivileges = dataPrivileges,
                PhotoId = user.PhotoId,
                FunctionPrivileges = functionPrivileges
            };

            JwtHelper.CacheUer(userInfo);

            var token = JwtHelper.BuildJwtToken(userInfo.Id.ToString(), userInfo.Name);
            return new JsonResult(token);
        }

        [HttpPost("heartbeat")]
        [ActionDescription("心跳")]
        [Authorize]
        public IActionResult Heartbeat()
        {
            if (LoginUserInfo == null)
                return NoContent();

            JwtHelper.Signin(LoginUserInfo.Id.ToString(), HttpContext);

            var token = JwtHelper.BuildJwtToken(LoginUserInfo.Id.ToString(), LoginUserInfo.Name);
            return new JsonResult(token);
        }

        [HttpPost("refresh_token")]
        [ActionDescription("刷新Token")]
        [Authorize]
        public IActionResult RefreshToken()
        {
            if (LoginUserInfo == null)
                return NoContent();

            JwtHelper.CacheUer(LoginUserInfo);

            var token = JwtHelper.BuildJwtToken(LoginUserInfo.Id.ToString(), LoginUserInfo.Name);
            return new JsonResult(token);
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

    public class SigninRequest
    {
        public string UserId { get; set; }

        public string Password { get; set; }
    }
}
