using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("test api")]
    [ApiController]
    [Route("api/test")]
	public partial class TestController : BaseApiController
    {
        [HttpGet("DllImportReady")]
        public IActionResult DllImportReady([FromServices] WtmFileProvider fp, string id)
        {
            return Ok();
        }

        [HttpPost("DllImportDB")]
        public IActionResult DllImportDB([FromServices] WtmFileProvider fp, string id, Guid projectId, List<string> selected)
        {
            return Ok();
        }


    }
}
