<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions"></wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { sexList } from "@/config/entity";
import UploadImg from "@/components/page/UploadImg.vue";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    @Action
    getFrameworkRoles;
    @Action
    getFrameworkGroups;
    @State
    getFrameworkRolesData;
    @State
    getFrameworkGroupsData;
    // 表单结构
    get formOptions() {
        const filterMethod = (query, item) => {
            return item.label.indexOf(query) > -1;
        };
        return {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.ITCode": {
                    type: "input",
                    label: this.$t("frameworkuser.ITCode"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkuser.pleaseEnterITCode"),
                        trigger: "blur"
                    }
                },
                "Entity.Password": {
                    type: "input",
                    label: this.$t("frameworkuser.Password"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkuser.pleaseEnterPassword"),
                        trigger: "blur"
                    },
                    isHidden: (res, status) =>
                        ["edit", "detail"].includes(status)
                },
                "Entity.Email": {
                    type: "input",
                    label: this.$t("frameworkuser.Email")
                },
                "Entity.Name": {
                    type: "input",
                    label: this.$t("frameworkuser.Name"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkuser.pleaseEnterName"),
                        trigger: "blur"
                    }
                },
                "Entity.Sex": {
                    type: "select",
                    label: this.$t("frameworkuser.Sex"),
                    children: sexList
                },
                "Entity.CellPhone": {
                    type: "input",
                    label: this.$t("frameworkuser.CellPhone")
                },
                "Entity.HomePhone": {
                    type: "input",
                    label: this.$t("frameworkuser.HomePhone")
                },
                "Entity.Address": {
                    type: "input",
                    label: this.$t("frameworkuser.Address")
                },
                "Entity.ZipCode": {
                    type: "input",
                    label: this.$t("frameworkuser.ZipCode")
                },
                "Entity.PhotoId": {
                    type: "wtmUploadImg",
                    label: this.$t("frameworkuser.PhotoId"),
                    props: {
                        isHead: true,
                        imageStyle: { width: "100px", height: "100px" }
                    }
                },
                "Entity.IsValid": {
                    type: "switch",
                    label: this.$t("frameworkuser.IsValid"),
                    defaultValue: true
                },
                "Entity.UserRoles": {
                    type: "transfer",
                    label: this.$t("frameworkuser.UserRoles"),
                    mapKey: "RoleId",
                    props: {
                        data: this.getFrameworkRolesData.map(item => ({
                            key: item.Value,
                            label: item.Text
                        })),
                        titles: [
                            this.$t("frameworkuser.All"),
                            this.$t("frameworkuser.Selected")
                        ],
                        filterable: true,
                        filterMethod: filterMethod,
                        "filter-placeholder": this.$t(
                            "frameworkuser.pleaseEnterUserRoles"
                        )
                    },
                    span: 24,
                    defaultValue: []
                },
                "Entity.UserGroups": {
                    type: "transfer",
                    label: this.$t("frameworkuser.UserGroups"),
                    mapKey: "GroupId",
                    props: {
                        data: this.getFrameworkGroupsData.map(item => ({
                            key: item.Value,
                            label: item.Text
                        })),
                        titles: [
                            this.$t("frameworkuser.All"),
                            this.$t("frameworkuser.Selected")
                        ],
                        filterable: true,
                        filterMethod: filterMethod,
                        "filter-placeholder": this.$t(
                            "frameworkuser.pleaseEnterRoleName"
                        )
                    },
                    span: 24,
                    defaultValue: []
                }
            }
        };
    }
    beforeOpen() {
        this.getFrameworkRoles();
        this.getFrameworkGroups();
    }
}
</script>
