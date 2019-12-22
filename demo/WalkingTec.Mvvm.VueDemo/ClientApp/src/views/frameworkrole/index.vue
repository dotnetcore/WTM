<template>
    <div class="dataprivilege">
        <card>
            <fuzzy-search ref="fuzzySearch" :search-label-width="75" placeholder="手机号" @onReset="onReset" @onSearch="onSearchForm">
                <el-form slot="search-content" ref="searchForm" class="form-class" :inline="true" label-width="75px">
                    <el-form-item label="角色编号">
                        <el-input v-model="searchForm.RoleCode" />
                    </el-form-item>
                    <el-form-item label="角色名称">
                        <el-input v-model="searchForm.RoleName" />
                    </el-form-item>
                </el-form>
            </fuzzy-search>
            <but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
            <table-box :is-selection="true" :tb-column="tableHeader" :data="tableData" :loading="loading" :page-date="pageDate" @size-change="handleSizeChange" @current-change="handleCurrentChange" @selection-change="onSelectionChange" @sort-change="onSortChange">
                <template #operate="rowData">
                    <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
                        详情
                    </el-button>
                    <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
                        修改
                    </el-button>
                    <el-button type="text" size="small" class="view-btn" @click="openPermission(rowData.row)">
                        分配权限
                    </el-button>
                    <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
                        删除
                    </el-button>
                </template>
            </table-box>
        </card>
        <dialog-form :ref="formRefName" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
        <permission :is-show.sync="dialogInfo.isShowPermission" ref="permissionRef"></permission>
        <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
    </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import baseMixin from "@/util/mixin/base";
import searchMixin from "@/util/mixin/search";
import actionMixin from "@/util/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogBox from "@/components/common/dialog/dialog-box.vue";
import UploadBox from "@/components/common/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import Permission from "./permission.vue";
import store from "@/store/system/frameworkrole";

// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogBox,
        DialogForm,
        ButBox,
        UploadBox,
        Permission
    }
})
export default class Index extends Vue {
    // 差异的方法 单独写出
    @Action("getFrameworkRoles") getFrameworkRoles;
    @Action("getFrameworkGroups") getFrameworkGroups;

    // 动作
    assembly = ASSEMBLIES;
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: "",
        isShowPermission: false
    };

    /**
     * 打开-分配权限
     */
    openPermission(data = {}) {
        this.dialogInfo.isShowPermission = true;
        this.$refs["permissionRef"].onGetFormData(data);
    }
}
</script>
