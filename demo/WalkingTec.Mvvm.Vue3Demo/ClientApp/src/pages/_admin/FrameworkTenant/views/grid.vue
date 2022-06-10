<template>
  <WtmGrid :PageController="PageController" :columnDefs="columnDefs" :gridOptions="gridOptions" />
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
  columnDefs: (ColDef | ColGroupDef)[] = [
    {
      headerName: EnumLocaleLabel.TCode,
      field: "TCode",
    },
    {
      headerName: EnumLocaleLabel.TName,
      field: "TName",
    },
    {
      headerName: EnumLocaleLabel.TDb,
      field: "TDb",
    },
    {
      headerName: EnumLocaleLabel.DbContext,
      field: "DbContext",
    },
     {
      headerName: EnumLocaleLabel.TDbType,
      field: "TDbType",
    },
     {
      headerName: EnumLocaleLabel.Enabled,
      field: "Enabled",
      cellRenderer: function (row) {
        return row.value == 'true' ? '启用' : '未启用'
      },
    },
    {
      headerName: EnumLocaleLabel.EnableSub,
      field: "EnableSub",
      cellRenderer: function (row) {
        return row.value == 'true' ? '允许' : '不允许'
      },
    },
    {
      headerName: EnumLocaleLabel.TDomain,
      field: "TDomain",
    }
  ];
  get gridOptions(): GridOptions {
    return {
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController)
      }
    };
  }
  created() { }
}
</script>
<style lang="less">
</style>

