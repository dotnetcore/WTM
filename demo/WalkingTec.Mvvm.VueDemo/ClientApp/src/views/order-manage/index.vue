<template>
  <article>
    <fuzzy-search ref="fuzzySearch" fuzzy-field="keyword" :search-label-width="75" placeholder="手机号" @onReset="onResetWrapped" @onSearch="onSearchWrapped">
      <span slot="operation">
        <!-- <el-button type="primary" size="medium">导出当前结果</el-button> -->
        <export-excel :params="exportParams" btn-name="导出当前结果" batch-type="EXPORT_REPLACEMENT" />
      </span>
      <div slot="collapse-content">
        <search-form @change="listenSearchForm" @search="onSearchForm" />
      </div>
    </fuzzy-search>
    <el-card class="table-card">
      <el-table v-loading="loading" class="list-table" :data="tableData.list" stripe border element-loading-text="拼命加载中">
        <el-table-column prop="order_no" width="220" align="center" label="订单编号" />
        <el-table-column width="160" align="center" label="头像">
          <template slot-scope="scope">
            <img v-if="!!scope.row.head_image" :src="scope.row.head_image" class="header-image">
          </template>
        </el-table-column>
        <el-table-column prop="person_mobile" width="220" align="center" label="手机号" />
        <el-table-column prop="car_model_series" align="center" label="意向车型" />
        <el-table-column prop="status_name" align="center" label="订单状态" />
        <el-table-column align="center" label="交付时间">
          <template slot-scope="scope">
            {{ scope.row.actual_delivery_time }}
          </template>
        </el-table-column>
        <el-table-column align="center" label="操作">
          <template slot-scope="scope">
            <el-button type="text" size="small" class="view-btn" @click="toDetail(scope.row)">
              查看详情
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination :current-page="parseInt(pageDate.currentPage)" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="(tableData&&tableData.amount)?parseInt(tableData.amount):0" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
    </el-card>
  </article>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import mixinFunc from "@/mixin/search";
import FuzzySearch from "@/components/common/fuzzy-search.vue";
import SearchForm from "./search-form.vue";
import { Action, Getter } from "vuex-class";
import store from "@/store/manage/order-manage";
import { validate } from "@/util/decorators/validator/validate";
import { isNum } from "@/util/decorators/validator/rules";
import ExportExcel from "@/components/common/export/export-excel.vue";
const mixin = mixinFunc({
    status: "",
    date: [],
    type: ""
});
@Component({
    mixins: [mixin],
    store,
    components: {
        FuzzySearch,
        SearchForm,
        ExportExcel
    }
})
export default class OrderManage extends Vue {
    loading: boolean = false;
    created() {
        this.fetch();
        // this.getOrderStatus();
    }
    onResetWrap() {}
    onSearchInventory() {}
    toDetail() {
        // this.$router.push()
    }
    @Action("getTableData")
    getTableData;
    @Action("getOrderStatus")
    getOrderStatus;
    @Getter("tableData")
    tableData;
    @Getter("orderStatus")
    orderStatus;
    formatSearchParams(params) {
        if (params.date.length && params.date[0]) {
            params.begin_time = params.date[0]
                ? Number(new Date(params.date[0])) / 1000
                : "";
        }
        if (params.date.length && params.date[1]) {
            params.end_time = params.date[1]
                ? Number(new Date(params.date[1])) / 1000
                : "";
        }
        delete params.date;
    }
    privateRequest(params) {
        this.formatSearchParams(params);
        params["user_campaign_source"] = "{campaign_source_code}";
        return this.getTableData(params);
    }
    listenSearchForm(val: Object) {
        this["searchForm"] = val;
        console.log("this.searchForm", this["searchForm"]);
    }
    @validate()
    onSearchWrapped(@isNum() val: Object) {
        this.onSearch(val);
    }
    onResetWrapped() {
        this["searchForm"]["mobile"] = ""; //重置模糊查询
        this["onReset"]();
    }
    get exportParams() {
        const params = { ...this["searchForm"] };
        for (const item in params) {
            if (!params[item]) {
                delete params[item];
            }
        }
        return params;
    }
}
</script>

<style lang='less'>
</style>
