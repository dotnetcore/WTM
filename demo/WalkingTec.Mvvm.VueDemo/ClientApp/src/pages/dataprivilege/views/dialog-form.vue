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
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                DpType: {
                    type: "radioGroup",
                    label: "模块",
                    span: 24,
                    defaultValue: 0,
                    children: [
                        { Value: 0, Text: "用户组权限" },
                        { Value: 1, Text: "用户权限" }
                    ],
                    rules: {
                        type: "number",
                        required: true,
                        message: "请选择权限类型",
                        trigger: "change"
                    }
                },
                "Entity.GroupId": {
                    type: "select",
                    label: "用户组",
                    children: this.getUserGroupsData,
                    isHidden: data => data.DpType === 1,
                    rules: {
                        required: true,
                        message: "请选择用户组",
                        trigger: "change"
                    }
                },
                UserItCode: {
                    type: "input",
                    label: "用户Id",
                    isHidden: data => data.DpType === 0,
                    rules: {
                        required: true,
                        message: "请输入用户Id",
                        trigger: "blur"
                    }
                },
                "Entity.TableName": {
                    type: "select",
                    label: "权限名称",
                    children: this.getPrivilegesData,
                    events: {
                        change: this.onPrivileges
                    },
                    rules: {
                        required: true,
                        message: "请选择权限名称",
                        trigger: "change"
                    }
                },
                IsAll: {
                    type: "select",
                    label: "全部权限",
                    children: whether,
                    rules: {
                        type: "boolean",
                        required: true,
                        message: "请选择全部权限",
                        trigger: "change"
                    }
                },
                SelectedItemsID: {
                    type: "select",
                    label: "允许访问",
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
