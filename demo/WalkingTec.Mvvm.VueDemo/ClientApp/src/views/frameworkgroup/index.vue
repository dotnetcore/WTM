<template>
  <div class="dataprivilege">
    <card>
      <wtm-fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="用户组编码">
            <el-input v-model="searchForm.GroupCode" />
          </el-form-item>
          <el-form-item label="用户组名称">
            <el-input v-model="searchForm.GroupName" />
          </el-form-item>
        </el-form>
      </wtm-fuzzy-search>
      <wtm-but-box :assembly="assembly" :action-list="permissionList" :selected-data="selectData" :eventFn="eventFn" />
      <wtm-table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="permissionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
            详情
          </el-button>
          <el-button v-visible="permissionList.edit" type="text" size="small" class="view-btn" @click="onEdit(rowData.row)">
            修改
          </el-button>
          <el-button v-visible="permissionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </wtm-table-box>
    </card>
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import baseMixin from "@/vue-custom/mixin/base";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkgroup";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    // 动作
    assembly = ASSEMBLIES;
}
</script>
