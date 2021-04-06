<template>
  <pro-layout v-bind="provider" @select="onSelect" @openKeys="onOpenKeys" @collapse="onCollapse">
    <template v-slot:rightContentRender>
      <RightContentRender />
    </template>
    <Tabs />
    <Container />
  </pro-layout>
</template>
<script lang="ts">
import { $i18n } from "@/client";
import lodash from "lodash";
import { Options, Vue } from "vue-property-decorator";
import Container from "./views/container.vue";
import RightContentRender from "./views/rightContentRender.vue";
import Tabs from "./views/tabs.vue";
import router from "../router";
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
        name: "development",
        meta: { icon: "SaveOutlined", title: "开发测试", target: "a" },
        children: lodash.map(router.RouterConfig, item => {
          const data = lodash.assign(lodash.pick(item, ['path', 'name']), {
            meta: {
              icon: "SaveOutlined",
              title: $i18n.t(`${lodash.get(item, 'name') as any}.PageName`),
            }
          })
          console.log("LENG ~ extends ~ data ~ data", data)
          return data
        })
        // [
        //   {
        //     path: "/frameworkuser",
        //     name: "frameworkuser",
        //     meta: {
        //       icon: "SaveOutlined",
        //       title: $i18n.t("frameworkuser.PageName"),
        //     },
        //   },
        //   {
        //     path: "/test",
        //     name: "test",
        //     meta: { icon: "SaveOutlined", title: "Dashboard" },
        //   },
        // ],
      },
      {
        name: "production",
        meta: { icon: "SaveOutlined", title: "正式菜单", target: "a" },
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
  created() { }
  mounted() { }
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
