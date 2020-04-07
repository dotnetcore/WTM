<template>
  <a-layout class="app-layout-login">
    <a-layout :class="sampleClass">
      <a-layout-header>
        <img class="app-login-logo" :src="imgs.logo" alt="" height="48" />
      </a-layout-header>
      <a-layout-content>
        <div class="app-login-form">
          <a-form :form="form" @submit="handleSubmit">
            <h1 v-text="$GlobalConfig.settings.title"></h1>
            <a-form-item>
              <a-input
                size="large"
                v-decorator="[
                  'userName',
                  {
                    rules: [
                      { required: true, message: 'Please input your username!' }
                    ],
                    initialValue: 'admin'
                  }
                ]"
                placeholder="Username"
              >
                <a-icon
                  slot="prefix"
                  type="user"
                  style="color: rgba(0,0,0,.25)"
                />
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-input
                size="large"
                v-decorator="[
                  'password',
                  {
                    rules: [
                      { required: true, message: 'Please input your Password!' }
                    ],
                    initialValue: '000000'
                  }
                ]"
                type="password"
                placeholder="Password"
              >
                <a-icon
                  slot="prefix"
                  type="lock"
                  style="color: rgba(0,0,0,.25)"
                />
              </a-input>
            </a-form-item>
            <a-form-item>
              <a-button
                :loading="UserStore.Loading"
                type="primary"
                html-type="submit"
                block
                size="large"
                class="login-form-button"
                >Log in</a-button
              >
            </a-form-item>
          </a-form>
        </div>
      </a-layout-content>
    </a-layout>
    <a-layout-footer>
      <a-row class=" app-login-links">
        <a-col class="ant-typography" :lg="18" :md="24">
          <h1>Quick Links</h1>
          <a-row>
            <a-col
              v-for="value in links"
              :key="value.name"
              :lg="6"
              :md="8"
              :xs="8"
              :sm="12"
            >
              <a :href="value.url" v-text="value.name" target="_blank"> </a>
            </a-col>
          </a-row>
        </a-col>
        <a-col :lg="6" :md="24">
          <img :src="imgs.code" alt="" width="88" height="88" />
        </a-col>
      </a-row>
      <div class="app-login-record">
        <a href="https://wtmdoc.walkingtec.cn/" target="_blank"> Help</a> © 2019
        WTM all rights reserved
      </div>
    </a-layout-footer>
  </a-layout>
</template>
<script lang="ts">
import lodash from "lodash";
import rootStore from "../../../rootStore";
import { Component, Prop, Vue } from "vue-property-decorator";
import code from "./code.png";
import logo from "../../../assets/img/logo.png";
// @observer
@Component
export default class Login extends Vue {
  UserStore = rootStore.UserStore;
  form;
  imgs = {
    code,
    logo
  };
  links = [
    { name: "GitHub", url: "https://github.com/dotnetcore/WTM" },
    { name: "Vue", url: "https://cn.vuejs.org/" },
    {
      name: "Ant Design of Vue",
      url: "https://www.antdv.com/docs/vue/introduce-cn/"
    },
    { name: "Rxjs", url: "https://rxjs.dev/" },
    { name: "Mobx", url: "https://mobx.js.org/" },
    { name: "Lodash", url: " https://lodash.com/" }
  ];
  get sampleClass() {
    return `app-login-back-${lodash.sample([1, 2, 3, 4, 5])}`;
  }
  beforeCreate() {
    this.form = this.$form.createForm(this, {});
  }
  handleSubmit(e) {
    e.preventDefault();
    this.form.validateFields(async (err, values) => {
      if (!err) {
        try {
          await this.UserStore.onLogin(values.userName, values.password);
        } catch (error) {
          this.$notification.error({
            key: "Login",
            message: "Login Error",
            description: error.message
          });
        }
      }
    });
  }
}
</script>
<style lang="less">
@import "./style.less";
</style>
<style scoped lang="less">
.app-login {
  min-height: 100vh;
}
.app-login-form {
  h1 {
    font-size: 50px;
    text-align: center;
    margin-bottom: 20px;
  }
}
.app-login-imgcode {
  .ant-popover-message-title {
    padding: 0;
  }
  .ant-popover-buttons {
    display: none;
  }
}
</style>
