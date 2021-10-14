<template>
  <a-space class="w-action-space" :size="size">
    <a-config-provider :locale="locale">
      <!-- 添加 -->
      <slot name="insert" v-if="onShow(EnumActionType.Insert)">
        <ActionInsert v-bind="$props" />
      </slot>
      <!-- 查看 -->
      <slot name="update" v-if="onShow(EnumActionType.Info)">
        <ActionInfo v-bind="$props" />
      </slot>
      <slot v-bind="slotProps" />

      <!-- 修改 -->
      <slot name="update" v-if="onShow(EnumActionType.Update)">
        <ActionUpdate v-bind="$props" />
      </slot>
      <!-- 删除 -->
      <slot name="delete" v-if="onShow(EnumActionType.Delete)">
        <ActionDelete v-bind="$props" />
      </slot>
      <!-- 导入  -->
      <slot name="import" v-if="onShow(EnumActionType.Import)">
        <ActionImport v-bind="$props" />
      </slot>
      <!-- 导出 -->
      <slot name="export" v-if="onShow(EnumActionType.Export)">
        <ActionExport v-bind="$props" />
      </slot>
    </a-config-provider>
    <!-- 追加内容 -->
  </a-space>
</template>
<script lang="ts">
import { EnumActionType } from "@/client";
import { Options, Prop, mixins } from "vue-property-decorator";
import en from "ant-design-vue/es/locale/en_US";
import zh from "ant-design-vue/es/locale/zh_CN";
import ActionDelete from "./action_delete.vue";
import ActionExport from "./action_export.vue";
import ActionImport from "./action_import.vue";
import ActionInsert from "./action_insert.vue";
import ActionUpdate from "./action_update.vue";
import ActionInfo from "./action_info.vue";
import { ActionBasics } from "./script";
@Options({
  components: {
    ActionInsert,
    ActionUpdate,
    ActionDelete,
    ActionExport,
    ActionImport,
    ActionInfo
  }
})
export default class extends mixins(ActionBasics) {
  /** 调试 不鉴权 */
  @Prop({ default: false }) debug;
  /** 包含 */
  @Prop({
    default: () => [
      EnumActionType.Info,
      EnumActionType.Insert,
      EnumActionType.Update,
      EnumActionType.Delete,
      EnumActionType.Import,
      EnumActionType.Export
    ]
  })
  include;
  /** 排除 */
  @Prop({ default: () => [] }) exclude;
  /** 请求参数 */
  @Prop({}) toQuery;
  /** 页面控制器 */
  @Prop() readonly PageController;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
  get locale() {
    return { en, zh }[this.$i18n.locale];
  }
  size = 10;
  get slotProps() {
    return this.lodash.pick(this, [
      "isRowAction",
      "isPageAction",
      "disabled",
      "ButtonProps",
      "dateKey"
    ]);
  }
  get disabled() {
    if (this.isRowAction) {
      return false;
    }
    return !this.Pagination.selectionDataSource.length;
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
  onShow(type: EnumActionType) {
    return (
      this.lodash.includes(this.include, type) &&
      !this.lodash.includes(this.exclude, type)
    );
  }
  created() {}
  mounted() {
    // console.log("LENG ~ extends ~ mounted ~ this.$props", this.$props)
  }
}
</script>
<style lang="less">
.w-action-space {
  width: 100%;
  display: block !important;
  text-align: right;
  > .ant-space-item {
    display: inline-block !important;
  }
}
</style>
