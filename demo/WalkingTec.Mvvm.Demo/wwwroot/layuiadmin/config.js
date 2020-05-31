
layui.define(['laytpl', 'layer', 'element', 'util'], function (exports) {
  exports('setter', {
    container: 'LAY_app' //container ID
    , base: layui.cache.base //path of layuiAdmin folder

    , views: '/' //path of views
    , entry: 'Home/FrontPage' //entry page
    , engine: '' //view surfix

    , pageTabs: $.cookie("pagemode") === 'Tab' //tab mode


    , name: 'WTM'
    , tableName: 'layuiAdmin' //local storage name
    , MOD_NAME: '_wtm' 

    , debug: true 

    , interceptor: false 

    
    , request: {
      tokenName: 'access_token' 
    }

    
    , response: {
      statusName: 'Code' 
      , statusCode: {
        ok: 200 
        , logout: 1001 
      }
      , msgName: 'Msg' 
      , dataName: 'Data' 
    }

    
    , indPage: [
      '/user/login' 
      , '/user/reg' 
      , '/user/forget' 
      , '/template/tips/test' 
    ]

   
    , extend: [
      'echarts', 
      'echartsTheme' 
    ]

    
    , theme: {
      
      color: [{
        main: '#20222A' 
        , selected: '#009688' 
        , alias: 'default'
      }, {
        main: '#03152A'
        , selected: '#3B91FF'
        , alias: 'dark-blue' //藏蓝Tibetan blue
      }, {
        main: '#2E241B'
        , selected: '#A48566'
        , alias: 'coffee' //咖啡Cofee
      }, {
        main: '#50314F'
        , selected: '#7A4D7B'
        , alias: 'purple-red' //紫红Purplish red
      }, {
        main: '#344058'
        , logo: '#1E9FFF'
        , selected: '#1E9FFF'
        , alias: 'ocean' //海洋ocean
      }, {
        main: '#3A3D49'
        , logo: '#2F9688'
        , selected: '#5FB878'
        , alias: 'green' //墨绿blackish green
      }, {
        main: '#20222A'
        , logo: '#F78400'
        , selected: '#F78400'
        , alias: 'red' //橙色orange
      }, {
        main: '#28333E'
        , logo: '#AA3130'
        , selected: '#AA3130'
        , alias: 'fashion-red' //时尚红fashion red
      }, {
        main: '#24262F'
        , logo: '#3A3D49'
        , selected: '#009688'
        , alias: 'classic-black' //经典黑classic black
      }, {
        logo: '#226A62'
        , header: '#2F9688'
        , alias: 'green-header' //墨绿头blackish green header
      }, {
        main: '#344058'
        , logo: '#0085E8'
        , selected: '#1E9FFF'
        , header: '#1E9FFF'
        , alias: 'ocean-header' //海洋头ocean header
      }, {
        header: '#393D49'
        , alias: 'classic-black-header' //经典黑classic black header
      }, {
        main: '#50314F'
        , logo: '#50314F'
        , selected: '#7A4D7B'
        , header: '#50314F'
        , alias: 'purple-red-header' //紫红头 Purplish red header
      }, {
        main: '#28333E'
        , logo: '#28333E'
        , selected: '#AA3130'
        , header: '#AA3130'
        , alias: 'fashion-red-header' //时尚红头fashion red header
      }, {
        main: '#28333E'
        , logo: '#009688'
        , selected: '#009688'
        , header: '#009688'
        , alias: 'green-header' //墨绿头blackish green header
      }]

      
      , initColorIndex: 0
    }
  });
});
