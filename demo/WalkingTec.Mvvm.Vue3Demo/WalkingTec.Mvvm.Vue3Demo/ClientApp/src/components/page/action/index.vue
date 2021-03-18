<template>
  <a-space :size="styles('space')">
    <!-- 添加 -->
    <slot name="insert">
      <a-button
        v-if="isInsert"
        v-bind="styles(EnumActionType.Insert)"
        @click="__wtmToDetails()"
      >
        <template #icon v-if="isPageAction">
          <DownloadOutlined />
        </template>
        <i18n-t keypath="action.insert" />
      </a-button>
    </slot>
    <!-- 修改 -->
    <slot name="update">
      <a-button
        v-if="isUpdate"
        v-bind="styles(EnumActionType.Update)"
        :disabled="!rowKey"
        @click="__wtmToDetails(rowKey)"
      >
        <template #icon v-if="isPageAction">
          <DownloadOutlined />
        </template>
        <i18n-t keypath="action.update" />
      </a-button>
    </slot>
    <!-- 删除 -->
    <slot name="delete">
      <a-button
        v-if="isDelete"
        v-bind="styles(EnumActionType.Delete)"
        @click="__wtmToDetails()"
      >
        <template #icon v-if="isPageAction">
          <DownloadOutlined />
        </template>
        <i18n-t keypath="action.delete" />
      </a-button>
    </slot>
    <!-- 导入  -->
    <slot name="import">
      <a-button v-if="isImport" v-bind="styles()" @click="__wtmToDetails()">
        <template #icon v-if="isPageAction">
          <DownloadOutlined />
        </template>
        <i18n-t keypath="action.import" />
      </a-button>
    </slot>
    <!-- 导出 -->
    <slot name="export">
      <a-button v-if="isExport" v-bind="styles()" @click="__wtmToDetails()">
        <template #icon v-if="isPageAction">
          <DownloadOutlined />
        </template>
        <i18n-t keypath="action.export" />
      </a-button>
    </slot>
    <!-- 追加内容 -->
    <slot />
  </a-space>
</template>
<script lang="ts">
import { ActionBasics } from "./script";
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() PageController;
  created() {}
  mounted() {
  }
}
</script>
<style lang="less">
.test {
  color: @primary-color;
}
</style>
