<template>
  <pro-layout
    v-bind="provider"
    @select="onSelect"
    @openKeys="onOpenKeys"
    @collapse="onCollapse"
  >
    <template v-slot:rightContentRender>
      <RightContentRender />
    </template>
    <!-- <PageContainer
        :fixedHeader="true"
        :title="false"
        :tabList="[
          { key: '1', tab: 'Details' },
          { key: '2', tab: 'Rule' },
        ]"
        :tabProps="{
          type: 'editable-card',
          hideAdd: true,
        }"
        :affixProps="{ offsetTop: 100 }"
      > -->
    <Tabs />
    <Container />
    <!-- </PageContainer> -->
  </pro-layout>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import { h } from "vue";
import RightContentRender from "./rightContentRender.vue";
import Container from "./container.vue";
import Tabs from "./tabs.vue";
@Options({
  components: { RightContentRender, Container, Tabs }
})
export default class extends Vue {
  provider = {
    menuData: this.menuData,
    headerHeight: 48,
    sideWidth: 208,
    // layout: "top",
    title: "暄桐小程序",
    collapsed: false,
    fixSiderbar: true,
    fixedHeader: true,
    openKeys: [],
    selectedKeys: [],
    locale: key => {
      console.log("LENG ~ extends ~ key", key);
      return key;
    },
    isMobile: false,
    hasFooterToolbar: false,
    hasSideMenu: true,
    hasHeader: true,
    setHasFooterToolbar: has => {
      this.provider.hasFooterToolbar = has;
    }
  };
  get menuData() {
    return [
      {
        path: "/a",
        name: "a",
        meta: { icon: "SaveOutlined", title: "测试页面", target: "a" },
        children: [
          {
            path: "/frameworkuser",
            name: "frameworkuser",
            meta: { icon: "SaveOutlined", title: "测试用户" }
          },
          {
            path: "/test",
            name: "test",
            meta: { icon: "SaveOutlined", title: "Dashboard" }
          }
        ]
      },
      {
        path: "/b",
        name: "b",
        meta: { icon: "SaveOutlined", title: "测试页面", target: "a" },
        children: [
          {
            path: "/frameworkuser2",
            // name: "frameworkuser2",
            meta: { icon: "SaveOutlined", title: "测试用户" }
          },
          {
            path: "/test3",
            // name: "test3",
            meta: { icon: "SaveOutlined", title: "Dashboard" }
          }
        ]
      }
    ];
  }
  created() {}
  mounted() {}
  onCollapse(collapsed) {
    this.provider.collapsed = collapsed;
  }
  onSelect(event) {
    this.provider.selectedKeys = event;
  }
  onOpenKeys(event) {
    this.provider.openKeys = event;
  }
}
</script>
<style lang="less">
.w-app {
  .ant-pro-basicLayout-content {
    margin: 10px;
    background: white;
    padding: 10px;
  }
}
</style>
