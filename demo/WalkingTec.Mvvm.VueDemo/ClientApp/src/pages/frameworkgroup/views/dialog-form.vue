<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import mixinForm from "@/vue-custom/mixin/form-mixin";

@Component({ mixins: [mixinForm()] })
export default class Index extends Vue {
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
                "Entity.GroupRemark": {
                    type: "input",
                    label: this.$t("frameworkgroup.GroupRemark")
                }
            }
        };
    }
}
</script>
<style lang='less'>
</style>
