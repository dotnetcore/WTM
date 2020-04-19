using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI.Common;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Test.Mock
{
    public class MockController
    {
        public static T CreateController<T>(IDataContext dataContext, string usercode) where T : BaseController, new()
        {
            var _controller = new T();
            _controller.WtmContext = MockWtmContext.CreateWtmContext(dataContext, usercode);
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(WTMContext))).Returns(_controller.WtmContext);
            GlobalServices.SetServiceProvider(mockService.Object);
            return _controller;
        }

        public static T CreateApi<T>(IDataContext dataContext, string usercode) where T : BaseApiController, new()
        {
            var _controller = new T();
            _controller.WtmContext = MockWtmContext.CreateWtmContext(dataContext, usercode);
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            mockService.Setup(x => x.GetService(typeof(WTMContext))).Returns(_controller.WtmContext);
            GlobalServices.SetServiceProvider(mockService.Object);
            return _controller;
        }

    }
}
