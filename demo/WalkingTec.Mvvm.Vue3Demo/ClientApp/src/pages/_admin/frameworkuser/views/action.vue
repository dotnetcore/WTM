<template>
  <WtmAction
    :PageController="PageController"
    :params="params"
    v-if="isRowAction"
  />
  <div v-else>
    <a-button @click="__wtmToDetails()">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <i18n-t :keypath="$locales.action_insert" />
    </a-button>
    <WtmActionDelete :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>我是删除按钮</span>
    </WtmActionDelete>
    <WtmActionExport :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>WtmActionExport</span>
    </WtmActionExport>
    <WtmActionImport :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>WtmActionImport</span>
    </WtmActionImport>
    <WtmActionInfo :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>WtmActionInfo</span>
    </WtmActionInfo>
    <WtmActionInsert :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>WtmActionInsert</span>
    </WtmActionInsert>
    <WtmActionUpdate :PageController="PageController" :params="params">
      <template #icon>
        <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
      </template>
      <span>WtmActionUpdate</span>
    </WtmActionUpdate>
  </div>
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
   * 行数据操作 有 aggrid 传入属性
   * @readonly
   * @memberof Action
   */
  get isRowAction() {
    return this.lodash.has(this.params, "node");
  }
  mounted() {}
}
</script>
<style lang="less"></style>
