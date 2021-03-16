<template>
  <route-context-provider :value="provider">
    <pro-layout
      v-bind="state"
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
  </route-context-provider>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import { h } from "vue";
import { createRouteContext } from "@ant-design-vue/pro-layout";
import RightContentRender from "./rightContentRender.vue";
import Container from "./container.vue";
import Tabs from "./tabs.vue";

const [RouteContextProvider] = createRouteContext();
// Component definition
@Options({
  components: { RouteContextProvider, RightContentRender, Container, Tabs },
})
export default class extends Vue {
  provider = {
    menuData: this.menuData,
    headerHeight: 48,
    sideWidth: 208,
    openKeys: [],
    selectedKeys: [],
    isMobile: false,
    hasFooterToolbar: false,
    hasSideMenu: true,
    hasHeader: true,
    fixSiderbar: true,
    fixedHeader: true,
    setHasFooterToolbar: (has) => {
      this.provider.hasFooterToolbar = has;
    },
  };
  state = {
    // layout:'top',
    title: "暄桐小程序",
    collapsed: false,
    fixSiderbar: true,
    fixedHeader: true,
    locale: (key) => {
      console.log("LENG ~ extends ~ key", key);
      return key;
    },
    // menuItemRender: (props) => null,
    // headerContentRender: (props) => h(headerContentRender),
  };
  get menuData() {
    return [
      {
        path: "/a",
        name: "a",
        meta: { icon: "SaveOutlined", title: "测试页面" },
        children: [
          {
            path: "/frameworkuser",
            name: "frameworkuser",
            meta: { icon: "SaveOutlined", title: "测试用户" },
          },
          {
            path: "/test",
            name: "test",
            meta: { icon: "SaveOutlined", title: "Dashboard" },
          },
        ],
      },
      {
        path: "/b",
        name: "b",
        meta: { icon: "SaveOutlined", title: "测试页面" },
        children: [
          {
            path: "/frameworkuser2",
            // name: "frameworkuser2",
            meta: { icon: "SaveOutlined", title: "测试用户" },
          },
          {
            path: "/test3",
            // name: "test3",
            meta: { icon: "SaveOutlined", title: "Dashboard" },
          },
        ],
      },
    ];
  }
  created() {}
  mounted() {}
  onCollapse(collapsed) {
    this.state.collapsed = collapsed;
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
