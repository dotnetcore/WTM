using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
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


namespace WalkingTec.Mvvm.BlazorDemo.Test
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
            _controller.Wtm.MSD = new ModelStateServiceProvider(_controller.ModelState);
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
            _controller.Wtm.MSD = new ModelStateServiceProvider(_controller.ModelState);
            return _controller;
        }

    }

    public class MockCookie : IRequestCookieCollection
    {

        public Dictionary<string, string> Kvs = new Dictionary<string, string>();

        public string this[string key] => Kvs[key];

        public int Count => Kvs.Count;

        public ICollection<string> Keys => Kvs.Keys;

        public bool ContainsKey(string key)
        {
            return Kvs.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Kvs.GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            return Kvs.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Kvs.GetEnumerator();
        }

        public void Add(string key, string value)
        {
            Kvs.Add(key, value);
        }
    }

    public class MockHttpSession : ISession
    {
        Dictionary<string, object> sessionStorage = new Dictionary<string, object>();
        public object this[string name]
        {
            get { return sessionStorage[name]; }
            set { sessionStorage[name] = value; }
        }

        string ISession.Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        bool ISession.IsAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        IEnumerable<string> ISession.Keys
        {
            get { return sessionStorage.Keys; }
        }
        void ISession.Clear()
        {
            sessionStorage.Clear();
        }
        Task ISession.CommitAsync(CancellationToken cancellationToken)
        {
            Task t = new Task(() =>
            {
            });
            t.Start();
            return t;
        }

        Task ISession.LoadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void ISession.Remove(string key)
        {
            sessionStorage.Remove(key);
        }

        void ISession.Set(string key, byte[] value)
        {
            sessionStorage[key] = value;
        }

        bool ISession.TryGetValue(string key, out byte[] value)
        {
            if (sessionStorage.ContainsKey(key) && sessionStorage[key] != null)
            {
                value = (byte[])sessionStorage[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }

    public class MockMSD : IModelStateService
    {
        private Dictionary<string,string> _states;

        public MockMSD()
        {
            this._states = new Dictionary<string, string>();
        }

        public List<MsdError> this[string name]
        {
            get
            {
                return _states.Where(x=>x.Key == name).Select(x => new MsdError { ErrorMessage = x.Value}).ToList();
            }
        }

        public void AddModelError(string key, string errorMessage)
        {
            _states.Add(key, errorMessage);
        }

        public void RemoveModelError(string key)
        {
            _states.Remove(key);
        }

        public void Clear()
        {
            _states.Clear();
        }

        public string GetFirstError()
        {
            string rv = "";
            foreach (var key in Keys)
            {
                if (this[key].Count > 0)
                {
                    rv = this[key].First().ErrorMessage;
                }
            }
            return rv;
        }

        public int Count => _states.Count;

        public IEnumerable<string> Keys => _states.Keys;

        bool IModelStateService.IsValid => _states.Count>0?true:false;
    }

    public static class MockWtmContext
    {
        public static WTMContext CreateWtmContext(IDataContext dataContext= null, string usercode = null)
        {
            GlobalData gd = new GlobalData();
            gd.AllAccessUrls = new List<string>();
            gd.AllAssembly = new List<System.Reflection.Assembly>();
            gd.AllModule = new List<WalkingTec.Mvvm.Core.Support.Json.SimpleModule>();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            Mock<HttpRequest> mockHttpRequest = new Mock<HttpRequest>();
            Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            MockHttpSession mockSession = new MockHttpSession();
            mockHttpRequest.Setup(x => x.Cookies).Returns(new MockCookie());
            mockService.Setup(x => x.GetService(typeof(IDistributedCache))).Returns(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())));
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(x => x.RequestServices).Returns(mockService.Object);
            var httpa = new HttpContextAccessor();
            httpa.HttpContext = mockHttpContext.Object;
            var wtmcontext = new WTMContext(null, new GlobalData(), httpa, new DefaultUIService(), null,dataContext, new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()));
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
            wtmcontext.LoginUserInfo = new LoginUserInfo { ITCode = usercode ?? "user" };
            wtmcontext.GlobaInfo.AllAccessUrls = new List<string>();
            wtmcontext.GlobaInfo.AllAssembly = new List<System.Reflection.Assembly>();
            wtmcontext.GlobaInfo.AllModule = new List<WalkingTec.Mvvm.Core.Support.Json.SimpleModule>();
            mockService.Setup(x => x.GetService(typeof(WtmFileProvider))).Returns(new WtmFileProvider(wtmcontext));
            return wtmcontext;
        }
    }
}
