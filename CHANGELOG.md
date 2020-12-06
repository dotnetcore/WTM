# 更新日志

## v3.x.x （2.x.x同步更新）

##3.7.9 以及 2.7.9 (2020-11-29)
* **修改：**  修复layui模式下搜索没有重置页码的bug
* **修改：**  修复DpWhere方法在某些情况下报错的bug

##3.7.8 以及 2.7.8 (2020-11-24)
* **修改：**  彻底修复GetProperty反射与自定义主键引发的一些冲突


##3.7.7 以及 2.7.7 (2020-11-21)
* **修改：**  修复了DpWhere方法的一些bug
* **修改：**  Layui默认的数据权限管理添加了搜索功能
* **修改：**  <wt:grid>增加了auto-search属性，用来控制是否自动进行搜索，默认是true
* **修改：**  优化了一些框架自动生成的表达式语句
* **修改：**  将内部引用的.netcore mvc和ef相关的包升级到了最新版

##3.7.6 以及 2.7.6 (2020-11-15)
* **修改：**  TreeContainer现在可以设置默认选中的项目，通过设置TreeSelectListItem中的Selected属性
* **修改：**  修复了数据权限在过滤自定义主键的模型时的bug
* **修改：**  修复若干小问题

##3.7.5 以及 2.7.5 (2020-10-19)
* **修改：**  Layui模式下Selector控件也可以使用trigger-url和link-field来联动其他控件了（鸣谢AaronLucas)
* **修改：**  修复了多表头列表导出excel时表头显示错误的问题
* **修改：**  修复VUE左侧菜单收缩后不显示图标的问题

##3.7.4 以及 2.7.4 (2020-9-22)
* **修改：**  修复Layui模式下多选Combobx联动的问题
* **修改：**  优化CheckBetween方法所生成的sql语句
* **修改：**  修复ListVM中以数组作为列生成的json错误的问题
* **修改：**  修复了使用保存的cookie在登出后仍然可以访问系统的问题

##3.7.3 以及 2.7.3 (2020-9-12)
* **修改：**  修复Layui模式下数据权限管理页面的bug
* **修改：**  修复Layui模式下用户管理无法取消角色和用户组的bug
* **修改：**  修复<wt:tree>控件无法取消选择的bug
* **修改：**  修复链接多个数据库时有一个无法连接就会导致系统无法启动的问题
* **修改：**  修复ListVM在对PersistPoco搜索时如果数据源不是PersistPoco报错的问题

##3.7.2 以及 2.7.2 (2020-9-7)
* **修改：**  修改默认跨域逻辑，不配置跨域信息会默认允许所有跨域
* **修改：**  修复RefreshToken方法在pgsql中报错的问题
* **修改：**  修复菜单管理模块显示的bug，以及修改菜单会造成其他角色都有权限的bug
* **修改：**  修复多表头导出时表头显示的问题
* **修改：**  修改导入逻辑，默认不启用sqlserver的bulkcopy，可以通过在ImportVM中设置UseBulkSave属性来启用

##3.7.1 以及 2.7.1 (2020-8-26)
* **新增：**  Layui模式下新增<wt:colorpicker>颜色选择器，参考文档 https://wtmdoc.walkingtec.cn/#/UI/ColorPicker
* **修改：**  修复<wt:checkbox>多选显示的bug
* **修改：**  修复了一些layui模式下自带的系统管理页面
* **修改：**  修复了导出excel时数字格式为null值时报错的问题
* **修改：**  修复了FixConnectionAttribute没有优先检查方法的问题
* **修改：**  修复了IssueTokenAsync方法并发时报错的问题

##3.7.0 以及 2.7.0 (2020-8-18)

