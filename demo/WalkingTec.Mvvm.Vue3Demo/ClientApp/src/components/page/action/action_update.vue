<template>
  <a-button
    v-if="isUpdate"
    v-bind="ButtonProps"
    :disabled="disabled"
    @click="onToDetails"
  >
    <template #icon v-if="isPageAction">
      <slot name="icon">
        <EditOutlined />
      </slot>
    </template>
    <slot>
      <i18n-t :keypath="$locales.action_update" />
    </slot>
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { $locales, ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
  get dateKey() {
    if (this.isRowAction) {
      return this.rowKey;
    }
    return this.lodash.get(
      this.lodash.head(this.Pagination.selectionDataSource),
      this.PageController.key
    );
  }
  /** 请求参数 */
  @Prop({}) toQuery;
  get disabled() {
    if (this.isRowAction) {
      return false;
    }
    return !this.Pagination.selectionDataSource.length;
  }
  getRowData() {
    if (this.isRowAction) {
      return this.lodash.cloneDeep(this.rowParams.data);
    }
    return this.lodash.cloneDeep(
      this.lodash.head(this.Pagination.selectionDataSource)
    );
  }
  onToDetails() {
    if (!this.isRowAction && this.Pagination.selectionDataSource.length > 1) {
      return this.__wtmToDetails({ details: "", _batch: "" });
    }
    const rowData = this.getRowData();
    let query = {};
    if (this.lodash.hasIn(this.$props, "toQuery")) {
      query = this.lodash.invoke(this.$props, "toQuery", rowData, this);
      if (!this.lodash.isEmpty(query)) {
        return this.__wtmToDetails(query);
      }
    }
    this.__wtmToDetails(
      this.lodash.assign(
        {
          [this.$WtmConfig.detailsVisible]: this.lodash.get(
            rowData,
            this.PageController.key
          )
        },
        query
      )
    );
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
