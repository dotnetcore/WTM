<template>
  <card class="dataprivilege">
    <wtm-search-box :searchEvent="searchEvent">
      <wtm-form-item label="ITCode">
        <el-input v-model="searchForm.ITCode" />
      </wtm-form-item>
      <wtm-form-item label="Url">
        <el-input v-model="searchForm.ActionUrl" />
      </wtm-form-item>
      <template slot="collapse-content">
        <wtm-form-item label="操作时间" :span="10">
          <el-date-picker v-model="searchForm.ActionTime" type="datetimerange" value-format="yyyy-MM-dd HH:mm:ss" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" />
        </wtm-form-item>
        <wtm-form-item label="IP">
          <el-input v-model="searchForm.IP" />
        </wtm-form-item>
        <wtm-form-item label="类型">
          <el-select v-model="searchForm.LogType" multiple :collapse-tags="false">
            <el-option v-for="(item, index) of logTypes" :key="index" :label="item.Text" :value="item.Value" />
          </el-select>
        </wtm-form-item>
      </template>
    </wtm-search-box>
    <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :eventFn="eventFn" />
    <wtm-table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
      <template #operate="rowData">
        <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
          详情
        </el-button>
        <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
          删除
        </el-button>
      </template>
    </wtm-table-box>
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </card>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/actionlog";
// 查询参数, table列 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER, logTypes } from "./config";

@Component({
    mixins: [searchMixin(SEARCH_DATA), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    // 动作
    assembly = ASSEMBLIES;

    tableHeader = TABLE_HEADER;
    logTypes = logTypes;

    // 查询接口 ★★★★★
    privateRequest(params) {
        if (params.LogType) {
            params.LogType = params.LogType.split(",");
        }
        if (params.ActionTime) {
            params.StartActionTime = params.ActionTime.split(",")[0];
            params.EndActionTime = params.ActionTime.split(",")[1];
        }
        return this.search(params);
    }
}
</script>
