<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" :events="formEvent">
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";

@Component({ mixins: [formMixin()] })
export default class extends Vue {
    @Action
    getFrameworkRoles;
    @Action
    getFrameworkGroups;
    @State
    getFrameworkRolesData;
    @State
    getFrameworkGroupsData;

    formOptions = {
        formProps: {
            "label-width": "100px"
        },
        formItem: {
            "Entity.ID": { isHidden: true },
            "Entity.RoleCode": {
                type: "input",
                label: "角色编号",
                rules: {
                    required: true,
                    message: "请输入角色编号",
                    trigger: "blur"
                }
            },
            "Entity.RoleName": {
                type: "input",
                label: "角色名称",
                rules: {
                    required: true,
                    message: "请输入角色名称",
                    trigger: "blur"
                }
            },
            "Entity.RoleRemark": {
                type: "input",
                label: "备注"
            }
        }
    };
}
</script>
