<template>
  <el-card class="table-card">
    <el-table v-loading="loading" class="list-table" :data="tableData" stripe border element-loading-text="拼命加载中" @selection-change="onSelectionChange">
      <el-table-column v-if="isSelection" type="selection" align="center" width="55" />
      <!-- 判断是否需要插槽,自定义列内容 -->
      <template v-for="(item, index) of tbColumn">
        <el-table-column v-if="item.isSlot" :key="index" :label="item.label" :width="item.width" align="center">
          <template slot-scope="scope">
            <slot :name="item.key" :row="{...scope.row}" />
          </template>
        </el-table-column>
        <el-table-column v-else :key="index" :sortable="item.sortable" :prop="item.key" :label="item.label" :width="item.width" align="center" />
      </template>
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
    isSelection; // 是否多选
    @Prop({ type: Array, default: [] })
    tbColumn; // 列字段数据
    @Prop(Array)
    tableData; // 列表数据
    @Prop({ type: Object, default: {} })
    pageDate; // 分页数据

    created() {
        console.log("$slots", this);
    }

    // 切换页码
    handleSizeChange(size) {
        this.$emit("handleSizeChange", size);
    }
    // 翻页
    handleCurrentChange(currentpage) {
        this.$emit("handleCurrentChange", currentpage);
    }
    // 选中数据
    onSelectionChange(selectData) {
        this.$emit("onSelectionChange", selectData);
        // console.log("selectData", selectData);
    }
}
</script>
<style lang="less" rel="stylesheet/less">
</style>
