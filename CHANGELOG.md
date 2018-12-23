# 更新日志

## v2.2.x (2018-12-20)
欢迎来到 WTM v2.2。此次更新主要是将 .Net Core 升级到 v2.2.0，并新增 Layui 组件。

### v2.2.1 (2018-12-23)

#### Features

* **修改：** 修改文件上传相关配置，将`SaveFileMode`及`UploadDir` 更改到 `FileUploadOptions`中

#### Bug Fixes

* 解决.net core 2.2下IIS Inprogress运行的问题 ([90256fe](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/90256fe))




### v2.2.0 (2018-12-20)

#### Features

* **新增：** 添加富文本组件
* **新增：** 添加自定义路由的简便入口
* **修改：** CrossDomainAttribute现在可以指定允许的域名
