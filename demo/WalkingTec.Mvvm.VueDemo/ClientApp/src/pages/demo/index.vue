<template>
    <div>
        <el-tabs type="border-card">
            <el-tab-pane label="wtm-create-form">
                <create-form />
            </el-tab-pane>
            <el-tab-pane label="search-box">
                <search-box />
            </el-tab-pane>
            <el-tab-pane label="角色管理">角色管理</el-tab-pane>
            <el-tab-pane label="定时任务补偿">定时任务补偿</el-tab-pane>
        </el-tabs>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";
import CreateForm from "./create-form.vue";
import SearchBox from "./search-box.vue";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import searchMixin from "@/vue-custom/mixin/search";
import store from "../actionlog/store/index";

@Component({
    name: "demo",
    mixins: [searchMixin(), actionMixin()],
    store,
    components: { CreateForm, SearchBox },
})
export default class extends Vue {
    form = {};
    status = "add";
    num = 2;
    dialogIsShows = false;
    // 表单结构
    get formOptions() {
        return {
            formProps: {
                "label-width": "100px",
            },
            formItem: {
                "Entity.ID": {
                    isHidden: true,
                },
                "Entity.Code": {
                    label: "层位编码",
                    rules: [
                        {
                            required: true,
                            message: "层位编码不能为空",
                            trigger: "blur",
                        },
                    ],
                    type: "input",
                },
                "Entity.LayerTrans": {
                    label: "层位翻译",
                    rules: [],
                    type: "input",
                },
                "Entity.LibraryStructId": {
                    label: "所属单位",
                    rules: [
                        {
                            required: true,
                            message: "所属单位不能为空",
                            trigger: "blur",
                        },
                    ],
                    type: "select",
                    children: [
                        {
                            Text: "天津2",
                            Value: "tianjin2",
                        },
                        {
                            Text: "深圳2",
                            Value: "shenzhen2",
                            disabled: true,
                        },
                        {
                            Text: "韶关2",
                            Value: "shaoguan2",
                        },
                    ],
                    props: {
                        clearable: true,
                    },
                },
            },
        };
    }

    treeData = [
        {
            label: "天津2",
            value: "tianjin2",
            children: [
                {
                    label: "深圳2",
                    value: "shenzhen2",
                    children: [
                        {
                            label: "韶关2",
                            value: "shaoguan2",
                        },
                    ],
                },
            ],
        },
    ];
    testValue = "";
    private defaultProps = {
        children: "children",
        label: "label",
    };

    options = {
        // 表单属性 非必需
        formProps: {
            "label-width": "80px",
        },
        // 表单项组件数据 必需
        formItem: {
            accout: {
                type: "input",
                label: "账号",
                span: 24,
                props: {
                    placeholder: "请输入账号...",
                    "suffix-icon": "el-icon-date",
                },
                rules: {
                    required: true,
                    message: "请输入账号",
                    trigger: "blur",
                },
            },
            pwd: {
                type: "input",
                label: "密码",
                props: {
                    placeholder: "请输入密码...",
                    "show-password": true,
                },
                rules: {
                    required: true,
                    message: "请输入密码",
                    trigger: "blur",
                },
            },
            city: {
                type: "select",
                label: "城市",
                children: [
                    {
                        Text: "天津",
                        Value: "tianjin",
                    },
                    {
                        Text: "深圳",
                        Value: "shenzhen",
                        disabled: true,
                    },
                    {
                        Text: "韶关",
                        Value: "shaoguan",
                    },
                    {
                        Text: "清空",
                        Value: "shaoguan2",
                    },
                ],
                events: {
                    change: this.onChange,
                },
                defaultValue: "shenzhen",
            },
            citys: {
                type: "select",
                label: "城市二级",
                children: [],
            },
            gender: {
                type: "radioGroup",
                label: "性别",
                children: [
                    {
                        Value: 0,
                        Text: "男",
                    },
                    {
                        Value: 1,
                        Text: "女",
                    },
                ],
                rules: {
                    required: true,
                    message: "请选择性别",
                    trigger: "change",
                },
            },
            phone: {
                type: "input",
                label: "手机",
                props: {
                    placeholder: "请输入手机号...",
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
                    },
                },
            },
            msg: {
                type: "input",
                label: "留言",
                props: {
                    placeholder: "留言...",
                    type: "textarea",
                },
                rules: {
                    required: true,
                    trigger: "blur",
                    message: "请留下您宝贵的意见",
                },
            },
            imgid: {
                type: "wtmUploadImg",
                label: "上传图片",
            },
            testsole: {
                type: "wtmSlot",
                label: "自定义",
                slotKey: "skey",
            },
            uploadId: {
                type: "upload",
                label: "上传文件",
            },
            selectone: {
                type: "select",
                label: "自定义",
                children: [
                    {
                        Text: "天津",
                        Value: "tianjin",
                        slot: `<span style="float: left">{Text}</span>-
                        <span style="float: right; color: #8492a6; font-size: 13px">{Value}</span>`,
                    },
                    {
                        Text: "上海",
                        Value: "shaoguan2",
                        slot: "<span>上海44</span>",
                    },
                ],
            },
        },
    };
    created() {}
    onTest() {
        this.status = this.status === "add" ? "detail" : "add";
    }
    onResetFields() {
        this.$refs.wForm.resetFields();
    }
    testSubmit() {
        console.log("formData", JSON.stringify(this.$refs.wForm.formData));
        console.log("sourceFormData", JSON.stringify(this.sourceFormData));
    }
    testFormItem() {
        const item = this.$refs.wForm.getFormItem("accout");
        item.showError("错误信息");
    }
    onChange(e) {
        if (e === "qingkong") {
            this.options.formItem["citys"].children = [];
        } else {
            this.options.formItem["citys"].children = [
                {
                    Text: "天津2",
                    Value: "tianjin2",
                },
                {
                    Text: "深圳2",
                    Value: "shenzhen2",
                    disabled: true,
                },
                {
                    Text: "韶关2",
                    Value: "shaoguan2",
                },
            ];
        }
    }
    onAdd() {
        console.log("this.dialogIsShow", this.dialogIsShows);
        this.dialogIsShows = true;
    }
    //树型点选触发
    private handleNodeClick(data) {
        this.testValue = data.value;
    }
}
</script>

<style lang="less" scoped></style>
