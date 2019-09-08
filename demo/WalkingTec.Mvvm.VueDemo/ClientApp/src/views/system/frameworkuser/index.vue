<template>
  <div class="dataprivilege">
    <card>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="账号">
            <el-input v-model="searchForm.ITCode" />
          </el-form-item>
          <el-form-item label="姓名">
            <el-input v-model="searchForm.Name" />
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['add', 'edit', 'delete', 'export', 'imported']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #PhotoId="rowData">
          <el-image style="width: 100px; height: 100px" :src="'/api/_file/downloadFile/'+rowData.row.PhotoId" fit="cover" />
        </template>
        <template #IsValid="rowData">
          <el-switch v-model="rowData.row.IsValid" disabled />
        </template>
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </card>
    <dialog-box :is-show.sync="dialogInfo.isShow">
      <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import actionMixin from "@/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import UploadBox from "@/components/common/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkuser";
// 查询参数 ★★★★★
const defaultSearchData = {
    ITCode: "",
    Name: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogForm,
        ButBox,
        UploadBox
    }
})
export default class Index extends Vue {
    @Action search;
    @Action batchDelete;
    @Action delete;
    @Action exportExcel;
    @Action exportExcelByIds;
    @Action getFrameworkRoles;
    @Action getFrameworkGroups;
    @Action imported;
    @Action getExcelTemplate;

    @State searchData;
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // ★★★★★
    tableCols = [
        { key: "ITCode", sortable: true, label: "账号" },
        { key: "Name", sortable: true, label: "姓名" },
        { key: "Sex", sortable: true, label: "性别" },
        { key: "PhotoId", label: "照片", isSlot: true },
        { key: "IsValid", label: "是否生效", isSlot: true },
        { key: "RoleName_view", label: "角色" },
        { key: "GroupName_view", label: "用户组" },
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
