<template>
  <a-config-provider :locale="locale">
    <keep-alive>
    <div
      class="w-user-spin"
      v-if="System.UserController.loading && System.UserController.LoginStatus"
    >
      <!-- System.UserController.LoginStatus && System.UserController.loading -->
      <div>
        <img :src="logo" />
        <h1>WTM</h1>
        <a-spin size="large" />
      </div>
    </div>
    <!-- 主界面 -->
    <Main key="Main" v-else-if="!System.UserController.loading && System.UserController.LoginStatus" />
    <!-- 登录界面 -->
    <Login key="Login" v-else/>
  </keep-alive>
  </a-config-provider>
</template>
<script lang="ts">
import { SystemController, $System } from "@/client";
import en from "ant-design-vue/es/locale/en_US";
import zh from "ant-design-vue/es/locale/zh_CN";
import { Options, Provide, Vue } from "vue-property-decorator";
import Main from "./layouts/main.vue";
import Login from "./layouts/login.vue";
import router from "./router";
@Options({ components: { Main, Login } })
export default class extends Vue {
  // 系统管理
  @Provide({ to: SystemController.Symbol, reactive: true }) System = $System;
  get locale() {
    return { en, zh }[this.$i18n.locale];
  }
  get logo() {
    return require("@/assets/img/logo.png");
  }
  async created() {
    await this.System.onInit();
    // router.onInit();
    // console.error("LENG ~ extends ~ created ~ this.System",this);
  }
  mounted() {}
}
</script>
<style lang="less">
.w-app {
  height: 100%;
}
.w-user-spin {
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
  img {
    width: 100px;
  }
}
</style>
