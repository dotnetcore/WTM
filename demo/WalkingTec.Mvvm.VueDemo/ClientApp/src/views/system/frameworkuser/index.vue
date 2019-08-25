<template>
  <div class="dataprivilege">
    <article>
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
      <but-box :assembly="['add', 'edit', 'delete', 'export']" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #PhotoId="rowData">
          <el-image style="width: 100px; height: 100px" :src="'/api/_file/downloadFile/'+rowData.row.PhotoId" fit="cover" />
        </template>
        <template #IsValid="rowData">
          <el-switch v-model="rowData.row.IsValid" active-value="true" inactive-value="false" disabled />
        </template>
        <template #operate="rowData">
          <el-button type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </article>
    <dialog-box :is-show.sync="dialogInfo.isShow">
      <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import DialogForm from "./dialog-form.vue";
import { listToString, exportXlsx } from "@/util/string";
import store from "@/store/system/frameworkuser";
// 查询参数 ★★★★★
const defaultSearchData = {
    ITCode: "",
    Name: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData)],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogBox,
        DialogForm,
        ButBox
    }
})
export default class Index extends Vue {
    @Action
    postFrameworkuserSearchList;
    @Action
    getFrameworkuserGetFrameworkRoles;
    @Action
    getFrameworkuserGetFrameworkGroups;
    @Action
    postFrameworkuserBatchDelete;
    @Action
    getFrameworkuserDelete;
    @Action
    postFrameworkuserExportExcel;
    @Action
    postFrameworkuserExportExcelByIds;

    @State
    fameworkuserSearchList;

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
    // 查询 ★★★★★
    created() {
        this["onSearch"]();
    }
    // 查询接口 ★★★★★
    privateRequest(params) {
        return this.postFrameworkuserSearchList(params);
    }
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        this.dialogInfo.isShow = true;
        this.dialogInfo.dialogStatus = status;
        this.dialogInfo.dialogData = data;
        this.$nextTick(() => {
            this.$refs["dialogform"].onGetFormData();
        });
    }
    // ★★★★★
    onDelete(params) {
        this["onConfirm"]().then(() => {
            const parameters = {
                ids: [params.ID]
            };
            this.postFrameworkuserBatchDelete(parameters).then(res => {
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
            this.postFrameworkuserBatchDelete(parameters).then(res => {
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
        this.postFrameworkuserExportExcel(parameters).then(res => {
            exportXlsx(res, "frameworkuserExportExcel");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    // ★★★★☆
    onExport() {
        const parameters = listToString(this["selectData"], "ID");
        this.postFrameworkuserExportExcelByIds(parameters).then(res => {
            exportXlsx(res, "frameworkuserExportExcelByIds");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
}
</script>
