<template>
  <div class="w-grid-content" :style="style" ref="gridContent">
    <Grid
      :columnDefs="getColumnDefs"
      :rowData="Pagination.dataSource"
      :gridOptions="options"
    />
  </div>
  <a-divider />
  <Pagination :PageController="PageController" />
</template>

<script lang="ts">
import { defineAsyncComponent } from "vue";
import { ControllerBasics } from "@/client";
import lodash from "lodash";
import { Options, Prop, Vue, Ref } from "vue-property-decorator";
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
import Loading from "./loading.vue";
import { Debounce } from "lodash-decorators";
@Options({
  components: {
    Pagination,
    // 拆分 包 异步加载
    Grid: defineAsyncComponent({
      loader: () => import("./grid.vue"),
      loadingComponent: Loading,
      delay: 0,
    }),
  },
})
export default class extends Vue {
  @Prop({ required: true }) PageController: ControllerBasics;
  @Prop({ default: () => [] }) columnDefs;
  @Prop({ default: () => ({}) }) gridOptions: GridOptions;
  @Prop({ default: () => true }) checkboxSelection: boolean;
  @Ref("gridContent") gridContent: HTMLDivElement;
  style = { height: "500px" };
  get Pagination() {
    return this.PageController.Pagination;
  }
  get getColumnDefs() {
    return this.lodash.concat(
      getColumnDefsCheckbox(this.checkboxSelection, "material"),
      this.columnDefs,
      getColumnDefsAction(this.gridOptions.frameworkComponents)
    );
  }

  get options() {
    const { frameworkComponents = {}, ...gridOptions } = this.gridOptions;
    const options = this.lodash.assign(
      {
        frameworkComponents: this.lodash.assign(
          {},
          frameworkComponents,
          framework
        ),
        // 行数据的 id
        getRowNodeId: (data) => lodash.get(data, this.PageController.key),
      },
      defaultOptions(this.$i18n as any),
      gridOptions,
      this.GridEvents
    );
    // console.log("LENG ~ extends ~ getoptions ~ options", options);
    return options;
  }
  GridEvents: GridOptions = {
    onSortChanged: (event) => {
      console.log(
        "LENG ~ extends ~ getoptions ~ event",
        event.api.getSortModel()
      );
      lodash.invoke(this.gridOptions, "onSortChanged", event);
    },
    // 数据选择
    onSelectionChanged: (event) => {
      this.PageController.Pagination.onSelectionChanged(
        event.api.getSelectedRows()
      );
      lodash.invoke(this.gridOptions, "onSelectionChanged", event);
    },
    // 初始化完成
    onGridReady: (event: GridReadyEvent) => {
      event.api.sizeColumnsToFit();
      lodash.invoke(this.gridOptions, "onGridReady", event);
    },
    // 数据更新
    onRowDataChanged: lodash.debounce((event: RowDataChangedEvent) => {
      event.columnApi.autoSizeColumn("RowAction");
      lodash.invoke(this.gridOptions, "onRowDataChanged", event);
    }, 300),
  };
  onReckon() {
    // console.dir(this.gridContent);
    let height = 500;
    height = window.innerHeight - this.gridContent.offsetTop - 120;
    this.style.height = height + "px";
  }
  created() {}
  mounted() {
    this.onReckon();
  }
}
</script>
<style  lang="less">
.w-grid-content {
  height: 500px;
  transition: all 0.2s;
}
</style>
