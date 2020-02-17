<template>
  <a-layout class="app-layout">
    <LayoutMenu />
    <a-layout>
      <a-layout-header class="app-layout-header">
        <LayoutHeader />
      </a-layout-header>
      <a-layout-content class="app-layout-content">
        <router-view></router-view>
      </a-layout-content>
    </a-layout>
    <a-drawer placement="right" :closable="false" @close="onVisible(false)" :visible="visible"></a-drawer>
  </a-layout>
</template>
<script lang="ts">
import LayoutMenu from "./menu.vue";
import LayoutHeader from "./header.vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import rootStore from "../../rootStore";
import lodash from "lodash";
import Globalconfig from "../../global.config";
const fileService = Globalconfig.onCreateFileService();
@Component({
  components: {
    LayoutMenu,
    LayoutHeader
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  visible = false;
  onVisible(visible = !this.visible) {
    this.visible = visible;
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
.app-layout-content {
  margin: 8px;
}
</style>
