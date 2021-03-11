<template>
  <Grid
    :columnDefs="getColumnDefs"
    :rowData="Pagination.dataSource"
    :gridOptions="options"
  />
  <a-divider />
  <Pagination />
</template>

<script lang="ts">
import { defineAsyncComponent } from "vue";
import { ControllerBasics } from "@/client";
import lodash from "lodash";
import { Options, Prop, Vue, Watch } from "vue-property-decorator";
import {
  GridOptions,
  SelectionChangedEvent,
  ColDef,
  GridReadyEvent,
  RowDataChangedEvent,
} from "ag-grid-community";
import framework from "./framework";
import defaultOptions, {
  getColumnDefsCheckbox,
  getColumnDefsAction,
} from "./defaultOptions";
import Pagination from "./pagination.vue";
import { Debounce } from "lodash-decorators";
@Options({
  components: {
    Pagination,
    Grid: defineAsyncComponent({
      loader: () => import("./grid.vue"),
      loadingComponent: framework.loadingOverlay,
    }),
  },
})
export default class extends Vue {
  @Prop({ required: true }) PageController: ControllerBasics;
  @Prop({ default: () => [] }) columnDefs;
  @Prop({ default: () => ({}) }) gridOptions: GridOptions;
  @Prop({ default: () => true }) checkboxSelection: boolean;

  get Pagination() {
    return this.PageController.Pagination;
  }
  get getColumnDefs() {
    return this.lodash.concat(
      getColumnDefsCheckbox(this.checkboxSelection, "balham"),
      this.columnDefs,
      getColumnDefsAction(this.gridOptions.frameworkComponents)
    );
  }
  get options() {
    const { frameworkComponents = {}, ...gridOptions } = this.gridOptions;
    const options = this.lodash.assign<GridOptions, GridOptions, GridOptions>(
      {
        frameworkComponents: this.lodash.assign(
          {},
          frameworkComponents,
          framework
        ),
        // 数据选择
        onSelectionChanged: (event) =>
          this.PageController.Pagination.onSelectionChanged(
            event.api.getSelectedRows()
          ),
        // 行数据的 id
        getRowNodeId: (data) => this.lodash.get(data, this.PageController.key),
        // 初始化完成
        onGridReady: this.onGridReady,
        // 数据更新
        onRowDataChanged: this.onRowDataChanged,
      },
      defaultOptions(this.$i18n as any),
      gridOptions
    );
    console.log("LENG ~ extends ~ getoptions ~ options", options);
    return options;
  }
  onGridReady = (event: GridReadyEvent) => {
    event.api.sizeColumnsToFit();
  };
  // @Debounce(100)
  onRowDataChanged = lodash.debounce((event: RowDataChangedEvent) => {
    event.columnApi.autoSizeColumn("RowAction");
  });
  created() {}
}
</script>
<style  lang="less">
</style>
