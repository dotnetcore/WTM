<template>
  <a-button
    v-if="isImport"
    v-bind="ButtonProps"
    @click="__wtmToDetails()"
  >
    <template #icon v-if="isPageAction">
      <CloudUploadOutlined />
    </template>
    <i18n-t keypath="action.import" />
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController:ControllerBasics;
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
