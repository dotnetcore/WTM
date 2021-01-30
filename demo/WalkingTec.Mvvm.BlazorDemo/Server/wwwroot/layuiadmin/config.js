/**

 @Name：全局配置
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License：LPPL（layui付费产品协议）

 */

layui.define(['laytpl', 'layer', 'element', 'util'], function (exports) {
  exports('setter', {
    container: 'LAY_app' //容器ID
    , base: layui.cache.base //记录layuiAdmin文件夹所在路径

    , views: '/' //视图所在目录
    , entry: 'Home/FrontPage' //默认视图文件名
    , engine: '' //视图文件后缀名
    , layout: 'Home/Layout'
    , pageTabs: $.cookie("pagemode") === 'Tab' //是否开启页面选项卡功能。单页版不推荐开启


    , name: 'WTM'
    , tableName: 'layuiAdmin' //本地存储表名
    , MOD_NAME: '_wtm' //模块事件名

    , debug: true //是否开启调试模式。如开启，接口异常时会抛出异常 URL 等信息

    , interceptor: false //是否开启未登入拦截

    //自定义请求字段
    , request: {
      tokenName: 'access_token' //自动携带 token 的字段名。可设置 false 不携带。
    }

    //自定义响应字段
    , response: {
      statusName: 'Code' //数据状态的字段名称
      , statusCode: {
        ok: 200 //数据状态一切正常的状态码
        , logout: 1001 //登录状态失效的状态码
      }
      , msgName: 'Msg' //状态信息的字段名称
      , dataName: 'Data' //数据详情的字段名称
    }

    //独立页面路由，可随意添加（无需写参数）
    , indPage: [
      '/user/login' //登入页
      , '/user/reg' //注册页
      , '/user/forget' //找回密码
      , '/template/tips/test' //独立页的一个测试 demo
    ]

    //扩展的第三方模块
    , extend: [
      'echarts', //echarts 核心包
      'echartsTheme' //echarts 主题
    ]

    //主题配置
    , theme: {
      //内置主题配色方案
      color: [{
        main: '#20222A' //主题色
        , selected: '#009688' //选中色
        , alias: 'default' //默认别名
      }, {
        main: '#03152A'
        , selected: '#3B91FF'
        , alias: 'dark-blue' //藏蓝
      }, {
        main: '#2E241B'
        , selected: '#A48566'
        , alias: 'coffee' //咖啡
      }, {
        main: '#50314F'
        , selected: '#7A4D7B'
        , alias: 'purple-red' //紫红
      }, {
        main: '#344058'
        , logo: '#1E9FFF'
        , selected: '#1E9FFF'
        , alias: 'ocean' //海洋
      }, {
        main: '#3A3D49'
        , logo: '#2F9688'
        , selected: '#5FB878'
        , alias: 'green' //墨绿
      }, {
        main: '#20222A'
        , logo: '#F78400'
        , selected: '#F78400'
        , alias: 'red' //橙色
      }, {
        main: '#28333E'
        , logo: '#AA3130'
        , selected: '#AA3130'
        , alias: 'fashion-red' //时尚红
      }, {
        main: '#24262F'
        , logo: '#3A3D49'
        , selected: '#009688'
        , alias: 'classic-black' //经典黑
      }, {
        logo: '#226A62'
        , header: '#2F9688'
        , alias: 'green-header' //墨绿头
      }, {
        main: '#344058'
        , logo: '#0085E8'
        , selected: '#1E9FFF'
        , header: '#1E9FFF'
        , alias: 'ocean-header' //海洋头
      }, {
        header: '#393D49'
        , alias: 'classic-black-header' //经典黑
      }, {
        main: '#50314F'
        , logo: '#50314F'
        , selected: '#7A4D7B'
        , header: '#50314F'
        , alias: 'purple-red-header' //紫红头
      }, {
        main: '#28333E'
        , logo: '#28333E'
        , selected: '#AA3130'
        , header: '#AA3130'
        , alias: 'fashion-red-header' //时尚红头
      }, {
        main: '#28333E'
        , logo: '#009688'
        , selected: '#009688'
        , header: '#009688'
        , alias: 'green-header' //墨绿头
      }]

      //初始的颜色索引，对应上面的配色方案数组索引
      //如果本地已经有主题色记录，则以本地记录为优先，除非请求本地数据（localStorage）
      , initColorIndex: 0
    }
  });
});
