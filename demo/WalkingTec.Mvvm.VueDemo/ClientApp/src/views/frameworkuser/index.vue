<template>
  <div class="dataprivilege">
    <card>
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
      <but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #PhotoId="rowData">
          <el-image v-if="!!rowData.row.PhotoId" style="width: 100px; height: 100px" :src="'/api/_file/downloadFile/'+rowData.row.PhotoId" fit="cover" />
        </template>
        <template #IsValid="rowData">
          <el-switch v-model="rowData.row.IsValid" disabled />
        </template>
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
    <!-- <dialog-box :is-show.sync="dialogInfo.isShow" :status="dialogInfo.dialogStatus"></dialog-box> -->
    <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/util/mixin/base";
import searchMixin from "@/util/mixin/search";
import actionMixin from "@/util/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import UploadBox from "@/components/common/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkuser";

// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogForm,
        ButBox,
        UploadBox
    }
})
export default class Index extends Vue {
    @Action getFrameworkRoles;
    @Action getFrameworkGroups;

    @State searchData;
    // 动作
    assembly = ASSEMBLIES;
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
}
</script>
