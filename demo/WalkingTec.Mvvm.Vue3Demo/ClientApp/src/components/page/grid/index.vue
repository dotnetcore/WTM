<template>
  <div class="w-grid-content" :style="style" ref="gridContent">
    <Grid
      :theme="theme"
      :columnDefs="getColumnDefs"
      :rowData="Pagination.dataSource"
      :gridOptions="options"
    />
  </div>
  <a-divider />
  <Pagination :PageController="PageController" />
</template>

<script lang="ts">
import { ControllerBasics } from "@/client";
import { ColumnApi, GridApi, GridOptions, GridReadyEvent, RowDataChangedEvent } from "ag-grid-community";
import lodash from "lodash";
import { fromEvent, Subscription } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { defineAsyncComponent } from "vue";
import { Options, Prop, Ref, Vue, Watch } from "vue-property-decorator";
import defaultOptions, { getColumnDefsAction, getColumnDefsCheckbox } from "./defaultOptions";
import framework from "./framework";
import Loading from "./loading.vue";
import Pagination from "./pagination.vue";
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
export default class Grid extends Vue {
  static Cache = new Map<string, any>()
  @Prop({ required: true }) readonly PageController: ControllerBasics;
  @Prop({ default: () => [] }) readonly columnDefs;
  @Prop({ default: () => ({}) }) readonly gridOptions: GridOptions;
  @Prop({ default: () => true }) readonly checkboxSelection: boolean;
  @Ref("gridContent") readonly gridContent: HTMLDivElement;
  theme: "balham" | "alpine" | "material" = "alpine";
  style = { height: "500px" };
  GridApi: GridApi = null;
  ColumnApi: ColumnApi = null;
  ResizeEvent: Subscription;
  LanguagesEvent: Subscription;
  isAutoSizeColumn = true;
  pageKey = '';
  get Pagination() {
    return this.PageController.Pagination;
  }
  get getColumnDefs() {
    return this.lodash.concat(
      getColumnDefsCheckbox(this.checkboxSelection, this.theme),
      this.columnDefs,
      getColumnDefsAction(this.gridOptions.frameworkComponents)
    );
  }

  get options(): GridOptions {
    const { frameworkComponents = {}, ...gridOptions } = this.gridOptions;
    const options: GridOptions = this.lodash.assign(
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
    console.log("LENG ~ extends ~ getoptions ~ options", options);
    return options;
  }
  get GridEvents(): GridOptions {
    return {
      onSortChanged: (event) => {
        const SortModel = lodash.head(event.api.getSortModel());
        // console.log("LENG ~ Grid ~ getGridEvents ~ event", SortModel)
        const sory = SortModel && SortModel.sort ? { Direction: lodash.capitalize(SortModel.sort), Property: SortModel.colId } : {}
        this.Pagination.onCurrentChange({ current: 1, sory })
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
        this.GridApi = event.api;
        this.ColumnApi = event.columnApi;
        event.api.sizeColumnsToFit();
        if (this.Pagination.loading) {
          this.GridApi.showLoadingOverlay();
        }
        lodash.invoke(this.gridOptions, "onGridReady", event);
        this.onReckon()
      },
      // onBodyScroll: (event) => {
      //   // console.log("LENG ~ extends ~ event", event.api);
      // },
      // 数据更新
      onRowDataChanged: lodash.debounce((event: RowDataChangedEvent) => {
        if (this.isAutoSizeColumn && this.Pagination.dataSource.length > 0) {
          // event.columnApi.autoSizeAllColumns(true)
          event.api.sizeColumnsToFit();
          // event.columnApi.autoSizeColumn("RowAction");
          this.isAutoSizeColumn = false;
        }
        lodash.invoke(this.gridOptions, "onRowDataChanged", event);
      }, 300),
    }
  };
  /**
   * 计算 表格高度
   */
  onReckon() {
    let height = 500;
    height = window.innerHeight - this.gridContent.offsetTop - 125;
    this.style.height = height + "px";
    if (lodash.eq(this.$WtmConfig.userAgent.platform.type, "mobile")) {
      this.style.height = "70vh";
    }
    Grid.Cache.set(this.pageKey, this.style.height)
  }
  @Watch("Pagination.loading")
  onLoading(val, old) {
    if (this.GridApi) {
      if (val) {
        this.GridApi.showLoadingOverlay();
      } else {
        this.GridApi.hideOverlay();
      }
    }
  }
  @Watch("$route.path")
  onRoute(val, old) {
    if (this.lodash.eq(val, this.pageKey)) {
      // this.autoSizeColumn()
    }
  }
  // autoSizeColumn = lodash.debounce(() => {
  //   this.onReckon()
  //   this.GridApi?.sizeColumnsToFit();
  //   this.ColumnApi?.autoSizeColumn("RowAction");
  // }, 300)
  created() {
    this.pageKey = this.$route.path
    if (Grid.Cache.has(this.pageKey)) {
      this.style.height = Grid.Cache.get(this.pageKey);
    }
  }
  mounted() {
    this.$nextTick(() => this.onReckon())
    this.ResizeEvent = fromEvent(window, "resize")
      .pipe(debounceTime(200))
      .subscribe(this.onReckon);
    // this.LanguagesEvent = fromEvent(window, "languages")
    //   .pipe(debounceTime(200))
    //   .subscribe(obx => {
    //     this.ColumnApi?.autoSizeColumn("RowAction");
    //   })
  }
  unmounted() {
    this.ResizeEvent && this.ResizeEvent.unsubscribe();
    this.ResizeEvent = undefined;
  }
}
</script>
<style  lang="less">
.w-grid-content {
  height: 500px;
  transition: all 0.2s;
}
</style>
