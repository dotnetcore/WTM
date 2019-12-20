<template>
  <Grid
    :PageStore="PageStore"
    :GridOptions="GridOptions"
    :rowData="rowData"
    :columnDefs="columnDefs"
  />
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Grid from "@/components/grid/index.vue";
import PageStore from "../store";
import { GridOptions } from "ag-grid-community";
import { toJS } from "mobx";
@Component({
  components: {
    Grid
  }
})
export default class ViewGrid extends Vue {
  @Prop() PageStore: PageStore;
  GridOptions: GridOptions = {};
  get rowData() {
    return toJS(this.PageStore.RowData);
  }
  get columnDefs() {
    return toJS(this.PageStore.ColumnDefs);
  }
  mounted() {
    
  }
}
</script>
<style scoped lang="less">
</style>