<template>
  <ag-grid-vue
    style="height: 100%"
    class="ag-theme-balham ag-grid-card"
    :columnDefs="columnDefs"
    :rowData="rowData"
  ></ag-grid-vue>
</template>

<script lang="ts">
import "ag-grid-community/dist/styles/ag-grid.css";
import "ag-grid-community/dist/styles/ag-theme-alpine.css";
import "ag-grid-community/dist/styles/ag-theme-balham.css";
import "ag-grid-community/dist/styles/ag-theme-material.css";
import {} from "lodash";
import { AgGridVue } from "ag-grid-vue3";
import { LicenseManager } from "ag-grid-enterprise";
import {
  GridOptions,
  GridReadyEvent,
  ColDef,
  ColGroupDef,
} from "ag-grid-community";
import { Subscription, fromEvent } from "rxjs";
import { Debounce } from "lodash-decorators";
import { cloneDeep, eq, delay, merge, get } from "lodash-es";
import frameworkComponents from "./frameworkComponents";
import { filter, debounceTime } from "rxjs/operators";
import { Vue, Options, Prop } from "vue-property-decorator";
LicenseManager.setLicenseKey(
  "ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2"
);
const lodash = { cloneDeep, eq, delay, merge, get };
@Options({
  components: {
    AgGridVue,
  },
})
export default class AgGrid extends Vue {
  columnDefs = [{ field: "make" }, { field: "model" }, { field: "price" }];
  rowData = [
    { make: "Toyota", model: "Celica", price: 35000 },
    { make: "Ford", model: "Mondeo", price: 32000 },
    { make: "Porsche", model: "Boxter", price: 72000 },
  ];
  created() {}
}
</script>
<style lang="less">
</style>
