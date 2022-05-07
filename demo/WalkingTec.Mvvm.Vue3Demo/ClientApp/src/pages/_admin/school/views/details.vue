<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="SchoolCode" />
      <WtmField entityKey="SchoolName" />
    </a-space>
    <a-space>
      <WtmField entityKey="SchoolType" />
      <WtmField entityKey="Remark" />
    </a-space>
    <a-space>
      <WtmField entityKey="Photos"  /> 
    </a-space>
    <WtmField entityKey="Majors" :fieldProps="{ columnDefs, gridOptions }" debug />
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    Entity: {
      ID: "",
      Majors: []
    }
  };
  // 编辑表格配置 https://www.ag-grid.com/vue-grid/cell-editing/
  get columnDefs(): (ColDef | ColGroupDef)[] {
    return [
      { field: "MajorCode", headerName: "MajorCode" },
      { field: "MajorName", headerName: "MajorName" },
      { field: "MajorType", headerName: "MajorType" },
      // {
      //   field: "Sex",
      //   headerName: "Sex",
      //   cellEditor: "agSelectCellEditor",
      //   cellEditorParams: {
      //     values: ["男", "女"]
      //   }
      // }
    ];
  }

  get gridOptions(): GridOptions {
    return {};
  }
  // /**
  //  * 传递给 details 组件的 提交函数 返回一个 Promise
  //  * @param values
  //  * @returns
  //  */
  // async onFinish(values) {
  //   console.log("LENG ~ extends ~ onFinish ~ values", values);
  // }
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