* **修改：**  导出excel时，如果ListVM中设置了多表头，导出的excel也会显示多表头
* **修改：**  修复了3.x版本中ListVM调用存储过程报错的问题，并更新了相关文档
* **修改：**  修复了layui下SearchPanel的重置按钮没有正确重置表单的问题
* **修改：**  修复了layui下多表头的列表显示错位的问题
* **修改：**  新生成的3.x的项目添加了RuntimeCompilation，并默认设置为调试模式下Razor页面可以动态编译
* **修改：**  修复VUE和React模式中菜单管理设置为不显示菜单时仍然显示的错误
* **修改：**  修复VUE和React模式中获取和显示文件的接口不受IsFilePublic属性控制的问题

##3.6.9 以及 2.6.9 (2020-7-23)
* **修改：**  修改了PersistPoco的假删除逻辑
* **修改：**  修复了layui下使用TreeContainer搜索，搜索条件会消失的bug
* **修改：**  导出excel时，如果时数字格式，现在会自动把excel的列设置成数值格式，方便再加工
* **修改：**  修复了使用代码生成器生成api时会生成一些无用代码的bug
* **修改：**  layui下<wt:grid>增加了line-height属性，可以指定行高，适合列表显示图片之类的情况使用
* **修改：**  ListVM中DateTime类型的字段默认格式改回yyyy-MM-dd hh:mm:ss
* **修改：**  修复了admin中修改用户密码，输入过长时显示的bug
* **修改：**  修复VUE中代码生成列表搜索框默认控件数量不对的bug
* **修改：**  修复VUE中列表没有充满屏幕的bug

##3.6.8 以及 2.6.8 (2020-7-6)
* **修改：**  增强了对oracle的支持，注：3.x版本的oracle仍然是beta版，可跑起框架，但可能有未知问题
* **修改：**  修复了layui搜索相关的一些bug
* **修改：**  代码生成器现在对于bool类型的变量在layui下默认使用<wt:switch>控件
* **修改：**  ListVM中DateTime类型的字段默认使用yyyy-MM-dd的格式
* **修改：**  appsettings中的UploadLimit修改为long类型，可是指定更大的数字
* **修改：**  修复VUE中没有角色的用户登录时重复刷新的bug
* **修改：**  修复VUE中列表没有充满屏幕的bug

##3.6.7 以及 2.6.7 (2020-6-30)
3.6.6/2.6.6 热更新

##3.6.6 以及 2.6.6 (2020-6-30)
* **修改：**  修复sqlserver bulk导入时枚举类型没有正确赋值的问题
* **修改：**  Layui修复了Combobox，CheckBox，Radio等控件设定default-value无效的问题
* **修改：**  Layui修复了Combobox禁用无效的问题
* **修改：**  Layui修复了取消某个搜索条件仍然按之前条件搜索的问题
* **修改：**  Layui修复了按钮组的显示问题
* **修改：**  VUE修复了编辑后再添加id重复的问题
* **修改：**  VUE修复了数据权限管理页面错误的问题
* **修改：**  VUE和React修复了菜单有目录的情况下排序的问题
* **修改：**  修改了一些多语言英文文本

##3.6.5 以及 2.6.5 (2020-6-15)
* **修改：**  修复操作列和菜单的多语言问题
* **修改：**  修复一对多删除时某些情况下失败的问题
* **修改：**  修复代码生成器生成的api单元测试报错的问题

##3.6.4 以及 2.6.4 (2020-6-3)
* **修改：**  修复了默认初始化数据找不到Action报错的问题
* **修改：**  修复了代码生成器在关联多个外键的同名字段时，生成的列表显示错误的问题
* **修改：**  修复了权限认证时没有正确处理Async方法的问题
* **修改：**  修复了LayUI模式下SetBindVisiableColName失效的问题
* **修改：**  修复了VUE模式下菜单模块的多语言显示错误的问题
* **修改：**  修复了React模式下一些文字错误

##3.6.3 以及 2.6.3 (2020-5-26)
* **修改：**  修复了上一版本引发的搜索报错的问题
* **修改：**  修复了bulk导入的一个小bug
* **修改：**  代码生成器生成页面时加入了多语言，老项目请在_ViewImports.cshtml文件中加入一行 @using Microsoft.Extensions.Localization;

