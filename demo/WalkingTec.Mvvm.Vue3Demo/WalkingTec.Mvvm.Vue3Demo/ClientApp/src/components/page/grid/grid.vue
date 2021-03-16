<template>
  <ag-grid-vue
    :key="GridKey"
    style="height: 100%"
    class="ag-grid-card"
    :class="themeClass"
    :columnDefs="columnDefs"
    :rowData="rowData"
    :gridOptions="gridOptions"
  ></ag-grid-vue>
</template>

<script lang="ts">
import { Vue, Options, Prop, Watch } from "vue-property-decorator";
// import { AgGridVue } from "ag-grid-vue3";
import { AgGridVue } from "./agGridVue/AgGridVue";
import { LicenseManager } from "ag-grid-enterprise";
import {
  GridApi,
  GridOptions,
  GridReadyEvent,
  RowDataChangedEvent,
} from "ag-grid-community";
import { Debounce } from "lodash-decorators";
LicenseManager.setLicenseKey(
  "ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2"
);
@Options({
  components: {
    AgGridVue,
  },
})
export default class AgGrid extends Vue {
  @Prop({ default: false }) loading;
  @Prop({ default: "material" }) theme;
  @Prop({ default: () => [] }) columnDefs;
  @Prop({}) rowData;
  @Prop({ default: () => ({}) }) gridOptions: GridOptions;
  get themeClass() {
    return `ag-theme-${this.theme}`;
  }
  get GridKey() {
    return this.$i18n.locale;
  }
  created() {}
}
</script>
<style lang="less">
@import "~ag-grid-community/dist/styles/ag-grid.css";
@import "~ag-grid-community/dist/styles/ag-theme-alpine.css";
// @import "~ag-grid-community/dist/styles/ag-theme-balham.css";
// @import "~ag-grid-community/dist/styles/ag-theme-material.css";
// 主题定制 https://www.ag-grid.com/vue-grid/themes-customising/
.ag-grid-card {
  // --ag-background-color: @primary-color
  &.ag-theme-balham,
  &.ag-theme-material,
  &.ag-theme-alpine {
    transition: height 0.2s;
    min-height: 350px;

    .ag-checkbox-input-wrapper.ag-checked::after {
      color: @primary-color;
    }

    .ag-rtl .ag-side-bar-left .ag-selected .ag-side-button-button,
    .ag-ltr .ag-side-bar-right .ag-selected .ag-side-button-button,
    .ag-tab-selected {
      border-color: @primary-color;
    }
  }
  .w-row-action {
    display: inline-flex;
  }
}
</style>
