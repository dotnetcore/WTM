<template>
  <ag-grid-vue
    style="height: 500px;"
    class="ag-theme-material"
    :gridOptions="GridOptions"
    :columnDefs="columnDefs"
    :rowData="rowData"
  ></ag-grid-vue>
</template>

<script lang="ts">
import { AgGridVue } from "ag-grid-vue/lib/AgGridVue";
import { LicenseManager } from "ag-grid-enterprise";
import { Component, Prop, Vue } from "vue-property-decorator";
import { GridOptions } from "ag-grid-community";
LicenseManager.setLicenseKey(
  "ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2"
);
@Component({
  components: {
    AgGridVue
  }
})
export default class HelloWorld extends Vue {
  columnDefs;
  rowData;
  GridOptions: GridOptions = {
    // columnDefs
    // groupUseEntireRow:true,
    sideBar: {
      toolPanels: [
        {
          id: "columns",
          labelDefault: "Columns",
          labelKey: "columns",
          iconKey: "columns",
          toolPanel: "agColumnsToolPanel",
          toolPanelParams: {
            // suppressRowGroups: true,
            suppressValues: true,
            suppressPivots: true,
            suppressPivotMode: true
            // suppressSideButtons: true,
            // suppressColumnFilter: true,
            // suppressColumnSelectAll: true,
            // suppressColumnExpandAll: true
          }
        },
        {
          id: "filters",
          labelDefault: "Filters",
          labelKey: "filters",
          iconKey: "filter",
          toolPanel: "agFiltersToolPanel"
        }
      ]
    },
    masterDetail: true,
    rowGroupPanelShow: "always",
    onGridReady: event => {
      event.api.sizeColumnsToFit();
    }
  };
  @Prop() private msg!: string;
  mounted() {
    console.log("TCL: HelloWorld -> mounted -> mounted");
  }
  destroyed() {
    console.log("TCL: HelloWorld -> destroyed -> destroyed");
  }
  beforeMount() {
    this.columnDefs = [
      { headerName: "Make", field: "make" },
      { headerName: "Model", field: "model" },
      { headerName: "Price", field: "price" }
    ];

    this.rowData = [
      { make: "Toyota", model: "Celica", price: 35000 },
      { make: "Ford", model: "Mondeo", price: 32000 },
      { make: "Porsche", model: "Boxter", price: 72000 }
    ];
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="less">
@import "~ag-grid-community/dist/styles/ag-grid.css";
@import "~ag-grid-community/dist/styles/ag-theme-balham.css";
@import "~ag-grid-community/dist/styles/ag-theme-material.css";
@import '~ant-design-vue/es/style/themes/default.less';
.ag-theme-material {
    .ag-icon-checkbox-checked {
        color: @primary-color !important;
    }
}
</style>