##3.6.2 以及 2.6.2 (2020-5-25)
* **修改：**  修复了包含自定义列名的模型导入失败的bug
* **修改：**  修复了Index页面没有正确判断页面权限的bug
* **修改：**  现在Searcher也可以写Validate方法，查询条件后台返回的错误可以正确显示
* **修改：**  修复了控制台Log没有显示时间的bug
* **修改：**  修复了React和Vue配置页面权限的文字错误

##3.6.1 以及 2.6.1 (2020-5-22)
3.6.0/2.6.0 的热更新，修复代码生成器生成Controller的一个bug

##3.6.0 以及 2.6.0 (2020-5-22)
* **新增：**  导出优化，支持xlsx格式，单个excel文件现在最大可导出100万行，可设置单个文件最大行数，超过最大行数时会自动下载包含多个excel文件的zip包。详情请参见文档https://wtmdoc.walkingtec.cn/#/VM/Export
* **新增：**  导入优化，支持xlsx格式，支持公式，使用sqlserver时自动使用bulk导入，提高大批量数据的导入速度。
* **新增：**  ListVM中的MakeGridHeader方法现在可以正确绑定任何其定义lambda表达式
* **新增：**  修复有关联关系的数据无法正常删除的bug
* **修改：**  Layui模式中所有按钮的TagHelper现在都可以指定confirm-text来弹出一个询问框
* **修改：**  Layui模式修复默认下载按钮失效的bug
* **修改：**  Layui模式修复Display TagHelper绑定附件时显示错误的bug
* **修改：**  React模式修复代码生成器生成一对多控件时的问题
* **修改：**  Vue模式增加多语言支持
* **修改：**  VUE模式修复一些近期反馈的小bug



##3.5.7 以及 2.5.7 (2020-5-6)
* **新增：**  SubmitButton中新增SubmitUrl属性，用于多个提交按钮提交到不同的地址
* **新增：**  BaseController和BaseApiController增加可重写的GetLoginUserInfo方法，用于自定义用户认证
* **修改：**  优化认证逻辑，加快响应速度
* **修改：**  修复jwt无效时返回登录界面的错误，现在可以正确返回401，修复jwt token过期时间不准确的问题
* **修改：**  完善多语言支持
* **修改：**  修复DoDelete和SetInclude冲突的bug
* **修改：**  VUE修复菜单空目录bug
* **修改：**  VUE修复权限配置和搜索的bug
* **修改：**  React完善多语言支持

##3.5.6 以及 2.5.5 (2020-4-13)
* **新增：**  ConnectionString配置中可以设置Version字段，用于控制mysql的具体版本
* **修改：**  移除了动态控制器，因为和动态编译页面产生冲突
* **修改：**  IsFilePublic现在可以正常工作
* **修改：**  更新了默认生成的VUE项目代码，修复了一些bug

##3.5.4 以及 2.5.4 (2020-4-3)
* **新增：**  新增了动态控制器，老项目需要手动在Project文件的 \<PropertyGroup\>中加入\<PreserveCompilationReferences>true</PreserveCompilationReferences\>节点

* **修改：**  修复vue代码生成没有正确生成某些api的bug
* **修改：**  修复vue自带系统管理中的一些bug，整体更稳定
* **修改：**  IsFilePublic配置在3.x下可以正常工作
* **修改：**  修复框架自带GetFile和ViewFile方法无法正常调用的bug

##3.5.2 以及 2.5.2 (2020-3-29)
* **修改：**  修复vue代码生成下拉菜单少了一个逗号的bug
* **修改：**  修复vue发布时的问题
* **修改：**  修复vue列表高度计算的问题
* **修改：**  修复vue数据权限列表的删除bug

* **新增：**  Layui. 现在ListVM中的GridAction可以通过SetButtonClass方法设置按钮颜色
* **新增：**  Layui. UIService中新增MakeButton方法替换之前有问题的MakeRedirectButton方法
* **修改：**  修复GetGridActions会被调用两次的问题（这其实是.netcore的bug...)


