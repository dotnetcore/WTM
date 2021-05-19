<template>
  <a-config-provider :locale="locale">
    <div class="w-user-spin" v-if="System.UserController.LoginStatus && System.UserController.loading">
      <!-- System.UserController.LoginStatus && System.UserController.loading -->
      <div>
        <img :src="logo" />
        <h1>WTM</h1>
        <a-spin size="large" />
      </div>
    </div>
    <!-- 主界面 -->
    <Layout v-else-if="System.UserController.LoginStatus" />
    <!-- 登录界面 -->
    <Login v-else />
  </a-config-provider>
</template>
<script lang="ts">
import { SystemController, $System } from "@/client";
import en from "ant-design-vue/es/locale/en_US";
import zh from "ant-design-vue/es/locale/zh_CN";
import { Options, Provide, Vue } from "vue-property-decorator";
import Layout from "./layouts/layout.vue";
import Login from "./layouts/login.vue";
@Options({ components: { Layout, Login } })
export default class extends Vue {
  // 系统管理
  @Provide({ to: SystemController.Symbol, reactive: true }) System = $System;
  get locale() {
    return { en, zh }[this.$i18n.locale];
  }
  get logo() {
    return require('@/assets/img/logo.png')
  }
  created() {
    this.System.onInit()
    console.log("LENG ~ extends ~ created ~ this.System", this.System)
  }
  mounted() {
  }
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
