<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions"></wtm-create-form>
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

    get formOptions() {
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": { isHidden: true },
                "Entity.RoleCode": {
                    type: "input",
                    label: this.$t("frameworkrole.RoleCode"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkrole.pleaseEnterRoleCode"),
                        trigger: "blur"
                    }
                },
                "Entity.RoleName": {
                    type: "input",
                    label: this.$t("frameworkrole.RoleName"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkrole.pleaseEnterRoleName"),
                        trigger: "blur"
                    }
                },
                "Entity.RoleRemark": {
                    type: "input",
                    label: this.$t("frameworkrole.RoleRemark")
                }
            }
        };
    }
}
</script>
