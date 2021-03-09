<template>
    <card>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" />
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
        <!-- 列表 -->
        <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
            <template #operate="rowData">
                <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
                    {{ $t("table.detail") }}
                </el-button>
                <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="onEdit(rowData.row)">
                    {{ $t("table.edit") }}
                </el-button>
                <el-button type="text" size="small" class="view-btn" @click="openPermission(rowData.row)">
                    {{ $t("frameworkrole.AssignPermissions") }}
                </el-button>
                <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
                    {{ $t("table.delete") }}
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
import DialogForm from "./views/dialog-form.vue";
import Permission from "./views/permission.vue";
import store from "./store/index";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER } from "./config";

@Component({
    name: "frameworkrole",
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
                "label-width": this.$t("frameworkrole.LabelWidth"),
                inline: true
            },
            formItem: {
                RoleCode: {
                    type: "input",
                    label: this.$t("frameworkrole.RoleCode")
                },
                RoleName: {
                    type: "input",
                    label: this.$t("frameworkrole.RoleName")
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
