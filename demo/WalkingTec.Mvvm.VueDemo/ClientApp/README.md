# vue 版简介

说明：

> 大部分的方法通过 vue 的混淆（mixin）写在，./src/vue-custom/mixin/中的 action-mixin，form-mixin 中
> 按钮的增删改查 都是通用写法，如果默认逻辑不符合要求，可以在vue组件中定义相同key名 方法或属性，边可以覆盖mixin中的代码；


## 目录
    ```shell script
    .
    ├── assets 
    │   ├── css
    │   └── icon
    ├── components // 组件
    │   ├── frame
    │   ├── layout
    │   └── page
    ├── config
    │   ├── entity.tx // 动作
    │   ├── enum.tx //枚举
    │   └── index.ts //配置
    ├── lang //公共多语言
    ├── pages //页面
    ├── router
    ├── service
    ├── store
    ├── util
    ├── vue-custom
    ├── index.ts
    ├── login.ts
    ├── settings.ts
    ├── shims-tsx.d.ts
    ├── shims-vue.d.ts
    └── subMenu.json
    ```
