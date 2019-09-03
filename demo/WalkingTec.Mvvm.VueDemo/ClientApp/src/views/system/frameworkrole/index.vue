<template>
  <div class="dataprivilege">
    <card>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="角色编号">
            <el-input v-model="searchForm.RoleCode" />
          </el-form-item>
          <el-form-item label="角色名称">
            <el-input v-model="searchForm.RoleName" />
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['add', 'edit', 'delete', 'export', 'imported']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button type="text" size="small" class="view-btn">
            分配权限
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </card>
    <dialog-box :is-show.sync="dialogInfo.isShow">
      <dialog-form :ref="formRefName" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import UploadBox from "@/components/common/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import { listToString, exportXlsx } from "@/util/string";
import store from "@/store/system/frameworkrole";
import { createBlob } from "@/util/files";

// 查询参数 ★★★★★
const defaultSearchData = {
    RoleCode: "",
    RoleName: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData)],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogBox,
        DialogForm,
        ButBox,
        UploadBox
    }
})
export default class Index extends Vue {
    @Action("search") searchList;
    @Action("getFrameworkRoles") getFrameworkRoles;
    @Action("getFrameworkGroups") getFrameworkGroups;
    @Action("batchDelete") deleteIDs;
    @Action("deleted") deleteAll;
    @Action("exportExcel") exportExcel;
    @Action("exportExcelByIds") exportExcelByIds;
    @Action("imported") imported;
    @Action("getExcelTemplate") getExcelTemplate;

    // 表单弹框ref名称
    formRefName: string = "dialogform";
    // 表单数据的key，下方相同，主要用在dialog-form组件中 传入数据，在action-mixin的方法openDialog有用到
    formDialogKey: string = "dialogInfo";
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // ★★★★★
    tableCols = [
        { key: "RoleCode", sortable: true, label: "角色编号" },
        { key: "RoleName", sortable: true, label: "角色姓名" },
        { key: "RoleRemark", sortable: true, label: "备注" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    // 导入
    uploadIsShow = false;
    // 查询 ★★★★★
    created() {
        this["onSearch"]();
    }
    // 查询接口 ★★★★★
    privateRequest(params) {
        return this.searchList(params);
    }
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        this[this.formDialogKey].isShow = true;
        this[this.formDialogKey].dialogStatus = status;
        this[this.formDialogKey].dialogData = data;
        this.$nextTick(() => {
            this.$refs[this.formRefName].onGetFormData();
        });
    }
    // ★★★★★
    onDelete(params) {
        this["onConfirm"]().then(() => {
            const parameters = {
                ids: [params.ID]
            };
            this.deleteAll(parameters).then(res => {
                this["$notify"]({
                    title: "删除成功",
                    type: "success"
                });
                this["onHoldSearch"]();
            });
        });
    }
    // ★★★★★
    onBatchDelete() {
        this["onConfirm"]().then(() => {
            const parameters = {
                ids: listToString(this["selectData"], "ID")
            };
            this.deleteIDs(parameters).then(res => {
                this["$notify"]({
                    title: "删除成功",
                    type: "success"
                });
                this["onHoldSearch"]();
            });
        });
    }
    // ★★★★☆
    onExportAll() {
        const parameters = {
            ...this["searchFormClone"],
            Page: this["pageDate"].currentPage,
            Limit: this["pageDate"].pageSize
        };
        this.exportExcel(parameters).then(res => {
            exportXlsx(res, "FrameworkroleExportExcel");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    // ★★★★☆
    onExport() {
        const parameters = listToString(this["selectData"], "ID");
        this.exportExcelByIds(parameters).then(res => {
            exportXlsx(res, "FrameworkroleExportExcelByIds");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    // 下载
    onDownload() {
        this.getExcelTemplate().then(res => createBlob(res));
    }
    // ★★★★☆
    onImport(fileData) {
        const parameters = {
            UploadFileId: fileData.Id
        };
        this.imported(parameters).then(res => {
            this["$notify"]({
                title: "导入成功",
                type: "success"
            });
            this["onHoldSearch"]();
        });
    }
}
</script>
