# 更新日志

## v2.2.x (2018-12-20)
欢迎来到 WTM v2.2。此次更新主要是将 .Net Core 升级到 v2.2.0，并新增 Layui 组件。

### v2.2.8 (2018-2-24)

#### Features

* **新增：** 框架开始支持前后端分离模式，可以创建React的前后端分离模式的项目，并生成前后端分离模式的代码
* **新增：** 新增MiddleTableAttribute，对于多对多的关系，可以在中间表的模型上标记[MiddleTable]，框架的代码生成器根据这个标记可以正确生成多对多关系的增删改查代码

### v2.2.4 (2019-01-11)

#### Bug Fixes

* 修改默认连接字符串的bug ([318840c](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/318840c))
* 修改密码的bug ([1427fb7](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/1427fb7))

### v2.2.3 (2019-01-08)

#### Bug Fixes

* 修改外部地址菜单刷新的bug ([918560f](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/918560f))

### v2.2.2 (2019-01-04)

#### Bug Fixes

* 修改富文本必填验证的bug ([e9f2cd0](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/e9f2cd0))


### v2.2.1 (2018-12-23)

#### Features

* **修改：** 修改文件上传相关配置，将 `SaveFileMode` 及 `UploadDir` 更改到 `FileUploadOptions` 中

#### Bug Fixes

* 解决 .net core 2.2下 IIS Inprogres s运行的问题 ([90256fe](https://github.com/WalkingTec/WalkingTec.Mvvm/commit/90256fe))


### v2.2.0 (2018-12-20)

#### Features

* **新增：** 添加富文本组件
* **新增：** 添加自定义路由的简便入口
* **修改：** CrossDomainAttribute现在可以指定允许的域名
