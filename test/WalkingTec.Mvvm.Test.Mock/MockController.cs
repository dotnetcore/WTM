using System;
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
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(WTMContext))).Returns(_controller.Wtm);
            return _controller;
        }

        public static T CreateApi<T>(IDataContext dataContext, string usercode) where T : BaseApiController, new()
        {
            var _controller = new T();
            _controller.Wtm = MockWtmContext.CreateWtmContext(dataContext, usercode);
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(WTMContext))).Returns(_controller.Wtm);
            return _controller;
        }

    }
}
