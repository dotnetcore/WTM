<template>
  <pro-layout
    v-bind="provider"
    :menuData="getMenuData()"
    @select="onSelect"
    @openKeys="onOpenKeys"
    @collapse="onCollapse"
  >
    <!-- 标题栏右侧选项 -->
    <template v-slot:rightContentRender>
      <RightContentRender />
    </template>
    <!-- <template v-slot:headerContentRender>
      <Tabs />
    </template>-->
    <!-- 标签页 -->
    <Tabs />
    <!-- 主界面 -->
    <Container />
    <!-- 修改密码 -->
    <ChangePassword />
  </pro-layout>
</template>
<script lang="ts">
import { SystemController } from "@/client";
import queryString from "query-string";
import lodash from "lodash";
import { Inject, Options, Vue, Watch } from "vue-property-decorator";
import router from "../router";
import Container from "./views/container.vue";
import RightContentRender from "./views/rightContentRender.vue";
import Tabs from "./views/tabs.vue";
import ChangePassword from "./views/changePassword.vue";
@Options({
  components: { RightContentRender, Container, Tabs, ChangePassword }
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
    logo: require("@/assets/img/logo.png"),
    // layout: "top",
    title: "WTM",
    // navTheme:"light",
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
      console.log(
        "🚀 ~ file: main.vue ~ line 69 ~ extends ~ getMenuData ~ production",
        production
      );
      return production;
    }
    const RouterConfig = router.RouterConfig.filter(
      item => !lodash.eq(item.name, "NotFound")
    );
    const menus = [
      {
        name: "development",
        path: "development",
        // <router-link> Props
        router: {},
        meta: {
          icon: "SaveOutlined",
          title: this.$t(this.$locales["PageName.development"]),
          target: "a"
        },
        children: lodash.map(RouterConfig, item => {
          const router = lodash.pick(item, ["path", "name"]);
          const data = lodash.assign(router, {
            // <router-link> Props
            router: { to: router },
            meta: {
              icon: "SaveOutlined",
              title: this.$t(`PageName.${lodash.get<any, any>(item, "name")}`)
            }
          });
          // console.log("LENG ~ extends ~ data ~ data", data)
          return data;
        })
      },
      {
        name: "test",
        path: "test",
        meta: {
          icon: "SaveOutlined",
          title: this.$t(this.$locales["PageName.test"])
        },
        children: [
          {
            path: "/test/1",
            router: { to: {} },
            meta: { icon: "SaveOutlined", title: "二级菜单" },
            children: [
              {
                path: "/test/2",
                router: { to: { path: "/test" } },
                meta: { icon: "SaveOutlined", title: "三级菜单" }
              }
            ]
          }
        ]
      }
    ];
    if (production.length) {
      menus.push({
        name: "production",
        path: "production",
        router: {},
        meta: {
          icon: "SaveOutlined",
          title: this.$t(this.$locales["PageName.production"]),
          target: "a"
        },
        children: production
      });
    }

    return menus;
  }
  created() {}
  mounted() {
    // console.error("LENG ~ extends ~ mounted ~ this", this.$route.name)
    this.provider.openKeys = [this.$route.path];
    this.onRoute();
  }
  @Watch("$route")
  onRoute(val?, old?) {
    let selectedKeys = this.$route.path;
    if (lodash.eq(this.$route.path, "/webview")) {
      selectedKeys = queryString.stringifyUrl({
        url: "/webview",
        query: lodash.pick(this.$route.query, ["src", "name"])
      });
    }
    this.provider.selectedKeys = [selectedKeys];
  }
  onCollapse(collapsed) {
    this.provider.collapsed = collapsed;
  }
  onSelect(event) {
    // console.log("LENG ~ extends ~ onSelect ~ event", event)
    // this.provider.selectedKeys = event;
  }
  onOpenKeys(event) {
    // console.log("LENG ~ extends ~ onOpenKeys ~ event", event)
    this.provider.openKeys = event;
  }
}
function toMenus() {}
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
