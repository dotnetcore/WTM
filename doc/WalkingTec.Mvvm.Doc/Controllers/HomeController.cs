using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            LoginUserInfo = new LoginUserInfo { ITCode = "admin" };
            ViewData["title"] = "WalkingTec MVVM Framework";
            ViewData["menu"] = FFMenus?.AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, null, x => x.Url, SortByName: false);
            return View();
        }

        [Public]
        public IActionResult PIndex()
        {
            return View();
        }

        [AllRights]
        public IActionResult FrontPage()
        {
            return Redirect("/QuickStart/Intro");
        }

        [Public]
        [ActionDescription("捐赠名单")]
        public IActionResult DonateList()
        {
            return PartialView();
        }

        [Public]
        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            return PartialView();
        }

        [Public]
        [HttpGet]
        public IActionResult Menu()
        {
            var resultMenus = new List<menuObj>();
            GenerateMenuTree(FFMenus, resultMenus);
            return Content(JsonConvert.SerializeObject(new { Code = 200, Msg = string.Empty, Data = resultMenus }, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }), "application/json");
        }

        /// <summary>
        /// genreate menu
        /// </summary>
        private void GenerateMenuTree(List<FrameworkMenu> menus, List<menuObj> resultMenus)
        {
            resultMenus.AddRange(menus.Where(x => x.ParentId == null).Select(x => new menuObj()
            {
                Id = x.ID,
                Title = x.PageName,
                Url = x.Url,
                Order = x.DisplayOrder,
                ICon = x.ICon ?? $"_wtmicon _wtmicon-wenjian{(string.IsNullOrEmpty(x.Url) ? "jia" : string.Empty)}"
            })
            .OrderBy(x => x.Order)
            .ToList());

            foreach (var menu in resultMenus)
            {
                var temp = menus.Where(x => x.ParentId == menu.Id).Select(x => new menuObj()
                {
                    Id = x.ID,
                    Title = x.PageName,
                    Url = x.Url,
                    Order = x.DisplayOrder,
                    ICon = x.ICon ?? $"_wtmicon _wtmicon-wenjian{(string.IsNullOrEmpty(x.Url) ? "jia" : string.Empty)}"
                })
                .OrderBy(x => x.Order)
                .ToList();
                if (temp.Count() > 0)
                {
                    menu.Children = temp;
                    foreach (var item in menu.Children)
                    {
                        item.Children = menus.Where(x => x.ParentId == item.Id).Select(x => new menuObj()
                        {
                            Title = x.PageName,
                            Url = x.Url,
                            Order = x.DisplayOrder,
                            ICon = x.ICon ?? $"_wtmicon _wtmicon-wenjian{(string.IsNullOrEmpty(x.Url) ? "jia" : string.Empty)}"
                        })
                        .OrderBy(x => x.Order)
                        .ToList();

                        if (item.Children.Count() == 0)
                            item.Children = null;
                    }
                }
            }
        }

        public class menuObj
        {
            [JsonIgnore]
            public Guid Id { get; set; }

            /// <summary>
            /// Name
            /// 默认用不上name，但是 v1.2.1 有问题：“默认展开了所有节点，并将所有子节点标蓝”
            /// </summary>
            /// <value></value>
            [JsonProperty("name")]
            public string Name => Title;

            /// <summary>
            /// Title
            /// </summary>
            /// <value></value>
            [JsonProperty("title")]
            public string Title { get; set; }

            /// <summary>
            /// 图标
            /// </summary>
            /// <value></value>
            [JsonProperty("icon")]
            public string ICon { get; set; }

            /// <summary>
            /// 是否展开节点
            /// </summary>
            /// <value></value>
            [JsonProperty("spread")]
            public bool? Expand { get; set; }

            /// <summary>
            /// Url
            /// </summary>
            /// <value></value>
            [JsonProperty("jump")]
            public string Url { get; set; }

            [JsonProperty("list")]
            public List<menuObj> Children { get; set; }

            /// <summary>
            /// order
            /// </summary>
            /// <value></value>
            [JsonIgnore]
            public int? Order { get; set; }

        }

    }
}
