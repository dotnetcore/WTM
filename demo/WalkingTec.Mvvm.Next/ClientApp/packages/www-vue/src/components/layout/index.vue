<template>
  <a-layout id="components-layout-demo-responsive">
    <LayoutMenu v-if="UserStore.OnlineState" />
    <a-layout v-show="UserStore.OnlineState">
      <a-layout-header :style="{ background: '#fff', padding: 0 }" />
      <a-layout-content :style="{ margin: '8px' }">
        <router-view></router-view>
      </a-layout-content>
    </a-layout>
    <div v-show="!UserStore.OnlineState" class="user-loading">
      <a-spin>
        <a-icon slot="indicator" type="loading" style="font-size: 50px" spin />
      </a-spin>
    </div>
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
  .user-loading {
    position: fixed;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    transition: all .2s;
  }
}
</style>
