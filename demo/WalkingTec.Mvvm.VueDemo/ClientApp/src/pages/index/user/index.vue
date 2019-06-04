<template>
    <div class="main-box">
        <el-row class="title-box">
            <el-col :span="24">
                <div>
                    <i class="el-icon-arrow-down"></i>
                    <span>搜索条件</span>
                </div>
            </el-col>
        </el-row>
        <el-form :model="formData" label-width="80px" size="small">
            <el-row type="flex" justify="space-around" class="serach-content">
                <el-col :span="6">
                    <el-form-item label="活动名称">
                        <el-input v-model="formData.name"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="6">
                    <el-form-item label="活动名称">
                        <el-input v-model="formData.name"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="6">
                    <el-form-item label="活动名称">
                        <el-input v-model="formData.name"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
        </el-form>
        <el-row class="list-box" type="flex" justify="space-between">
            <el-col :span="5">
                <div>
                    <i class="el-icon-arrow-down"></i>
                    <span>数据列表</span>
                </div>
            </el-col>
            <el-col :span="15" class="list-box-buts">
                <el-button type="primary" size="mini" icon="el-icon-plus" @click="onAdd">新建</el-button>
                <el-button type="primary" size="mini" icon="el-icon-edit">修改</el-button>
                <el-button type="primary" size="mini" icon="el-icon-finished">批量操作</el-button>
            </el-col>
        </el-row>
        <el-table :data="tableData" stripe style="width: 100%" size="small">
            <el-table-column prop="date" label="日期" width="180">
            </el-table-column>
            <el-table-column prop="name" label="姓名" width="180">
            </el-table-column>
            <el-table-column prop="address" label="地址">
            </el-table-column>
            <el-table-column fixed="right" label="操作" width="100">
                <template slot-scope="scope">
                    <el-button @click="handleClick(scope.row)" type="text" size="small">查看</el-button>
                    <el-button type="text" size="small">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pageDate.currentPage" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="400">
        </el-pagination>

        <add ref="addComp"></add>
    </div>
</template>
<script>
import baseMixin from "@/mixin/base.js";
import searchMixin from "@/mixin/search.js";
import add from "./add";

export default {
    mixins: [baseMixin, searchMixin()],
    data() {
        return {
            formData: {
                name: ""
            },
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
    mounted() {
        console.log("test");
    },
    methods: {
        privateRequest() {},
        handleClick() {},
        onAdd() {
            this.$refs.addComp.onOpen();
        }
    },
    computed: {},
    components: {
        add: add
    }
};
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
@import "~@/assets/css/mixin.less";

.main-box {
    background-color: #f2f3f5;

    .list-box,
    .title-box {
        padding: 15px;
        font-size: 14px;
        .list-box-buts {
            text-align: right;
        }
    }

    .serach-content {
        padding: 15px;
        background-color: #fff;
        .el-form-item--mini.el-form-item,
        .el-form-item--small.el-form-item {
            margin-bottom: 0;
        }
    }
}
</style>
