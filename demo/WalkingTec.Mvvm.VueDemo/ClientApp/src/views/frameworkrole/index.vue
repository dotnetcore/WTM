<template>
    <div class="dataprivilege">
        <card>
            <wtm-fuzzy-search ref="fuzzySearch" :search-label-width="75" @onReset="onReset" @onSearch="onSearchForm">
                <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
                    <el-form-item label="角色编号">
                        <el-input v-model="searchForm.RoleCode" />
                    </el-form-item>
                    <el-form-item label="角色名称">
                        <el-input v-model="searchForm.RoleName" />
                    </el-form-item>
                </el-form>
            </wtm-fuzzy-search>
            <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :eventFn="eventFn" />
            <wtm-table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
                <template #operate="rowData">
                    <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
                        详情
                    </el-button>
                    <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="onEdit(rowData.row)">
                        修改
                    </el-button>
                    <el-button type="text" size="small" class="view-btn" @click="openPermission(rowData.row)">
                        分配权限
                    </el-button>
                    <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
                        删除
                    </el-button>
                </template>
            </wtm-table-box>
        </card>
        <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
        <permission :is-show.sync="isShowPermission" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
        <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
    </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import baseMixin from "@/vue-custom/mixin/base";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import UploadBox from "@/components/page/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import Permission from "./permission.vue";
import store from "@/store/system/frameworkrole";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        DialogForm,
        UploadBox,
        Permission
    }
})
export default class Index extends Vue {
    // 差异的方法 单独写出
    @Action("getFrameworkRoles")
    getFrameworkRoles;
    @Action("getFrameworkGroups")
    getFrameworkGroups;
    // 动作(按钮)
    assembly: Array<string> = ASSEMBLIES;
    //
    isShowPermission: boolean = false;

    /**
     * 打开-分配权限
     */
    openPermission(data = {}) {
        this.dialogData = data;
        this.dialogStatus = this["$actionType"].edit;
        this.isShowPermission = true;
    }
}
</script>
