<template>
  <div class="dataprivilege">
    <card>
      <wtm-fuzzy-search ref="fuzzySearch" :search-label-width="75" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="权限名称" prop="car_model">
            <el-select v-model="searchForm.TableName" clearable multiple placeholder="请选择权限名称">
              <el-option v-for="(item,index) of getPrivilegesData" :key="index" :label="item.Text" :value="item.Value" />
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
      </wtm-fuzzy-search>
      <wtm-but-box :assembly="['add', 'edit', 'delete', 'export']" :action-list="actionList" :selected-data="selectData" :eventFn="eventFn" />
      <wtm-table-box :is-selection="true" :tb-column="tableCols" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="onEdit(rowData.row)">
            修改
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </wtm-table-box>
    </card>
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinFunc from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import store from "@/store/system/dataprivilege";
import DialogForm from "./dialog-form.vue";
// 查询参数
const defaultSearchData = {
    DpType: "0",
    DomainID: ""
};
@Component({
    mixins: [mixinFunc(defaultSearchData), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    @Action
    getPrivileges;
    @State
    getPrivilegesData;
    exportParams = {};

    tableCols = [
        { key: "Name", sortable: true, label: "授权对象" },
        { key: "TableName", sortable: true, label: "权限名称" },
        { key: "RelateIDs", sortable: true, label: "权限" },
        { key: "operate", label: "操作", isSlot: true }
    ];
    created() {
        this.getPrivileges();
        this["onSearch"]();
    }
}
</script>
