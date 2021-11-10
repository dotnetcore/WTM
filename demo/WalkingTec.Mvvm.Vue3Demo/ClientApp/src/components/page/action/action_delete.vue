<template>
  <a-popconfirm :title="title" :disabled="disabled" @confirm="onConfirm">
    <a-button v-if="isDelete" v-bind="ButtonProps" :disabled="disabled">
      <template #icon v-if="isPageAction">
        <DeleteOutlined />
      </template>
      <i18n-t :keypath="$locales.action_delete" />
    </a-button>
  </a-popconfirm>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  get disabled() {
    if (this.isRowAction) {
      return false;
    }
    return !this.Pagination.selectionDataSource.length;
  }
  get title() {
    return this.$t(this.$locales.action_deleteConfirm, { text: this.isRowAction ? 1 : this.Pagination.selectionDataSource.length })
  }
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation)
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation)
  }
  async onConfirm() {
    // 得先获取 内容 删除后获取 组件卸载就娶不到了
    const { successMsg, errorMsg } = this;
    try {
      const remKey = this.isRowAction ? this.rowKey : this.Pagination.selectionDataSource
      await this.PageController.onRemove(remKey);
      this.$message.success(successMsg)
    } catch (error) {
      this.$message.error(errorMsg)
    }
  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
