<template>
  <WtmAction :PageController="PageController" :params="params">
    <!-- 作用域插槽 其他插槽 看 components>page>action-->
    <template v-slot:default="{ isPageAction, ButtonProps }">
      <a-button
        v-if="__wtmAuthority('EditPrivilege', PageController)"
        v-bind="ButtonProps"
        :disabled="disabled"
        @click="onPrivilege"
      >
        <template #icon v-if="isPageAction">
          <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
        </template>
        <i18n-t :keypath="$locales.action_privilege" />
      </a-button>
    </template>
    <!-- <template v-slot:insert="{ isPageAction, ButtonProps }">
      <a-button>
        <template #icon v-if="isPageAction">
          <span class="_wtmicon _wtmicon-shoucang_shixin"></span>
        </template>
        我是添加按钮
      </a-button>
    </template> -->
  </WtmAction>
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends Vue {
  // page Inject 注入 row 为 toRowAction 注入
  @Inject() readonly PageController: PageController;
  get Pagination() {
    return this.PageController.Pagination;
  }
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
  get disabled() {
    if (this.isRowAction) {
      return false;
    }
    return this.Pagination.selectionDataSource.length !== 1;
  }
  /**
   * 分配权限
   */
  onPrivilege() {
    const dataKey = this.isRowAction
      ? this.lodash.get(this.params, "data.ID")
      : this.lodash.get(
          this.lodash.head(this.Pagination.selectionDataSource),
          this.PageController.key
        );
    this.__wtmToDetails({ privilege: dataKey });
  }
  mounted() {}
}
</script>
<style lang="less"></style>
