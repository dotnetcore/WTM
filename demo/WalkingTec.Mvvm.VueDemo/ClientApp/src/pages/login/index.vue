<template>
  <el-card class="app-login-form">
    <el-form :model="formData" label-width="0">
      <h1>WalkingTec MVVM</h1>
      <el-form-item>
        <el-input
          v-model="formData.account"
          :placeholder="$t('login.pleaseEnterUsername')"
          prefix-icon="el-icon-user"
        />
      </el-form-item>
      <el-form-item>
        <el-input
          v-model="formData.password"
          @keyup.enter.native="onSubmit"
          :placeholder="$t('login.pleaseEnterPassword')"
          prefix-icon="el-icon-lock"
          show-password
        />
      </el-form-item>
      <el-form-item>
        <el-input
          v-model="formData.tenant"
          @keyup.enter.native="onSubmit"
          :placeholder="$t('login.pleaseEnterTenant')"
          prefix-icon="el-icon-user"
        />
      </el-form-item>
      <el-form-item>
        <el-button
          class="submit-but"
          type="primary"
          :loading="isloading"
          :disabled="isDisabled"
          @click="onSubmit"
        >
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
  @Action
  loginRemote;

  formData = {
    account: config.development ? "admin" : "",
    password: config.development ? "000000" : "",
    tenant: ""
  };
  isloading: boolean = false;
  redirect: string = "";
  get isDisabled() {
    const { account, password } = this.formData;
    return !(!!account && !!password);
  }

  mounted() {
    const searchParams = new URL(location.href).searchParams;
    const remotetoken = searchParams.get("_remotetoken");
    this.redirect = searchParams.get("redirect");
    if (remotetoken) {
      this.checkLogin({ _remotetoken: remotetoken, redirect: this.redirect });
    }
  }

  onSubmit() {
    this.isloading = true;
    this["login"](this.formData)
      .then(res => {
        this.isloading = false;
        setCookie(config.tokenKey, res.Id);
        location.href = `/index.html#/?redirect=${this.redirect}`;
      })
      .catch(() => {
        this.isloading = false;
      });
  }

  checkLogin(data) {
    this["loginRemote"](data).then(res => {
      setCookie(config.tokenKey, res.Id);
      location.href = `/index.html#/?redirect=${this.redirect}`;
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
