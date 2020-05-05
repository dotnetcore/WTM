<template>
  <wtm-dialog-box class="password-wrap" width="450px" :is-show.sync="isShow" @close="onClose" @onSubmit="onSubmit">
    <wtm-create-form ref="refForm" :options="formOptions" :sourceFormData="formData"></wtm-create-form>
  </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";

@Component({
    name: "password",
    components: {},
})
export default class extends Vue {
    isShow: Boolean = false;

    formData = {
        UserId: UserModule.info.Id,
        OldPassword: "",
        NewPassword: "",
        NewPasswordComfirm: "",
    };

    formOptions: Object = {
        formProps: {
            "label-width": "80px",
        },
        formItem: {
            UserId: {
                isHidden: true,
            },
            OldPassword: {
                label: "旧密码",
                type: "input",
                span: 24,
                props: {
                    "show-password": true,
                },
                rules: {
                    required: true,
                    message: "请输入旧密码",
                    trigger: "blur",
                },
            },
            NewPassword: {
                label: "新密码",
                type: "input",
                span: 24,
                props: {
                    "show-password": true,
                },
                rules: {
                    required: true,
                    message: "请输入新密码",
                    trigger: "blur",
                },
            },
            NewPasswordComfirm: {
                label: "确认密码",
                type: "input",
                span: 24,
                props: {
                    "show-password": true,
                },
                rules: {
                    required: true,
                    message: "请输入确认密码",
                    trigger: "blur",
                },
            },
        },
    };

    onOpen() {
        this.isShow = true;
    }
    onClose() {
        this.formData.OldPassword = "";
        this.formData.NewPassword = "";
        this.formData.NewPasswordComfirm = "";
        this.isShow = false;
    }
    onSubmit() {
        UserModule.ChangePassword(this.formData).then((res) => {
            if (res.error) {
                const ref = this.$refs["refForm"];
                _.mapKeys(res.data.Form, (value, key) => {
                    const formItem = ref.getFormItem(key);
                    if (formItem) {
                        formItem.showError(value);
                    }
                });
            } else {
                this["$notify"]({
                    title: "操作成功",
                    type: "success",
                });
                this.onClose();
            }
        });
    }
}
</script>

<style lang="less" scoped>
</style>
