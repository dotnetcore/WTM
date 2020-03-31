<template>
    <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
        <wtm-create-form :ref="refName" :status="status" :options="formOptions"></wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import UploadImg from "@/components/page/UploadImg.vue";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    @Prop()
    testValue;
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
                "Entity.Code": {
                    label: "层位编码",
                    rules: [
                        {
                            required: true,
                            message: "层位编码不能为空",
                            trigger: "blur"
                        }
                    ],
                    type: "input"
                },
                "Entity.LayerTrans": {
                    label: "层位翻译",
                    rules: [],
                    type: "input"
                },
                "Entity.LibraryStructId": {
                    label: "所属单位",
                    rules: [
                        {
                            required: true,
                            message: "所属单位不能为空",
                            trigger: "blur"
                        }
                    ],
                    type: "select",
                    children: [
                        {
                            Text: "天津2",
                            Value: "tianjin2"
                        },
                        {
                            Text: "深圳2",
                            Value: "shenzhen2",
                            disabled: true
                        },
                        {
                            Text: "韶关2",
                            Value: "shaoguan2"
                        }
                    ],
                    props: {
                        clearable: true
                    }
                }
            }
        };
    }
    afterOpen() {
        if (this["status"] === this["$actionType"].add) {
            this.FormComp().setFormDataItem(
                "Entity.LibraryStructId",
                this.testValue
            );
        }
    }
}
</script>
