<template>
  <WtmGrid
    :PageController="PageController"
    :columnDefs="columnDefs"
    :gridOptions="gridOptions"
  />
</template>
<script lang="ts">
import { Vue, Options, Inject } from "vue-property-decorator";
import { defineComponent } from "vue";
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
import RowAction from "./action.vue";
@Options({ components: {} })
export default class extends Vue {
  @Inject() PageController: PageController;
  onText() {
    this.PageController.Pagination.onSet([{ ITCode: 123, Name: "测试" }]);
  }
  columnDefs: (ColDef | ColGroupDef)[] = [
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
      field: "Sex",
    },
    {
      headerName: EnumLocaleLabel.Photo,
      field: "Photo",
    },
    {
      headerName: EnumLocaleLabel.IsValid,
      field: "IsValid",
    },
    {
      headerName: EnumLocaleLabel.RoleName,
      field: "RoleName",
    },
    {
      headerName: EnumLocaleLabel.GroupName,
      field: "GroupName",
    },
  ];
  get gridOptions(): GridOptions {
    return {
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController),
      },
    };
  }
  created() {
    this.onText();
    console.log("LENG ~ extends ~ created ~ this", this);
  }
}
</script>
<style lang="less">
</style>
