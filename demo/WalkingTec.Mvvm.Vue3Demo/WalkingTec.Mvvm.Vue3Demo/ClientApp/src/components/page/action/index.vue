<template>
  <a-space :size="size">
    <!-- 添加 -->
    <slot name="insert">
      <ActionInsert :PageController="PageController" :params="params" />
    </slot>
    <!-- 修改 -->
    <slot name="update">
      <ActionUpdate :PageController="PageController" :params="params" />
    </slot>
    <!-- 删除 -->
    <slot name="delete">
      <ActionDelete :PageController="PageController" :params="params" />
    </slot>
    <!-- 导入  -->
    <slot name="import">
      <ActionExport :PageController="PageController" :params="params" />
    </slot>
    <!-- 导出 -->
    <slot name="export">
      <ActionImport :PageController="PageController" :params="params" />
    </slot>
    <!-- 追加内容 -->
    <slot />
  </a-space>
</template>
<script lang="ts">
import { Options,Prop,Vue } from "vue-property-decorator";
import ActionDelete from "./action_delete.vue";
import ActionExport from "./action_export.vue";
import ActionImport from "./action_import.vue";
import ActionInsert from "./action_insert.vue";
import ActionUpdate from "./action_update.vue";
@Options({
  components: {
    ActionInsert,
    ActionUpdate,
    ActionDelete,
    ActionExport,
    ActionImport,
  },
})
export default class extends Vue {
  @Prop() readonly PageController;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
  size = 10;
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
