<template>
  <div class="dataprivilege">
    <card>
      <wtm-fuzzy-search ref="fuzzySearch" :search-label-width="75" @onReset="onReset" @onSearch="onSearchForm">
        <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
          <el-form-item label="账号">
            <el-input v-model="searchForm.ITCode" />
          </el-form-item>
          <el-form-item label="姓名">
            <el-input v-model="searchForm.Name" />
          </el-form-item>
        </el-form>
      </wtm-fuzzy-search>
      <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :eventFn="eventFn" />
      <wtm-table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #PhotoId="rowData">
          <el-image v-if="!!rowData.row.PhotoId" style="width: 100px; height: 100px" :src="'/api/_file/downloadFile/'+rowData.row.PhotoId" fit="cover" />
        </template>
        <template #IsValid="rowData">
          <el-switch :value="rowData.row.IsValid === 'true' || rowData.row.IsValid === true" disabled />
        </template>
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
import baseMixin from "@/vue-custom/mixin/base";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import UploadBox from "@/components/page/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkuser";

// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        DialogForm,
        UploadBox
    }
})
export default class Index extends Vue {
    @Action
    getFrameworkRoles;
    @Action
    getFrameworkGroups;

    // 动作
    assembly = ASSEMBLIES;
}
</script>
