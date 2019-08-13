<template>
  <article>
    <fuzzy-search ref="fuzzySearch" fuzzy-field="fuzzy_mobile" :search-label-width="75" placeholder="手机号" @onSearch="onSearchWrapped" @onReset="onResetWrapped" />
    <el-card class="table-card">
      <el-table v-loading="loading" class="list-table" :data="tableList?tableList.list:[]" stripe border element-loading-text="拼命加载中">
        <el-table-column prop="header" width="180" align="center" label="头像">
          <template slot-scope="scope">
            <img v-if="!!scope.row.head_image" :src="scope.row.head_image" class="header-image">
          </template>
        </el-table-column>
        <el-table-column prop="name" width="200" align="center" label="姓名" />
        <el-table-column prop="nickname" align="center" label="昵称" />
        <el-table-column prop="mobile" width="220" align="center" label="手机号" />
        <el-table-column align="center" label="注册时间">
          <template slot-scope="scope">
            {{ scope.row.register_time | formatTime }}
          </template>
        </el-table-column>
      </el-table>
      <el-pagination :current-page="parseInt(pageDate.currentPage)" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="(tableList&&tableList.amount)?parseInt(tableList.amount):0" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
    </el-card>
  </article>
</template>

<script lang='ts'>
import { Component, Vue, Watch } from "vue-property-decorator";
import mixinFunc from "@/mixin/search";
import FuzzySearch from "@/components/common/fuzzy-search.vue";
import { Action, Getter } from "vuex-class";
import store from "@/store/manage/user-manage";
import { validate } from "@/util/decorators/validator/validate";
import { isNum } from "@/util/decorators/validator/rules";
@Component({
    mixins: [mixinFunc({})],
    store,
    components: {
        FuzzySearch
    }
})
export default class UserManage extends Vue {
    dataList: Object[] = [];
    loading: boolean = false;
    @Action("getTableData")
    getTableData;
    @Getter("tableList")
    tableList;
    privateRequest(params) {
        // params["app_id"] = 100189;
        params["sig"] = "{sig}";
        params["campaign_code_list"] = "{campaign_source_code}";
        return this.getTableData(params);
    }
    async created() {
        this.fetch();
        // console.log("tableList", this.tableList);
    }
    @Watch("tableList")
    watchData(vals) {
        console.log("tableList", this.tableList);
    }
    @validate()
    onSearchWrapped(@isNum() val: Object) {
        this.onSearch(val);
    }
    onResetWrapped() {
        this["searchForm"]["mobile"] = ""; //重置模糊查询
        this["onReset"]();
    }
}
</script>

<style lang='less'>
</style>
