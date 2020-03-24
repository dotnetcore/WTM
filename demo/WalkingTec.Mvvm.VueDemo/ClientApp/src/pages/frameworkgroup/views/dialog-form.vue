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
    formOptions = {
        formProps: {
            "label-width": "100px"
        },
        formItem: {
            "Entity.ID": { isHidden: true },
            "Entity.GroupCode": {
                type: "input",
                label: "用户组编码",
                rules: {
                    required: true,
                    message: "请输入用户组编码",
                    trigger: "blur"
                }
            },
            "Entity.GroupName": {
                type: "input",
                label: "用户组名称",
                rules: {
                    required: true,
                    message: "请输入用户组名称",
                    trigger: "blur"
                }
            },
            "Entity.GroupRemark": {
                type: "input",
                label: "备注"
            }
        }
    };
}
</script>
<style lang='less'>
</style>
