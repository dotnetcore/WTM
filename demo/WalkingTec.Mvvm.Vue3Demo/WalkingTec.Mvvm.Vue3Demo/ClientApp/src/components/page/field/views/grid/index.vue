<template>
  <!-- <template v-if="_readonly">
    <span v-text="value"></span>
  </template> -->
  <!-- <template v-else> -->
  <div>
    <a-button @click="onAddGrid">
      <template #icon>
        <FormOutlined />
      </template>
      <i18n-t :keypath="$locales.action_insert" />
    </a-button>
    <a-divider style="margin: 3px;" />
    <WtmAgGrid
      theme="alpine"
      :rowData="value"
      :columnDefs="columnDefs"
      :gridOptions="gridOptions"
    />
  </div>

  <!-- </template> -->
</template>
<script lang="ts">
import { Vue, Options, Prop, mixins, Inject } from "vue-property-decorator";
import {
  ColDef,
  ColGroupDef,
  GridOptions,
  GridReadyEvent
} from "ag-grid-community";
import { FieldBasics } from "../../script";
import Action from "./action.vue";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  // 表单类型
  @Inject({ default: "" }) readonly formType;
  get columnDefs(): (ColDef | ColGroupDef)[] {
    return this.lodash.concat<ColDef | ColGroupDef>(
      [{ field: "_rowKey", headerName: "No", width: 80, editable: false }],
      this.lodash.get(this._fieldProps, "columnDefs"),
      [
        {
          width: 80,
          headerName: "action_name",
          field: "RowAction",
          cellRenderer: "RowAction",
          cellClass: "w-row-action",
          pinned: window.innerWidth > 701 ? "right" : "",
          sortable: false,
          suppressMenu: true,
          editable: false,
          suppressColumnsToolPanel: true
        }
      ]
    );
  }
  get gridOptions(): GridOptions {
    return this.lodash.assign<GridOptions, GridOptions>(
      {
        frameworkComponents: {
          RowAction: Action
        },
        domLayout: "autoHeight",
        editType: "fullRow",
        // enableCellChangeFlash: true,
        stopEditingWhenGridLosesFocus: true,
        singleClickEdit: true,
        defaultColDef: {
          headerValueGetter: params => {
            try {
              return this.$t(this.lodash.get(params, "colDef.headerName"));
            } catch (error) {
              return "";
            }
          },
          filter: false,
          resizable: true,
          sortable: false,
          editable: true,
          minWidth: 80
        },
        onGridReady: (event: GridReadyEvent) => {
          // this.GridApi = event.api;
          // this.ColumnApi = event.columnApi;
          event.api.sizeColumnsToFit();
        }
      },
      this.lodash.get(this._fieldProps, "gridOptions")
    );
  }
  onAddGrid() {
    const rowData = this.lodash.isArray(this.value)
      ? this.lodash.clone(this.value)
      : [];
    rowData.push({ _rowKey: rowData.length + 1 });
    this.value = rowData;
  }
  async mounted() {
    // this.onRequest();
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
}
</script>
<style lang="less"></style>
