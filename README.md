English | [简体中文](./README.zh-CN.md) 

# WalkingTec.Mvvm for aspnet core

Walkingtec.mvvm framework (WTM) is a rapid development framework based on. Netcore. It supports LayUI, React, VUE. WTM has built-in code generator to maximize development efficiency. It is a powerful tool for efficient development.

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

Main features of the framework:

The framework provides 4 types of ViewModel, covering the common functions of mainstream web applications. They are:

CrudVM provides the most common function of data addition, deletion and modification;

ListVM provides the function of paging list and exporting

ImportVM & TemplateVM provides the function of importing via excel ;

BatchVM provides the function of batch operation.

The framework has its own code generator, which makes the development efficient and fast;

The framework provides dozens of client-side controls, including Form,Grid,Panel,Dialog and almost all common controls. Without client-side, the back-end developers can easily write beautiful client-side pages. Currently, the framework only supports LayUI as the front-end UI, and we will support more later;

The framework provides built-in user, role, user group, Data permission, page permission, menu, log, mail, SMS, file and other common back-end management functions;

The framework supports single sign on, portal and distributed database;

The framework provides simplified operation of common back-end development libraries such as Redis, DFS etc.

The framework provides both the server-side and client-side mode.


| Mode | UI | Status  |
|--------- |------------- |---------|
|Server-side |LayUI |Stable|
|Client-side |React |Stable|
|Client-side |VUE |RTM|


Under WTM framework's client-side mode, you can also use code generator to generate server-side and client-side code at the same time, greatly reducing the communication cost of front-end and back-end developers, essentially improving the development efficiency, so that "separation" is no longer complex and expensive.

Framework document address: http://wtmdoc.walkingtec.cn

Framework Q&A community: https://community.walkingtec.cn

Frame QQ communication group: 694148336

You can click here http://wtmdoc.walkingtec.cn/setup to start a WTM project online and experience the beauty of WTM immediately~~~

At present, we are a team of 7 developers. We are looking for all kinds of C#, React, VUE experts to join us! 

If WTM hepls you:

<a href="https://www.paypal.me/dotnetWTM" target="_blank"><img src="https://wtmdoc.walkingtec.cn/imgs/pp_h_rgb.webp"  width="150"></a>
