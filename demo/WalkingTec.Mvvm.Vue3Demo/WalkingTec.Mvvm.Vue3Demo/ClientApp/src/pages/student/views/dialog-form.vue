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
import { SexTypes } from "../config";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    @Action
    getMajor;
    @State
    getMajorData;

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
             "Entity.LoginName":{
                 label: "账号",
                 rules: [{ required: true, message: "账号"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.Password":{
                 label: "密码",
                 rules: [{ required: true, message: "密码"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.Email":{
                 label: "邮箱",
                 rules: [],
                    type: "input"
            },
             "Entity.Name":{
                 label: "姓名",
                 rules: [{ required: true, message: "姓名"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "input"
            },
             "Entity.Sex":{
                 label: "性别",
                 rules: [],
                    type: "select",
                    children: SexTypes,
                    props: {
                        clearable: true
                    }
            },
             "Entity.CellPhone":{
                 label: "手机",
                 rules: [],
                    type: "input"
            },
             "Entity.Address":{
                 label: "住址",
                 rules: [],
                    type: "input"
            },
             "Entity.ZipCode":{
                 label: "邮编",
                 rules: [],
                    type: "input"
            },
             "Entity.PhotoId":{
                 label: "照片",
                 rules: [],
                type: "wtmUploadImg",
                    props: {
                        isHead: true,
                        imageStyle: { width: "100px", height: "100px" }
                    }

            },
             "Entity.IsValid":{
                 label: "是否有效",
                 rules: [{ required: true, message: "是否有效"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "switch"
            },
             "Entity.EnRollDate":{
                 label: "日期",
                 rules: [{ required: true, message: "日期"+this.$t("form.notnull"),trigger: "blur" }],
                    type: "datePicker"
            },
             "Entity.StudentMajor":{
                 label: "专业",
                 rules: [],
                    type: "transfer",
                    mapKey: "MajorId",
                    props: {
                        data: this.getMajorData.map(item => ({
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
        this.getMajor();

    }
}
</script>
