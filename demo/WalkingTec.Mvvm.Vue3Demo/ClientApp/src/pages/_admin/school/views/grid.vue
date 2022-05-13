<template>
  <WtmGrid :PageController="PageController" :columnDefs="columnDefs" :gridOptions="gridOptions" oprationWidth="400" />
</template>
<script lang="ts">
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
import RowAction from "./action.vue";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  get columnDefs(): (ColDef | ColGroupDef)[] {
    return [
      {
        headerName: EnumLocaleLabel.ID,
        field: "ID",
        maxWidth:100,
      },
      {
        headerName: EnumLocaleLabel.SchoolCode,
        field: "SchoolCode",
      },
      {
        headerName: EnumLocaleLabel.SchoolName,
        field: "SchoolName",
      },
      {
        headerName: EnumLocaleLabel.SchoolType,
        field: "SchoolType"
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
<style lang="less">
</style>
