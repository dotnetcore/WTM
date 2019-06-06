using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI.Common;

namespace WalkingTec.Mvvm.Admin.Test
{
    public class MockController
    {
        public static T CreateController<T>(string dataseed, string usercode) where T:BaseController,new()
        {
            var _controller = new T();
            _controller.DC = new FrameworkContext(dataseed, DBTypeEnum.Memory);
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            MockHttpSession mockSession = new MockHttpSession();
            mockSession["UserInfo"] = new LoginUserInfo { ITCode = usercode??"user" };
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;
            _controller.ConfigInfo = new Configs();
            _controller.GlobaInfo = new GlobalData();
            _controller.UIService = new LayuiUIService();
            return _controller;
        }
    }
}
