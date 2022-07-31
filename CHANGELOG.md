# 更新日志

## v6.x.x

##6.3.1(2022-7-25)
* **修改：**  修复了LayUI模式下Selector联动的问题
* **修改：**  修复了非外键关联的Include字段问题
* **修改：**  修复了多租户根据url判断租户的问题

##6.3.0(2022-7-22)
* **修改：**  优化Blazor的用户信息，提升页面效率
* **修改：**  配合BootStrapBlazor控件的最新版本，更新树形列表
* **修改：**  Blazor文件上传控件增加了文件下载
* **修改：**  修复多数据库导入的bug
* **修改：**  修复Layui模式Selector绑定非主键时显示的问题
* **修改：**  修复Layui模式Display控件的样式

##6.2.6(2022-7-5)
* **修改：**  修复Jwt登录时间验证的问题
* **修改：**  修复Layui模式中Tree控件禁用的问题
* **修改：**  移除过时引用
* **修改：**  修复主键类型为string时Crud的问题
* **修改：**  修复WtmJob Displose时的bug
* **修改：**  优化Blazor菜单，感谢akin的PR

##6.2.4(2022-6-16)
* **修改：**  优化登录
* **修改：**  修复WtmFileProvider直接使用的问题
* **新增：**  为配合WtmPlus的新功能，框架底层增加SoftKey,SoftFK属性，用于标记非主键关联的模型

##6.2.3(2022-6-12)
* **新增：**  QuartzRepeatAttribute增加了DelaySeconds参数，可以控制延迟多少秒启动服务
* **修改：**  恢复LoginUserInfo中的UserId以兼容老系统
* **修改：**  修复Layui模式下Combobox处理默认值的问题
* **修改：**  修复登录时Token会加长的问题
* **修改：**  现在DoEdit方法会自动检查继承TreePoco的模型，其父级ID不能被修改为本身ID
* **修改：**  增加了一些验证，避免中间件报错，提高性能

##6.2.2(2022-6-7)
* **HotFix：**  修复附件上传问题

##6.2.1(2022-6-7)

* **新增：**  新增CanNotEditAttribute，用于标记在模型属性上，指明该字段不应该被修改。
* **新增：**  优化VM内包含其他VM时框架默认的处理方法，现在框架默认会自动给子VM赋必须的值，并和表单提交的值对应。
* **修改：**  修复MainTenantOnly属性会导致权限失效的问题
* **修改：**  修复登录时Token会加长的问题
* **修改：**  修复Blazor模式无法删除租户的问题
* **修改：**  修复Blazor模式添加外部菜单的显示问题
* **修改：**  修复LayUI模式Combobox在Https下无法下载数据的问题

##6.2.0(2022-6-5)

本次为大版本更新，包含中断性更改，老项目升级时需要手动更新旧数据库以及覆盖默认生成的项目文件

* **新增：**  新增多租户支持，支持单数据库，独立数据库以及混合模式，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/MultiTenant
* **新增：**  新增单点登录支持，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/SSO
* **新增：**  新增统一用户，角色，用户组管理支持，WTM现在可以用来架构微服务风格的分布式系统。
* **新增：**  Layui和Blazor新增默认的多租户管理界面，其他UI后续会添加
* **新增：**  新增MainTenantOnlyAttribute，用于标记方法不能被子租户使用
* **修改：**  用户组修改为树形结构，可作为部门组织结构使用，为下一步工作流做好准备
* **修改：**  由于用户组修改为树形结构，用户组的数据权限也可以向下继承
* **修改：**  重构用户登录，重新登陆，权限验证等逻辑，更大程度上使用缓存，大幅提高性能
* **修改：**  用户表的其他字段现在会被自动读取到LoginUserInfo.Attributes中
* **修改：**  Blazor支持最新的BB控件库
* **修改：**  修复了文件上传的一些安全性问题
* **修改：**  优化导出操作
* **修改：**  修复Layui Combobox和Tree控件的一些bug
* **中断性修改：**  移除了系统自带的PersistedGrant表,简化了jwt登录流程，现在不再需要一个单独的RefreshToken来刷新Token，而是登陆后调用RefreshToken接口刷新当前用户的Token
* **中断性修改：**  系统自带的表，除了FrameworkMenu外，都新增了TenantCode字段
* **中断性修改：**  系统自带的FrameworkGroup字段发生了改变，变为树形结构，且增加了Manager字段
* **中断性修改：**  新增新的系统表FrameworkTenant
* **中断性修改：**  Appsettings文件中新增EnableTenant配置



