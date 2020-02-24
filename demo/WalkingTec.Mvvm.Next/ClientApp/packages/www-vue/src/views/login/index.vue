<template>
  <div class="login-form">
    <a-form :form="form" @submit="handleSubmit">
      <a-form-item>
        <a-input
          v-decorator="[
          'userName',
          { rules: [{ required: true, message: 'Please input your username!' }],initialValue:'admin' },
        ]"
          placeholder="Username"
        >
          <a-icon slot="prefix" type="user" style="color: rgba(0,0,0,.25)" />
        </a-input>
      </a-form-item>
      <a-form-item>
        <a-input
          v-decorator="[
          'password',
          { rules: [{ required: true, message: 'Please input your Password!' }],initialValue:'000000' },
        ]"
          type="password"
          placeholder="Password"
        >
          <a-icon slot="prefix" type="lock" style="color: rgba(0,0,0,.25)" />
        </a-input>
      </a-form-item>
      <a-form-item>
        <a-button
          :loading="UserStore.Loading"
          type="primary"
          html-type="submit"
          class="login-form-button"
        >Log in</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>
<script lang="ts">
// import { observer } from "mobx-vue";
import rootStore from "../../rootStore";
import { Component, Prop, Vue } from "vue-property-decorator";
// @observer
@Component
export default class Login extends Vue {
  UserStore = rootStore.UserStore;
  form;
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
<style scoped lang="less">
.login-form {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  .ant-form {
    width: 300px;
  }
}
.login-form-forgot {
  float: right;
}
.login-form-button {
  width: 100%;
}
</style>