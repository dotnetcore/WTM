<template>
  <div class="dataprivilege">
    <article>
      <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="权限名称" prop="car_model">
            <el-select v-model="searchForm.type" clearable placeholder="请选择" value="" multiple>
              <el-option key="22" value="dd" label="ss" />
              <el-option key="33" value="eee" label="xx" />
            </el-select>
          </el-form-item>
          <el-form-item label="权限类型" prop="car_model">
            <el-radio v-model="searchForm.DpType" label="0">
              用户组权限
            </el-radio>
            <el-radio v-model="searchForm.DpType" label="1">
              用户权限
            </el-radio>
          </el-form-item>
        </el-form>
      </fuzzy-search>
      <but-box :assembly="['add', 'edit', 'delete', 'export']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" />
      <table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @handleSizeChange="handleSizeChange" @handleCurrentChange="handleCurrentChange" @onSelectionChange="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @onDelete="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </article>
    <dialog-box :is-show.sync="dialogInfo.isShow">
      <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" />
    </dialog-box>
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import actionMixin from "@/mixin/action-mixin";
import store from "@/store/system/dataprivilege";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import DialogForm from "./dialog-form.vue";
// 查询参数
const defaultSearchData = {
    DpType: "0",
    DomainID: ""
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
    @Action delete;
    @Action exportExcel;
    @Action exportExcelByIds;
    @Action privilegesList;
    @State
    privilegesListData;
    exportParams = {};
    // 弹出框内容
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    tableCols = [
        { key: "Name", sortable: true, label: "授权对象" },
        { key: "TableName", sortable: true, label: "授权对象" },
        { key: "RelateIDs", sortable: true, label: "权限" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    created() {
        this.privilegesList();
        this["onSearch"]();
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
}
</script>
