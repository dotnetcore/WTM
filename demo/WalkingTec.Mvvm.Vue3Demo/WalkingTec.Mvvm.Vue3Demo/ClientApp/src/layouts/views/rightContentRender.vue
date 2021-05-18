<template>
  <a-space>
    <a-button type="link" shape="circle" @click="System.UserController.onLogOut()">
      <template #icon>
        <SettingOutlined />
      </template>
    </a-button>
    <div>
      <a-avatar size="small" :src="getAvatar(System.UserController.UserInfo)">
        <!-- <template #icon>
          <UserOutlined />
        </template>-->
      </a-avatar>
      <a-divider type="vertical" style="margin: 0 3px;" />
      <span v-text="System.UserController.UserInfo.Name"></span>
    </div>
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
  created() { }
  mounted() { }
}
</script>
<style lang="less">
</style>
