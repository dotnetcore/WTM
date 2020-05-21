<template>
    <el-card class="app-login-form">
        <el-form :model="formData" label-width="0">
            <h1>WalkingTec MVVM</h1>
            <el-form-item>
                <el-input v-model="formData.userid" :placeholder="$t('login.pleaseEnterUsername')" prefix-icon="el-icon-user" />
            </el-form-item>
            <el-form-item>
                <el-input v-model="formData.password" @keyup.enter.native="onSubmit" :placeholder="$t('login.pleaseEnterPassword')" prefix-icon="el-icon-lock" show-password />
            </el-form-item>
            <el-form-item>
                <el-button class="submit-but" type="primary" :loading="isloading" :disabled="isDisabled" @click="onSubmit">
                    Log in
                </el-button>
            </el-form-item>
        </el-form>
    </el-card>
</template>
<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action } from "vuex-class";
import { setCookie } from "@/util/cookie";
import config from "@/config/index";

@Component({
    mixins: []
})
export default class Login extends Vue {
    @Action
    login;
    formData = {
        userid: config.development ? "admin" : "",
        password: config.development ? "000000" : ""
    };
    isloading: boolean = false;
    get isDisabled() {
        const { userid, password } = this.formData;
        return !(!!userid && !!password);
    }

    onSubmit() {
        this.isloading = true;
        this["login"](this.formData)
            .then(res => {
                this.isloading = false;
                setCookie(config.tokenKey, res.Id);
                location.href = "/index.html";
            })
            .catch(() => {
                this.isloading = false;
            });
    }
}
</script>
<style lang="less">
@import "~@/assets/css/mixin.less";
.app-login-form {
    padding: 40px;
    box-sizing: border-box;
    min-width: 500px;
    min-height: 400px;
    margin-right: 144px;
    & h1 {
        font-size: 50px;
        text-align: center;
        margin-bottom: 20px;
    }
    .submit-but {
        width: 100%;
    }
}
</style>
