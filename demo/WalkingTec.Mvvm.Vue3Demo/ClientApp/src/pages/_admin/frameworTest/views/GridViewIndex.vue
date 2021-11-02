
<template>
  <WtmGrid :PageController="PageController" :columnDefs="columnDefs" :gridOptions="gridOptions" />
</template>



<script lang="ts">
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
//import { EnumLocaleLabel } from "../locales";
import RowAction from "./ActionViewIndex.vue";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  get columnDefs(): (ColDef | ColGroupDef)[] {
    return [
      {
        headerName:"账号",
        field: "ITCode",
      },
      {
        headerName:"姓名",
        field: "Name",
      },
      {
        headerName:"性别",
        field: "Gender",
      },
      {
        headerName:"手机",
        field: "CellPhone",
      },
      {
        headerName:"角色",
        field: "Role",
      },
      {
        headerName:"用户组",
        field: "Group",
      },
      {
        headerName:"是否有效",
        field: "IsValid",
        cellRenderer: this.$FrameworkComponents.switch,
      },
      {
        headerName:"照片",
        field: "Photo",
        cellRenderer: this.$FrameworkComponents.image,
      },

    ]
  };
  get gridOptions(): GridOptions {
    return {
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController),
      },
    };
  }
  created() { }
}
</script>

