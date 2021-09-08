<template>
  <!-- <template v-if="_readonly">
    <span v-text="value"></span>
  </template>-->
  <!-- <template v-else> -->
  <div>
    <a-button @click="onAddGrid" v-if="!isReadonly">
      <template #icon>
        <FormOutlined />
      </template>
      <i18n-t :keypath="$locales.action_insert" />
    </a-button>
    <a-divider style="margin: 3px;" />
    <WtmAgGrid
      theme="alpine"
      :rowData="rowData"
      :columnDefs="columnDefs"
      :gridOptions="gridOptions"
    />
    <a-list size="small" bordered :data-source="dataError" v-show="dataError.length">
      <template #renderItem="{ item }">
        <a-list-item style="color: #ff4d4f;">{{ item.key }} | {{ item.value.help }}</a-list-item>
      </template>
    </a-list>
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
  // get GridKey() {
  //   return this.$Encryption.MD5(this.lodash.assign({}, { columnDefs: this.columnDefs }, this.gridOptions))
  // }
  get dataError() {
    const name = this.lodash.join(this._name, ".");
    const keys = this.lodash
      .keys(this.formValidate)
      .filter(item => this.lodash.includes(item, name));
    return this.lodash.map(keys, item => {
      return {
        key: item,
        value: this.lodash.get(this.formValidate, item)
      };
    });
  }
  get isReadonly() {
    return this._readonly || this.disabled;
  }
  get rowData() {
    return this.value || [];
  }
  get columnDefs(): (ColDef | ColGroupDef)[] {
    let columnDefs = this.lodash.concat<ColDef | ColGroupDef>(
      [{ field: "_rowKey", headerName: "No", width: 80, editable: false }],
      this.lodash.get(this._fieldProps, "columnDefs")
    );
    if (!this.isReadonly) {
      columnDefs = this.lodash.concat(columnDefs, [
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
      ]);
    }
    return columnDefs;
  }
  get defaultColDef(): ColDef {
    return {
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
      editable: !this.isReadonly,
      minWidth: 80
    };
  }
  get gridOptions(): GridOptions {
    return this.lodash.assign<GridOptions, GridOptions>(
      {
        frameworkComponents: {
          RowAction: Action
        },
        context: {
          onRemove: this.onRemove
        },
        domLayout: "autoHeight",
        editType: "fullRow",
        // enableCellChangeFlash: true,
        stopEditingWhenGridLosesFocus: true,
        // singleClickEdit: true,
        defaultColDef: this.defaultColDef,
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
  onRemove(params) {
    const { rowIndex } = params;
    const rowData = this.lodash.clone(this.value);
    this.lodash.remove(rowData, (item, index) =>
      this.lodash.eq(index, rowIndex)
    );
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
