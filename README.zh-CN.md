[English](./README.md) | 简体中文 


# WalkingTec.Mvvm for aspnet core

WalkingTec.Mvvm框架（简称WTM）是基于.net core的快速开发框架。支持Layui(前后端不分离), React(前后端分离),VUE(前后端分离)，内置代码生成器，最大程度的提高开发效率，是一款高效开发的利器。

[![Coverage Status](https://coveralls.io/repos/github/dotnetcore/WTM/badge.svg?branch=master)](https://coveralls.io/github/dotnetcore/WTM?branch=master)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![Gitter](https://badges.gitter.im/dotnetcore/WTM.svg)](https://gitter.im/dotnetcore/WTM?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![GitHub license](https://img.shields.io/github/license/dotnetcore/WTM.svg)](https://github.com/dotnetcore/WTM/blob/master/LICENSE)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdotnetcore%2FWTM.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdotnetcore%2FWTM?ref=badge_shield)

## CI Build Status

| Platform | Build Server | SDK | Master Status  | Develop Status  |
| -------- | ------------ | ---- |---------|---------|
| Azure Pipelines |  Windows/Linux/OSX |  v2.2.300 | [![Build Status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI?branchName=master)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=12&branchName=master) | [![Build Status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI?branchName=develop)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=12&branchName=develop) |
| Azure Pipelines |  Windows/Linux/OSX |  v3.1.101 | [![Build Status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI-sdk_v3.1?branchName=feature/dotnet3)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=28&branchName=master) | [![Build Status](https://dev.azure.com/vitowu/WTM/_apis/build/status/WTM-CI-sdk_v3.1?branchName=feature/dotnet3)](https://dev.azure.com/vitowu/WTM/_build/latest?definitionId=28&branchName=feature/dotnet3) |
| AppVeyor |  Windows/Linux |  v2.2.300 | - | - |
| Travis   | Linux/OSX |  v2.2.300 | - | - |

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
|前后端分离 |React |稳定|
|前后端分离 |VUE |RTM|

WTM框架的前后端分离模式同样可以使用代码生成器同时生成前台和后台的代码，极大的降低了前后端人员的沟通成本，从本质上提升了开发效率，让“分离”不再复杂和昂贵。



![WTM框架微信公众号](https://wtmdoc.walkingtec.cn/imgs/gongzhonghao.jpg)

关注WTM微信公众号，及时了解框架更新，公众号还会定期发布视频教程

框架文档地址：http://wtmdoc.walkingtec.cn

框架问答社区：https://community.walkingtec.cn

框架QQ交流群：694148336

## 点<a href="http://wtmdoc.walkingtec.cn/setup">这里</a>在线一键生成WTM的项目，立刻开始体验WTM之美~~~

****
目前我们是一个7人团队在维护这个项目，诚征各路C#，React，VUE高手加入！有意者可以进QQ群加群主详聊。
****


最后感谢各位老板的支持和捐赠，具体捐赠信息请见 https://wtmdoc.walkingtec.cn/#/Home/DonateList

如果WTM帮助到了你，欢迎赞助点让我们持续建造更好的轮子：

<img src="https://wtmdoc.walkingtec.cn/imgs/WTM-Ali.png"  height="300" width="300">

