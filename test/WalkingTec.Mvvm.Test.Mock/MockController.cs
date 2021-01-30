using System;
using Microsoft.AspNetCore.Http;
using Moq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Test.Mock
{
    public class MockController
    {
        public static T CreateController<T>(IDataContext dataContext, string usercode) where T : BaseController, new()
        {
            var _controller = new T();
            _controller.Wtm = MockWtmContext.CreateWtmContext(dataContext, usercode);
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            MockHttpSession mockSession = new MockHttpSession();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            mockHttpContext.Setup(x => x.Request).Returns(new DefaultHttpContext().Request);
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;
            return _controller;
        }

        public static T CreateApi<T>(IDataContext dataContext, string usercode) where T : BaseApiController, new()
        {
            var _controller = new T();
            _controller.Wtm = MockWtmContext.CreateWtmContext(dataContext, usercode);
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            MockHttpSession mockSession = new MockHttpSession();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            mockHttpContext.Setup(x => x.Request).Returns(new DefaultHttpContext().Request);
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;
            return _controller;
        }

    }
}
