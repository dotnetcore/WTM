<template>
  <a-config-provider :locale="locale">
    <div class="w-user-spin" v-if="UserStore.LoginStatus && UserStore.loading">
      <a-spin size="large" />
    </div>
    <!-- 主界面 -->
    <Layout v-else-if="UserStore.LoginStatus" />
    <!-- 登录界面 -->
    <Login v-else />
  </a-config-provider>
</template>
<script lang="ts">
import { UserController } from "@/client";
import en from "ant-design-vue/es/locale/en_US";
import zh from "ant-design-vue/es/locale/zh_CN";
import { Options, Provide, Vue } from "vue-property-decorator";
import Layout from "./layouts/layout.vue";
import Login from "./layouts/login.vue";
@Options({ components: { Layout, Login } })
export default class extends Vue {
  // 用户
  @Provide({ reactive: true }) UserStore = new UserController();
  get locale() {
    return { en, zh }[this.$i18n.locale];
  }
  created() {
    this.UserStore.onInit()
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
}
</style>
