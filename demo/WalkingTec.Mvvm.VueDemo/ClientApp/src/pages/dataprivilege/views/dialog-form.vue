<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";
import { whether } from "@/config/entity";

@Component({ mixins: [mixinForm()] })
export default class Index extends Vue {
    @Action("get")
    detail;
    @Action
    getUserGroups;
    @Action
    getPrivileges;
    @Action
    getPrivilegeByTableName;

    @State
    getUserGroupsData;
    @State
    getPrivilegesData;
    @State
    getPrivilegeByTableNameData;

    // 表单结构
    get formOptions() {
        return {
            formProps: {
                "label-width": this.$t("dataprivilege.LabelWidthForm")
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                DpType: {
                    type: "radioGroup",
                    label: this.$t("dataprivilege.DpType"),
                    span: 24,
                    defaultValue: 0,
                    children: [
                        { Value: 0, Text: this.$t("dataprivilege.UserGroup") },
                        { Value: 1, Text: this.$t("dataprivilege.UserRights") }
                    ],
                    rules: {
                        type: "number",
                        required: true,
                        message: this.$t(
                            "dataprivilege.PleaseSelectPermissionType"
                        ),
                        trigger: "change"
                    }
                },
                "Entity.GroupId": {
                    type: "select",
                    label: this.$t("dataprivilege.UserGroupList"),
                    children: this.getUserGroupsData,
                    isHidden: data => data.DpType === 1,
                    rules: {
                        required: true,
                        message: this.$t("dataprivilege.PleaseSelectUserGroup"),
                        trigger: "change"
                    }
                },
                UserItCode: {
                    type: "input",
                    label: this.$t("dataprivilege.UserID"),
                    isHidden: data => data.DpType === 0,
                    rules: {
                        required: true,
                        message: this.$t("dataprivilege.pleaseEnterUserID"),
                        trigger: "blur"
                    }
                },
                "Entity.TableName": {
                    type: "select",
                    label: this.$t("dataprivilege.TableName"),
                    children: this.getPrivilegesData,
                    events: {
                        change: this.onPrivileges
                    },
                    rules: {
                        required: true,
                        message: this.$t("dataprivilege.PleaseSelectTableName"),
                        trigger: "change"
                    }
                },
                IsAll: {
                    type: "select",
                    label: this.$t("dataprivilege.AllPermissions"),
                    children: whether,
                    rules: {
                        type: "boolean",
                        required: true,
                        message: this.$t(
                            "dataprivilege.PleaseSelectAllPermissions"
                        ),
                        trigger: "change"
                    }
                },
                SelectedItemsID: {
                    type: "select",
                    label: this.$t("dataprivilege.AllowAccess"),
                    props: {
                        multiple: true,
                        filterable: true
                    },
                    children: this.getPrivilegeByTableNameData
                }
            }
        };
    }

    created() {
        this.getUserGroups();
        this.getPrivileges();
    }

    /**
     * 查询详情-after-调用
     */
    afterOpen(data) {
        this.onPrivileges(data && data.Entity.TableName);
    }
    /**
     * 权限名称
     */
    onPrivileges(SelectedModule?: string) {
        this.getPrivilegeByTableName({
            table: SelectedModule
        });
    }
}
</script>