##6.1.1(2022-4-1) 
* **新增：**  集成Quartz作业调度，为后续工作流所需内部定时任务做好准备，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/Quartz
* **修改：**  日志分类中增加了“作业”一项
* **修改：**  Layui模式下登录现在会统一调用/api/_account/login方法，为后续单点登录做好准备
* **修改：**  现在当用户缓存失效时，框架会自动调用/api/_account/login或/api/_account/loginJwt获取用户信息，如无特殊情况，不需要再重写ReloadUser方法了
* **修改：**  修复layui数据权限维护时下拉菜单的bug
老项目更新时，除了升级Nuget包，还应该通过官网或Plus生成新项目，将系统自带的Controller和View覆盖一下

##6.0.7(2022-3-21) 
* **HotFix：**  修复使用用xmselect引发的联动和TreeContainer的bug

##6.0.6(2022-3-20) 
* **修改：**  使用xmselect控件重写了Layui的Combobox和Tree，老用户更新的时候需要手动添加xmselect的js文件：https://gitee.com/maplemei/xm-select/releases/v1.2.4
把这个文件放到wwwroot/layui下面。LayUI项目的用户还需要手动在/Views/Shared/_Layout.cshtml文件中加入对这个js的引用， 其他UI类型的项目不需要，但是因为其他UI类型的项目代码生成器也是用的layui的界面，所以还是需要copy xm-select的js文件到wwwroot/layui下面的

* **修改：**  Layui模式下修复了数据列表指定line-height的bug
* **修改：**  Layui模式下MakeViewButton方法现在可以生成更美观的图片预览
* **修改：**  验证码现在会自动读取系统安装的字体，linux下部署的时候不会出现找不到字体的问题了
* **修改：**  Blazor项目默认使用自带的字体文件
* **修改：**  修复Blazor项目中向localstorage存储大量数据报错的问题
* **修改：**  Blazor的弹出窗口现在可以最大化和拖动
* **修改：**  修复Blazor项目中切换多语言有可能报错的问题

##6.0.5(2022-2-28) 
* **修改：**  默认初始化数据中加入了普通用户角色
* **修改：**  优化了数据权限查询语句
* **修改：**  修复登录用户保存信息是并发的问题
* **修改：**  修复了layui模式radio控件绑定空值的bug


##6.0.2(2021-12-26) 
* **修改：**  修改Blazor默认代码生成，适应BootstrapStrap 6.x新版本
* **修改：**  再次更新数据权限逻辑，新的默认逻辑为，如果设定了数据权限的表本身继承了BasePoco，而且没有给当前用户配置任何数据权限，那么用户默认可以看到自己加的数据。如果给用户配置了数据权限，则根据数据权限的配置显示数据，不管是否是当前用户添加的。
* **修改：**  修改了layui transfer控件绑定数据的bug
* **修改：**  修改了layui多选控件设置必填的bug

## v5.x.x

##5.10.1(2022-7-25)
* **修改：**  修复了LayUI模式下Selector联动的问题
* **修改：**  修复了非外键关联的Include字段问题
* **修改：**  修复了多租户根据url判断租户的问题

##5.10.0(2022-7-22)
* **修改：**  优化Blazor的用户信息，提升页面效率
* **修改：**  配合BootStrapBlazor控件的最新版本，更新树形列表
* **修改：**  Blazor文件上传控件增加了文件下载
* **修改：**  修复多数据库导入的bug
* **修改：**  修复Layui模式Selector绑定非主键时显示的问题
* **修改：**  修复Layui模式Display控件的样式

##5.9.6(2022-7-5)
* **修改：**  修复Jwt登录时间验证的问题
* **修改：**  修复Layui模式中Tree控件禁用的问题
* **修改：**  移除过时引用
* **修改：**  修复主键类型为string时Crud的问题
* **修改：**  修复WtmJob Displose时的bug
* **修改：**  优化Blazor菜单，感谢akin的PR

##5.9.4(2022-6-16)
* **修改：**  优化登录
* **修改：**  修复WtmFileProvider直接使用的问题
* **新增：**  为配合WtmPlus的新功能，框架底层增加SoftKey,SoftFK属性，用于标记非主键关联的模型


