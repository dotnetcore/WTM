<template>
  <div>
    <WtmAction :PageController="PageController" :params="rowParams">
    </WtmAction>
  </div>
</template>
<script lang="ts">
import { Vue, Options, mixins, Inject } from "vue-property-decorator";
import { ActionBasics } from "@/components";
import PageController from "../controller";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  // page Inject 注入 row 为 toRowAction 注入
  @Inject() PageController;
  created() {}
}
</script>
<style lang="less">
</style>
