<template>
  <a-layout class="app-layout">
    <LayoutMenu />
    <a-layout>
      <a-layout-header class="app-layout-header">
        <a-row>
          <a-col :span="12"></a-col>
          <a-col :span="12" class="app-layout-user">
            <a-dropdown>
              <a href="javascript:;" class="user-item">
                <a-avatar src="https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png" />
                <span v-html="UserStore.Name"></span>
              </a>
              <a-menu slot="overlay">
                <a-menu-item>
                  <a href="javascript:;">1st menu item</a>
                </a-menu-item>
                <a-menu-item>
                  <a href="javascript:;">2nd menu item</a>
                </a-menu-item>
                <a-menu-item @click="UserStore.onOutLogin">
                  <a href="javascript:;">
                    <a-icon type="logout" /> 注销
                  </a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
            <a-divider type="vertical" />
            <a href="javascript:;" class="user-item">
              <a-icon type="setting" />
            </a>
            <a-divider type="vertical" />
            <a-dropdown>
              <a href="javascript:;" class="user-item">
                <a-icon type="global" />
              </a>
              <a-menu slot="overlay">
                <a-menu-item>
                  <a href="javascript:;">
                    <span v-html="languageIcons['zh-CN']"></span> 简体中文
                  </a>
                </a-menu-item>
                <a-menu-item>
                  <a href="javascript:;">
                     <span v-html="languageIcons['en-US']"></span> English
                  </a>
                </a-menu-item>
              </a-menu>
            </a-dropdown>
          </a-col>
        </a-row>
      </a-layout-header>
      <a-layout-content class="app-layout-content">
        <router-view></router-view>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>
<script lang="ts">
import LayoutMenu from "./menu.vue";
import { observable, action } from "mobx";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import rootStore from "../../rootStore";
@observer
@Component({
  components: {
    LayoutMenu
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  get languageIcons() {
    return {
      "zh-CN": "🇨🇳",
      "zh-TW": "🇭🇰",
      "en-US": "🇬🇧",
      "pt-BR": "🇧🇷"
    };
  }
}
</script>
<style lang="less">
.app-layout {
  min-height: 100vh;
  .logo {
    height: 32px;
    background: rgba(143, 57, 57, 0.2);
    margin: 16px;
  }
}
.app-layout-header {
  background: #fff;
  padding: 0 10px;
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
