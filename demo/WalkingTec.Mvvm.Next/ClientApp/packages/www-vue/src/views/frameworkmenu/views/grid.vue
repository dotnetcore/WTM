<template>
  <w-grid :PageStore="PageStore" :GridOptions="GridOptions" />
  <!-- <w-grid
    :PageStore="PageStore"
    :GridOptions="GridOptions"
    :rowData="rowData"
    :columnDefs="columnDefs"
  />-->
</template>
<script lang="ts">
import lodash from "lodash";
import { Component, Prop, Vue, Inject } from "vue-property-decorator";
import PageStore from "../store";
import { GridOptions } from "ag-grid-community";
import { toJS } from "mobx";
import Action from "./action.vue";
import { GridIcon } from "./icon.vue";
@Component
export default class ViewGrid extends Vue {
  @Inject("PageStore")
  PageStore: PageStore;
  GridOptions: GridOptions = {
    frameworkComponents: {
      // 传递 行 操作组件 自动注册 Action 列
      Action,
      GridIcon
    },
    context: {
      PageStore: this.PageStore
    },
    rowGroupPanelShow: null,
    treeData: true,
    groupDefaultExpanded: -1,
    getDataPath: data => data.treePath,
    autoGroupColumnDef: {
      // headerName: "页面名称",
      headerValueGetter: params =>
        ({ "zh-CN": "页面名称", "en-US": "PageName" }[
          lodash.get(params, "context.locale")
        ]),
      cellRendererParams: { suppressCount: true }
    },
    columnDefs: [
      {
        headerName: "顺序",
        field: "DisplayOrder"
        // 自定义 多语言
        // headerValueGetter: (params) => ({ 'zh-CN': '姓名', 'en-US': "Name" }[lodash.get(params, 'context.locale')])
      },
      {
        headerName: "图标",
        field: "ICon",
        cellRenderer: "GridIcon"
      }
    ]
  };
  // get rowData() {
  //   return toJS(this.PageStore.RowData);
  // }
  // get columnDefs() {
  //   return toJS(this.PageStore.ColumnDefs);
  // }
  mounted() {}
}
</script>
<style scoped lang="less"></style>