##3.5.1 以及 2.5.1 (2020-3-26)
* **修改：**  修复vue菜单相关的一些bug
* **修改：**  修复vue代码生成器对于布尔值的控件生成的bug
* **修改：**  修复vue代码生成器对于下拉菜单生成的bug

##3.5.0 以及 2.5.0 发布，你心心念的Vue来了！！！vue目前还属于预览版，欢迎大家多提宝贵意见
* **新增：**  现在官网可以生成Vue的项目了
* **新增：**  VUE项目可以使用和Layui，React相同的代码生成
* **新增：**  appsettings文件中增加了Domains的配置，用来注册httpclient。在Controller和VM中通过ConfigInfo.Domains["key"].CallAPI来方便高效的调用其他网站的api
* **修改：**  修复代码生成器会将bool的搜索条件啊生成两次的bug
* **修改：**  修复继承自TopBasePoco的Model在DoAdd中没有正确的添加子表数据的bug
* **修改：**  修复用户没有权限时没有正确返回401错误的bug

## v3.1.x

3.1版本正式发布，支持.netcore 3.1，与2.4.x最新版本在功能上同步更新


## v2.4.x

v2.4.9(2020-3-15)
* **修改：**  重构日志，使用.netcore默认的日志记录流程和规则。 在.ConfigureLogging中可以使用AddWTMLogger来添加WTM的日志功能，并可以在appsetting文件中配置Logging来指定需要记录日志的级别，就像你操作其他Console，Debug这些日志一样。
* **修改：**  修复layui下日期控件默认显示当前日期的问题
* **修改：**  修复form和其中的searchpanel同时指定label-width会报错的问题
* **修改：**  代码生成现在会默认为DateTime类型的搜索条件生成时间区间的搜索
* **修改：**  修复了jwt认证失败没有正确返回401的问题

v2.4.7(2020-3-9)

* **新增：**  现在Layui模式下列表可以列筛选和打印
* **新增：**  现在ListVM中的Action按钮可以通过SetPromptMessage设置询问对话框
* **新增：**  现在数据权限可以识别多对多和树形结构

* **修改：**  修改了新生成的项目LoginVM和RegVM错位的问题
* **修改：**  修复了设置不分页不起作用的bug
* **修改：**  修改了view强制要求model继承BaseVM的bug
* **修改：**  修复了Combobox在disable状态下的显示问题
* **修改：**  修复了代码生成器在多个DataContext时候的生成问题
* **修改：**  修复了SearchPanel中Combobox多选时提交数据错误的问题

v2.4.6(2020-2-22)
本次更新加入了在连接字符串上指定数据库类型和DataContext的功能，并修复了近一阶段的bug。
* **新增：**  现在在appsettings中的ConnectionStrings里面可以指定每一个连接字符串的DbType和DbContext
* **新增：**  现在新增了一个EmptyContext基类，FrameworkContext会包含框架自带的表，而EmptyContext不会，这对于我们使用WTM连接其他系统的数据库十分有用
* **注意：**  老版本升级时需要在DataContext文件中加入一个新的构造函数：
        public DataContext(CS cs)
             : base(cs)
        {
        }
* **新增：**  增加了NoLog标记，用来指定某个方法不记录系统日志
* **修改：**  移除了不必要的验证，提升webapi的响应速度
* **新增：**  Layui模式下登陆页面新增了用户注册的演示页面
* **修改：**  修复了layui模式下autocomplete textbox在有初始值时的js错误
* **修改：**  修复了layui模式下可编辑grid表头错位的bug
* **修改：**  修复了PIndex页面的js错误
* **修改：**  移除了React模式下对node-sass的依赖
* **修改：**  移除了React模式下数据权限管理的bug
* **修改：**  修复了WebApi在非调试模式下权限认证的bug
* **修改：**  修复了代码生成器生成React菜单的bug
* **修改：**  修复了代码生成器生成标记了[Range(xxx.Max)]字段的bug






