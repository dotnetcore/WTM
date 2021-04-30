<template>
  <WtmGrid :PageController="PageController" :columnDefs="columnDefs" :gridOptions="gridOptions" />
</template>
<script lang="ts">
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import lodash from "lodash";
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
import RowAction from "./action.vue";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  columnDefs: (ColDef | ColGroupDef)[] = [
    {
      headerName: EnumLocaleLabel.LogType,
      field: "LogType",
    },

    {
      headerName: EnumLocaleLabel.ModuleName,
      field: "ModuleName",
    },

    {
      headerName: EnumLocaleLabel.ActionName,
      field: "ActionName",
    },

    {
      headerName: EnumLocaleLabel.ITCode,
      field: "ITCode",
    },

    {
      headerName: EnumLocaleLabel.ActionUrl,
      field: "ActionUrl",
    },
    {
      headerName: EnumLocaleLabel.ActionTime,
      field: "ActionTime",
    },
    {
      headerName: EnumLocaleLabel.Duration,
      field: "Duration",
      cellStyle: ({ data }) => {
        return { color: lodash.get(data, 'Duration__forecolor') }
      }
    },
    {
      headerName: EnumLocaleLabel.IP,
      field: "IP",
    },
    {
      headerName: EnumLocaleLabel.Remark,
      field: "Remark",
    },

  ];
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