##5.9.3(2022-6-12)
* **新增：**  QuartzRepeatAttribute增加了DelaySeconds参数，可以控制延迟多少秒启动服务
* **修改：**  恢复LoginUserInfo中的UserId以兼容老系统
* **修改：**  修复Layui模式下Combobox处理默认值的问题
* **修改：**  修复登录时Token会加长的问题
* **修改：**  现在DoEdit方法会自动检查继承TreePoco的模型，其父级ID不能被修改为本身ID
* **修改：**  增加了一些验证，避免中间件报错，提高性能

##5.9.2(2022-6-7)
* **HotFix：**  修复附件上传问题

##5.9.1(2022-6-7)

* **新增：**  新增CanNotEditAttribute，用于标记在模型属性上，指明该字段不应该被修改。
* **新增：**  优化VM内包含其他VM时框架默认的处理方法，现在框架默认会自动给子VM赋必须的值，并和表单提交的值对应。
* **修改：**  修复MainTenantOnly属性会导致权限失效的问题
* **修改：**  修复登录时Token会加长的问题
* **修改：**  修复Blazor模式无法删除租户的问题
* **修改：**  修复Blazor模式添加外部菜单的显示问题
* **修改：**  修复LayUI模式Combobox在Https下无法下载数据的问题


##5.9.0(2022-6-5)

本次为大版本更新，包含中断性更改，老项目升级时需要手动更新旧数据库

* **新增：**  新增多租户支持，支持单数据库，独立数据库以及混合模式，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/MultiTenant
* **新增：**  新增单点登录支持，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/SSO
* **新增：**  新增统一用户，角色，用户组管理支持，WTM现在可以用来架构微服务风格的分布式系统。
* **新增：**  Layui和Blazor新增默认的多租户管理界面，其他UI后续会添加
* **修改：**  用户组修改为树形结构，可作为部门组织结构使用，为下一步工作流做好准备
* **修改：**  由于用户组修改为树形结构，用户组的数据权限也可以向下继承
* **修改：**  重构用户登录，重新登陆，权限验证等逻辑，更大程度上使用缓存，大幅提高性能
* **修改：**  Blazor支持最新的BB控件库
* **修改：**  修复了文件上传的一些安全性问题
* **修改：**  优化导出操作
* **修改：**  修复Layui Combobox和Tree控件的一些bug
* **中断性修改：**  移除了系统自带的PersistedGrant表,简化了jwt登录流程，现在不再需要一个单独的RefreshToken来刷新Token，而是登陆后调用RefreshToken接口刷新当前用户的Token
* **中断性修改：**  系统自带的表，除了FrameworkMenu外，都新增了TenantCode字段
* **中断性修改：**  系统自带的FrameworkGroup字段发生了改变，变为树形结构，且增加了Manager字段
* **中断性修改：**  新增新的系统表FrameworkTenant
* **中断性修改：**  Appsettings文件中新增EnableTenant配置


##5.8.3(2022-4-1) 
* **新增：**  集成Quartz作业调度，为后续工作流所需内部定时任务做好准备，使用方法参见文档 https://wtmdoc.walkingtec.cn/#/Global/Quartz
* **修改：**  日志分类中增加了“作业”一项
* **修改：**  Layui模式下登录现在会统一调用/api/_account/login方法，为后续单点登录做好准备
* **修改：**  现在当用户缓存失效时，框架会自动调用/api/_account/login或/api/_account/loginJwt获取用户信息，如无特殊情况，不需要再重写ReloadUser方法了
* **修改：**  修复layui数据权限维护时下拉菜单的bug
* **修改：**  图形操作改用ImageSharp
老项目更新时，除了升级Nuget包，还应该通过官网或Plus生成新项目，将系统自带的Controller和View覆盖一下

##5.7.9(2022-3-21) 
* **HotFix：**  修复使用用xmselect引发的联动和TreeContainer的bug

##5.7.7(2022-3-20) 
* **修改：**  使用xmselect控件重写了Layui的Combobox和Tree，老用户更新的时候需要手动添加xmselect的js文件：https://gitee.com/maplemei/xm-select/releases/v1.2.4
把这个文件放到wwwroot/layui下面。LayUI项目的用户还需要手动在/Views/Shared/_Layout.cshtml文件中加入对这个js的引用， 其他UI类型的项目不需要，但是因为其他UI类型的项目代码生成器也是用的layui的界面，所以还是需要copy xm-select的js文件到wwwroot/layui下面的

