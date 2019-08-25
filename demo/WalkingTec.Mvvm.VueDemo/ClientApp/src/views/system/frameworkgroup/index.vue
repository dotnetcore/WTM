<template>
  <div class="dataprivilege">
    <article>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="用户组编码">
            <el-input v-model="searchForm.GroupCode" />
          </el-form-item>
          <el-form-item label="用户组名称">
            <el-input v-model="searchForm.GroupName" />
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['add', 'edit', 'delete', 'export']" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
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
import store from "@/store/system/frameworkgroup";
// 查询参数 ★★★★★
const defaultSearchData = {
    GroupCode: "",
    GroupName: ""
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
    postFrameworkgroupSearchList;
    @Action
    postFrameworkgroupBatchDelete;
    @Action
    getFrameworkgroupDelete;
    @Action
    postFrameworkgroupExportExcel;
    @Action
    postFrameworkgroupExportExcelByIds;

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
        { key: "GroupCode", sortable: true, label: "用户组编码" },
        { key: "GroupName", sortable: true, label: "用户组名称" },
        { key: "GroupRemark", sortable: true, label: "备注" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    // 查询 ★★★★★
    created() {
        this["onSearch"]();
    }
    // 查询接口 ★★★★★
    privateRequest(params) {
        return this.postFrameworkgroupSearchList(params);
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
            this.postFrameworkgroupBatchDelete(parameters).then(res => {
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
            this.postFrameworkgroupBatchDelete(parameters).then(res => {
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
        this.postFrameworkgroupExportExcel(parameters).then(res => {
            exportXlsx(res, "FrameworkgroupExportExcel");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    // ★★★★☆
    onExport() {
        const parameters = listToString(this["selectData"], "ID");
        this.postFrameworkgroupExportExcelByIds(parameters).then(res => {
            exportXlsx(res, "FrameworkgroupExportExcelByIds");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
}
</script>
