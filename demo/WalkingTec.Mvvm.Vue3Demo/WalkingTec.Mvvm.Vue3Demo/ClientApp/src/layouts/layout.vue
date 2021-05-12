<template>
  <pro-layout
    v-bind="provider"
    :menuData="getMenuData()"
    @select="onSelect"
    @openKeys="onOpenKeys"
    @collapse="onCollapse"
  >
    <template v-slot:rightContentRender>
      <RightContentRender />
    </template>
    <Tabs />
    <Container />
  </pro-layout>
</template>
<script lang="ts">
import { SystemController } from "@/client";
import lodash from "lodash";
import { Inject, Options, Vue } from "vue-property-decorator";
import router from "../router";
import Container from "./views/container.vue";
import RightContentRender from "./views/rightContentRender.vue";
import Tabs from "./views/tabs.vue";
@Options({
  components: { RightContentRender, Container, Tabs }
})
export default class extends Vue {
  /**
   * 从 Aapp 中 注入 系统管理
   */
  @Inject({ from: SystemController.Symbol }) System: SystemController;
  provider = {
    // menuData: this.menuData,
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
    setHasFooterToolbar: has => {
      this.provider.hasFooterToolbar = has;
    }
  };
  getMenuData() {
    const production = this.System.UserController.UserMenus.getMenus();
    if (this.$WtmConfig.production) {
      console.log("🚀 ~ file: layout.vue ~ line 57 ~ extends ~ getMenuData ~ production", production)
      return production;
    }
    const menus = [
      {
        name: "development",
        meta: { icon: "SaveOutlined", title: "本地页面", target: "a" },
        children: lodash.map(router.RouterConfig, item => {
          const data = lodash.assign(lodash.pick(item, ["path", "name"]), {
            meta: {
              icon: "SaveOutlined",
              title: this.$t(`PageName.${lodash.get(item, "name") as any}`)
            }
          });
          // console.log("LENG ~ extends ~ data ~ data", data)
          return data;
        })
      }
    ];
    if (production.length) {
      menus.push({
        name: "production",
        meta: { icon: "SaveOutlined", title: "正式菜单", target: "a" },
        children: production
      });
    }

    return menus;
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
