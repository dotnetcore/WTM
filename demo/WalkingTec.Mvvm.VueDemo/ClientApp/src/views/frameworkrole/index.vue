<template>
    <card class="dataprivilege">
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" />
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
        <!-- 列表 -->
        <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
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
        <!-- 弹出框 -->
        <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
        <!-- 导入 -->
        <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
        <permission :is-show.sync="isShowPermission" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    </card>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import UploadBox from "@/components/page/upload/index.vue";
import DialogForm from "./dialog-form.vue";
import Permission from "./permission.vue";
import store from "@/store/frameworkrole";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER } from "./config";

@Component({
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
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
    // 权限窗口
    isShowPermission: boolean = false;

    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": "75px",
                inline: true
            },
            formItem: {
                RoleCode: {
                    type: "input",
                    label: "角色编号"
                },
                RoleName: {
                    type: "input",
                    label: "角色名称"
                }
            }
        };
    }
    /**
     * 打开-分配权限
     */
    openPermission(data = {}) {
        this.dialogData = data;
        this.dialogStatus = this.$actionType.edit;
        this.isShowPermission = true;
    }
}
</script>
