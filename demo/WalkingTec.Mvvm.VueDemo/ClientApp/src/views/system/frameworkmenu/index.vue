<template>
  <div class="dataprivilege">
    <article>
      <but-box :assembly="['add', 'edit', 'delete', 'export']" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :tree-props="{children: 'Children', hasChildren: 'hasChildren'}" row-key="ID" @selection-change="onSelectionChange" @sort-change="onSortChange">
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
import store from "@/store/system/frameworkmenu";
// 查询参数 ★★★★★
const defaultSearchData = {
    menuCode: "",
    menuName: ""
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
    frameworkmenuSearchList;
    @Action
    frameworkmenuBatchDelete;
    @Action
    frameworkmenuDelete;
    @Action
    frameworkmenuExportExcel;
    @Action
    frameworkmenuExportExcelByIds;

    @State
    fameworkuserSearchListData;

    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // ★★★★★
    tableCols = [
        { key: "PageName", sortable: true, label: "页面名称", align: "left" },
        { key: "DisplayOrder", sortable: true, label: "顺序" },
        { key: "ICon", sortable: true, label: "图标" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    // 查询 ★★★★★
    created() {
        this["onSearch"]();
    }
    // 查询接口 ★★★★★
    privateRequest(params) {
        return this.frameworkmenuSearchList(params);
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
            this.frameworkmenuBatchDelete(parameters).then(res => {
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
            this.frameworkmenuBatchDelete(parameters).then(res => {
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
        this.frameworkmenuExportExcel(parameters).then(res => {
            exportXlsx(res, "FrameworkmenuExportExcel");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    // ★★★★☆
    onExport() {
        const parameters = listToString(this["selectData"], "ID");
        this.frameworkmenuExportExcelByIds(parameters).then(res => {
            exportXlsx(res, "FrameworkmenuExportExcelByIds");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
}
</script>
