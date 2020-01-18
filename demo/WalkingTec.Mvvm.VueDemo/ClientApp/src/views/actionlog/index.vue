<template>
  <div class="dataprivilege">
    <card>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" :inline="true" label-width="75px">
          <el-form-item label="ITCode">
            <el-input v-model="searchForm.ITCode" />
          </el-form-item>
          <el-form-item label="Url">
            <el-input v-model="searchForm.ActionUrl" />
          </el-form-item>
        </el-form>
        <el-form slot="collapse-content" :inline="true" label-width="75px">
          <el-form-item label="操作时间">
            <el-date-picker v-model="searchForm.ActionTime" type="datetimerange" value-format="yyyy-MM-dd HH:mm:ss" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" />
          </el-form-item>
          <el-form-item label="IP">
            <el-input v-model="searchForm.IP" />
          </el-form-item>
          <el-form-item label="类型">
            <el-input v-model="searchForm.LogType" />
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['delete', 'export']" :action-list="actionList" :selected-data="selectData" @onAdd="onAdd" @onEdit="onEdit(arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </card>
    <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/vue-custom/mixin/base";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/actionlog";
// 查询参数, table列 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    @State
    searchData;
    // 动作
    assembly = ASSEMBLIES;
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    tableHeader = TABLE_HEADER;
    // 查询接口 ★★★★★
    privateRequest(params) {
        params.StartActionTime = params.ActionTime.split(",")[0];
        params.EndActionTime = params.ActionTime.split(",")[1];
        return this.search(params);
    }
}
</script>