v2.4.5 (2020-1-4)
本次为累积更新，修复了一个月以来issue上提出的主要bug
* **修改：**  修复了获取PersistPoco的下拉选项时，没有过滤IsVaild=false的问题
* **修改：**  修复了弹出窗口在手机上显示不全的问题
* **修改：**  修复了UEditor单图上传错误的问题
* **修改：**  修复当搜索条件只有一个时，在搜索框中按回车键会出现异常页面的问题
* **修改：**  修复了Transfer 穿梭框显示问题的问题
* **修改：**  修复了在form表达外使用ImageTagHelper 会获得一个异常的问题
* **修改：**  修复了textbox加了padding-text之后tab无法切换的问题
* **修改：**  修复了selector display="true"时，显示错误的问题
* **修改：**  修复了使用一些第三方控件导致view无法显示的问题
* **修改：**  修复了代码生成器中点击关闭按钮报错的问题
* **修改：**  修复了连续三级空菜单没有隐藏的问题
* **修改：**  React模式系统自带管理模块加入了中英文多语言

* **新增：**  upload控件增加了进度条，通过设置ShowProgress可以选择是否显示（鸣谢 ‘阿拉斯没有家’同学 https://github.com/buffonlwx）
* **新增：**  ListVM中的GridAction中增加了下载类型的按钮
* **新增：**  BaseController中的CreateDC方法现在可以使用连接字符串的key，而不需要写死整个连接字符串
* **新增：**  BaseController中的CreateDC方法现在可以指定数据库类型
* **新增：**  菜单维护时外部菜单可以使用/aaa/bbb的形式来指定一个内部地址，这样方便大家把一个具体方法配置到左侧菜单上

### v2.4.3 / v3.0.4 (2019-12-8)

* **新增：**  增加多附件上传控件，特别鸣谢‘草监牛寺’同学，参见文档 https://wtmdoc.walkingtec.cn/#/UI/UploadMulti
* **修改：**  Appsettings文件中增加了IsOldSqlServer配置，对于使用sqlserver 2008以前的用户使用
* **修改：**  修复某些模型生成单元测试时的bug
* **修改：**  修复自定义ID的模型attach时可能失败的bug
* **修改：**  修复主子表操作时没有判断PersistPoco的bug


### v2.4.2 (2019-11-22)

* **修改：**  修复Add-Migration报错的问题
* **修改：**  修复刷新菜单无效的问题
* **修改：**  修复无法添加多级菜单的问题
* **修改：**  修复在导出时SearchMode仍然为Search的问题
* **修改：**  修复权限控制无法识别中文url的问题

### v2.4.1 (2019-11-16)

* **修改：**  修复同时使用Cookie和jwt登陆时报错的bug（不建议混合两种模式）

#### 前后端不分离模式
* **修改：**  修复Combobox联动由于没有图表而报错的bug

#### React前后端分离模式
* **新增：**  多语言支持
* **修改：**  修复菜单地址bug
* **修改：**  修复菜单管理，数去权限管理页面问题

### v2.4.0 (2019-11-5)
本次更新为大版本更新，废弃了之前Session的模式，使用Jwt和cookie两种方式进行登陆认证。
框架目前支持Cookie和Jwt两种模式，继承BaseController和BaseApiController的控制器将默认支持Cookie模式。
已有使用session认证的代码不需要修改，用户使用过程中并不会感觉到变化。
用户可以通过[AuthorizeCookie],[AuthorizeJwt],[AuthorizeJwtWithCookie]三种标签来指定Controller的验证方式。
详情请参考https://wtmdoc.walkingtec.cn/#/Global/jwt
系统增加了persistedgrants表来存储jwt持久化信息，另外菜单的默认数据也发生了改变，建议已有系统重新生成数据库或手动同步数据库

* **新增：**  Jwt支持
* **新增：**  Swagger jwt支持
* **修改：**  修复多语言验证信息bug
* **修改：**  菜单管理支持不同Area下同名Controller的配置


## v2.3.x

### v2.3.9 (2019-10-19)

