<template>
  <a-layout id="components-layout-demo-responsive">
      <LayoutMenu />
      <a-layout>
        <a-layout-header :style="{ background: '#fff', padding: 0 }" />
        <a-layout-content :style="{ margin: '8px' }">
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
@observer
@Component({
  components: {
    LayoutMenu,
  }
})
export default class extends Vue {
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
