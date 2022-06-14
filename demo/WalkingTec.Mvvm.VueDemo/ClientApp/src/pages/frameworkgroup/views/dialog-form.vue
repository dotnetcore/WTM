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

@Component({ mixins: [mixinForm()] })
export default class Index extends Vue {

        @Action
    getFrameworkGroups;

    @State
    getFrameworkGroupsData;


    get formOptions() {
        return {
            formProps: {
                "label-width": "110px"
            },
            formItem: {
                "Entity.ID": { isHidden: true },
                "Entity.GroupCode": {
                    type: "input",
                    label: this.$t("frameworkgroup.GroupCode"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkgroup.pleaseEnterGroupCode"),
                        trigger: "blur"
                    },
                    props: {
                        disabled: this['status'] !== 'add'
                    }
                },
                "Entity.GroupName": {
                    type: "input",
                    label: this.$t("frameworkgroup.GroupName"),
                    rules: {
                        required: true,
                        message: this.$t("frameworkgroup.pleaseEnterGroupName"),
                        trigger: "blur"
                    }
                },
                 "Entity.ParentId": {
                    type: "select",
                    label: this.$t("frameworkgroup.ParentId"),
                    children: this.getFrameworkGroupsData
                },
                "Entity.Manager": {
                    type: "input",
                    label: this.$t("frameworkgroup.Manager")
                },
               "Entity.GroupRemark": {
                    type: "input",
                    label: this.$t("frameworkgroup.GroupRemark")
                }
            }
        };
    }
      created() {
        this.getFrameworkGroups();
    }
}
</script>
<style lang='less'>
</style>
