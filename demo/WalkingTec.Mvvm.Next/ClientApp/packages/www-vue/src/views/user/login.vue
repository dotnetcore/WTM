<template>
  <a-form
    id="components-form-demo-normal-login"
    :form="form"
    class="login-form"
    @submit="handleSubmit"
  >
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
      <a-button :loading="UserStore.Loading" type="primary" html-type="submit" class="login-form-button">Log in</a-button>
    </a-form-item>
  </a-form>
</template>
<script>
import { observer } from "mobx-vue";
import UserStore from "./user";
import { Component, Prop, Vue } from "vue-property-decorator";
@observer
@Component
export default class HelloWorld extends Vue {
  UserStore = UserStore;
  beforeCreate() {
    this.form = this.$form.createForm(this, {});
  }
  handleSubmit(e) {
    e.preventDefault();
    this.form.validateFields((err, values) => {
      if (!err) {
        UserStore.onLogin(values.userName, values.password);
      }
    });
  }
}
</script>
<style>
#components-form-demo-normal-login.login-form {
  max-width: 300px;
}
#components-form-demo-normal-login .login-form-forgot {
  float: right;
}
#components-form-demo-normal-login .login-form-button {
  width: 100%;
}
</style>