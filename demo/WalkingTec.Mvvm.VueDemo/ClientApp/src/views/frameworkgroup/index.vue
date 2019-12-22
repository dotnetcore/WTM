<template>
  <div class="dataprivilege">
    <card>
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
      <but-box :assembly="['add', 'edit', 'delete', 'export', 'imported']" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
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
      <dialog-form :ref="formRefName" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    </dialog-box>
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import baseMixin from "@/util/mixin/base";
import mixinFunc from "@/util/mixin/search";
import actionMixin from "@/util/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkgroup";
// 查询参数/列表 ★★★★★
import { SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, mixinFunc(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogForm,
        ButBox
    }
})
export default class Index extends Vue {
    // 表单弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
}
</script>
