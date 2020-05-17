<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
        </wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";

@Component({ mixins: [formMixin()] })
export default class Index extends Vue {
    // 表单结构
    get formOptions() {
        return {
            formProps: {
                "label-width": this.$t("actionlog.LabelWidthForm")
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.ModuleName": {
                    type: "input",
                    label: this.$t("actionlog.ModuleName")
                },
                "Entity.ActionName": {
                    type: "input",
                    label: this.$t("actionlog.ActionName")
                },
                "Entity.ITCode": {
                    type: "input",
                    label: "ITCode"
                },
                "Entity.ActionUrl": {
                    type: "input",
                    label: "Url"
                },
                "Entity.ActionTime": {
                    type: "input",
                    label: this.$t("actionlog.ActionTime")
                },
                "Entity.Duration": {
                    type: "input",
                    label: this.$t("actionlog.Duration")
                },
                "Entity.IP": {
                    type: "input",
                    label: "IP"
                },
                "Entity.Remark": {
                    type: "input",
                    label: this.$t("actionlog.Remark")
                }
            }
        };
    }
}
</script>
