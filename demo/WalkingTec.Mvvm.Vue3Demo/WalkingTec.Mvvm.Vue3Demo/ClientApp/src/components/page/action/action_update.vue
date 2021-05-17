<template>
  <a-button v-if="isUpdate" v-bind="ButtonProps" :disabled="disabled" @click="onToDetails">
    <template #icon v-if="isPageAction">
      <EditOutlined />
    </template>
    <i18n-t :keypath="$locales.action_update" />
  </a-button>
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
    return !this.lodash.eq(this.Pagination.selectionDataSource.length, 1);
  }
  get dateKey() {
    if (this.isRowAction) {
      return this.rowKey;
    }
    return this.lodash.get(
      this.lodash.head(this.Pagination.selectionDataSource),
      this.PageController.key
    );
  }
  onToDetails() {
    this.__wtmToDetails(this.dateKey)
  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
