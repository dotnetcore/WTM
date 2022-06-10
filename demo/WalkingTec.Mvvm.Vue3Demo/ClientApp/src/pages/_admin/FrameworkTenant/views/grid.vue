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
  get columnDefs(): (ColDef | ColGroupDef)[] {
      return [
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
        cellRenderer: this.$FrameworkComponents.switch
      },
      {
        headerName: EnumLocaleLabel.EnableSub,
        field: "EnableSub",
        cellRenderer: this.$FrameworkComponents.switch
      },
      {
        headerName: EnumLocaleLabel.TDomain,
        field: "TDomain",
      }
    ];
  }

  get gridOptions(): GridOptions {
    console.log(this.$FrameworkComponents)
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

