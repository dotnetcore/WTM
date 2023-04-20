using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Test.Mock
{
    public static class MockWtmContext
    {
        public static WTMContext CreateWtmContext(IDataContext dataContext= null, string usercode = null)
        {
            GlobalData gd = new GlobalData();
            gd.AllAccessUrls = new List<string>();
            gd.AllAssembly = new List<System.Reflection.Assembly>();
            gd.AllModule = new List<Core.Support.Json.SimpleModule>();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            Mock<HttpRequest> mockHttpRequest = new Mock<HttpRequest>();
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            MockHttpSession mockSession = new MockHttpSession();
            mockHttpRequest.Setup(x => x.Cookies).Returns(new MockCookie());
            var cache = new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
            var res = new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory());
            mockService.Setup(x => x.GetService(typeof(IDistributedCache))).Returns(cache);
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.RequestServices).Returns(mockService.Object);
            var httpa = new HttpContextAccessor();
            httpa.HttpContext = mockHttpContext.Object;
            var wtmcontext = new WTMContext(null, new GlobalData(), httpa, new DefaultUIService(), null,dataContext, res, cache:cache);
            wtmcontext.MSD = new BasicMSD();
            wtmcontext.Session = new SessionServiceProvider(mockSession);
            if (dataContext == null)
            {
                wtmcontext.DC = new EmptyContext(Guid.NewGuid().ToString(), DBTypeEnum.Memory);
            }
            else
            {
                wtmcontext.DC = dataContext;
            }
            wtmcontext.SetLoginUserInfo (new LoginUserInfo { ITCode = usercode ?? "user" });
            wtmcontext.GlobaInfo.AllAccessUrls = new List<string>();
            wtmcontext.GlobaInfo.AllAssembly = new List<System.Reflection.Assembly>();
            wtmcontext.GlobaInfo.AllModule = new List<Core.Support.Json.SimpleModule>();
            mockService.Setup(x => x.GetService(typeof(WtmFileProvider))).Returns(new WtmFileProvider(wtmcontext));
            return wtmcontext;
        }
    }
}
