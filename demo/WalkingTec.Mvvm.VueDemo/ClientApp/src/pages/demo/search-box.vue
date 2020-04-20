<template>
    <div>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" :sourceFormData="formData">
            <template #skey="data">
                <el-input-number v-model="formData.Entity.testsole" :min="1" :max="10"></el-input-number>
            </template>
        </wtm-search-box>
        <el-button @click="testSubmit">提交</el-button>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import searchMixin from "@/vue-custom/mixin/search";

@Component({
    mixins: [searchMixin()],
})
export default class Index extends Vue {
    formData = {
        Entity: {
            Name: "2",
            testsole: "3",
        },
    };

    // 表单结构
    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": "100px",
            },
            formItem: {
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
                "Entity.testsole": {
                    type: "wtmSlot",
                    label: "自定义",
                    slotKey: "skey",
                },
            },
        };
    }
    onResetFields() {
        this.$refs.searchRefName.resetFields();
    }

    testSubmit() {
        console.log(
            "formData",
            JSON.stringify(this.$refs.searchRefName.formData)
        );
    }
    privateRequest(prams) {
        return new Promise((resolve, reject) => {
            resolve([]);
        });
    }
}
</script>
