<template>
  <el-form ref="searchForm" class="form-class" :inline="true" label-width="75px">
    <el-row>
      <el-col :span="11">
        <el-form-item label="意向车型" prop="car_model">
          <el-select v-model="searchForm.type" clearable placeholder="请选择" value="" multiple>
            <el-option key="22" value="dd" label="ss" />
            <el-option key="33" value="eee" label="xx" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="11">
        <el-form-item label="交付时间范围" prop="deliver-time" label-width="120px">
          <el-date-picker v-model="searchForm.date" type="daterange" align="right" unlink-panels range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" :picker-options="pickerOptions" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row>
      <el-col :span="11">
        <el-form-item label="订单状态" prop="order-status">
          <el-select v-model="searchForm.status" clearable placeholder="请选择" value="" multiple>
            <el-option key="22" value="dd" label="ss" />
            <el-option key="33" value="eee" label="xx" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-button type="primary" class="search-btn" @click="onSearchForm">
        <i class="fa fa-search" />查询
      </el-button>
    </el-row>
  </el-form>
</template>
<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { pickerOptions } from "@/util/vue-util";
type searchFormType = {
    status: string;
    date: Date[];
    type: string;
};
@Component
export default class SearchForm extends Vue {
    pickerOptions: Object = pickerOptions;
    searchForm: searchFormType = { status: "", date: [], type: "" };
    selectedCarModels: Array<String> = [];
    @Watch("searchForm", { deep: true })
    listenSearchForm() {
        this.$emit("change", this.searchForm);
    }
    onSearchForm() {
        this.$emit("search");
    }
}
</script>
<style lang="less">
.search-btn {
    position: absolute;
    right: 0;
}
.form-class {
    position: relative;
}
</style>

