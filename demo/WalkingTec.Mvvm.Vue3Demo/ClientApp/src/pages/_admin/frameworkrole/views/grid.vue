<template>
  <WtmGrid
    :PageController="PageController"
    :columnDefs="columnDefs"
    :gridOptions="gridOptions"
  />
</template>
<script lang="ts">
import { ColDef,ColGroupDef,GridOptions } from "ag-grid-community";
import { Inject,Options,Vue } from "vue-property-decorator";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
import RowAction from "./action.vue";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  columnDefs: (ColDef | ColGroupDef)[] = [
    {
      headerName: EnumLocaleLabel.RoleCode,
      field: "RoleCode",
    },
    {
      headerName: EnumLocaleLabel.RoleName,
      field: "RoleName",
    },
    {
      headerName: EnumLocaleLabel.RoleRemark,
      field: "RoleRemark",
    },
  ];
  get gridOptions(): GridOptions {
    return {
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController),
      },
    };
  }
  created() {}
}
</script>
<style lang="less">
</style>
