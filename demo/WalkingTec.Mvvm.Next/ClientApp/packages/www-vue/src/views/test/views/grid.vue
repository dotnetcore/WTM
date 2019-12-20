<template>
  <Grid
    :PageStore="PageStore"
    :GridOptions="GridOptions"
    :rowData="rowData"
    :columnDefs="GridOptions.columnDefs"
  />
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Grid from "@/components/grid/index.vue";
import PageStore from "../store";
import { GridOptions } from "ag-grid-community";
import { toJS } from "mobx";
@Component({
  components: {
    Grid
  }
})
export default class ViewGrid extends Vue {
  @Prop() PageStore: PageStore;
  GridOptions: GridOptions = {
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
  get rowData() {
    return toJS(this.PageStore.RowData);
  }
  mounted() {
    console.log("TCL: ViewGrid -> PageStore", this);
  }
}
</script>
<style scoped lang="less">
</style>