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
        headerName: "类型",
        field: "LogType"
      },
      {
        headerName: "模块",
        field: "ModuleName"
      },
      {
        headerName: "动作",
        field: "ActionName"
      },
      {
        headerName: "ITCode",
        field: "ITCode"
      },
      {
        headerName: "Url",
        field: "ActionUrl"
      },
      {
        headerName: "操作时间",
        field: "ActionTime"
      },
      {
        headerName: "时长",
        field: "Duration"
      },
      {
        headerName: "IP",
        field: "IP"
      },
      {
        headerName: "备注",
        field: "Remark",
        enableRowGroup: false
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