<template>
  <a-form :form="form" @submit="onSubmit" class="page-filter-form">
    <a-row :gutter="24" type="flex">
      <slot></slot>
      <a-col class="page-filter-btns">
        <a-button type="primary" htmlType="submit">提交</a-button>
        <a-divider type="vertical" />
        <a-button @click="onReset">Clear</a-button>
      </a-col>
    </a-row>
  </a-form>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue } from "vue-property-decorator";
import { EntitiesPageStore } from "@leng/public/src";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subscriber, Subject } from "rxjs";
@Component({ components: {} })
export default class ViewAction extends Vue {
  @Prop() private PageStore: EntitiesPageStore;
  @Prop() private FieldsChange: Subject<{
    props: any;
    fields: any;
    form: WrappedFormUtils;
  }>;
  form: WrappedFormUtils;
  spinning = false;
  beforeCreate() {
    this.form = this.$form.createForm(this, {
      onFieldsChange: (props, fields) => {
        this.FieldsChange.next({ props, fields, form: this.form });
      }
    });
  }
  mounted() {
    this.onSubmit();
  }
  onSearch(body?) {
    this.PageStore.EventSubject.next({
      EventType: "onSearch",
      AjaxRequest: {
        body: {
          Page: 1,
          Limit: this.PageStore.PageSize,
          ...body
        }
      }
    });
  }

  onSubmit(e?) {
    e && e.preventDefault();
    this.form.validateFields((error, values) => {
      console.log("TCL: ViewAction -> onSubmit -> values", values);
      this.onSearch(values);
    });
  }
  onReset() {
    this.form.resetFields();
    this.onSubmit();
  }
}
</script>
<style  lang="less">
.page-filter-form {
  user-select: none;

  .page-filter-btns {
    text-align: right;
    flex: auto;
  }

  .ant-form-item {
    display: flex;
    margin-bottom: 5px;
  }

  .ant-form-item-control-wrapper {
    flex: 1;
  }
}
</style>