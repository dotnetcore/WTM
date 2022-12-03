using Microsoft.AspNetCore.SignalR;
using static Aliyun.OSS.Model.CreateSelectObjectMetaInputFormatModel;
using System.Threading.Tasks;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.ViewModels.ImVMs;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Demo.Hubs
{
    public class ChatHub : Hub
    {

        private WTMContext _wTMContext;
        public ChatHub(WTMContext wTMContext, IDataContext DC)
        {
            _wTMContext = wTMContext;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        #region IM
        /// <summary>
        /// 发送文字聊天
        /// </summary>
        /// <param name="connid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SenOneIm(string msg)
        {

            ImMsgVM imMsgVM = JsonSerializer.Deserialize<ImMsgVM>(msg);
            imMsgVM.timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            bool mine = imMsgVM.toid.ToUpper() == _wTMContext.LoginUserInfo.UserId.ToUpper() ? true : false;
            imMsgVM.mine = mine;
            string jsonString = JsonSerializer.Serialize(imMsgVM);
            var toconnid = ConnectionManagement.SignalRConns.FirstOrDefault(x => x.UserId.ToLower() == imMsgVM.toid.ToLower());
            await Clients.Client(toconnid?.ConnectionId).SendAsync("ReceiveOneIm", jsonString);
        }
        //改变状态
        public async Task ChangeStatus(string status)
        {
            if (status == "online")
            {
                await Clients.AllExcept(Context.ConnectionId).SendAsync("online", _wTMContext.LoginUserInfo.UserId);
            }
            else
            {
                await Clients.AllExcept(Context.ConnectionId).SendAsync("offline", _wTMContext.LoginUserInfo.UserId);
            }


        }

        //修改签名
        public async Task ChangeSign(string status)
        {
            var userinfo = await _wTMContext.DC.Set<FrameworkUser>().AsNoTracking().FirstOrDefaultAsync(x => x.ID == new Guid(_wTMContext.LoginUserInfo.UserId));
            userinfo.Sign = status;
            _wTMContext.DC.Set<FrameworkUser>().Update(userinfo);
            int x = await _wTMContext.DC.SaveChangesAsync();
            if (x > 0)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SignBack", "修改签名成功");
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SignBack", "修改签名失败");
            }

        }




        #endregion

        #region 连接事件
        /// <summary>
        /// 连接初始化事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            var connid = Context.ConnectionId;
            var userid = _wTMContext.LoginUserInfo.UserId;
            var chk = ConnectionManagement.IsConn(userid);
            if (chk == false)
            {
                SignalRConn signalRConn = new SignalRConn { UserId = userid, ConnectionId = connid };
                ConnectionManagement.AddConnInfo(signalRConn);
            }
            ConnectionManagement.EditConnInfo(userid, connid);


            Clients.AllExcept(Context.ConnectionId).SendAsync("online", _wTMContext.LoginUserInfo.UserId).Wait();


            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connid = Context.ConnectionId;
            ConnectionManagement.DelConnInfo(connid);
            Clients.AllExcept(Context.ConnectionId).SendAsync("offline", _wTMContext.LoginUserInfo.UserId).Wait();
            return base.OnDisconnectedAsync(exception);
        }

        #endregion




    }
}