* **修改：**  Layui模式下修复了数据列表指定line-height的bug
* **修改：**  Layui模式下MakeViewButton方法现在可以生成更美观的图片预览
* **修改：**  验证码现在会自动读取系统安装的字体，linux下部署的时候不会出现找不到字体的问题了
* **修改：**  Blazor项目默认使用自带的字体文件
* **修改：**  修复Blazor项目中向localstorage存储大量数据报错的问题
* **修改：**  Blazor的弹出窗口现在可以最大化和拖动
* **修改：**  修复Blazor项目中切换多语言有可能报错的问题

##5.7.6(2022-2-28) 
* **修改：**  默认初始化数据中加入了普通用户角色
* **修改：**  优化了数据权限查询语句
* **修改：**  修复登录用户保存信息是并发的问题
* **修改：**  修复了layui模式radio控件绑定空值的bug

##5.7.3 (2021-12-26) 
* **修改：**  再次更新数据权限逻辑，新的默认逻辑为，如果设定了数据权限的表本身继承了BasePoco，而且没有给当前用户配置任何数据权限，那么用户默认可以看到自己加的数据。如果给用户配置了数据权限，则根据数据权限的配置显示数据，不管是否是当前用户添加的。
* **修改：**  修改了layui transfer控件绑定数据的bug
* **修改：**  修改了layui多选控件设置必填的bug

##5.7.1 (2021-12-8) 
* **修改：**  还原了数据权限的默认逻辑
* **修改：**  修复了批量修改时将某些字段清空的bug
* **修改：**  修复了TreeContainer联动列表的bug
* **修改：**  修复了更新时因字段大小写引发的验证失败的bug

##5.7.0 (2021-12-2) 
* **修改：**  添加了BoolStringConverter，用于将字符串序列化为布尔值
* **修改：**  修复了导入时唯一性条件的判断
* **修改：**  现在用户缓存里会写入TenantCode，方便扩展多租户
* **修改：**  修复了layui控件中关于默认值的逻辑，现在绑定字段没有值的情况下才会使用默认值
* **修改：**  修复了子表控件在最大化弹出窗口中显示不完全的问题
* **修改：**  修复了全局错误处理会引发其他http错误的问题
* **修改：**  修复了某些情况下文件无法删除的问题
* **修改：**  其他配合WtmPlus的修改

##5.6.3 (2021-10-29) 
* **修改：**  修复了上个版本引发的权限配置搜索的bug
* **修改：**  修复了自带的代码生成器中生成BatchVM中的日期类型字段的错误

##5.6.1 (2021-10-27) 
* **修改：**  修复了上个版本引发的Selector报错的问题
* **修改：**  修复了Blazor模式下删除文件的bug

##5.6.0 (2021-10-26) 
* **修改：**  修复了数据权限读取的问题
* **修改：**  修复了layui模式下英文界面搜索后列表分页栏显示中文的问题
* **修改：**  修复了layui模式下修改了刷新列表会回到第一页的问题
* **修改：**  修复了批量导入时有的字段不识别的问题
* **修改：**  修复了公共页面配置无效的问题
* **修改：**  修复了Blazor模式下js加载的问题

##5.5.5 (2021-10-3) 
* **修改：**  修复了图表显示的一些问题
* **修改：**  修复了WtmContext中引用HttpContext造成非web项目使用报错的问题
* **修改：**  默认的Cors添加了Content-Disposition头
* **修改：**  修复了Blazor文件下载url错误的问题
* **修改：**  修复了Layui模式编辑菜单目录报错的问题
* **修改：**  修复了Layui Transfer控件默认值的bug
* **修改：**  修复了导出Excel时枚举描述为数字时产生的bug

##5.5.0 (2021-9-16) 
* **修改：**  升级自带的BootStrapBlazor到5.10.8版本，修复了wasm模式下刷新页面报错的问题
* **修改：**  修复了Blazor编写公开页面的问题
* **修改：**  修复了layui模式中一些控件的高度问题
* **修改：**  修复了layui模式环状图表会显示坐标的问题
* **修改：**  修改了Blazor模式中导入的默认代码
* **修改：**  修改了layui模式中Selector单选不能清空的问题

##5.4.9 (2021-9-12) 
* **修改：**  修复<wt:radio>控件绑定布尔值的bug
* **修改：**  新增生成测试数据的方法
* **修改：**  修复了Blazor模式公开页面的bug

