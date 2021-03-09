# vue 版简介

说明：

> 大部分的方法通过 vue 的混淆（mixin）写在，./src/vue-custom/mixin/中的 action-mixin，form-mixin 中
> 按钮的增删改查 都是通用写法，如果默认逻辑不符合要求，可以在 vue 组件中定义相同 key 名 方法或属性，边可以覆盖 mixin 中的代码；

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
    │   ├── enum.tx // 枚举
    │   └── index.ts // 配置
    ├── lang // 公共多语言
    ├── pages // 页面集合
    │   ├── modulePage
    │   │   ├── store // 模块store
    │   │   ├── views // 页面集合
    │   │   ├── config.ts // 配置，列表/动作事件等
    │   │   ├── index.vue // 当前模块主页面
    │   │   └── local.ts // 当前模块多语言
    ├── router // 路由
    ├── service // 公共模块api
    ├── store // 公共store
    ├── util // 工具
    │   ├── ...
    │   └── service.tx // 请求axios
    ├── vue-custom // vue 复用&组合
    │   ├── component // 公共组件配置
    │   ├── directive // vue 指令
    │   ├── filters // vue 过滤器
    │   ├── mixin // vue 混入
    │   └── prototype // 全局属性
    ├── index.ts // 首页入口
    ├── login.ts // 登陆入口
    ├── settings.ts // 样式设置
    ├── shims-tsx.d.ts
    ├── shims-vue.d.ts
    └── subMenu.json // 默认菜单数据
    ```

## 前端思路

    1. 为了规范可控，集成了TypeScript；
    2. 统一基础动作事件（增，删，改，查），代码提取到了/vue-custom/mixin/... 中；
    3. 业务模块组件化，代码 components/page/... 中；
    4. 业务逻辑尽量写在page中;
    5. 数据与请求业务模块在 pages/模块/store/index.ts中，[@/store/base/index]会根据api.ts的所有项生成vuex所需的结构，最终通过util/service.tx请求输出;

## 数据与请求，store 实现（当前部分需要 vuex 的基础）

> 业务代码的 store，会根据业务 api.ts 的内容生成，创建 store 初始结构，目前只创建 state，actions，mutations 部分，生成 vuex store 的代码在/store/base/index；

#### /store/base/index 生成规则：

1. api.ts 配置所有业务需要的请求及参数；然后（/store/base/index）根据 api.ts 中所有对象的 key，创建对应的 state，actions，mutations；
2. 命名规则，state >>> {首字母大写的 key}Data; mutations >>> set{首字母大写的 key}\_mutations； actions >>> key；
3. 只有 get 请求才会生成 state， mutations 部分；post 请求会在 action 方法中返回 response；
4. actions 默认只有 request，和 state 的赋值逻辑；如果逻辑不通可以在 store/index.ts 中复写，action 的命名与 key 一样便可以覆盖；

#### 例：

用法可以看注释

```JavaScript
// frameworkuser/store/api.ts

const search = {
  url: "/api/_FrameworkUserBase/search", // 请求api的地址；支持{}写法，如果url有 {ID}会根据请求参数中的ID替换内容；
  method: "post", // 请求类型
  dataType: "array", // 返回的数据类型 array | object
  contentType: "" // 请求herder.Content-Type值，默认json接口
};

const add = {
  url: reqPath + "Add",
  method: "post"
};
...

export default {
  search,
  add,
  ...
};

// frameworkuser/store/index.ts
// 默认写法如下，生成的stroe在对象newStore中；
import Vuex from "vuex";
import service from "./api";
import createStore from "@/store/base/index";
const newStore = createStore(service);
// 可以打印查看store结构
console.log('newStore:', newStore);
export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});

```

覆盖默认 store 写法，api.ts 不需要更改；

```JavaScript
// frameworkuser/store/index.ts
// 默认写法如下，生成新的stroe在对象newStore中；
import Vuex from "vuex";
import service from "./api";
import createStore from "@/store/base/index";
const newStore = createStore(service);
// 写法一
newStore.action = {
    ...newStore.action,
    search({ commit, state }, params) {
        // 新逻辑
    }
}
// 写法二
newStore.action.search = ({ commit, state }, params) => {
    // 新逻辑
}
export default new Vuex.Store({
  strict: true,
  getters: {},
  ...newStore
});

