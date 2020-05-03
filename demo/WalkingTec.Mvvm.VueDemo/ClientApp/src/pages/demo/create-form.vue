<template>
    <div>
        <el-button type="primary" @click="onStatus">切换status</el-button>
        <el-button type="primary" @click="onResetFields">清空</el-button>
        <el-button @click="testSubmit">提交</el-button>
        <wtm-create-form :ref="refName" :status="status" :options="formOptions">
            <template #skey="data">
                <span v-if="data.status === $actionType.detail">{{
                    JSON.stringify(data)
                    }}</span>
                <el-input-number v-else v-model="mergeFormData.Entity.testsole" :min="1" :max="10"></el-input-number>
            </template>
        </wtm-create-form>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";

@Component({
    mixins: [formMixin()]
})
export default class Index extends Vue {
    status = "add";

    mergeFormData = {
        Entity: {
            testsole: ""
        }
    };

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
                "Entity.Barcode": {
                    label: "图书条码号",
                    rules: [],
                    type: "input"
                },
                "Entity.Author": {
                    label: "作者",
                    rules: [],
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
    }

    onStatus() {
        this.status = this.status === "add" ? "detail" : "add";
    }
    onResetFields() {
        this.$refs.refName.resetFields();
    }

    testSubmit() {
        console.log("formData", this.$refs[this.refName]);
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
        console.log("diagremoteMethod");
        setTimeout(() => {
            const list: Array<any> = [];
            list.push({
                Text: "1111",
                Value: 11
            });
            list.push({
                Text: "222",
                Value: 22
            });
            this.diagremoteMethodData = list;
        }, 200);
    }
}
</script>
