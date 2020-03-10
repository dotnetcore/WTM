<template>
  <div>
    <!-- Page Action-->
    <div v-if="isPageAction" class="page-action">
      <slot name="pageActionLeft" ></slot>
      <slot name="pageAction">
        <a-button v-if="CurrentPageActions.insert" @click="onInsert">
          <a-icon type="plus" />
          <span v-t="'action.insert'" />
        </a-button>
        <a-divider v-if="CurrentPageActions.update" type="vertical" />
        <a-button v-if="CurrentPageActions.update" @click="onUpdate(PageStoreContext.SelectedRows)" :disabled="disabledUpdate">
          <a-icon type="edit" />
          <span v-t="'action.update'" />
        </a-button>
        <a-divider v-if="CurrentPageActions.delete" type="vertical" />
        <a-popconfirm
          v-if="CurrentPageActions.delete"
          :title="$t('action.deleteConfirmMultiple', { text:SelectedRowsLength  })"
          @confirm="onDelete(PageStoreContext.SelectedRows)"
          :trigger="disabledDelete?'focus':'click'"
          okText="Yes"
          cancelText="No"
        >
          <a-button :disabled="disabledDelete">
            <a-icon type="delete" />
            <span v-t="'action.delete'" />
          </a-button>
        </a-popconfirm>
        <a-divider v-if="CurrentPageActions.import" type="vertical" />
        <a-button v-if="CurrentPageActions.import" @click="onImport">
          <a-icon type="cloud-upload" />
          <span v-t="'action.import'" />
        </a-button>
        <a-divider v-if="CurrentPageActions.export" type="vertical" />
        <a-dropdown v-if="CurrentPageActions.export">
          <a-menu slot="overlay">
            <a-menu-item key="1" @click="onExport" v-t="'action.exportAll'"></a-menu-item>
            <a-menu-item
              :disabled="disabledDelete"
              key="2"
              @click="onExportByIds(PageStoreContext.SelectedRows)"
              v-t="'action.exportSelect'"
            ></a-menu-item>
          </a-menu>
          <a-button>
            <span v-t="'action.export'" />
            <a-icon type="down" />
          </a-button>
        </a-dropdown>
      </slot>
      <slot name="pageActionRight"></slot>
    </div>
    <!-- 行 数据  Action-->
    <div v-else-if="isRowAction" class="row-action">
      <slot name="rowActionLeft"></slot>
      <slot name="rowAction">
        <a-button
          v-if="CurrentPageActions.details"
          type="link"
          size="small"
          @click="onDetails(RowData)"
        >
          <a-icon type="eye" />
        </a-button>
        <a-button
          v-if="CurrentPageActions.update"
          type="link"
          size="small"
          @click="onUpdate(RowData)"
        >
          <a-icon type="edit" />
        </a-button>
        <a-popconfirm
          v-if="CurrentPageActions.delete"
          :title="$t('action.deleteConfirm')"
          @confirm="onDelete([RowData])"
          okText="Yes"
          cancelText="No"
        >
          <a-button type="link" size="small">
            <a-icon type="delete" />
          </a-button>
        </a-popconfirm>
      </slot>
      <slot name="rowActionRight"></slot>
    </div>
    <w-modal
      :visible="visible"
      :width="width"
      :slotName="slotName"
      :title="$t(title)"
      :PageStore="PageStoreContext"
      @cancel="onVisible(false)"
      @submit="onSubmit"
    >
      <template>
        <a-form layout="vertical" :form="form" :key="slotName">
          <a-spin :spinning="spinning">
            <a-icon slot="indicator" type="loading" style="font-size: 50px" spin />
            <a-row :gutter="24">
              <slot :name="slotName"></slot>
            </a-row>
          </a-spin>
        </a-form>
      </template>
    </w-modal>
  </div>
</template> 
<script lang="ts">
import { ViewAction } from "./actions";
export default  ViewAction
</script>
<style scoped lang="less">
.page-action {
  text-align: right;
}
</style>