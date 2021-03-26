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
import RightContentRender from "./views/rightContentRender.vue";
import Container from "./views/container.vue";
import Tabs from "./views/tabs.vue";
import { $i18n } from "@/client";
@Options({
  components: { RightContentRender, Container, Tabs },
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
    // locale: (key) => {
    //   return key
    //   // return $i18n.t(key);
    // },
    isMobile: false,
    hasFooterToolbar: false,
    hasSideMenu: true,
    hasHeader: true,
    setHasFooterToolbar: (has) => {
      this.provider.hasFooterToolbar = has;
    },
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
            meta: {
              icon: "SaveOutlined",
              title: $i18n.t("frameworkuser.PageName"),
            },
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
        meta: { icon: "SaveOutlined", title: "测试页面", target: "a" },
        children: [
          {
            path: "/frameworkuser2",
            // name: "frameworkuser2",
            meta: { icon: "SaveOutlined", title: "frameworkuser.PageName" },
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
