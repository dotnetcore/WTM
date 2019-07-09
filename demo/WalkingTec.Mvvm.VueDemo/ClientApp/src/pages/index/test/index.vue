<template>
  <div class="main-box">
    <el-table :data="tableData" stripe style="width: 100%">
      <el-table-column prop="date" label="日期" width="180" />
      <el-table-column prop="name" label="姓名" width="180" />
      <el-table-column prop="address" label="地址" />
    </el-table>
    <el-pagination :current-page="pageDate.currentPage" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="400" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
  </div>
</template>
<script>
import baseMixin from "@/mixin/base";
import searchMixin from "@/mixin/search";
export default {
    mixins: [baseMixin, searchMixin()],
    data() {
        return {
            tableData: [
                {
                    date: "2016-05-02",
                    name: "王小虎",
                    address: "上海市普陀区金沙江路 1518 弄"
                },
                {
                    date: "2016-05-04",
                    name: "王小虎",
                    address: "上海市普陀区金沙江路 1517 弄"
                },
                {
                    date: "2016-05-01",
                    name: "王小虎",
                    address: "上海市普陀区金沙江路 1519 弄"
                },
                {
                    date: "2016-05-03",
                    name: "王小虎",
                    address: "上海市普陀区金沙江路 1516 弄"
                }
            ]
        };
    },
    computed: {},
    mounted() {
        console.log("test");
    },
    methods: {
        privateRequest() { }
    }
};
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
@import "~@/assets/css/mixin.less";

.main-box {
    background-color: #f2f3f5;
}
</style>
