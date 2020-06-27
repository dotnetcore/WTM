<template>
    <wtm-dialog-box class="password-wrap" width="600px" :is-show.sync="isShow" @close="onClose" @onSubmit="onSubmit">
        <wtm-create-form ref="refForm" :options="formOptions" :sourceFormData="formData"></wtm-create-form>
    </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";

@Component({
    name: "password",
    components: {}
})
export default class extends Vue {
    isShow: Boolean = false;

    formData = {
        UserId: UserModule.info.Id,
        OldPassword: "",
        NewPassword: "",
        NewPasswordComfirm: ""
    };

    get formOptions() {
        return {
            formProps: {
                "label-width": "180px"
            },
            formItem: {
                UserId: {
                    isHidden: true
                },
                OldPassword: {
                    label: this.$t("navbar.oldPassword"),
                    type: "input",
                    span: 24,
                    props: {
                        "show-password": true
                    },
                    rules: {
                        required: true,
                        message: this.$t("navbar.pleaseEnterOldPassword"),
                        trigger: "blur"
                    }
                },
                NewPassword: {
                    label: this.$t("navbar.newPassword"),
                    type: "input",
                    span: 24,
                    props: {
                        "show-password": true
                    },
                    rules: {
                        required: true,
                        message: this.$t("navbar.pleaseEnterNewPassword"),
                        trigger: "blur"
                    }
                },
                NewPasswordComfirm: {
                    label: this.$t("navbar.confirmNewPassword"),
                    type: "input",
                    span: 24,
                    props: {
                        "show-password": true
                    },
                    rules: {
                        required: true,
                        message: this.$t(
                            "navbar.pleaseEnterConfirmNewPassword"
                        ),
                        trigger: "blur"
                    }
                }
            }
        };
    }

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
        UserModule.ChangePassword(this.formData).then(res => {
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
                    type: "success"
                });
                this.onClose();
            }
        });
    }
}
</script>

<style lang="less" scoped>
</style>
