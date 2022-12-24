using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Demo.Hubs;
using WalkingTec.Mvvm.Demo.ViewModels.ImVMs;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [AllRights]
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ActionDescription("imapi控制器")]
    public class ImApiController : BaseApiController
    {


        #region 采用按wtm自带分组

        [HttpGet]
        public async Task<IActionResult> GetinitData()
        {

            var userinfo = await DC.Set<FrameworkUser>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == new Guid(Wtm.LoginUserInfo.UserId));

            ImUserInfoVM mineinfo = new()
            {
                username = Wtm.LoginUserInfo.Name,
                id = Wtm.LoginUserInfo.UserId,
                status = "online",
                avatar = Wtm.LoginUserInfo.PhotoId != null ? "/api/_file/getuserphoto/" + Wtm.LoginUserInfo.PhotoId : "",
                sign = userinfo.Sign,

            };
            var conn = ConnectionManagement.SignalRConns;
            var frameworkGroup = await DC.Set<FrameworkGroup>().AsNoTracking().ToListAsync();
            List<FriendInfoVM> FriendInfos = new();
            frameworkGroup.ForEach(async group =>
            {
                FriendInfoVM friendInfo = new();
                friendInfo.groupname = group.GroupName;
                friendInfo.id = group.ID.ToString();

                var groupUser = await DC.Set<FrameworkUserGroup>().AsNoTracking().Where(x => x.GroupCode == group.GroupCode).Select(x => x.UserCode).ToArrayAsync();

                var users = await DC.Set<FrameworkUser>().AsNoTracking()
                .Where(x=> groupUser.Any(y=>y== x.ITCode))
                .Select(user => new ImUserInfoVM
                {
                    username = user.Name,
                    id = user.ID.ToString().ToLower(),
                    status = ConnectionManagement.IsConn(user.ID.ToString()) == true ? "online" : "offline",
                    avatar = user.PhotoId != null ? "/api/_file/getuserphoto/" + user.PhotoId : "",
                    sign = user.Sign,

                })
                .ToListAsync();
                friendInfo.list = users;
                FriendInfos.Add(friendInfo);

            });

            var imInitUserData = new ImInitUserDataVM
            {
                mine = mineinfo,
                friend = FriendInfos
            };

            ImInitDataVM initData = new()
            {
                code = 0,
                msg = "",
                data = imInitUserData
            };

            return Ok(initData);


        }


        #endregion

        #region 用单位获取，暂时弃用
        //[HttpGet]
        //public async Task<IActionResult> GetinitData()
        //{

        //    var userinfo = await DC.Set<FrameworkUser>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == new Guid(Wtm.LoginUserInfo.UserId));

        //    ImUserInfoVM mineinfo = new()
        //    {
        //        username = Wtm.LoginUserInfo.Name,
        //        id = Wtm.LoginUserInfo.UserId,
        //        status = "online",
        //        avatar = Wtm.LoginUserInfo.PhotoId != null ? "/api/_file/getuserphoto/" + Wtm.LoginUserInfo.PhotoId : "",
        //        sign = userinfo.Sign,

        //    };
        //    var conn = ConnectionManagement.SignalRConns;
        //    var department = await DC.Set<Department>().AsNoTracking().ToListAsync();
        //    List<FriendInfoVM> FriendInfos = new();
        //    department.ForEach(async work =>
        //    {
        //        FriendInfoVM friendInfo = new();
        //        friendInfo.groupname = work.DepName;
        //        friendInfo.id = work.ID.ToString();
        //        var users = await DC.Set<FrameworkUser>().AsNoTracking()
        //        .Where(user => user.DepartmentId == work.ID)
        //        .Select(user => new ImUserInfoVM
        //        {
        //            username = user.Name,
        //            id = user.ID.ToString().ToLower(),
        //            status = ConnectionManagement.IsConn(user.ID.ToString()) == true ? "online" : "offline",
        //            avatar = user.PhotoId != null ? "/api/_file/getuserphoto/" + user.PhotoId : "",
        //            sign = user.Sign,

        //        })
        //        .ToListAsync();
        //        friendInfo.list = users;
        //        FriendInfos.Add(friendInfo);

        //    });

        //    var imInitUserData = new ImInitUserDataVM
        //    {
        //        mine = mineinfo,
        //        friend = FriendInfos
        //    };

        //    ImInitDataVM initData = new()
        //    {
        //        code = 0,
        //        msg = "",
        //        data = imInitUserData
        //    };

        //    return Ok(initData);


        //}
        #endregion


    }
}
