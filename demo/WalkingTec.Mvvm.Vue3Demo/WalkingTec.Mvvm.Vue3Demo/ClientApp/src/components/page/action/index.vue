<template>
  <a-space :size="size">
    <!-- 添加 -->
    <slot name="insert" v-if="onShow(EnumActionType.Insert)">
      <ActionInsert :PageController="PageController" :params="params" />
    </slot>
    <!-- 查看 -->
    <slot name="update" v-if="onShow(EnumActionType.Info)">
      <ActionInfo :PageController="PageController" :params="params" />
    </slot>
    <!-- 修改 -->
    <slot name="update" v-if="onShow(EnumActionType.Update)">
      <ActionUpdate :PageController="PageController" :params="params" />
    </slot>
    <!-- 删除 -->
    <slot name="delete" v-if="onShow(EnumActionType.Delete)">
      <ActionDelete :PageController="PageController" :params="params" />
    </slot>
    <!-- 导入  -->
    <slot name="import" v-if="onShow(EnumActionType.Import)">
      <ActionExport :PageController="PageController" :params="params" />
    </slot>
    <!-- 导出 -->
    <slot name="export" v-if="onShow(EnumActionType.Export)">
      <ActionImport :PageController="PageController" :params="params" />
    </slot>
    <!-- 追加内容 -->
    <slot />
  </a-space>
</template>
<script lang="ts">
import { EnumActionType } from "@/client";
import { Options, Prop, Vue } from "vue-property-decorator";
import ActionDelete from "./action_delete.vue";
import ActionExport from "./action_export.vue";
import ActionImport from "./action_import.vue";
import ActionInsert from "./action_insert.vue";
import ActionUpdate from "./action_update.vue";
import ActionInfo from "./action_info.vue";
@Options({
  components: {
    ActionInsert,
    ActionUpdate,
    ActionDelete,
    ActionExport,
    ActionImport,
    ActionInfo,
  },
})
export default class extends Vue {
  /** 包含 */
  @Prop({ default: () => [EnumActionType.Info, EnumActionType.Insert, EnumActionType.Update, EnumActionType.Delete, EnumActionType.Import, EnumActionType.Export] }) include;
  /** 排除 */
  @Prop({ default: () => [] }) exclude;
  /** 页面控制器 */
  @Prop() readonly PageController;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
  size = 10;
  onShow(type: EnumActionType) {
    return this.lodash.includes(this.include, type) && !this.lodash.includes(this.exclude, type)
  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
