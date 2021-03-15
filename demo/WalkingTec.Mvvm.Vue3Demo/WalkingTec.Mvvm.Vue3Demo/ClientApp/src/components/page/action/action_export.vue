<template>
  <a-button
    v-if="isExport"
    v-bind="ButtonProps"
    @click="__wtmToDetails()"
  >
    <template #icon v-if="isPageAction">
      <CloudDownloadOutlined />
    </template>
    <i18n-t keypath="action.export" />
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() PageController:ControllerBasics;
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
