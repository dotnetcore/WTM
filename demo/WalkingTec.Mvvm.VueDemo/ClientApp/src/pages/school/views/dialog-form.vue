<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions" ></wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import UploadImg from "@/components/page/UploadImg.vue";
import { SchoolTypeTypes } from "../config";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {

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
             "Entity.SchoolCode":{
                 label: "学校编码",
                 rules: [{ required: true, message: "学校编码不能为空",trigger: "blur" }],
                    type: "input"
            },
             "Entity.SchoolName":{
                 label: "学校名称",
                 rules: [{ required: true, message: "学校名称不能为空",trigger: "blur" }],
                    type: "input"
            },
             "Entity.FileId":{
                 label: "文件",
                 rules: [],
                type: "upload"
            },
             "Entity.SchoolType":{
                 label: "学校类型",
                 rules: [{ required: true, message: "学校类型不能为空",trigger: "blur" }],
                    type: "select",
                    children: SchoolTypeTypes
            },
             "Entity.Remark":{
                 label: "备注",
                 rules: [{ required: true, message: "备注不能为空",trigger: "blur" }],
                    type: "input"
            }
                
            }
        };
    }
    created() {

    }
}
</script>
