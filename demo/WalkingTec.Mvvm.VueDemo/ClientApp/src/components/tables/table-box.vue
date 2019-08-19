<template>
  <el-card class="table-card">
    <el-table v-loading="loading" class="list-table" :data="tableData" stripe border element-loading-text="拼命加载中" @selection-change="onSelectionChange">
      <el-table-column v-if="!isSelection" type="selection" width="55" />
      <el-table-column v-for="(item, index) of columnList" :key="index" :sortable="item.sortable" :prop="item.prop" :label="item.label" align="center" />
      <el-table-column v-if="isOperate" align="center" label="操作">
        <template slot-scope="scope">
          <slot name="operate" :rowData="{...scope}" />
        </template>
      </el-table-column>
    </el-table>
    <el-pagination :current-page="parseInt(pageDate.currentPage)" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pageDate.pageTotal" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
  </el-card>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class FuzzySearch extends Vue {
    @Prop(Boolean)
    loading;
    @Prop({ type: Boolean, default: false })
    isSelection;
    @Prop({ type: Boolean, default: false })
    isOperate;
    @Prop({ type: Object, default: {} })
    tbColumn;
    @Prop(Array)
    tableData;
    @Prop({ type: Object, default: {} })
    pageDate;

    created() {
        console.log("$slots", this);
    }

    get columnList() {
        return Object.keys(this.tbColumn).map(item => {
            const param = this.tbColumn[item];
            return {
                sortable: param["sortable"],
                prop: item,
                label: param["label"],
                width: param["width"] || ""
            };
        });
    }

    handleSizeChange() {
        this.$emit("handleSizeChange");
    }
    handleCurrentChange() {
        this.$emit("handleCurrentChange");
    }
    onSelectionChange(selectData) {
        console.log("selectData", selectData);
    }
}
</script>
<style lang="less" rel="stylesheet/less">
</style>