##5.4.8 (2021-9-10) 
* **修改：**  Layui模式下SearchPanel可以通过设置ChartId来对图表进行数据搜索
* **修改：**  修复了在菜单管理中设置页面公开无效的问题
* **修改：**  修复了Blazor模式中导出有时报错的问题
* **修改：**  修复了默认的字符过滤逻辑，防止xss攻击

##5.4.7 (2021-9-4) 
* **修改：**  修复了Layui下图表在Tab页和弹出窗口中刷新的问题
* **修改：**  修复了Taghelper组合style有可能无效的问题

##5.4.6 (2021-9-3) 
* **新增：**  受益于WtmPlus的需求，Layui新增了图表控件，支持柱状，饼图，环状，折线和散点，以及多种主题。具体请参考文档。
* **修改：**  Layui默认登陆页面统一成React，Vue，Blazor相同的样式
* **修改：**  移除了Layui自带的echartjs，使用echart官方的版本
* **修改：**  修复了Layui模式下Radio和CheckBox绑定布尔值无效的bug
* **修改：**  修改了自带代码生成器生成单元测试的逻辑
* **修改：**  修复了偶发登录后又跳出的问题
* **修改：**  修复了Layui中维护菜单时添加api报错的问题


##5.4.5 (2021-8-27) 
* **修改：**  修复了Layui模式下上传图片控件异步加载的问题
* **修改：**  修复了Layui模式下ListVM中使用SetFixed方法会导致内部按钮不能点击的bug
* **修改：**  修改了框架默认的生成缩略图的逻辑
* **修改：**  Blazor模式下搜索框默认的打开状态现在可以正确读取appsettings里的全局设置
* **修改：**  修复了使用低版本IE无法登录的问题

##5.4.4 (2021-8-25) 
* **修改：**  增强了Layui模式下图片上传控件的预览效果，感谢Rea同学的代码
* **修改：**  修复Layui模式下可能出现的联动的问题
* **修改：**  Layui模式下现在可以在combobox,radio,checkbox 和 transfer控件上使用 item-url 来替代items实现类似前后端分离的获取初始数据的效果
* **修改：**  AddWtmSwagger方法现在可以设置一个参数来指定是否使用全类名
* **修改：**  更新了框架对.netcore的相关类库依赖到5.0.9
* **修改：**  自带的代码生成器会提示使用WtmPlus可以提高工作效率，哈哈。

##5.3.8 (2021-8-11) 
* **修改：**  修复GetTreeSelectListItems方法数据权限过滤的bug
* **修改：**  修复LayUI模式下联动的问题
* **修改：**  修复Blazor模式下代码生成器的一些问题

##5.3.5 (2021-8-3) 
* **HotFix：**  修复Blazor代码生成器

##5.3.4 (2021-8-3) 
* **修改：**  优化了layui模式中联动的功能，多选下拉菜单现在不受联动的影响
* **修改：**  Blazor模式更新到BootStrap Blazor控件库的最新版本
* **修改：**  修复了主子表同时修改的一些bug

##5.3.2 (2021-7-26) 
* **修改：**  修复了主子表导入的问题
* **修改：**  修复了用户组中设置数据权限的bug
* **修改：**  为配合WtmPlus的发布，layui模式中的upload和multiupload现在可以点击预览图看大图，并支持disabled模式
* **修改：**  默认生成的代码中RefreshToken方法不再需要登陆

##5.3.1 (2021-7-18) 
* **hotfix：**  修复了上一个版本引发的Selector不能正确显示的bug

##5.3.0 (2021-7-18) 
* **修改：**  修复了有些地址权限控制无法识别的bug
* **修改：**  默认的用户加载现在会读取用户名和头像
* **修改：**  修复了一些EF生成的语句不支持sqlite的问题
* **修改：**  日志现在可以根据当前的数据库连接字符串记录到不同的数据库里
* **修改：**  Blazor模式中配置外部链接现在可以正常显示
* **修改：**  Blazor模式中在页面上添加 @layout EmptyLayout 和 @attribute [Public] 属性，可以制作不受权限控制的独立的页面

##5.2.8 (2021-7-12) 
* **修改：**  修复了layui模式下在弹出窗口刷新grid的问题
* **修改：**  修复了Blazor模式下关联主表后提交表单失败的问题
* **修改：**  修复了Vue模式下导出错误的问题

##5.2.7 (2021-7-4) 
* **修改：**  修复了layui模式下表单错误的显示位置问题
* **修改：**  修复了初始化默认数据有可能报错的问题
* **修改：**  TokenService现在会默认使用名为tokendefault的连接字符串，便于将token相关表保存在其他数据库或内存中

