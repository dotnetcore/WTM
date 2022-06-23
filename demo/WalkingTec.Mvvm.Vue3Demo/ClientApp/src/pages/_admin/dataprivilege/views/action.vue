<template>
  <WtmAction :PageController="PageController" :params="params" :toQuery="toQuery" :include="include" v-if="!isRowAction"></WtmAction>

  <div v-else>
    <WtmAction :PageController="PageController" :params="params" :toQuery="toQuery" :include="includeIn">
      <a-popconfirm :title="title" :disabled="disabled" @confirm="onConfirm">
        <a-button  v-bind="ButtonProps" :disabled="disabled" class="ant-btn ant-btn-link ant-btn-sm">
          <template #icon v-if="isPageAction">
            <slot name="icon">
              <DeleteOutlined />
            </slot>
          </template>
          <slot>
            <i18n-t :keypath="$locales.action_delete" />
          </slot>
        </a-button>
      </a-popconfirm>
      </WtmAction>
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
  readonly params = {}
  toQuery(rowData) {
    console.log(rowData)
    rowData['details'] = rowData.ID
    return this.lodash.pick(rowData, ['TableName', 'TargetId', 'DpType','details'])
  }
  get include() {
    return [ this.EnumActionType.Insert, this.EnumActionType.Export]
  }
  get includeIn(){
    return [ this.EnumActionType.Info, this.EnumActionType.Update ]
  }
  get Pagination() {
    return this.PageController.Pagination;
  }
  get isRowAction() {
    return this.lodash.has(this.params, "node");
  }
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation);
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation);
  }
  get title() {
    return this.$t(this.$locales.action_deleteConfirm, {
      text: 1
    });
  }
  mounted() { }


  /**
   * 删除
   */
  async onConfirm() {
      const { successMsg, errorMsg } = this;
      const data = {
        ModelName:this.lodash.get(this.params, "data.TableName") || this.lodash.get(this.lodash.head(this.Pagination.selectionDataSource),'TableName'),
        Id:this.lodash.get(this.params, "data.TargetId") || this.lodash.get(this.lodash.head(this.Pagination.selectionDataSource),'TargetId'),
        Type:this.lodash.get(this.params, "data.DpType") || this.lodash.get(this.lodash.head(this.Pagination.selectionDataSource),'DpType'),
      }
      try {
        await this.PageController.onRemoveSimple(this.lodash.get(this.params, "data.ID"),data);
        this.$message.success(successMsg);
      } catch (error) {
        this.$message.error(errorMsg);
      }
  }
}
</script>
<style lang="less">
</style>
