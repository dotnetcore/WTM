<template>
  <a-layout class="app-layout">
    <LayoutMenu />
    <a-layout>
      <LayoutHeader />
      <LayoutContent />
    </a-layout>
    <a-drawer
      placement="right"
      :closable="false"
      @close="onVisible(false)"
      :visible="visible"
    ></a-drawer>
  </a-layout>
</template>
<script lang="ts">
import LayoutMenu from "./menu.vue";
import LayoutContent from "./content.vue";
import LayoutHeader from "./header.vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import rootStore from "../../rootStore";
import lodash from "lodash";
@Component({
  components: {
    LayoutMenu,
    LayoutHeader,
    LayoutContent
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
  > .ant-layout-sider {
    position: fixed;
    top: 0;
    left: 0;
    min-height: 100vh;
    z-index: 10;
  }
  > .ant-layout {
    padding-left: 200px;
    @media (max-width: 992px) {
      padding-left: 0;
    }
  }
  .app-layout-header {
    position: fixed;
    top: 0;
    right: 0;
    // width: calc(100% - 200px);
    left: 200px;
    @media (max-width: 992px) {
      left: 0;
    }
  }
  .app-layout-content {
    padding-top: 104px;
  }
  .layout-tabs > .ant-tabs-bar {
    position: fixed;
    right: 0;
    top: 64px;
    z-index: 7;
    left: 200px;
    @media (max-width: 992px) {
      left: 0;
    }
  }
}
</style>
