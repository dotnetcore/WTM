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
      <but-box :assembly="['add', 'edit', 'delete', 'export']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
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
    </article>
    <dialog-box :is-show.sync="formDialog.isShow">
      <dialog-form :ref="formRefName" :is-show.sync="formDialog.isShow" :dialog-data="formDialog.dialogData" :status="formDialog.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import actionMixin from "@/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkgroup";
// 查询参数 ★★★★★
const defaultSearchData = {
    GroupCode: "",
    GroupName: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData), actionMixin],
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
    @Action search;
    @Action batchDelete;
    @Action deleted;
    @Action exportExcel;
    @Action exportExcelByIds;
    @Action detail;
    // 表单弹框ref名称
    formRefName: string = "dialogform";
    // 表单数据的key，下方相同，主要用在dialog-form组件中 传入数据，在action-mixin的方法openDialog有用到
    formDialogKey: string = "formDialog";
    // 表单弹出框内容 ★★★★☆
    formDialog = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // 列表 col ★★★★★
    tableCols = [
        { key: "GroupCode", sortable: true, label: "用户组编码" },
        { key: "GroupName", sortable: true, label: "用户组名称" },
        { key: "GroupRemark", sortable: true, label: "备注" },
        { key: "operate", label: "操作", isSlot: true }
    ];
}
</script>
