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
import { Component, Prop, Vue } from "vue-property-decorator";
import PageStore from "../store";
import { GridOptions } from "ag-grid-community";
import { toJS } from "mobx";
import Action from "./action.vue";
@Component({
  components: {}
})
export default class ViewGrid extends Vue {
  @Prop() PageStore: PageStore;
  GridOptions: GridOptions = {
    frameworkComponents: {
      // 传递 行 操作组件 自动注册 Action 列
      Action
    },
    context: {
      PageStore: this.PageStore
    },
    columnDefs: [
      {
        headerName: "授权对象",
        field: "Name"
        // 自定义 多语言
        // headerValueGetter: (params) => ({ 'zh-CN': '姓名', 'en-US': "Name" }[lodash.get(params, 'context.locale')])
      },
      {
        headerName: "权限名称",
        field: "TableName"
      },
      {
        headerName: "权限",
        field: "RelateIDs"
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
<style scoped lang="less">
</style>