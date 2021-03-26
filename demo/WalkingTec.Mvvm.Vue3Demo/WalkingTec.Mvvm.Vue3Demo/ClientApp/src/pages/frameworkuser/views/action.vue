<template>
  <WtmAction :PageController="PageController" :params="params" />
</template>
<script lang="ts">
import { Vue, Options, mixins, Inject } from "vue-property-decorator";
import { ActionBasics } from "@/components";
import PageController from "../controller";
@Options({ components: {} })
export default class extends Vue {
  // page Inject 注入 row 为 toRowAction 注入
  @Inject() readonly PageController;
  /**
   * 行 操作 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  readonly params = {};
  mounted() {}
}
</script>
<style lang="less">
</style>
