<template>
  <!-- <route-context-provider  :value="state"> -->
  <pro-layout v-bind="state" @collapse="onCollapse">
    <router-view />
  </pro-layout>
  <!-- </route-context-provider> -->
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import { createRouteContext } from "@ant-design-vue/pro-layout";

const [RouteContextProvider] = createRouteContext();
// Component definition
@Options({ components: { RouteContextProvider } })
export default class extends Vue {
  state = {
    title:"暄桐测试",
    collapsed: false,
    openKeys: ["/dashboard"],
    selectedKeys: ["/welcome"],
    isMobile: false,
    fixSiderbar: true,
    fixedHeader: true,
    menuData: [],
    sideWidth: 208,
    hasSideMenu: true,
    hasHeader: true,
    hasFooterToolbar: false,
    setHasFooterToolbar: (has) => {
      this.state.hasFooterToolbar = has;
    },
  };
  // The behavior in class is the same as the current
  count = 0;
  created() {
  }
  mounted() {
    // console.log("LENG ~ extends ~ created ~ this", this, this.$route);
  }
  onCollapse(collapsed) {
    this.state.collapsed = collapsed;
  }
  increment() {
    this.count++;
  }
}
</script>
<style lang="less">
</style>
