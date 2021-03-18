<template>
  <!-- <route-context-provider  :value="state"> -->
  <pro-layout v-bind="state" @collapse="onCollapse">
    <!-- <template v-slot:headerContentRender>
      <a-avatar size="small">
        <template #icon><UserOutlined /></template>
      </a-avatar>
    </template> -->
    <template v-slot:rightContentRender>
      <a-dropdown>
        <a class="ant-dropdown-link" @click.prevent>
          <span v-text="$i18n.locale"></span>
          <DownOutlined />
        </a>
        <template #overlay>
          <a-menu @click="changeLanguage">
            <a-menu-item
              v-for="item in languages"
              :key="item"
              v-text="item"
            ></a-menu-item>
          </a-menu>
        </template>
      </a-dropdown>
    </template>
    <router-view />
  </pro-layout>
  <!-- </route-context-provider> -->
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import { h } from "vue";
import { createRouteContext } from "@ant-design-vue/pro-layout";
import headerContentRender from "./headerContentRender.vue";

const [RouteContextProvider] = createRouteContext();
// Component definition
@Options({ components: { RouteContextProvider } })
export default class extends Vue {
  state = {
    // layout:'mix',
    title: "暄桐小程序",
    collapsed: false,
    openKeys: ["/dashboard"],
    selectedKeys: ["/welcome"],
    isMobile: false,
    fixSiderbar: true,
    fixedHeader: true,
    menuData: [],
    sideWidth: 208,
    hasSideMenu: true,
    // hasHeader: true,
    hasFooterToolbar: false,
    // headerRender: false,
    setHasFooterToolbar: (has) => {
      this.state.hasFooterToolbar = has;
    },
    headerContentRender: (props) => h(headerContentRender),
  };
  created() {}
  mounted() {}
  onCollapse(collapsed) {
    this.state.collapsed = collapsed;
  }
  get languages() {
    return this.lodash.keys(this.lodash.get(this.$i18n, "messages"));
  }
  changeLanguage(event) {
    this.$i18n.locale = event.key;
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
