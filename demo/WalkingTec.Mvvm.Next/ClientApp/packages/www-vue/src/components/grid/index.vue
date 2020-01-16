<template>
  <a-spin :spinning="PageStore.Loading" class="grid-spin">
    <a-icon slot="indicator" type="loading" style="font-size: 30px" spin />
    <grid :GridOptions="GridOptionsProps" :rowData="rowDataProps" :columnDefs="columnDefsProps" />
    <div style="height:8px"></div>
    <div class="grid-pagination">
      <pagination :PageStore="PageStore" :Pagination="Pagination" />
    </div>
  </a-spin>
</template>

<script lang="ts">
import { EntitiesPageStore } from "@leng/public/src";
import { Component, Prop, Vue } from "vue-property-decorator";
import pagination from "./pagination.vue";
import { Pagination } from "ant-design-vue";
import { toJS } from "mobx";
import lodash from "lodash";
import { GridOptions, SelectionChangedEvent } from "ag-grid-community";
const grid = () =>
  ({
    // 需要加载的组件 (应该是一个 `Promise` 对象)
    component: import("./grid.vue"),
    // 异步组件加载时使用的组件
    // loading: '<div style="height: 500px;"></div>',
    // 加载失败时使用的组件
    //   error: ErrorComponent,
    // 展示加载时组件的延时时间。默认值是 200 (毫秒)
    //   delay: 200,
    // 如果提供了超时时间且组件加载也超时了，
    // 则使用加载失败时使用的组件。默认值是：`Infinity`
    timeout: 3000
  } as any);
@Component({
  components: {
    grid,
    pagination
  }
})
export default class Grid extends Vue {
  @Prop() PageStore: EntitiesPageStore;
  @Prop() GridOptions;
  @Prop() Pagination: Pagination;
  @Prop({ default: () => [] }) rowData!: any;
  @Prop({ default: () => [] }) columnDefs!: any;
  get rowDataProps() {
    return toJS(this.PageStore.RowData);
  }
  get columnDefsProps() {
    return toJS(this.PageStore.ColumnDefs);
  }
  get GridOptionsProps(): GridOptions {
    return lodash.merge<GridOptions, GridOptions>(
      {
        /**
         * 选择的 行 数据 回调
         * @param event
         */
        onSelectionChanged: (event: SelectionChangedEvent) => {
          this.PageStore.onSelectionChanged(event.api.getSelectedRows());
        }
      },
      this.GridOptions
    );
  }
  mounted() {}
  destroyed() {}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
.grid-spin {
  min-height: 400px;
}
.grid-pagination {
  display: flex;
  justify-content: flex-end;
}
</style>
