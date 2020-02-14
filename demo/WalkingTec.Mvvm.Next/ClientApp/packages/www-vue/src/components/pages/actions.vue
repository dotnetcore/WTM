<template>
  <div>
    <!-- Page Action-->
    <div v-if="isPageAction" class="page-action">
      <slot name="pageAction">
        <a-button @click="onInsert">
          <a-icon type="plus" />
          <span v-t="'action.insert'" />
        </a-button>
        <a-divider type="vertical" />
        <a-button @click="onUpdate" :disabled="disabledUpdate">
          <a-icon type="edit" />
          <span v-t="'action.update'" />
        </a-button>
        <a-divider type="vertical" />
        <a-popconfirm
          title="Are you sure delete this task?"
          @confirm="onDelete(PageStoreContext.SelectedRows)"
          okText="Yes"
          cancelText="No"
        >
          <a-button :disabled="disabledDelete">
            <a-icon type="delete" />
            <span v-t="'action.delete'" />
          </a-button>
        </a-popconfirm>
        <a-divider type="vertical" />
        <a-button @click="onImport">
          <a-icon type="cloud-upload" />
          <span v-t="'action.import'" />
        </a-button>
        <a-divider type="vertical" />
        <a-dropdown>
          <a-menu slot="overlay">
            <a-menu-item key="1" @click="onExport">导出全部</a-menu-item>
            <a-menu-item
              :disabled="disabledDelete"
              key="2"
              @click="onExportByIds(PageStoreContext.SelectedRows)"
            >导出勾选</a-menu-item>
          </a-menu>
          <a-button>
            <span v-t="'action.export'" />
            <a-icon type="down" />
          </a-button>
        </a-dropdown>
      </slot>
    </div>
    <!-- 行 数据  Action-->
    <div v-else-if="isRowAction" class="row-action">
      <slot name="rowAction">
        <a-button type="link" size="small" @click="onDetails(RowData)">
          <a-icon type="eye" />
        </a-button>
        <a-button type="link" size="small" @click="onUpdate(RowData)">
          <a-icon type="edit" />
        </a-button>
        <a-popconfirm
          title="Are you sure delete this task?"
          @confirm="onDelete([RowData])"
          okText="Yes"
          cancelText="No"
        >
          <a-button type="link" size="small">
            <a-icon type="delete" />
          </a-button>
        </a-popconfirm>
      </slot>
    </div>
    <a-modal
      class="page-action-modal"
      :title="$t(title)"
      :destroyOnClose="true"
      :visible="visible"
      @cancel="onVisible(false)"
      @ok="onOk"
    >
      <a-form layout="vertical" :form="form" :key="slotName">
        <a-spin :spinning="spinning">
          <a-icon slot="indicator" type="loading" style="font-size: 50px" spin />
          <a-row :gutter="24">
            <slot :name="slotName"></slot>
          </a-row>
        </a-spin>
      </a-form>
    </a-modal>
  </div>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue } from "vue-property-decorator";
import { EntitiesPageStore } from "@leng/public/src";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subscriber, Subject } from "rxjs";
import { toJS } from "mobx";
import ImportUpload from "./importUpload.vue";
import { messages } from "./messages";
// import { i18n } from "../../locale";
@Component({ components: {} })
export default class ViewAction extends Vue {
  /**
   * page 状态
   */
  @Prop() private PageStore: EntitiesPageStore;
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
  /**
   * 表单域
   */
  form: WrappedFormUtils;
  /**
   * 加载状态
   */
  spinning = false;
  /**
   * 显示 隐藏 状态
   */
  visible = false;
  /**
   * 标题
   */
  title = "";
  /**
   * slot key
   */
  slotName = "";
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
    return this.PageStoreContext.SelectedRows.length !== 1;
  }
  /**
   * 禁用 删除按钮
   */
  get disabledDelete() {
    return !this.PageStoreContext.IsSelectedRows;
  }
  /**
   * 组件 创建 初始化 表单域
   */
  beforeCreate() {
    const options = {
      onFieldsChange: (props, fields) => {
        this.FieldsChange.next({ props, fields, form: this.form });
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
  }
  beforeMount() {
    lodash.map(this.Entities, ent => {
      if (lodash.isFunction(ent.onComplete)) {
        ent.onComplete({ FieldsChange: this.FieldsChange });
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
  onOk(e?) {
    e && e.preventDefault();
    if (this.slotName === "Details") {
      return this.onVisible(false);
    }
    this.form.validateFieldsAndScroll(async (error, values) => {
      if (error) {
        console.log("TCL: onOk -> values", values);
        return console.error(error);
      }
      lodash.map(this.Entities, ({ mapKey }, key) => {
        // 转换 mapKey
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
        if (this.slotName === "Insert") {
          await this.PageStoreContext.onInsert({ body: values });
        }
        if (this.slotName === "Update") {
          await this.PageStoreContext.onUpdate({ body: values });
        }
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
    this.title = "action.update";
    this.slotName = "Update";
    this.onVisible(true);
    this.onGetDetails(item);
  }
  onImport() {
    this.$confirm({
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
  onExport() {
    this.PageStoreContext.onExport();
  }
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
        message: `Delete Success`
      });
      this.onSearch();
    } catch (error) {
      console.error("TCL: onDelete -> error", error);
      // this.$notification.error({
      //   description: "",
      //   message: error.message
      // });
    }
  }
  /**
   * 获取 详情 数据
   */
  async onGetDetails(item) {
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
</script>
<style scoped lang="less">
.page-action {
  text-align: right;
}
</style>
<style  lang="less">
.page-action-modal.ant-modal {
  min-width: 70vw;
  max-width: 99vw;
  overflow: hidden;
  top: 50px;
  .ant-modal-body {
    max-height: calc(100vh - 200px);
    overflow: auto; // padding-bottom: 65px;
  }
}
.page-import {
  .ant-modal-confirm-body .ant-modal-confirm-content {
    margin: 0;
  }
  .ant-modal-confirm-body > .anticon {
    display: none;
  }
}
</style>