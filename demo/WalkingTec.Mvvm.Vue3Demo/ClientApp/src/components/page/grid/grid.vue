<template>
  <ag-grid-vue
    :key="GridKey"
    style="height: 100%"
    class="ag-grid-card"
    :class="themeClass"
    :columnDefs="option.columnDefs"
    :rowData="rowData"
    :gridOptions="option.gridOptions"
  ></ag-grid-vue>
</template>

<script lang="ts">
import { GridOptions } from "ag-grid-community";
import { LicenseManager } from "ag-grid-enterprise";
import { Options, Prop, Vue } from "vue-property-decorator";
// import { AgGridVue } from "ag-grid-vue3";
import { AgGridVue } from "./agGridVue/AgGridVue";
// 'CompanyName=CROPLAND,LicensedGroup=Cropland,LicenseType=MultipleApplications,LicensedConcurrentDeveloperCount=1,LicensedProductionInstancesCount=1,AssetReference=AG-016521,ExpiryDate=15_June_2022_[v2]_MTY1NTI0NzYwMDAwMA==a2408a3e80d1e62fc6a847821ffef8e4';
LicenseManager.setLicenseKey(
  "ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2"
);
@Options({
  components: {
    AgGridVue
  }
})
export default class AgGrid extends Vue {
  @Prop({ default: false }) loading;
  @Prop({ default: "material" }) theme;
  @Prop({ default: () => [] }) columnDefs;
  @Prop({}) rowData;
  @Prop({ default: () => ({}) }) gridOptions: GridOptions;
  option = {
    columnDefs: [],
    gridOptions: {}
  };
  get themeClass() {
    return `ag-theme-${this.theme}`;
  }
  get GridKey() {
    return this.$Encryption.MD5(
      this.lodash.assign(
        { __locale: this.$i18n.locale, columnDefs: this.columnDefs },
        this.gridOptions
      )
    ); //this.$i18n.locale;
  }
  created() {
    this.option.columnDefs = this.lodash.cloneDeep(this.columnDefs);
    this.option.gridOptions = this.lodash.cloneDeep(this.gridOptions);
  }
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
    // min-height: 350px;

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
