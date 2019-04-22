# 更新日志

## v2.2.x (2018-12-20)
欢迎来到 WTM v2.2。此次更新主要是将 .Net Core 升级到 v2.2.0，并新增 Layui 组件。

### v2.2.35 (2019-4-22)

* **修改：** 修复代码生成器在Mac系统下无法工作的问题
* **修改：** 修复UpdateTime和UpdateBy字段没有自动写入的问题 

#### 前后端不分离模式

* **修改：** 修复checkbox为false，表单字段不提交的bug 

#### 前后端分离模式

* **修改：** 修复api在标记了Area属性时框架无法识别的bug 

### v2.2.34 (2019-4-20)

#### 前后端不分离模式

* **修改：** 修复菜单管理中，修改菜单的bug

#### 前后端分离模式

* **修改：** 修复IsQuickDebug为false时，api无法正常访问的问题
* **修改：** 修复Excel导入的bug

### v2.2.33 (2019-4-16)

#### Features

* **修改：** 修改前后端分离模式中表单提交的逻辑，现在表单中没有定义的字段将不会被更新
* **修改：** 修改前后端分离模式中框架自带的一些前台页面的bug
* **修改：** 修改BaseCRUDVM中的删除逻辑，现在可以正确删除include了其他关联类的Entity

### v2.2.32 (2019-4-16)

#### Features

* **新增：** React前后端分离模式RTM版本正式发布！欢迎大家使用

### v2.2.28 (2019-4-9)

#### Features

* **修改：** 系统菜单表新增ClassName和MethodName两个字符串字段，已有项目升级时可手动在库中添加这两个字段
* **修改：** React的前后端分离模式接近完成

### v2.2.26 (2019-4-1)

#### Features

* **新增：** 新增对pgsql的支持
* **修改：** 重写菜单管理逻辑，菜单配置更简便
* **修改：** React的前后端分离模式稳步推进

### v2.2.22 (2019-3-20)

#### Features

* **修改：** 代码生成器会在Controller和VM的类上加入partial，这样方便大家另起一个文件写自定义的代码，而不用担心再次生成代码的时候会被覆盖
* **修改：** 修改<wt:combobox>支持多选枚举的绑定
* **修改：** React的前后端分离模式增加了多个控件，修改了若干实际项目中反应的问题，向正式版又前进了一大步

### v2.2.10 (2019-2-27)

#### Features

* **新增：** 新增在线生成项目，http://wtmdoc.walkingtec.cn/setup ，生成WTM项目更快捷
* **修改：** 进一步完善React的前后端分离模式，当然目前还是预览

### v2.2.8 (2019-2-24)

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