```

#### api.ts

定义 key 需要包涵 search,add,edit,detail,batchDelete,imported，exportExcel,getExcelTemplate；mixin 部分会用到这些 keys；

## vue-custom/mixin vue 混入部分：

> 混入 (mixin) 提供了一种非常灵活的方式，来分发 Vue 组件中的可复用功能。一个混入对象可以包含任意组件选项。当组件使用混入对象时，所有混入对象的选项将被“混合”进入该组件本身的选项， https://cn.vuejs.org/v2/guide/mixins.html

#### search.ts 查询方法复用(混入)

位置： vue-custom/mixin/search.ts
参数： TABLE_HEADER: 列表结构配置；
方法： 查询，排序，分页，列表选中数据，等方法

> 在混入的组件 methods 中添加命名为“privateRequest”的查询方法,查询组件自己的调用 action,当前方法（privateRequest）放到 action-mixin 中；

提供给 wtm-table-box 组件 参数/事件；

```JavaScript
    /**
     * 返回wtm-table-box事件集合
     * 包含el-table/el-pagination组件事件，命名定义要与组件（el-table，el-pagination）事件相同 可生效
     * el-table：sort-change
     * el-pagination：size-change，current-change，selection-change
     * 注：
     *    1. 需要添加elementui其他事件，可以在组件中写，不用添加此对象里；
     *    2. 可以定义方法覆盖事件；
     * 例如：
     * <wtm-table-box :events="searchEvent" @sort-change="()=>{覆盖方法}" @header-click="()=>{自定义方法}">
     */
    get searchEvent() {
      return {
        onSearch: this.onSearch,
        "size-change": this.handleSizeChange,
        "current-change": this.handleCurrentChange,
        "selection-change": this.onSelectionChange,
        "sort-change": this.onSortChange
      };
    }

    /**
     * 返回wtm-table-box属性集合
     * 包含el-table/el-pagination组件属性，命名与组件事件相同
     * wtm-table-box：loading组件加载判断。。。
     * el-table：data,is-selection,tb-header
     * el-pagination：pageSizes,pageSize,currentPage,pageTotal
     * 注：
     *    如searchEvent对象 一样；
     */
    get searchAttrs() {
      return {
        loading: this.loading,
        data: this.tableData,
        isSelection: true,
        tbHeader: this.tableHeader,
        ...this.pageDate
      };
    }
```

#### action-mixin 页中的按钮部分，添加/修改/删除/导出/导出 复用(混入)

提供业务所需的方法，可以参考 actionEvent 属性；

```JavaScript
    /**
     * 事件方法list
     * 提供wtm-but-box组件 参数/事件；
     */
    get actionEvent(): IActionEvent {
      return {
        onAdd: this.onAdd,
        onEdit: this.onEdit,
        onDetail: this.onDetail,
        onDelete: this.onDelete,
        onBatchDelete: this.onBatchDelete,
        onImported: this.onImported,
        onExportAll: this.onExportAll,
        onExport: this.onExport
      };
    }
```

#### form-mixin 操作代码 复用(混入)

弹出框（详情/编辑/创建）
参数：
dialogData：被编辑数据
status：弹出框的状态
actionType: 弹出框的状态-枚举

> 目前与 CreateForm 组件高度依赖，做一需要配合 CreateForm 组件

新增/编辑 提交所需的表单数据，是从 CreateForm 组件的 getFormData 方法获取的；如果组件中定义了 mergeFormData 对象，获取表单数据的时候会 merge 到一起；

> mergeFormData：组件中有需要自定义的表单参数，又不依赖 CreateForm 组件，就需要此参数；

```JavaScript
    /**
     * get merge formdata
     */
    private getFormData(data: object | null = null) {
      let formData = data || this.FormComp().getFormData(); // 从CreateForm组件中获取表单数据
      // 处理 array
      const customizer = (objValue, srcValue) => {
        if (_.isArray(objValue)) {
          return srcValue;
        }
      };
      formData = _.mergeWith(formData, this.mergeFormData, customizer); // merge （mergeFormData）数据
      return formData;
    }

```

定义：store 中的 action(新增 add | 修改 edit | 详情 detail)，写法：@Action("【api.tx 中的 key】") 【当前组件自定义 key】；
调用: 调用此处定义的 action，可以直接触发 api.ts 中 key 对应接口的请求；
状态: status，记录当前操作数据的状态；包含：新增 add | 修改 edit | 详情 detail

```JavaScript
    @Action("add") add; // 添加 》store
    @Action("edit") edit; // 修改 》store
    @Action("detail") detail; // 详情 》store
    // 表单状态
    @Prop({ type: String, default: "" })
    status;
