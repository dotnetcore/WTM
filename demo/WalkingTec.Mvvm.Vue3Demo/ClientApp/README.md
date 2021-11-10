# 页面组成
-   [locales.ts] 页面的属性多语言枚举
-   [index.vue] 页面入口 Provide 当前页面的 PageController & PageEntity 所有子页面 都可以 Inject
-   [controller/entity.ts] 基础页面的实体属性 配置表单 Field
-   [controller/index.ts] 基础页面控制器所有的业务方法 继承 ControllerBasics 
-   [views/action.vue] 基础页面所有的操作按钮
-   [views/details.vue] WtmView 展示的 表单页面 Provide formState 供 WtmField 使用
-   [views/filter.vue] grid 搜索条件 默认slot展开的条件 slot#more 是收起条件
-   [views/grid.vue] grid 表格
## 一个基础页面由以上部分组成。
- 每个页面都有自己的 entity 和 controller 。
- entity中定义了当前页面所需要的 Field 字段。
- controller 继承 ControllerBasics 基础控制器，里面包含了 基础页面所需要的【数据状态】和【添加，修改，删除，导入，导出】等方法。
- 在每个页面的 index.vue 初始化好了当前页面的 PageController & PageEntity 在所有需要使用的子页面中使用 [Provide/Inject](https://vue3js.cn/docs/zh/guide/component-provide-inject.html) 使用。
- 数据的初始化 也就是第一次 拉取数据 在 filter.vue onFinish 函数中。当filter.vue初始化完成和查询按钮都会调用这个方法。
- 详情页面通过路由和【WtmView】组件控制，它会检测路由中的参数显示对应的页面，默认参数是 details 如需其他配置 通过props 更改。示例 【frameworkrole>views>privilege.vue】权限配置
-  
# 组件列表
-   [WtmView] 弹出框 视图 
-   [WtmAction] 页面&数据操作
-   [WtmField] 表单项
# Entity 配置
  ```ts
  // 类型定义 详见：src/client/declare.ts
  declare type WTM_EntitiesField = {
    /** 表单 Name 
     * 
     * ['Entity', 'ID'] -> Entity.ID
     * 'Name' -> Name
     *   */
    name: NamePath;
    /** 表单 label  */
    label?: string;
    /** 描述 */
    placeholder?: string;
    /** 默认值 */
    initialValue?: any;
    /** 校验规则 */
    rules?: ValidationRule[];
    /**
     *  数据源
     * 
     * 自定义数据
     * request: async (formState) => {
     *       return [
     *          { label: $i18n.t(EnumLocaleLabel.Sex_Male), value: 'Male' },
     *          { label: $i18n.t(EnumLocaleLabel.Sex_Female), value: 'Female'}
     *       ]
     *   },
     * 接口数据
     * request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles')
     */
    request?: ProFieldRequestData | undefined;
    /** 
     * 联动字段 数据更新 触发 request
     */
    linkage?: Array<string>;
    /** 数据类型 */
    valueType?: WTM_ValueType
}
  ```
# WTM_ValueType 类型
```ts
    /** 密码框 */
    password = "password",
    /** 文本域 */
    textarea = "textarea",
    /** 时间 */
    date = "date",
    dateWeek = "dateWeek",
    dateMonth = "dateMonth",
    dateRange = "dateRange",
    /** 文本 */
    text = "text",
    /** 选择框 */
    select = "select",
    /** 滑动输入 */
    slider = "slider",
    /** 多选框 */
    checkbox = "checkbox",
    /** 评分 */
    rate = "rate",
    /** 单选 */
    radio = "radio",
    /** 开关 */
    switch = "switch",
    /** 图片上传 */
    image = "image",
    /** 穿梭框 */
    transfer = "transfer",
    /** 文件 */
    upload = "upload", 
```
  
# WtmView 配置
```ts
   /** 
    * 路由 query参数的Key
    * 默认取 detailsVisible 
    * 检查到路由中存在显示
    * */
   queryKey: string;
  /** 标题  */
   title: string;
  /** 弹框类型 */
   modalType: "modal" | "drawer";
```
# WtmAction 配置
```ts
   /** 调试模式 true 不鉴权 */
  @Prop({ default: false }) debug;
  /** 
   * 包含 的 按钮 
   * 枚举 EnumActionType
   *  */
  @Prop({ default: () => [] }) include;
  /** 
   * 排除 的 按钮
   * 枚举 EnumActionType
   *  */
  @Prop({ default: () => [] }) exclude;
  /** 
   * 请求参数
   * 用于在路由中追加参数 默认只取 ID 追加 
   * 用途参考 dataprivilege/views/action.vue
   * */
  @Prop({}) toQuery;
  /** 页面控制器 */
  @Prop() readonly PageController;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
```

# WtmField 配置
## 当存在 entityKey 时 会从 PageEntity 中获取 优先级最低
```ts
    // form label 没有 取 name 数据
    @Prop({ type: String }) readonly label;
    // form name
    @Prop({ type: String }) readonly name;
    // 输入提示
    @Prop({ type: String }) readonly placeholder;
    // 校验
    @Prop({ type: Array }) readonly rules;
    // 联动
    @Prop({ type: Array }) readonly linkage;
    // 当前实体对应的 属性key
    @Prop({ type: String }) readonly entityKey;
    /** 给 field组件的 fieldProps */
    @Prop({}) readonly fieldProps;
    // 值类型
    @Prop({ type: String, default: "text" }) readonly valueType: WTM_ValueType;
    // 只读
    @Prop({ type: Boolean, default: false }) readonly readonly;
    // 禁用
    @Prop({ type: Boolean, default: false }) readonly disabled;
    // 测试日志
    @Prop({ type: Boolean, default: false }) readonly debug;
    // 数据源
    @Prop({ type: Function, default: () => [] }) readonly request;
    // 表单状态值 组件内 Inject
    // @Inject() 
    readonly formState;
    // 自定义校验状态 details/index.vue 中服务器返回 组件内 Inject
    // @Inject() 
    readonly formValidate;
    // 实体
    // @Inject() 组件内 Inject
    readonly PageEntity;
    // 表单类型
    // @Inject() 组件内 Inject
    readonly formType: 'details';
```