<template>
  <a-layout id="components-layout-demo-responsive">
    <template v-if="UserStore.OnlineState">
      <LayoutMenu />
      <a-layout>
        <a-layout-header :style="{ background: '#fff', padding: 0 }" />
        <a-layout-content :style="{ margin: '8px' }">
          <!-- <transition name="fade" mode="out-in"> -->
          <router-view></router-view>
          <!-- </transition> -->
        </a-layout-content>
        <!-- <a-layout-footer style="textAlign: center">Ant Design ©2018 Created by Ant UED</a-layout-footer> -->
      </a-layout>
    </template>
    <template v-else>
      <Login />
    </template>
  </a-layout>
</template>
<script lang="ts">
import LayoutMenu from "@/components/layout/menu.vue";
import { observable, action } from "mobx";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import { Basics } from "./views";
import rootStore from "./rootStore";
rootStore.UserStore.OnlineState;
@observer
@Component({
  components: {
    LayoutMenu,
    Login: Basics.login
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
}
</script>
<style lang="less">
#components-layout-demo-responsive {
  min-height: 100vh;
  .logo {
    height: 32px;
    background: rgba(143, 57, 57, 0.2);
    margin: 16px;
  }
}
</style>
