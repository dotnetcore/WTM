<template>
  <a-layout id="components-layout-demo-responsive">
    <LayoutMenu />
    <a-layout>
      <a-layout-header :style="{ background: '#fff', padding: 0 }" />
      <a-layout-content :style="{ margin: '24px 16px 0' }">
        <!-- <transition name="fade" mode="out-in"> -->
        <router-view></router-view>
        <!-- </transition> -->
      </a-layout-content>
      <a-layout-footer style="textAlign: center">Ant Design ©2018 Created by Ant UED</a-layout-footer>
    </a-layout>
  </a-layout>
</template>
<script>
import LayoutMenu from "@/components/layout/menu.vue";
export default {
  components: {
    LayoutMenu
  },
};
</script>
<style lang="less">
#components-layout-demo-responsive {
  min-height: 100vh;
  .logo {
    height: 32px;
    background: rgba(255, 255, 255, 0.2);
    margin: 16px;
  }
}
</style>