```

## 组件介绍

大部分组件都是基于 element-ui 开发,所以尽量兼容或复用 element-ui 组件的参数/方法/事件等；
自己开发的组件可以在 vue-custom/component/index.ts 配置到全局；

### wtm-table-box 列表&分页 组件组合

支持 element-ui 的 el-table 和 el-pagination 的所有属性于方法，可以按照 element 文档使用；还有一些自定义属性；

#### 参数

|       参数        |                      说明                       |                 默认值                  |
| :---------------: | :---------------------------------------------: | :-------------------------------------: |
|      loading      |                    加载状态                     |                  必填                   |
|    isSelection    |   是否多选。表格第一列的复选框，用于删除/编辑   |                  false                  |
|     tbHeader      |                 列字段数据配置                  |                    -                    |
| defaultHeaderKeys |      默认列字段，默认展示列，用于多列隐藏       |                    -                    |
|      height       |         table height，不设置自动算高度          |                    -                    |
|    currentPage    |               当前页（分页属性）                |                    0                    |
|     pageSizes     |              页码集合（分页属性）               |            [10, 25, 50, 100]            |
|     pageSize      |              页码大小（分页属性）               |                    0                    |
|     pageTotal     |                总量（分页属性）                 |                    0                    |
|      layout       |              分页参数（分页属性）               | total, sizes, prev, pager, next, jumper |
|    languageKey    |                   多语言 key                    |                    -                    |
|      events       | 事件集合；分页，查询等事件，正常 search.ts 提供 |                   {}                    |
|       attrs       |          属性集合，正常 search.ts 提供          |                   {}                    |

### wtm-create-form (wtm-search-box 继承关系) 创建表单

根据 json 结构定义表单 ui，实现对数据的 增/改/展示 等操作；

> 参数

|      参数      |                                       说明                                       | 默认值 |
| :------------: | :------------------------------------------------------------------------------: | :----: |
|     status     |                                 状态，增/改/展示                                 |  add   |
|    options     |                                 表单数据结构配置                                 |  必填  |
| sourceFormData | 表单数据，非必填；如果填写此参数，组件的创建表单数据的方法便会失效；优先级最高； |   -    |
|   elDisabled   |                         控制表单中所有组件 是否 Disabled                         | false  |
|  languageKey   |                                    多语言 key                                    |   -    |

options.formProps: 表单的参数；

options.formItem: 表单数据项，key 的定义便是生成的表单结构；

> 参数:

```JavaScript
 /**
   * 组件
   */
  type?: string;
  /**
   * 标签文案
   */
  label?: string;
  /**
   * 校验
   */
  rules?: object;
  /**
   * el-form-item span
   */
  span?: number | string;
  /**
   * 组件children项
   */
  children?: ChildrenItem[];
  /**
   * 默认值
   */
  defaultValue?: any;
  /**
   * type对应组件的props, element-ui组件的参数
   */
  props?: object;
  /**
   * 组件数据结构 转换 接口组件数据结构 => { [mapKey]:value }
   */
  mapKey?: string;
  /**
   * 自定义 slot 数据，调用方 定义 mergeFormData
   */
  slotKey?: string;
  /**
   * 事件
   */
  events?: object;
  /**
   * 是否隐藏组件
   */
  isHidden?: boolean | HiddenFun;
```

> 方法

|    方法/属性    |             参数              |                                      说明                                      |       返回        |
| :-------------: | :---------------------------: | :----------------------------------------------------------------------------: | :---------------: |
|     elForm      |               -               |                               透传 el-form 组件                                | 返回 el-form 组件 |
|    validate     |           callback            | 透传 el-form，validate 事件，重定义 callback，验证通过，data 参数 改为表单数据 |         -         |
|   resetFields   |               -               |                               清空 el-form 验证                                |         -         |
|   getFormData   |               -               |                                  返回表单数据                                  |     表单数据      |
|   setFormData   |         data，修改值          |                                  设置表单数据                                  |     表单数据      |
| setFormDataItem | path: 表单 key, value: 修改值 |                                   设置表单项                                   |         -         |
|   getFormItem   |        path: 表单 key         |                                                                                | 返回 wtmformItem  |
