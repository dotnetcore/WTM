<template>
  <div class="verification" :class="{'disable': disable}" @click="onSend">
    {{ msg }}
  </div>
</template>
<script lang="ts">
import { Action } from "vuex-class";
import { Component, Vue, Prop } from "vue-property-decorator";
import validator from "@/util/validator";

@Component
export default class Verification extends Vue {
    @Prop({ type: String, default: "" })
    mobile;
    disable: boolean = false;
    msg: string = "获取验证码";
    seconds: number = 60;
    @Action postVerificationCode;
    // 发送验证码
    onSend() {
        if (this.disable) {
            return false;
        }
        if (!validator.validateMobile(this.mobile)) {
            // this.message('请写入正确的手机号', 1500)
            this.$emit("onError", "请写入正确的手机号");
        } else {
            // 验证码倒计时
            this.disable = true;
            this.seconds = 60;
            this.msg = `${this.seconds}s重发`;
            const timeout = setInterval(() => {
                if (this.seconds !== 0) {
                    this.seconds--;
                    this.msg = `${this.seconds}s重发`;
                } else {
                    this.disable = false;
                    this.msg = "获取验证码";
                    clearInterval(timeout);
                }
            }, 1000);
            this["postVerificationCode"](this.verificationCode).then(res => {
                console.log(res);
            });
        }
    }

    // 验证码参数
    get verificationCode() {
        return {
            country_code: "86",
            mobile: this.mobile,
            classifier: "login",
            captcha_type: null,
            captcha_ticket: null
        };
    }
}
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
@import "~@/assets/css/mixin.less";
// vwUnit
.verification {
    width: 140vw * @vwUnit;
    height: 40vw * @vwUnit;
    border-radius: 21vw * @vwUnit;
    border: 1px solid rgba(255, 255, 255, 1);
    color: #fff;
    text-align: center;
    cursor: pointer;
    .center();
    &.disable {
        color: #909399;
        border: 1px solid rgba(144, 147, 153, 1);
    }
}
</style>