* **新增：**  多语言支持。https://wtmdoc.walkingtec.cn/#/Global/MultiLanguages
老版本升级后会遇到单元测试项目中MockController.cs文件报错，将报错的行替换为
_controller.GlobaInfo.SetModuleGetFunc(() => new List\<FrameworkModule\>());
即可。

* **新增：**  dotnet 3.0支持，线上新建项目时可选择dotnetcore3.0版本的项目

#### 前后端不分离模式
* **新增：**  集成了UEditor。https://wtmdoc.walkingtec.cn/#/UI/UEditor
* **新增：**  列表按钮现在可以设置Max属性，来控制打开窗体时最大化
* **修改：**  现在View页面不再强制要求Model必须继承BaseVM
* **修改：**  修改菜单无法删除的历史遗留bug

### v2.3.6 (2019-9-27)

* **新增：**  Debug模式下，debug窗口会输出ef执行的sql语句
* **修改：**  移除EnableCors属性，集成dotnetcore自带的Cors实现跨域，并可在appsettings文件中进行配置

#### 前后端不分离模式
* **修改：**  代码生成器会为Controller生成独立的搜索和导出方法，方便对搜索和导出进行权限控制，之前公共方法仍然保留
* **修改：**  修复IE11下的显示问题
* **修改：**  修复PersistPoco导入时没有给IsValid赋值的问题
* **修改：**  修复SearchPanel中显示树形列表的问题

### v2.3.5 (2019-9-19)

本次更新增加了自定主键功能，除了默认的guid主键外，框架现在还支持自增整形和string类型的主键。
同时代码生成器也可以准确识别主键类型，生成对应的代码。
具体使用方式参见文档 https://wtmdoc.walkingtec.cn/#/Model/CustomKey

由于主键不一定是guid了，老项目更新的时候需要手动修改之前的文件，主要是两部分：
1. Controller里 BatchEdit，BatchDelete中的ids参数由guid[] 变为 string[]
2. batchvm中的CheckIfCanDelete方法，第一个参数由guid变为object
3. 老数据库中DataPrivileges表RelatedId字段类型由Guid变为Nvarchar
改起来还是比较简单的

#### 前后端不分离模式
* **修改：**  修复layui模式下三级菜单无法显示的bug
* **修改：**  修复selector控件不能搜索，不初始化的bug

#### React前后端分离模式
* **新增：**  增加菜单对字体图标的支持
* **修改：**  修复react模式下三级菜单无法显示的bug

### v2.3.4 (2019-9-5)

#### 前后端不分离模式
* **新增：**  现在Layui模式可以直接用代码生成器生成api
* **新增：**  现在Layui模式的菜单管理也可以配置api的权限，包括框架自带的api
* **新增：**  新建layui项目时自动添加swagger的支持，可以查看api文档
* **修改：**  修复grid排序时搜索条件不起作用的bug

#### React前后端分离模式
* **新增：**  新增Tab页关闭其他，关闭当前，关闭所有的功能

### v2.3.3 (2019-9-3)

* **新增：**  新增对Oracle的支持（鸣谢：hd2y）Oracle database version 18c is required.
* **新增：**  新增对DC使用事务的支持（鸣谢：AaronLucas）
* **修改：**  更新了默认数据的添加逻辑，现在使用EF的migration不会担心没有初始数据了
* **修改：**  新生成的项目会在DataContext.cs中自动加入IDesignTimeDbContextFactory类，方便使用Migration

#### 前后端不分离模式
* **修改：**  现在首页默认不展开菜单
* **修改：**  直接访问具体url可以准确定位左侧菜单
* **修改：**  更新默认生成的Login单元测试
* **修改：**  修复批量修改的一些bug
* **修改：**  修复维护菜单会改变菜单id的bug

#### React前后端分离模式
* **修改：**  修复grid中分组之后排序的bug
* **修改：**  修复grid中有时出现横向滚动条的bug
* **修改：**  新登陆页面


### v2.3.1 (2019-8-31)
修复了2.3.0版本中的一些bug
* **修改：**  修复菜单图标显示的问题
* **修改：**  修复主子表修改报错的问题
* **修改：**  修复Selector在IIS下无法显示的问题

