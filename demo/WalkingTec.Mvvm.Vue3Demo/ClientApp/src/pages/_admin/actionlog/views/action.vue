<template>
  <WtmAction :include="include" :PageController="PageController" :params="params" />
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends Vue {
  // page Inject 注入 row 为 toRowAction 注入
  @Inject() readonly PageController: PageController;
  /**
   * 行 操作 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  readonly params = {};
  /**
   * 包含的操作按钮
   */
  get include() {
    return [this.EnumActionType.Info, this.EnumActionType.Delete, this.EnumActionType.Export]
  }
  mounted() { }
}
</script>
<style lang="less">
</style>
