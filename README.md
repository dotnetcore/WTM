English | [简体中文](./README.zh-CN.md) 

# WalkingTec.Mvvm for asp.net core

Walkingtec.mvvm framework (WTM) is a rapid development framework based on .net core. It supports LayUI, React, VUE. WTM has built-in code generator to maximize development efficiency. It is a powerful tool for efficient web development.

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

## WTM Features

WTM provides 4 types of ViewModel, covering all of the common functionalities of mainstream web applications. 

- CrudVM provides most common functionalities for data addition, deletion and modification.

- ListVM provides paging and exporting functionality.

- ImportVM & TemplateVM provides importing via excel functionality.

- BatchVM provides batch operation functionality.

- WTM has its own code generator, which makes development efficient and fast.

- WTM provides dozens of client-side controls, including Form, Grid, Panel, Dialog and quite alot of other common controls. 

- WTM provides built-in user, role, user group, Data permission, page permission, menu, log, mail, SMS, file and other common back-end  functionalities;

- WTM supports single sign on, portal and distributed database;

- WTM provides simplified integration with libraries such as Redis, DFS etc.

- WTM provides both server-side and client-side frameworks for building user interfaces.


| Mode | UI | Status  |
|--------- |------------- |---------|
|Server-side |LayUI |Stable|
|Client-side |React |Stable|
|Client-side |VUE |RTM|


Under WTM framework's client-side mode, you can also use code generator to generate server-side and client-side code at the same time, greatly reducing the communication cost of front-end and back-end developers, essentially improving the development efficiency, so that "separation" is no longer complex and expensive.

Framework document address: http://wtmdoc.walkingtec.cn

Framework Q&A community: https://community.walkingtec.cn

Frame QQ communication group: 694148336

## Click <a href="http://wtmdoc.walkingtec.cn/setup">here</a>  to generate a WTM project online and experience the beauty of WTM immediately~~~

At present, we are a team of 7 developers. We are looking for all kinds of C#, React, VUE experts to join us! 

If WTM hepls you:

<a href="https://www.paypal.me/dotnetWTM" target="_blank"><img src="https://wtmdoc.walkingtec.cn/imgs/pp_h_rgb.webp"  width="150"></a>