### v2.3.0 (2019-8-30)

本次更新是一个大版本的更新，彻底重构了不分离模式的前端UI，大家可以愉快且免费的使用LayuiAdmin了

老项目更新说明：
本次更新全面支持了图标字体，放弃了使用图片附件作为菜单图标。FrameworkMenu表去掉了IconId和CustomIcon字段，新加入了字符串格式的Icon字段
老用户最快的升级方法是线上生成一个同名的新LayUI的项目，然后把你的model，viewmodel，controller考过去。。。

#### 前后端不分离模式
* **新增：**  LayUI升级为LayUIAdmin
* **新增：**  添加对图标字体的支持
* **新增：**  更新默认登录页
* **新增：**  新增Tree和TreeContainer控件
* **新增：**  新增Combobox和DateTime控件的默认配置（鸣谢：AaronLucas）
* **新增：**  新增对Sqlite数据库支持（鸣谢：xuegaoge)
* **修改：**  修复单元格内控件错位的bug
* **修改：**  修复多表头列表导出的bug
* **修改：**  修复LinkButton无法在当前页显示的问题

#### React前后端分离模式
* **修改：**  更新菜单管理模块，支持图标字体
* **修改：**  更新列表显示的逻辑

#### React前后端分离模式
还在路上，预计下个版本会和大家见面~~~

## v2.2.x

### v2.2.52 (2019-8-7)

本次更新主要修复了近期用户反馈的一些bug

#### 前后端不分离模式

* **新增：** GridAction的按钮现在可以通过IsRedirect属性设置在当前页或者Tab页，或者新窗口中显示
* **修改：** 修复layui普通checkbox样式问题
* **修改：** 修复菜单添加报错的bug
* **修改：** 修复多表头显示问题
* **修改：** 修复GridAction中IconCls属性不起作用的问题

#### React前后端分离模式

* **修改：** 修复Area下的api地址解析问题
* **修改：** 修复下拉菜单显示枚举错误的问题
* **修改：** 修复列表中switch现实问题

### v2.2.50 (2019-7-30)

* **修改：** 代码生成器加入对模型基类的验证
* **修改：** 修复日志过长导致截断的bug
* **修改：** 使用新Logo

#### 前后端不分离模式

* **新增：** 新增slider滑块控件
* **新增：** 新增transfer穿梭框控件
* **新增：** 新增对列表汇总行的支持
* **修改：** 修复了绑定字段为数组引起的bug
* **修改：** 修复菜单管理和数据权限管理中历史遗留的bug
* **修改：** 控件的默认id添加vm名称前缀，防止多tab页时出现id重名的控件
* **修改：** 修复checkbox无法触发change-func函数的bug
* **修改：** 使用layui的template重写列表前景色和背景色的实现

#### React前后端分离模式

* **修改：** 优化页面异步加载机制 路由规则调整
* **新增：** 新增aggrid，替代antd自带的grid

### v2.2.48 (2019-7-12)

* **修改：** 修复导入时，同样唯一性数据没有自动更新的bug
* **修改：** 修复批量修改时的验证bug
* **修改：** 修复前后端分离模式菜单排序的bug
* **修改：** 修复layui模式下，searchpanel中oldpost参数无效的bug

### v2.2.47 (2019-7-5)

* **新增：** 新增ValidateFormItemOnly属性，可以加在controller的方法上，用来指示框架只验证表单提交的字段

#### 前后端不分离模式

* **修改：** 修复Admin中更新用户的bug
* **修改：** 修复grid中form控件id的错误

### v2.2.46 (2019-7-3)

* **优化：** 优化ListVM查询速度
* **优化：** 修改ListVM对存储过程的支持，并配套相关文档

#### 前后端不分离模式

* **修改：** 修复grid中使用表单组件的bug
* **修改：** 修复GetFile方法没有正确输入视频content-type的bug

#### React前后端分离模式

* **优化：** 优化配置文件
* **修改：** 修复添加操作没有正确验证model的bug

