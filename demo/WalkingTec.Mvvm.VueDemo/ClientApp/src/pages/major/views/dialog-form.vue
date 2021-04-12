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
import { MajorTypeTypes } from "../config";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    @Action
    getSchool;
    @State
    getSchoolData;
    @Action
    getStudent;
    @State
    getStudentData;

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
             "Entity.MajorCode":{
                 label: "专业编码",
                 rules: [{ required: true, message: "专业编码"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.MajorName":{
                 label: "专业名称",
                 rules: [{ required: true, message: "专业名称"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.MajorType":{
                 label: "专业类别",
                 rules: [{ required: true, message: "专业类别"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "select",
                    children: MajorTypeTypes,
                    props: {
                        clearable: true
                    }
            },
             "Entity.Remark":{
                 label: "备注",
                 rules: [],
                    type: "input"
            },
             "Entity.SchoolId":{
                 label: "所属学校",
                 rules: [{ required: true, message: "所属学校"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "select",
                    children: this.getSchoolData,
                    props: {
                        clearable: true
                    }
            },
             "Entity.StudentMajors":{
                 label: "学生",
                 rules: [],
                    type: "transfer",
                    mapKey: "StudentId",
                    props: {
                        data: this.getStudentData.map(item => ({
                            key: item.Value,
                            label: item.Text
                        })),
                        titles: [this.$t("form.all"), this.$t("form.selected")],
                        filterable: true,
                        filterMethod: filterMethod
                    },
                    span: 24,
                    defaultValue: []
            }
                
            }
        };
    }
    beforeOpen() {
        this.getSchool();
        this.getStudent();

    }
}
</script>
