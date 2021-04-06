<template>
  <a-space>
    <a-button type="link" shape="circle" @click="System.UserController.onLogOut()">
      <template #icon>
        <SettingOutlined />
      </template>
    </a-button>
    <div>
      <a-avatar size="small">
        <template #icon>
          <UserOutlined />
        </template>
      </a-avatar>
      <span v-text="System.UserController.UserInfo.Name"></span>
    </div>
    <a-dropdown>
      <a class="ant-dropdown-link" @click.prevent>
        <span v-text="$i18n.locale"></span>
        <DownOutlined />
      </a>
      <template #overlay>
        <a-menu @click="changeLanguage">
          <a-menu-item v-for="item in languages" :key="item" v-text="item"></a-menu-item>
        </a-menu>
      </template>
    </a-dropdown>
  </a-space>
</template>
<script lang="ts">
import { SystemController } from "@/client";
import { Vue, Options, Inject } from "vue-property-decorator";
// Component definition
@Options({ components: {} })
export default class extends Vue {
  /**
   * 从 Aapp 中 注入 系统管理
   */
  @Inject({ from: SystemController.Symbol }) System: SystemController;
  get languages() {
    return this.lodash.keys(this.lodash.get(this.$i18n, "messages"));
  }
  changeLanguage(event) {
    this.$i18n.locale = event.key;
  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
