<template>
  <a-button
    v-if="isUpdate"
    v-bind="ButtonProps"
    :disabled="disabled"
    @click="__wtmToDetails(dateKey)"
  >
    <template #icon v-if="isPageAction">
      <EditOutlined />
    </template>
    <i18n-t keypath="action.update" />
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() PageController: ControllerBasics;
  get disabled() {
    if (this.isRowAction) {
      return false;
    }
    return !this.lodash.eq(this.Pagination.selectionDataSource.length, 1);
  }
  get dateKey() {
    if (this.isRowAction) {
      return this.rowKey;
    }
    console.log(this.Pagination);
    return this.lodash.get(
      this.lodash.head(this.Pagination.selectionDataSource),
      this.PageController.key
    );
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
