using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [Public]
    public class SessionController : BaseController
    {
        [Public]
        public IActionResult SessionTest([FromQuery]string num)
        {
            string[] lastTime = HttpContext.Session.Get<string[]>("sessiontest");
            //Random rand = new Random();
            //var res = new string[5];
            //for (int i = 0; i < 4; i++)
            //{
            //    res[i] = rand.Next(0, 10).ToString();
            //}
            //res[4] = HttpContext.Session.Id;
            var res = new string[2] { num, HttpContext.Session.Id };
            HttpContext.Session.Set<string[]>("sessiontest", res);
            string[] res1 = HttpContext.Session.Get<string[]>("sessiontest");
            string last = null;
            if (lastTime != null)
            {
                last = string.Join(',', lastTime);
            }
            //return new JsonResult(new
            //{
            //    Last = last,
            //    Current = string.Join(',', res),
            //    Writed = string.Join(',', res1)
            //});
            return Content(JsonConvert.SerializeObject(new
            {
                Last = last,
                Current = string.Join(',', res),
                Writed = string.Join(',', res1)
            }));
        }
    }
}
