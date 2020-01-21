<template>
  <ag-grid-vue
    :style="{
      height:height+'px',
    }"
    class="ag-theme-material ag-grid-card"
    :gridOptions="GridOptionsProps"
    :rowData="rowData"
    :columnDefs="columnDefsProp"
  ></ag-grid-vue>
</template>

<script lang="ts">
import lodash from "lodash";
import { AgGridVue } from "ag-grid-vue/lib/AgGridVue";
import { LicenseManager } from "ag-grid-enterprise";
import { Component, Prop, Vue } from "vue-property-decorator";
import {
  GridOptions,
  GridReadyEvent,
  ColDef,
  ColGroupDef
} from "ag-grid-community";
import { Subscription, fromEvent } from "rxjs";
import { Debounce } from "lodash-decorators";
import localeText from "./localeText";
import frameworkComponents from "./frameworkComponents";
LicenseManager.setLicenseKey(
  "ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2"
);
@Component({
  components: {
    AgGridVue
  }
})
export default class AgGrid extends Vue {
  @Prop() GridOptions: GridOptions;
  @Prop({ default: [] }) rowData!: any;
  @Prop({ default: [] }) columnDefs!: any;
  get columnDefsProp(): (ColDef | ColGroupDef)[] {
    return [
      {
        // pivotIndex: 0,
        rowDrag: false,
        dndSource: false,
        lockPosition: true,
        // dndSourceOnRowDrag: false,
        suppressMenu: true,
        suppressSizeToFit: true,
        suppressMovable: true,
        suppressNavigable: true,
        suppressCellFlash: true,
        // rowGroup: false,
        enableRowGroup: false,
        enablePivot: false,
        enableValue: false,
        suppressResize: false,
        editable: false,
        suppressColumnsToolPanel: true,
        filter: false,
        resizable: false,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        width: 70,
        maxWidth: 70,
        minWidth: 70,
        pinned: "left"
        // ...checkboxSelectionProps
      },
      ...this.columnDefs,
      // 存在 Action 添加
      lodash.has(this.GridOptionsProps, "frameworkComponents.Action") && {
        headerName: lodash.eq(this.$root.$i18n.locale, "zh-CN")
          ? "操作"
          : "Action",
        rowDrag: false,
        dndSource: false,
        lockPosition: true,
        // dndSourceOnRowDrag: false,
        suppressMenu: true,
        suppressSizeToFit: true,
        suppressMovable: true,
        suppressNavigable: true,
        suppressCellFlash: true,
        // rowGroup: false,
        enableRowGroup: false,
        enablePivot: false,
        enableValue: false,
        suppressResize: false,
        editable: false,
        suppressColumnsToolPanel: true,
        filter: false,
        field: "Action",
        cellRenderer: "Action",
        pinned: "right"
        // minWidth: 50,
        // ...rowActionProps
      }
    ].filter(Boolean);
  }
  // 事件对象
  ResizeEvent: Subscription;
  GridReadyEvent: GridReadyEvent;
  GridOptionsProps: GridOptions = lodash.merge<GridOptions, GridOptions>(
    {
      frameworkComponents,
      // suppressMenuHide:true,
      // columnDefs
      // groupUseEntireRow:true,
      localeText: lodash.eq(this.$root.$i18n.locale, "zh-CN")
        ? localeText
        : undefined,
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
      suppressNoRowsOverlay: true,
      rowGroupPanelShow: "always",
      rowSelection: "multiple",
      defaultColDef: {
        minWidth: 150,
        enableRowGroup: true,
        resizable: true
      },
      onGridReady: event => {
        this.GridReadyEvent = event;
      },
      onColumnVisible: () => {
        this.onCalculation();
      }
    },
    this.GridOptions
  );
  GridEvents = {
    sizeColumnsToFit: () => {
      this.GridReadyEvent.columnApi.autoSizeColumn("Action");
      this.GridReadyEvent.api.sizeColumnsToFit();
    }
  };
  height = 500;
  @Debounce(200)
  onCalculation() {
    this.GridEvents.sizeColumnsToFit();
  }
  @Debounce(200)
  onSetHeight() {
    try {
      const offsetTop = lodash.get(this, "$parent.$el.offsetTop", 0),
        innerHeight = window.innerHeight,
        height = innerHeight - offsetTop - 55;
      this.height = height < 400 ? 400 : height;
    } catch (error) {
      console.error("TCL: AgGrid -> onSetHeight", error);
      this.height = 400;
    }
  }
  mounted() {
    this.onSetHeight();
    this.onCalculation();

    this.ResizeEvent = fromEvent(window, "resize").subscribe(e => {
      this.onCalculation();
      this.onSetHeight();
    });
  }
  destroyed() {
    this.ResizeEvent && this.ResizeEvent.unsubscribe();
  }
  beforeMount() {}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="less">
.ag-grid-card {
  transition: all 0.2s;
}
</style>
