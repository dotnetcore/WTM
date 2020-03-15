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
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.ModuleName": {
                    type: "input",
                    label: "模块"
                },
                "Entity.ActionName": {
                    type: "input",
                    label: "动作"
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
                    label: "操作时间"
                },
                "Entity.Duration": {
                    type: "input",
                    label: "时长"
                },
                "Entity.IP": {
                    type: "input",
                    label: "IP"
                },
                "Entity.Remark": {
                    type: "input",
                    label: "备注"
                }
            }
        };
    }
}
</script>
