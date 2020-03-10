import { EntitiesPageStore } from "@leng/public/src";
import { ICellRendererParams } from "ag-grid-community";
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import lodash from "lodash";
import { toJS } from "mobx";
import { Observable, Subject } from "rxjs";
import { Component, Prop, Vue } from "vue-property-decorator";
import ImportUpload from "../upload/import.vue";
import { messages } from "./messages";
@Component({ components: {} })
export class ViewAction extends Vue {
  /**
   * page 状态
   */
  @Prop() private PageStore: EntitiesPageStore;
  /**
   * 拥有哪些操作
   */
  @Prop() private PageActions: any;
  /**
   * Field Change 变更 Subject
   */
  @Prop({
    default: () => new Subject()
  })
  private FieldsChange: Subject<{
    props: any;
    fields: any;
    form: WrappedFormUtils;
  }>;
  /**
   * ag grid 行 参数
   */
  @Prop() private params: ICellRendererParams;

  /**
   * 实体
   */
  @Prop() Entities: any;
  /**
   * 详情 数据 唯一标识 属性
   */
  @Prop({ default: () => "ID" }) GUID: string;
  @Prop({ default: () => "70vw" }) width: string | number;
  /**
   * 表单域
   */
  form: WrappedFormUtils;
  /**
   * 加载状态
   */
  title = "";
  /**
   * 加载状态
   */
  spinning = false;
  /**
   * 显示 隐藏 状态
   */
  visible = false;
  /**
   * slot key
   */
  slotName = "";
  /**
   * 是否 是 页面 操作
   */
  get CurrentPageActions() {
    return lodash.merge(
      {
        insert: lodash.hasIn(this.PageStoreContext, "options.Insert.url"),
        update: lodash.hasIn(this.PageStoreContext, "options.Update.url"),
        delete: lodash.hasIn(this.PageStoreContext, "options.Delete.url"),
        import: lodash.hasIn(this.PageStoreContext, "options.Import.url"),
        export: lodash.hasIn(this.PageStoreContext, "options.Export.url"),
        details: lodash.hasIn(this.PageStoreContext, "options.Details.url")
        // update:true,
      },
      this.PageActions
    );
  }
  /**
   * 是否 是 页面 操作
   */
  get isPageAction() {
    return !lodash.hasIn(this, "params.rowIndex");
  }
  /**
   * 是否 是 aggrid 行操作
   */
  get isRowAction() {
    return lodash.hasIn(this, "params.data");
  }
  /**
   * 行 操作 的 当前操作 数据
   */
  get RowData() {
    return lodash.get(this, "params.data");
  }
  /**
   * 获取 页面 状态，aggrid 行操作的状态在 context 中
   */
  get PageStoreContext(): EntitiesPageStore {
    return lodash.get(this, "params.context.PageStore", this.PageStore);
  }
  /**
   * 禁用 修改按钮
   */
  get disabledUpdate() {
    return this.SelectedRowsLength !== 1;
  }
  /**
   * 禁用 删除按钮
   */
  get disabledDelete() {
    return !this.PageStoreContext.IsSelectedRows;
  }
  /**
   * 选择的行 数据 长度
   */
  get SelectedRowsLength() {
    return this.PageStoreContext.SelectedRows.length;
  }
  /**
   * 组件 创建 初始化 表单域
   */
  beforeCreate() {
    const options = {
      onFieldsChange: (props, fields) => {
        this.FieldsChange.next({ props, fields, form: this.form });
        this.$emit("fieldsChange", props, fields);
      },
      validateMessages: messages
      // onValuesChange: (props, values) => {
      //   console.warn("TCL: ViewAction -> beforeCreate -> props", props, values);
      // }
    };
    if (this.$root.$i18n.locale === "en-US") {
      delete options.validateMessages;
    }
    this.form = this.$form.createForm(this, options);
    // console.log("TCL: ViewAction -> beforeCreate -> this", this);
  }
  beforeMount() {
    // 初始化  异步 组件
    lodash.map(this.Entities, ent => {
      if (lodash.isFunction(ent.onComplete)) {
        ent.onComplete({ FieldsChange: this.FieldsChange, form: this.form });
      }
    });
  }
  /**
   * 组件挂载
   */
  mounted() {
    // this.FormFieldsChange.subscribe(({ props, fields }) => {
    // console.log("TCL: ViewAction -> beforeCreate -> props", this.Entities);
    // });
  }
  /**
   *  更改 显示 弹框状态
   */
  onVisible(visible = !this.visible) {
    this.visible = visible;
  }
  /**
  * 表单 提交处理
  */
  async onSubmitValues(values) {
    if (this.slotName === "Insert") {
      // 提交内置的 添加插入
      await this.PageStoreContext.onInsert({ body: values });
    } else if (this.slotName === "Update") {
      // 提交内置的 修改
      await this.PageStoreContext.onUpdate({ body: values });
    } else {
      // 提交自定义 
      await new Observable(sub => {
        this.$emit(`submit${this.slotName}`, values, (backValues) => {
          sub.next(backValues)
          sub.complete();
        });
      }).toPromise();
    }
  }
  /**
   * 表单 提交处理
   */
  onSubmit(e?) {
    e && e.preventDefault();
    if (this.slotName === "Details") {
      return this.onVisible(false);
    }
    this.form.validateFieldsAndScroll(async (error, values) => {
      if (error) {
        console.log("TCL: onOk -> values", values);
        return console.error(error);
      }
      // 提供了 submit 事件 走提供的事件
      if (lodash.has(this, "_events.submit")) {
        return this.$emit("submit", values, this.slotName, this);
      }
      // 转换 mapKey
      lodash.map(this.Entities, ({ mapKey }, key) => {
        if (mapKey) {
          lodash.update(values, key, newValue => {
            if (lodash.isArray(newValue)) {
              return lodash.map(newValue, val => ({ [mapKey]: val }));
            }
            return newValue;
          });
        }
      });
      try {
        await this.onSubmitValues(values);
        this.onVisible(false);
        this.$notification.success({
          description: "",
          message: `${this.slotName} Success`
        });
        this.onSearch();
      } catch (error) {
        console.log("TCL: onOk -> error", error);
        const FormError = lodash.get(error, "response.Form");
        if (FormError) {
          const { setFields, getFieldValue } = this.form;
          setFields(
            lodash.mapValues(FormError, (error, key) => {
              return {
                value: getFieldValue(key),
                errors: [new Error(error)]
              };
            })
          );
        }
      }
    });
  }
  /**
   * 重置 搜索
   */
  onSearch() {
    this.PageStoreContext.EventSubject.next({
      EventType: "onSearch",
      AjaxRequest: {
        body: {
          ...toJS(this.PageStoreContext.SearchParams),
          Page: 1
        }
      }
    });
  }
  /**
   * 添加 按钮 事件
   */
  onInsert() {
    this.title = "action.insert";
    this.slotName = "Insert";
    this.onVisible(true);
  }
  /**
   * 修改 按钮 事件
   */
  onUpdate(item) {
    if (lodash.isArray(item)) {
      item = lodash.head(item)
    }
    this.title = "action.update";
    this.slotName = "Update";
    this.onVisible(true);
    this.onGetDetails(item);
  }
  /**
   * 导入
   */
  onImport() {
    this.$confirm({
      parentContext: null,
      class: "page-import",
      // destroyOnClose:true,
      // content: new Vue({ i18n, render: h => h(ImportUpload) })
      content: h =>
        h(ImportUpload, {
          props: {
            PageStore: this.PageStoreContext,
            i18n: lodash.get(this, "_i18n")
          }
          // $i18n: this.$root.$i18n
        })
    });
  }
  /**
   * 导出
   */
  onExport() {
    this.PageStoreContext.onExport();
  }
  /**
   * 导出 勾选
   */
  onExportByIds(items) {
    this.PageStoreContext.onExport("ExportByIds", {
      body: lodash.map(items, this.GUID)
    });
  }
  /**
   * 详情按钮事件
   */
  onDetails(item) {
    this.title = "action.info";
    this.slotName = "Details";
    this.onVisible(true);
    this.onGetDetails(item);
  }
  /**
   * 删除 选择的数据
   */
  async onDelete(items) {
    try {
      await this.PageStoreContext.onDelete({
        body: lodash.map(items, this.GUID)
      });
      this.$notification.success({
        description: "",
        message: this.$t("tips.success.operation") as any
      });
      this.onSearch();
    } catch (error) {
      console.error("TCL: onDelete -> error", error);
      this.$notification.error({
        description: "",
        message: this.$t("tips.error.operation") as any
      });
    }
  }
  /**
   * 获取 详情 数据
   */
  async onGetDetails(item = this.RowData) {
    this.spinning = true;
    let details = await this.PageStoreContext.onDetails({ body: item });
    // 转换 mapKey
    details = lodash.mapValues(this.Entities, (value, key) => {
      const { mapKey } = value;
      let newValue = lodash.get(details, key);
      if (mapKey && lodash.isArray(newValue)) {
        newValue = lodash.map(newValue, mapKey);
      }
      return newValue;
    });
    this.spinning = false;
    this.form.setFieldsValue(details);
  }
}