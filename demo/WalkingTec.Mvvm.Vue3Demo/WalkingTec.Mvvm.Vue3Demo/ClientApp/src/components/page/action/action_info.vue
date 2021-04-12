<template>
  <a-button v-if="isInfo" v-bind="ButtonProps" :disabled="disabled" @click="onToDetails">
    <template #icon v-if="isPageAction">
      <EditOutlined />
    </template>
    <i18n-t keypath="action.info" />
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
    let rowKey = ''
    if (this.isRowAction) {
      rowKey = this.rowKey
    } else {
      rowKey = this.lodash.get(
        this.lodash.head(this.Pagination.selectionDataSource),
        this.PageController.key
      )
    }
    return {
      [this.$WtmConfig.detailsVisible]: rowKey,
      _readonly: ''
    };
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
