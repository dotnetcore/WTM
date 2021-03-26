<template>
  <a-popconfirm
    title="Are you sure delete this task?"
    ok-text="Yes"
    cancel-text="No"
    :disabled="disabled"
    @confirm="onConfirm"
  >
    <a-button v-if="isDelete" v-bind="ButtonProps" :disabled="disabled">
      <template #icon v-if="isPageAction">
        <DeleteOutlined />
      </template>
      <i18n-t keypath="action.delete" />
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
  onConfirm() {
    this.Pagination.onRemove(this.rowKey);
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
