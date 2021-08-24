<template>
  <WtmGrid
    :PageController="PageController"
    :columnDefs="columnDefs"
    :gridOptions="gridOptions"
  />
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
        headerName: EnumLocaleLabel.DisplayOrder,
        field: "DisplayOrder"
      },
      {
        headerName: EnumLocaleLabel.Icon,
        field: "Icon",
        cellRenderer: this.$FrameworkComponents.icons
      },
      {
        headerName: EnumLocaleLabel.IsPublic,
        field: "IsPublic",
        cellRenderer: this.$FrameworkComponents.switch
      },
      {
        headerName: EnumLocaleLabel.ShowOnMenu,
        field: "ShowOnMenu",
        cellRenderer: this.$FrameworkComponents.switch
      }
    ];
  }
  get gridOptions(): GridOptions {
    return {
      treeData: true,
      groupDefaultExpanded: -1,
      getDataPath: data => data.treePath,
      frameworkComponents: {
        RowAction: this.__wtmToRowAction(RowAction, this.PageController)
      }
    };
  }
  created() {}
}
</script>
<style lang="less"></style>