### v2.2.45 (2019-6-17)

* **新增：** wt:grid 增加MultiLine属性，用于控制是否允许单元格自动换行
* **修改：** 修改默认首页的样式，增加了对于调试模式的提示，以免新用户迷茫

### v2.2.44 (2019-6-14)

* **新增：** 新增自动化单元测试。在线生成项目时会同时生成单元测试项目，代码生成器也会在生成代码的同时为Controller生成单元测试。同时支持分离和不分离两种模式
* **修改：** 修复主子表导入bug，文档中ImportVM中同时加入了主子表导入的范例
* **修改：** 修复默认多字段排序的bug

#### 前后端不分离模式

* **修改：** 修复用户组数据权限修改时的bug

#### React前后端分离模式

* **新增：** 顶部菜单配置，可以在全局设置里将菜单配置成顶部显示
* **新增：** 富文控件中增加文件上传
* **新增：** 增加时间区间控件
* **新增：** 添加了默认的修改密码页面
* **优化：** 优化表格

#### VUE前后端分离模式

VUE目前进展稍慢，距离和大家见面还需要一段时间

### v2.2.42 (2019-5-28)

* **修改：** FrameworkMenu表中加入string类型的CustumIcon字段，已有项目可以手动修改数据库，添加这个字段

#### React前后端分离模式

* **新增：** 菜单维护加入自定义图标，可以设置antd自带图标
* **优化：** 优化编译速度，优化布局

#### 前后端不分离模式

* **新增：** Appsettings中增加TabMode配置，设置layui模式下Tab页样式 ，可选配置有Default和Simple
* **新增：** Appsettings中增加IsFilePublic配置，可以设置附件是否不需要登陆就可以查看和下载

### v2.2.40 (2019-5-18)

* **修改：** 修复枚举导出时不显示的bug

#### 前后端分离模式

* **修改：** 修复React依赖报错的问题

### v2.2.39 (2019-5-10)

* **修改：** 修复代码生成器对关联字段生成ImportVM时的bug

#### 前后端不分离模式

* **修改：** upload控件增加修改尺寸的选项，当UploadType为ImageFile时，通过设置ThumbWidth和ThumbHeight，可以让服务器保存缩小后的图片
* **修改：** upload控件增加缩略图预览，当UploadType为ImageFile时默认开启缩略图预览，通过ShowPreview，PreviewWidth，PreviewHeight等参数可配置
* **修改：** 更改绑定布尔值时checkbox的样式
* **修改：** 修复layui模式下登录用户头像显示的问题，已经生成的项目可以将以下两个文件覆盖自己的项目对应的文件
    1. [LoginVM.cs](https://github.com/dotnetcore/WTM/blob/develop/demo/WalkingTec.Mvvm.Demo/ViewModels/HomeVMs/LoginVM.cs)
    1. [Header.cshtml](https://github.com/dotnetcore/WTM/blob/develop/demo/WalkingTec.Mvvm.Demo/Views/Home/Header.cshtml)

### v2.2.38 (2019-4-29)

* **修改：** 修改DpWhere逻辑，DpWhere中多个字段参数之间现在是or的关系

#### 前后端不分离模式

* **新增：** 新增layui下根据数据控制行内动作按钮是否显示。现在可以在Action上调用BindVisiableColName方法来指定某个隐藏列的名称，该隐藏列值为字符串'true'的时候，action按钮才显示
* **修改：** 修改了tab页模式下，多层弹出窗口grid不刷新的bug
* **修改：** 修复selector在searchpanel里，清空了不起作用的bug
* **修改：** 修改pindex页面使用tab，导致代码生成器显示错位的bug

### v2.2.36 (2019-4-25)

#### 前后端不分离模式

* **新增：** 新增layui下列表后台排序功能，在ListVM中设置列的时候，对需要排序的列调用SetSort(true)即可

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

* **新增：** 新增在线生成项目，[https://wtmdoc.walkingtec.cn/setup](https://wtmdoc.walkingtec.cn/setup) ，生成WTM项目更快捷
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
