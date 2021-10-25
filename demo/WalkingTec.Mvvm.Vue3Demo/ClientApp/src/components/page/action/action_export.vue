<template>
  <a-dropdown v-if="isExport">
    <template #overlay>
      <a-menu @click="onMenuClick">
        <a-menu-item
          :key="EnumActionType.Export"
          v-if="onAuthority(EnumActionType.Export)"
        >
          <i18n-t :keypath="$locales.action_exportAll" />
        </a-menu-item>
        <a-menu-item
          :key="EnumActionType.ExportIds"
          :disabled="disabled"
          v-if="onAuthority(EnumActionType.ExportIds)"
        >
          <i18n-t :keypath="$locales.action_exportSelect" />
        </a-menu-item>
      </a-menu>
    </template>
    <a-button v-bind="ButtonProps">
      <template #icon v-if="isPageAction">
        <slot name="icon">
          <CloudDownloadOutlined />
        </slot>
      </template>
      <slot>
        <i18n-t :keypath="$locales.action_export" />
        <DownOutlined />
      </slot>
    </a-button>
  </a-dropdown>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
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
  get disabled() {
    return !this.Pagination.selectionDataSource.length;
  }
  onMenuClick(event) {
    switch (event.key) {
      case this.EnumActionType.Export:
        this.PageController.onExport(this.Pagination.oldBody);
        break;
      case this.EnumActionType.ExportIds:
        this.PageController.onExportIds();
        break;
    }
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
