<template>
  <a-space>
    <!-- 设置 -->
    <div>
      <a-button type="link" shape="circle">
        <template #icon>
          <SettingOutlined />
        </template>
      </a-button>
    </div>
    <!-- 用户菜单 -->
    <a-dropdown>
      <div>
        <a-avatar size="small" :src="getAvatar(System.UserController.UserInfo)" />
        <a-divider type="vertical" style="margin: 0 3px;" />
        <span v-text="System.UserController.UserInfo.Name"></span>
      </div>
      <template #overlay>
        <a-menu @click="changeUserMenu">
          <a-menu-item :key="$locales.action_user_codeGenerator">
            <i18n-t :keypath="$locales.action_user_codeGenerator" />
          </a-menu-item>
          <a-menu-item :key="$locales.action_user_apiDocument">
            <i18n-t :keypath="$locales.action_user_apiDocument" />
          </a-menu-item>
          <a-menu-item :key="$locales.action_user_changePassword">
            <i18n-t :keypath="$locales.action_user_changePassword" />
          </a-menu-item>
          <a-menu-item :key="$locales.action_user_logout">
            <i18n-t :keypath="$locales.action_user_logout" />
          </a-menu-item>
        </a-menu>
      </template>
    </a-dropdown>
    <!-- 多语言 -->
    <a-dropdown>
      <a class="ant-dropdown-link" @click.prevent>
        <GlobalOutlined />
        <a-divider type="vertical" style="margin: 0 3px;" />
        <span v-text="getlanguageLabel($i18n.locale)"></span>
        <DownOutlined />
      </a>
      <template #overlay>
        <a-menu @click="changeLanguage">
          <a-menu-item v-for="item in languages" :key="item.key">
            <span v-text="item.icon"></span>
            <a-divider type="vertical" style="margin: 0 3px;" />
            <span v-text="item.text"></span>
          </a-menu-item>
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
    const languageIcons = {
      'zh': '🇨🇳',
      // 'zh-TW': '🇭🇰',
      'en': '🇬🇧',
      // 'pt-BR': '🇧🇷',
    };
    return this.lodash.keys(this.lodash.get(this.$i18n, "messages")).map(x => {
      return { key: x, text: this.getlanguageLabel(x), icon: this.lodash.get(languageIcons, x) }
    });
  }
  getlanguageLabel(key) {
    const languageLabels = {
      'zh': '简体中文',
      // 'zh-TW': '繁体中文',
      'en': 'English',
      // 'pt-BR': 'Português',
    };
    return this.lodash.get(languageLabels, key)
  }
  getAvatar(Info) {
    return Info.PhotoId ? this.System.FilesController.getDownloadUrl(Info.PhotoId) : require('@/assets/img/user.png')
  }
  changeLanguage(event) {
    this.$i18n.locale = event.key;
    localStorage.setItem('locale', event.key);
    this.System.UserController.onCheckLogin()
    // 触发事件
    // dispatchEvent(new CustomEvent('languages'));
    // window.location.reload()
  }
  changeUserMenu(event) {
    switch (event.key) {
      case this.$locales.action_user_codeGenerator:
        this.$router.push({ name: 'webview', query: { src: encodeURIComponent('http://localhost:8598/_codegen?ui=vue3'), name: this.$locales.action_user_codeGenerator } })
        break;
      case this.$locales.action_user_apiDocument:
        this.$router.push({ name: 'webview', query: { src: encodeURIComponent('http://localhost:8598/swagger/index.html'), name: this.$locales.action_user_apiDocument } })
        break;
      case this.$locales.action_user_changePassword:
        this.$router.replace({ query: { changePassword: '' } })
        break;
      case this.$locales.action_user_logout:
        this.System.UserController.onLogOut()
        break;
    }
  }
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
