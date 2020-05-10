<template>
    <div>
        <el-button type="primary" @click="onStatus">切换status</el-button>
        <el-button type="primary" @click="onResetFields">清空</el-button>
        <el-button @click="testSubmit">提交</el-button>
        <wtm-create-form :ref="refName" :status="tempStatus" :options="formOptions" :sourceFormData="sourceFormData">
            <template #skey="data">
                <span v-if="data.status === $actionType.detail">{{
                    JSON.stringify(data)
                    }}</span>
                <el-input-number v-else v-model="sourceFormData.Entity.testsole" :min="1" :max="10"></el-input-number>
            </template>
        </wtm-create-form>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { setTimeout } from "timers";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    tempStatus = "add";
    testList = [];
    sourceFormData = {
        Entity: {
            ID: "",
            Name: "",
            Price: "",
            BookIndex: [],
            testsole: ""
        }
    };

    // 表单结构
    get formOptions() {
        const filterMethod = (query, item) => {
            return item.label.indexOf(query) > -1;
        };
        const obj = {
            formProps: {
                "label-width": "100px"
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true
                },
                "Entity.Name": {
                    label: "图书名称",
                    rules: [
                        {
                            required: true,
                            message: "图书名称不能为空",
                            trigger: "blur"
                        }
                    ],
                    type: "input"
                },
                "Entity.Price": {
                    label: "价格",
                    rules: [
                        {
                            required: true,
                            message: "价格不能为空",
                            trigger: "blur"
                        }
                    ],
                    type: "input"
                },
                "Entity.BookIndex": {
                    type: "select",
                    label: "下拉框",
                    mapKey: "DicDiagnoseId",
                    children: this.diagremoteMethodData,
                    events: {
                        change: this.onChange
                    },
                    props: {
                        filterable: true,
                        reservekeyword: true,
                        multiple: true,
                        placeholder: "请输入",
                        remote: true,
                        clearable: true,
                        "remote-method": this.diagremoteMethod
                    }
                },
                "Entity.testsole": {
                    type: "wtmSlot",
                    label: "自定义",
                    slotKey: "skey"
                }
            }
        };
        if (this.testList.length > 0) {
            this.testList.forEach(item => {
                obj.formItem[`Entity.testData_${item.ID}`] = {
                    label: item.Attr_name,
                    rules: [
                        {
                            required: true,
                            message: item.Attr_name + "不能为空",
                            trigger: "blur"
                        }
                    ],
                    type: "input"
                };
            });
        }
        return obj;
    }

    onStatus() {
        this.tempStatus = this.tempStatus === "add" ? "detail" : "add";
    }
    onResetFields() {
        this.$refs.refName.resetFields();
    }

    testSubmit() {
        console.log("formData", this.sourceFormData);
        console.log(JSON.stringify(this.$refs[this.refName].getFormData()));
    }

    mounted() {
        this.setFormData({
            Entity: {
                ID: "",
                Name: "",
                Barcode: "",
                Author: "",
                Price: "",
                BookIndex: [],
                SchoolType: 1
            }
        });
    }

    diagremoteMethodData: Array<any> = [];

    diagremoteMethod() {
        setTimeout(() => {
            const list: Array<any> = [];
            list.push({
                Text: "酒",
                Value: 1
            });
            list.push({
                Text: "烟",
                Value: 2
            });
            this.diagremoteMethodData = list;
        }, 200);
    }

    onChange() {
        setTimeout(() => {
            this.testList = [
                {
                    Attr_name: "产地",
                    Attr_type: 1,
                    Search_type: true,
                    Value_select: "",
                    Enable: true,
                    Pms_category: null,
                    Pms_categoryId: "6e440945-67f6-458c-b213-c43b56a153d0",
                    ID: "dc33b234-fa65-4e8e-815b-01340d556b34"
                },
                {
                    Attr_name: "厂家联系方式",
                    Attr_type: 0,
                    Search_type: true,
                    Value_select: "",
                    Enable: true,
                    Pms_category: null,
                    Pms_categoryId: "6e440945-67f6-458c-b213-c43b56a153d0",
                    ID: "bfa20ea1-8130-4ffc-8759-7aa0ee05260f"
                },
                {
                    Attr_name: "酒精度",
                    Attr_type: 0,
                    Search_type: true,
                    Value_select: "",
                    Enable: true,
                    Pms_category: null,
                    Pms_categoryId: "6e440945-67f6-458c-b213-c43b56a153d0",
                    ID: "8e04585b-89f9-40cf-a6a9-e83b273cd487"
                },
                {
                    Attr_name: "保质期",
                    Attr_type: 0,
                    Search_type: true,
                    Value_select: "",
                    Enable: true,
                    Pms_category: null,
                    Pms_categoryId: "6e440945-67f6-458c-b213-c43b56a153d0",
                    ID: "0b052c0f-eadd-4799-abd7-f8f97f09d208"
                }
            ];
            this.testList.forEach(item => {
                this.$set(
                    this.sourceFormData.Entity,
                    `testData_${item.ID}`,
                    ""
                );
            });
        }, 200);
    }
}
</script>
