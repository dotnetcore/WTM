<template>
    <wtm-create-form :ref="refName" :status="'add'" :options="formOptions">

    </wtm-create-form>
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
        Pages: [],
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
            },
        };
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
