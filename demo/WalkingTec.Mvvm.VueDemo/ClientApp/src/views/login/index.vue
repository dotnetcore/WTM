<template>
  <div class="app-login-form">
    <h1>WalkingTec MVVM</h1>
    <el-input v-model="formData.username" class="form-item" placeholder="请输入账号" prefix-icon="el-icon-user" size="mini" />
    <el-input v-model="formData.password" class="form-item" placeholder="请输入密码" prefix-icon="el-icon-lock" size="mini" show-password />
    <el-button class="form-item" size="mini" type="primary" :loading="isloading" @click="onSubmit">
      Log in
    </el-button>
  </div>
</template>
<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import cache from "@/util/cache";
import config from "@/config/index";
import baseMixin from "@/mixin/base";

@Component({
    mixins: [baseMixin]
})
export default class Login extends Vue {
    formData = {
        username: "admin",
        password: "000000"
    };
    isloading: boolean = false;
    // 登陆
    @Action postLogin;

    onSubmit() {
        this.isloading = true;
        const params = {
            userid: this.formData.username,
            password: this.formData.password
        };
        this["postLogin"](params)
            .then(res => {
                this.isloading = false;
                cache.setStorage(config.tokenKey, res);
                this["onHref"]("/index.html");
            })
            .catch(() => {
                this.isloading = false;
            });
    }
}
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
@import "~@/assets/css/mixin.less";
// vwUnit
.app-login-form {
    color: #fff;
    .center();
    flex-direction: column;
    .login-header-logo {
        width: 179vw * @vwUnit;
        height: 61vw * @vwUnit;
    }
    & > h1 {
        font-size: 40px;
        margin-top: 30vw * @vwUnit;
        font-weight: 300;
        color: rgba(255, 255, 255, 1);
        letter-spacing: 8vw * @vwUnit;
        & > span {
            font-weight: 600;
        }
    }
    & > h4 {
        font-size: 20px;
        margin-top: 1vw * @vwUnit;
        font-weight: 300;
    }
    .form-item {
        width: 390vw * @vwUnit;
        margin-top: 30vw * @vwUnit;
        &.item-verificationCode {
            margin-top: 40vw * @vwUnit;
            display: flex;
            .verific {
                margin-left: 8vw * @vwUnit;
                align-self: flex-end;
            }
        }
    }
    .submit-but {
        margin-top: 40vw * @vwUnit;
        width: 210vw * @vwUnit;
        height: 50vw * @vwUnit;
        border-radius: 25vw * @vwUnit;
        background: #fff;
        color: #000000;
        font-size: 16px;
        font-weight: 400;
    }
}
</style>
