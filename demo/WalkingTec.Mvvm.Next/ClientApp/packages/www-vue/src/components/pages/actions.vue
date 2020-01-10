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
        <a-button @click="onUpdate">
          <a-icon type="edit" />
          <span v-t="'action.update'" />
        </a-button>
        <a-divider type="vertical" />
        <a-button @click="onInsert">
          <a-icon type="delete" />
          <span v-t="'action.delete'" />
        </a-button>
        <a-divider type="vertical" />
        <a-button @click="onInsert">
          <a-icon type="cloud-upload" />
          <span v-t="'action.import'" />
        </a-button>
        <a-divider type="vertical" />
        <a-dropdown>
          <a-menu slot="overlay">
            <a-menu-item key="1">1st item</a-menu-item>
            <a-menu-item key="2">2nd item</a-menu-item>
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
        <a-button type="link" size="small" @click="onDetails">
          <a-icon type="eye" />
        </a-button>
        <a-button type="link" size="small" @click="onUpdate">
          <a-icon type="edit" />
        </a-button>
        <a-popconfirm title="Are you sure delete this task?" okText="Yes" cancelText="No">
          <a-button type="link" size="small">
            <a-icon type="delete" />
          </a-button>
        </a-popconfirm>
      </slot>
    </div>
    <a-modal
      class="page-action-modal"
      :title="title"
      :destroyOnClose="true"
      :visible="visible"
      @cancel="onVisible(false)"
      @ok="onOk"
    >
      <a-form layout="vertical" :form="form" :key="slotName">
        <a-row :gutter="24">
          <slot :name="slotName"></slot>
        </a-row>
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
@Component({ components: {} })
export default class ViewAction extends Vue {
  @Prop() private PageStore: EntitiesPageStore;
  @Prop() private params: ICellRendererParams;
  form: WrappedFormUtils;
  visible = false;
  title = "";
  slotName = "";
  get isPageAction() {
    return !lodash.hasIn(this, "params.rowIndex");
  }
  get isRowAction() {
    return lodash.hasIn(this, "params.data");
  }
  beforeCreate() {
    this.form = this.$form.createForm(this, {});
  }
  mounted() {}
  onVisible(visible = !this.visible) {
    this.visible = visible;
  }
  onOk(e?) {
    e && e.preventDefault();
    this.form.validateFields((error, values) => {
      console.log("TCL: ViewAction -> onOk -> values", values);
    });
  }
  onInsert() {
    this.title = "Insert";
    this.slotName = "Insert";
    this.onVisible(true);
  }
  onUpdate() {
    this.title = "Update";
    this.slotName = "Update";
    this.onVisible(true);
  }
  onDetails() {
    this.title = "Details";
    this.slotName = "Details";
    this.onVisible(true);
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
</style>