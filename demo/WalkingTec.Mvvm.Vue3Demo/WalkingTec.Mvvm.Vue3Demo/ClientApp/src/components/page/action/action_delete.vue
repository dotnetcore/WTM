<template>
  <a-button
    v-if="isDelete"
    v-bind="ButtonProps"
    @click="__wtmToDetails()"
    :disabled="disabled"
  >
    <template #icon v-if="isPageAction">
      <DeleteOutlined />
    </template>
    <i18n-t keypath="action.delete" />
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
    return !this.Pagination.selectionDataSource.length;
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