##5.2.6 (2021-6-27) 
* **修改：**  修复了layui模式下设置组件style和class有时无效的问题
* **修改：**  Layui模式下添加了wt:card组件
* **修改：**  修复了Wtm中的Cache依赖HttpContext的问题

##5.2.5 (2021-6-20) 
* **修改：**  修复了layui模式下slider控件无法提交的bug
* **修改：**  修复了GetSelectListItems and GetTreeSelectListItems 方法在sqlite下有可能引发错误的bug

##5.2.4 (2021-6-9) 
* **修改：**  修复了Layui的Selector读取数据的bug
* **修改：**  修复了Layui的Display没有自动换行的问题
* **修改：**  修改了Blazor项目默认代码，更新了BB版本，并修复了访问不存在的地址会报错的问题

##5.2.3 (2021-6-1) 
* **修改：**  修复了项目默认代码Blazor模式中配置角色页面权限的bug
* **修改：**  修复了项目默认代码删除用户时没有同时删除角色及用户组关联数据的bug
* **修改：**  修复了项目默认代码LoginJwt方法对密码进行了小写操作的bug
* **修改：**  修复了Layui模式中Selector控件只能绑定id值的bug
* **修改：**  默认生成的Blazor项目更改为server模式

##5.2.2 (2021-5-24) 
* **修改：**  修复了代码生成器生成单元测试的一些bug
* **修改：**  优化了DC.RunSql函数，现在可以在事务中间使用DC.RunSql
* **修改：**  更新了默认项目中的FrameworkUserVM，使其在修改和删除用户时刷新用户权限缓存。
* **修改：**  更新了Blazor项目默认代码，修复了修改密码无法找到Api的问题，修复了wasm模式中必须指定后台地址的问题
* **修改：**  修复了Layui模式中Selecor设置disable无效的问题
* **修改：**  Layui模式中，子表Grid单元格编辑时，现在可以设置SetEditType(EditTypeEnum.Datetime)来支持日期选择
* **修改：**  优化了数据唯一性的查询语句，修复了mysql中生成的语句报错的问题

##5.2.1 (2021-5-15) 
* **修改：**  修复了代码生成器生成中间表相关ViewModel和页面错误的bug
* **修改：**  修复了BaseCrudVM中默认修改方法没有过滤掉[NotMapped]字段的错误

##5.2.0 (2021-5-14) 
* **新增：**  新增对Blazor的支持，现在可以在官网生成Blazor模式的项目，代码生成器现在也可以生成Blazor的代码，具体请见文档http://wtmdoc.walkingtec.cn/#/Blazor/Intro
* **新增：**  [ActionDescription]中加入了IsPage属性，设置了这个属性的方法可以在菜单管理中添加，解决了一个Controller下只能配置一个主页面的问题
* **修改：**  修改了代码生成器的一些内部实现
* **修改：**  修复了默认代码中非管理员设置数据权限出现的错误
* **修改：**  修复了DynamicDataConverter读取数据报错的问题
* **修改：**  修复了在配置文件中将jwt的SecurityKey设置的过短会报错的问题
* **修改：**  修复了Layui模式下，搜索框折叠后下方列表错位的问题

##5.1.9 (2021-4-30) 
* **修改：**  移除了默认的 wt:grid onedit 方法重载，用户可以自己写js来实现功能
* **修改：**  修复了OssFileHandler读取文件时没有正确使用groupname的问题
* **修改：**  修复了layui模式下selector没有正确读取[FixedConnection]中指定DC的问题
* **修改：**  修改了BaseCrunVM中的DoEdit方法中主子表同时开启事务修改数据会报错的问题
* **修改：**  修改了api提交多层嵌套的json数据，有时没有正确验证的问题
* **突变：**  修改了读取appsettings文件的问题，老用户可以在线生成新项目，替换默认的program和startup文件

##5.1.7 (2021-4-19) 
* **修改：**  修复了json同时提交主子表数据不能自动更新子表的bug
* **修改：**  修复了更新失败时没有返回错误信息的bug

##5.1.6 (2021-4-18) 
* **修改：**  默认Json序列化加入了JsonNumberHandling.WriteAsString
* **修改：**  默认Json序列化加入了自定义日期，去掉了默认日期和时间之间的那个T
* **修改：**  修复了导入时IsOverWriteExistData=false的时候没有验证重复数据的bug
* **修改：**  修复了导入时已经删除的PersistPoco会被更新的bug

