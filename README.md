# WalkingTec.Mvvm for aspnet core

WalkingTec.Mvvm框架（简称WTM）最早开发与2013年，基于Asp.net MVC3 和 最早的Entity Framework, 当初主要是为了解决公司内部开发效率低，代码风格不统一的问题。经历了四年间数十个项目的考验，框架逐步的完善，推出了四个主要版本。 2017年9月，我们将代码移植到了.Net Core上，并进行了深度优化和重构，推出了基于Asp.net Core和EF Core的全新框架，新框架在架构，稳定性，速度上都有长足进步，真正成为一款高效开发的利器。

[![Coverage Status](https://coveralls.io/repos/github/dotnetcore/WTM/badge.svg?branch=master)](https://coveralls.io/github/dotnetcore/WTM?branch=master)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![Gitter](https://badges.gitter.im/dotnetcore/WTM.svg)](https://gitter.im/dotnetcore/WTM?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![GitHub license](https://img.shields.io/github/license/dotnetcore/WTM.svg)](https://github.com/dotnetcore/WTM/blob/master/LICENSE)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdotnetcore%2FWTM.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdotnetcore%2FWTM?ref=badge_shield)

## CI Build Status

| Platform | Build Server | Master Status  | Develop Status  |
|--------- |------------- |---------|---------|
| Azure Pipelines |  Linux |[![Build status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI-master-nuget.org)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=4) |[![Build status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI-develop-nuget.sundot)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=3)|
| AppVeyor |  Windows/Linux | - | - |
| Travis   | Linux/OSX | - | - |

## Nuget Packages

Package name                              | Version                     | Downloads
------------------------------------------|-----------------------------|-------------
`WalkingTec.Mvvm.Core` | [![NuGet](https://img.shields.io/nuget/v/WalkingTec.Mvvm.Core.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/WalkingTec.Mvvm.Core/) | ![downloads](https://img.shields.io/nuget/dt/WalkingTec.Mvvm.Core.svg)
`WalkingTec.Mvvm.Mvc` | [![NuGet](https://img.shields.io/nuget/v/WalkingTec.Mvvm.Mvc.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/WalkingTec.Mvvm.Mvc/) | ![downloads](https://img.shields.io/nuget/dt/WalkingTec.Mvvm.Mvc.svg)
`WalkingTec.Mvvm.Mvc.Admin` | [![NuGet](https://img.shields.io/nuget/v/WalkingTec.Mvvm.Mvc.Admin.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/WalkingTec.Mvvm.Mvc.Admin/) | ![downloads](https://img.shields.io/nuget/dt/WalkingTec.Mvvm.Mvc.Admin.svg)
`WalkingTec.Mvvm.TagHelpers.LayUI` | [![NuGet](https://img.shields.io/nuget/v/WalkingTec.Mvvm.TagHelpers.LayUI.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/WalkingTec.Mvvm.TagHelpers.LayUI/) | ![downloads](https://img.shields.io/nuget/dt/WalkingTec.Mvvm.TagHelpers.LayUI.svg)

框架主要特点：

框架提供了4类ViewModel，涵盖了主流Web应用程序常见的功能，分别是：
  BaseCRUDVM 提供最常见的数据增删改的功能
  PagedListVM 提供分页列表以及导出的功能
  ImportVM & TemplateVM 提供数据导入的功能
  BatchVM 提供批量操作的功能

框架自带代码生成器，开发高效快捷

框架提供了数十种前台控件，包括了Form,Grid,Panel,Dialog等几乎所有常用控件，在不进行前后分离的情况下，后端人员也可以轻松写出漂亮的前台页面。目前框架只支持Layui作为前端UI，后期我们会支持更多。

框架提供了内置的用户，角色，用户组，数据权限，页面权限，菜单，日志，邮件，短信，文件等后台常用管理功能

框架支持单点登录，门户Portal，分布式数据库

框架提供了Redis,DFS等后台开发常用库的简化操作


框架提供了前后端分离和不分离两种模式

| 模式 | UI | 状态  |
|--------- |------------- |---------|
|不分离 |LayUI |稳定|
|前后端分离 |React |RTM|
|前后端分离 |VUE |开发中|

WTM框架的前后端分离模式同样可以使用代码生成器同时生成前台和后台的代码，极大的降低了前后端人员的沟通成本，从本质上提升了开发效率，让“分离”不再复杂和昂贵。

框架文档地址：http://wtmdoc.walkingtec.cn  文档还在不断完善中。。。
框架QQ交流群：694148336


![WTM框架二维码](https://mmbiz.qpic.cn/mmbiz_jpg/L66Un3Tp12ria8hmdkjlfYQdOjA9dusW5xOOlS26GZTfk9Hs2uzHiaMXG4df96849seoGFiatGrqODTWPr7SsOqoA/0?wx_fmt=jpeg)

关注WTM微信公众号，及时了解框架更新，公众号还会定期发布视频教程

您可以点这里 http://wtmdoc.walkingtec.cn/setup 在线一键生成WTM的项目，立刻开始体验WTM之美~~~

