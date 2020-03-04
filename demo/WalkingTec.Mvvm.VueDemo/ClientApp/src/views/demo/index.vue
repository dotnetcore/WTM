<template>
    <div>

        json创建form:
        <el-button type="primary" @click="onTest">onTest</el-button>
        <wtm-create-form :status="status" :options="options" />

    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";

@Component({
    name: "demo",
    components: {}
})
export default class extends Vue {
    form = {};
    status = "add";

    options = {
        // 表单属性 非必需
        formProps: {
            "label-width": "80px"
        },
        // 表单数据 必需
        formData: {
            accout: "",
            pwd: "",
            mail: "",
            gender: "",
            msg: "",
            phone: ""
        },
        // 表单项组件数据 必需
        formItem: [
            {
                type: "input",
                key: "accout",
                label: "账号",
                props: {
                    placeholder: "请输入账号..."
                },
                rules: {
                    required: true,
                    message: "请输入账号",
                    trigger: "blur"
                }
            },
            {
                type: "input",
                key: "pwd",
                label: "密码",
                props: {
                    placeholder: "请输入密码...",
                    type: "password"
                },
                rules: {
                    required: true,
                    message: "请输入密码",
                    trigger: "blur"
                }
            },
            {
                type: "input",
                key: "mail",
                label: "邮箱",
                props: {
                    placeholder: "请输入邮箱..."
                },
                rules: {
                    required: true,
                    message: "请输入邮箱",
                    trigger: "blur"
                }
            },
            {
                type: "radioGroup",
                key: "gender",
                label: "性别",
                children: [
                    {
                        label: "女"
                    },
                    {
                        label: "男"
                    }
                ],
                rules: {
                    required: true,
                    message: "请选择性别",
                    trigger: "change"
                }
            },
            {
                type: "input",
                key: "phone",
                label: "手机",
                props: {
                    placeholder: "请输入手机号..."
                },
                rules: {
                    required: true,
                    trigger: "blur",
                    validator(rule, value, callback) {
                        if (value === "") {
                            callback(new Error("请输入手机号"));
                        } else if (value.length != 11) {
                            callback(new Error("请输入正确的手机号"));
                        } else {
                            callback();
                        }
                    }
                }
            },
            {
                type: "input",
                label: "留言",
                key: "msg",
                props: {
                    placeholder: "留言...",
                    type: "textarea"
                },
                rules: {
                    required: true,
                    trigger: "blur",
                    message: "请留下您宝贵的意见"
                }
            }
        ]
    };
    created() {}
    onTest() {
        this.status = this.status === "add" ? "aaa" : "add";
    }
}
</script>

<style lang="less" scoped>
</style>