##5.1.4 (2021-4-11) 
* **修改：**  修复了数据权限指向自定义ID表时无法设置的bug
* **修改：**  现在生成的新项目使用微软默认的加载config文件的方式
* **修改：**  appsettings文件中的CookieOptions里面可以指定"Domain"，比如"Domain":".abc.com",可以允许所有abc.com的二级域名设置cookie，解决前后台分别部署在不同域名设置cookie无效的问题。
* **修改：**  修复了重启服务获取不到userid的问题
* **修改：**  ISubFile中的order字段改为Order，纯粹是强迫症，老项目可以批量替换一下，影响不大
* **修改：**  修复了React模式下数据权限报错的问题
* **修改：**  修复了获取中文文件名的一些问题

##5.1.2 (2021-4-6) 
* **修改：**  新生成的项目加入了默认的favicon文件
* **修改：**  新生成的Layui项目更新至layui至2.6.3
* **修改：**  新生成的Vue项目修复了设置默认搜索条件不生效的问题
* **修改：**  修复了预览pdf文件时不显示的问题
* **修改：**  更新了BaseImportVM中的报错逻辑
* **修改：**  TreePoco中加入了HasChildren字段

对于在线新建项目时生成的默认文件，更新时可以生成一个新项目，并将生成的默认文件覆盖老项目

##5.1.1 (2021-3-28) 
* **修改：**  wt:image现在也可以显示label，默认不显示，通过设置hide-label=false来显示label
* **修改：**  ListVM种和wt:display中显示时间时，使用yyyy-MM-dd或者yyyy-MM-dd HH:mm:ss作为默认格式
* **修改：**  修复了wt:tree显示上的一些bug
* **修改：**  修复了代码生成器生成多对多假删除时的逻辑

##5.1.0 (2021-3-22) 
* **修改：**  修复了wtm.CreateDC时，connectionstring名称只能认小写字母的bug
* **修改：**  修复了_framework/getfile方法有时不能显示附件的bug
* **修改：**  代码生成时加入了一些模型字段验证，可以更快的发现模型设计的问题
* **修改：**  修复了代码生成器生成单元测试的一些bug

##5.0.9 (2021-3-14) 
* **修改：**  现在导入的模板会优先从ImportVM中字段的[DisplayName]上读取文字作为模板的列名
* **修改：**  修复在事务中操作附件造成阻塞的bug
* **新增：**  新增DynamicData类，在DC.RunSql<>和wtm.CallApi<>以及其他自定义方法中都可以使用DynamicData作为泛型，框架会自动序列化这种类型的变量。当我们懒得为了返回格式化的json定义新类的时候，可以使用这个类。
* **修改：**  修复了导入时唯一性验证的bug
* **修改：**  修复了导入时必填验证的bug
* **修改：**  修复了layui模式下在空间中指定Required无效的问题
* **修改：**  修复了layui模式下Combobox在禁用状态下指定emptytext无效的问题
* **修改：**  修复了layui模式下多个selector冲突的问题
* **修改：**  修复了layui模式下对于列名相同的不同ListVM设置背景色冲突的问题
* **修改：**  修复了默认项目生成的一些基础代码

##5.0.8 (2021-3-6) 
* **修改：**  现在附件保存在本地时，如果配置的是相对路径，数据库中也会记录相对路径，便于迁移
* **新增：**  修复了导出枚举字段没有正确显示文字的问题
* **修改：**  修复了layui模式中FF.RefreshPage()不能正确刷新页面的问题
* **修改：**  修复了layui模式中登陆过期后，重新登陆跳转到原有页面地址错误的问题。（老项目可以从官网重新生成新项目，然后替换HomeVms中的LoginVM文件）
* **修改：**  修改了默认生成的单元测试项目中的MockUtility文件，可测试ModelState中保存的内容

##5.0.7 (2021-2-27) 
* **修改：**  修复React修改当前用户密码失败的bug
* **新增：**  修复了React登录失败时的文字提示
* **修改：**  修复了代码生成器对于非Guid主键的关联表也会生成new Guid()语句的bug
* **修改：**  修复了有的空菜单目录没有隐藏的bug
* **修改：**  修复了Layui中对enum字段使用SetFormat不生效的bug
* **修改：**  修复了Layui中对指定NeedPage=false的不分页的列表，再次点击搜索还是会分页的bug
* **修改：**  修复了excel导入时没有验证模型字段的bug

