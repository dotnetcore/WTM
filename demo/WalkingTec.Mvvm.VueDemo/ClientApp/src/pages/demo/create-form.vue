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
    mixins: [formMixin()],
})
export default class Index extends Vue {
    mergeFormData = {
        Entity: {
            testsole: "",
        },
    };

    // 表单结构
    get formOptions() {
        const filterMethod = (query, item) => {
            return item.label.indexOf(query) > -1;
        };

        return {
            formProps: {
                "label-width": "100px",
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true,
                },
                "Entity.Name": {
                    label: "图书名称",
                    rules: [
                        {
                            required: true,
                            message: "图书名称不能为空",
                            trigger: "blur",
                        },
                    ],
                    type: "input",
                },
                "Entity.Barcode": {
                    label: "图书条码号",
                    rules: [],
                    type: "input",
                },
                "Entity.Author": {
                    label: "作者",
                    rules: [],
                    type: "input",
                },
                "Entity.Price": {
                    label: "价格",
                    rules: [
                        {
                            required: true,
                            message: "价格不能为空",
                            trigger: "blur",
                        },
                    ],
                    type: "input",
                },
                "Entity.BookIndex": {
                    label: "索书号",
                    rules: [],
                    type: "input",
                },
                "Entity.testsole": {
                    type: "wtmSlot",
                    label: "自定义",
                    slotKey: "skey",
                },
            },
        };
    }

    onStatus() {
        this.status = this.status === "add" ? "detail" : "add";
    }
    onResetFields() {
        this.$refs.refName.resetFields();
    }

    testSubmit() {
        console.log("formData", JSON.stringify(this.$refs.refName.formData));
        // console.log("sourceFormData", JSON.stringify(this.sourceFormData));
    }

    mounted() {
        this.setFormData({
            Entity: {
                ID: "212121212",
                Name: "212121212",
                Barcode: "212121212",
                Author: "212121212",
                Price: "212121212",
                BookIndex: "32323",
                SchoolType: 1,
            },
        });
    }
}
</script>
