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
        headerName: EnumLocaleLabel.ITCode,
        field: "ITCode",
      },
      {
        headerName: EnumLocaleLabel.Name,
        field: "Name",
      },
      {
        headerName: EnumLocaleLabel.Sex,
        field: "Gender",
      },
      {
        headerName: EnumLocaleLabel.Photo,
        field: "PhotoId",
        cellRenderer: this.$FrameworkComponents.image,
      },
      {
        headerName: EnumLocaleLabel.IsValid,
        field: "IsValid",
        cellRenderer: this.$FrameworkComponents.switch,
      },
      {
        headerName: EnumLocaleLabel.RoleName,
        field: "RoleName_view",
      },
      {
        headerName: EnumLocaleLabel.GroupName,
        field: "GroupName_view",
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