##5.0.6 (2021-2-21) 
* **修改：**  修复了ListVM处理枚举列的一个小bug
* **新增：**  AddWtmMultiLanguages函数现在可以指定一个option，用来指定自定义的多语言文件
* **修改：**  现在默认允许集合结尾多写逗号的json格式
* **修改：**  修复了React模式下switch控件不选择无法提交的bug
* **修改：**  oracle终于支持.net5了，更新了对他的引用，现在不会再报警告了
* **突变：**  移除了默认项目中引用的Microsoft.EntityFrameworkCore.Tools和Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation，因为他们已经在wtm的dll中引用了。5.0以上的项目可以手动从依赖中删除这两个包。

##5.0.4 (2021-2-7)
* **HotFix：**  紧急修复5.0.3版本在各层分离的多项目模式下无法登录的bug

##5.0.3 (2021-2-7)
* **突变：**  用了新的方式修复了用户表不能自定义主键的问题。老版本更新后请手动将原始代码中的FrameworkUesrBase的引用统一更新成FrameworkUser，或者下载新项目把公共部分copy过去
* **突变：**  wtm.CallApi方法重构，增加了返回code，错误信息等字段。默认文件中HomeController中的GetGithubInfo方法使用了CallApi，老版本请手动将返回值修改为返回CallApi的结果.Data
* **新增：**  Layui的文件预览现在可以播放mp4的附件
* **新增：**  ImportVM中的GenerateTemplate现在可以重写，通过重写该方法可以指定上传模板的文件名
* **修改：**  修复了api中Json序列化引起的循环引用的错误

##5.0.2 (2021-1-31)
* **新增：**  新增IBasePoco和IPersistPoco两个接口，提升定义模型的灵活性。比如继承了TreePoco的树形模型如果同时实现IBasePoco框架也会自动设置添加人，添加事件，修改人，修改时间。详情请查阅文档中模型定义的部分。
* **新增：**  支持发布为单一文件
* **修改：**  修复了<wt:switch>为false时没有正确提交的bug
* **修改：**  修复了删除主表有时没有正确删除附件子表中的文件的bug
* **修改：**  修复了<wt:selector>中combobox赋值的bug
* **修改：**  现在api默认也会过滤<>
* **修改：**  修复了某些情况下上传大文件会被限制的bug
* **修改：**  修复了VUE使用element控件多次刷新界面的bug

##5.0.1 (2021-1-23)
* **修改：**  修复了<wt:Transfer>控件绑定字符串数组的bug
* **修改：**  修改了<wt:MultiUpload>控件，使其正确的删除文件
* **修改：**  修改了<wt:Combobox>控件默认值的bug
* **修改：**  修复了代码生成器生成单元测试代码的问题
* **修改：**  修复了代码生成器生成修改页面时默认文字的多语言错误
* **修改：**  新生成的项目自带的layui版本升级到2.5.7
* **修改：**  新生成的项目Admin模块中增加了用户批量修改角色的代码

##5.0.0 (2021-1-17)
* **新增：**  全面支持.net 5.0
* **新增：**  全新的WtmContext类
* **新增：**  针对性能做了大幅代码优化，访问速度肉眼可见的提高
* **新增：**  重构了文件上传和下载的功能，内置支持阿里云OSS
* **修改：**  移除了对NewtonJson的引用，使用微软默认的System.Text.Json
* **修改：**  Startup文件回归
* **修改：**  内置管理模块的代码直接包含在项目中
* **修改：**  将FrameworkUser用户表提取出来直接生成在项目中，便于大家扩展
* **修改：**  修改了内置一些数据库表结构，为后续功能扩展做好准备

## v3.x.x （2.x.x同步更新）

##3.8.0 以及 2.8.0 (2020-12-27)
* **新增：**  增加了CallApiStream方法用户调用返回二进制数组的api
* **修改：**  修复了枚举类型在多表头列表导出时不显示的问题
* **修改：**  修复了SubmitButton上添加ComformText不提交的问题
* **修改：**  修复了ImportVM中SetDuplicateCheck方法的bug，框架现在会优先使用ImportVM中的SetDuplicateCheck方法
* **修改：**  PersistPoco中IsValid字段默认值改为true
* **修改：**  修复了按钮无法指定layui-btn-normal样式的问题

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
