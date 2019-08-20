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
        <span slot="operation">
          <export-excel :params="exportParams" btn-name="导出当前结果" batch-type="EXPORT_REPLACEMENT" />
        </span>
      </fuzzy-search>
      <but-box :assembly="['add', 'edit']" @onAdd="openDialog(dialogType.add)" />
      <table-box :is-operate="true" :tb-column="tableCols" :table-data="tableData" :loading="loading" :page-date="pageDate" @handleSizeChange="handleSizeChange" @handleCurrentChange="handleCurrentChange">
        <template #operate="rowData">
          <el-button type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button type="text" size="small" class="view-btn" @click="openDialog(dialogType.add)">
            删除
          </el-button>
        </template>
      </table-box>
    </article>
    <dialog-box :is-show.sync="detailShow">
      <dialog-form :is-show.sync="detailShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" />
    </dialog-box>
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/mixin/base";
import mixinFunc from "@/mixin/search";
import store from "@/store/system/dataprivilege";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import ExportExcel from "@/components/common/export/export-excel.vue";
import DialogForm from "./dialog-form.vue";
// 查询参数
const defaultSearchData = {
    DpType: "0",
    DomainID: ""
};
@Component({
    mixins: [baseMixin, mixinFunc(defaultSearchData)],
    store,
    components: {
        FuzzySearch,
        ExportExcel,
        TableBox,
        DialogBox,
        DialogForm,
        ButBox
    }
})
export default class Index extends Vue {
    @Action
    getPrivilegesList;
    @Action
    postDataprivilegeSearchList;
    @State
    privilegesList;
    exportParams = {};
    detailShow: Boolean = false;
    // 弹出框内容
    dialogInfo = {
        dialogData: {},
        dialogStatus: ""
    };
    tableCols = {
        Name: { sortable: true, label: "授权对象" },
        TableName: { sortable: true, label: "授权对象" },
        RelateIDs: { sortable: true, label: "权限" }
    };
    created() {
        this.getPrivilegesList();
        this["onSearch"]();
    }
    // 查询接口
    privateRequest(params) {
        return this.postDataprivilegeSearchList(params);
    }
    openDialog(status, data?) {
        this.detailShow = true;
        this.dialogInfo.dialogStatus = status;
        if (data) {
            this.dialogInfo.dialogData = data;
        }
    }
    // toDelete() {}
}
</script>
