<template>
  <div class="dataprivilege">
    <card>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="ITCode">
            <el-input v-model="searchForm.ITCode" />
          </el-form-item>
          <el-form-item label="Url">
            <el-input v-model="searchForm.ActionUrl" />
          </el-form-item>
        </el-form>
        <el-form slot="collapse-content" class="form-class">
          <el-form-item label="操作时间">
            <el-input v-model="searchForm.ActionTime" />
          </el-form-item>
          <el-form-item label="IP">
            <el-input v-model="searchForm.IP" />
          </el-form-item>
          <el-form-item label="类型">
            <el-input v-model="searchForm.LogType" />
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['delete', 'export']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </card>
    <dialog-box :is-show.sync="dialogInfo.isShow" :status="dialogInfo.dialogStatus">
      <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/util/mixin/base";
import mixinFunc from "@/util/mixin/search";
import actionMixin from "@/util/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/actionlog";
// 查询参数 ★★★★★
const defaultSearchData = {
    LogType: "",
    ActionName: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogForm,
        ButBox
    }
})
export default class Index extends Vue {
    @Action search;
    @Action batchDelete;
    @Action deleted;
    @Action exportExcel;
    @Action exportExcelByIds;
    @Action imported;
    @Action getExcelTemplate;

    @State
    searchData;

    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // ★★★★★
    tableCols = [
        { key: "LogType", label: "类型" },
        { key: "ModuleName", label: "模块" },
        { key: "ActionName", label: "动作" },
        { key: "ITCode", label: "ITCode" },
        { key: "ActionUrl", label: "ActionUrl" },
        { key: "ActionTime", label: "操作时间" },
        { key: "Duration", label: "时长" },
        { key: "IP", label: "IP" },
        { key: "Remark", label: "备注" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        this.dialogInfo.isShow = true;
        this.dialogInfo.dialogStatus = status;
        this.dialogInfo.dialogData = data;
        this.$nextTick(() => {
            this.$refs["dialogform"].onGetFormData();
        });
    }
}
</script>
