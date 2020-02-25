<template>
      <a-layout-header class="app-layout-header">
        <a-row>
          <a-col :span="12">
          </a-col>
          <a-col :span="12" class="app-layout-user">
            <a-dropdown>
              <a href="javascript:;" class="user-item">
                <a-avatar :src="UserStore.Avatar" />
                <a-divider type="vertical" />
                <span v-html="UserStore.Name"></span>
              </a>
              <a-menu slot="overlay">
                <a-menu-item>
                  <a href="/_codegen?ui=react" target="_blank">
                    <a-icon type="code" />
                    <a-divider type="vertical" />
                    <span v-t="'action.user.codeGenerator'"></span>
                  </a>
                </a-menu-item>
                <a-menu-item>
                  <a href="/swagger" target="_blank">
                    <a-icon type="bars" />
                    <a-divider type="vertical" />
                    <span v-t="'action.user.apiDocument'"></span>
                  </a>
                </a-menu-item>
                <a-menu-item @click="UserStore.onOutLogin">
                  <a href="javascript:;">
                    <a-icon type="logout" />
                    <a-divider type="vertical" />
                    <span v-t="'action.user.logout'"></span>
                  </a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
            <!-- <a-divider type="vertical" />
                  <a href="javascript:;" class="user-item" @click="onVisible(true)">
                    <a-icon type="setting" />
            </a>-->
            <a-divider type="vertical" />
            <a-dropdown>
              <a href="javascript:;" class="user-item">
                <a-icon type="global" />
              </a>
              <a-menu @click="onLanguage" :selectedKeys="[$i18n.locale]" slot="overlay">
                <a-menu-item key="zh-CN">
                  <a href="javascript:;">
                    <span v-html="languageIcons['zh-CN']"></span>
                    <a-divider type="vertical" />简体中文
                  </a>
                </a-menu-item>
                <a-menu-item key="en-US">
                  <a href="javascript:;">
                    <span v-html="languageIcons['en-US']"></span>
                    <a-divider type="vertical" />English
                  </a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </a-col>
        </a-row>
      </a-layout-header>
</template>
<script lang="ts">
import LayoutMenu from "./menu.vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import rootStore from "../../rootStore";
import lodash from "lodash";
@Component({
  components: {
    LayoutMenu
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  // get src() {
  //   const src = lodash.get(this.UserStore.Avatar, "params.value");
  //   if (src) {
  //     return `${fileService.fileGet.src}/${src}`;
  //   }
  //   return "https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png";
  //   // return ''
  // }
  get languageIcons() {
    return {
      "zh-CN": "🇨🇳",
      "zh-TW": "🇭🇰",
      "en-US": "🇬🇧",
      "pt-BR": "🇧🇷"
    };
  }
  onLanguage({ item, key, keyPath }) {
    this.$i18n.locale = key;
    this.$GlobalConfig.onSetLanguage(key);
  }
}
</script>
<style lang="less">
.app-layout-header {
  background: #fff;
  padding: 0 10px;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
      position: relative;
    z-index: 8;
}
.app-layout-content {
  margin: 8px;
}
.app-layout-user {
  text-align: right;
  .user-item {
    display: inline-block;
    color: rgba(0, 0, 0, 0.65);
    outline-style: none;
  }
}
</style>
