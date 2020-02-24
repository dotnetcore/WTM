<template>
  <a-layout-sider
    breakpoint="lg"
    collapsedWidth="0"
    @collapse="onCollapse"
    @breakpoint="onBreakpoint"
  >
    <router-link to="/">
      <div class="layout-title" v-text="$GlobalConfig.settings.title" />
    </router-link>
    <a-menu
      :defaultSelectedKeys="selectedKeys"
      :defaultOpenKeys="openKeys"
      mode="inline"
      theme="dark"
    >
      <template v-for="item in UserStore.MenuTrees">
        <a-menu-item v-if="!item.children.length" :key="item.key">
          <router-link :to="item.path">
            <a-icon :type="item.icon || 'pie-chart'" />
            <span>{{ item.name }}</span>
          </router-link>
        </a-menu-item>
        <sub-menu v-else :menu-info="item" :key="item.key" />
      </template>
    </a-menu>
  </a-layout-sider>
</template>


<script lang="ts">
import SubMenu from "./subMenu.vue";
import lodash from "lodash";
import rootStore from "../../rootStore";
// import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
// @observer
@Component({
  components: {
    "sub-menu": SubMenu
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  get pageMenu() {
    const path = lodash.find(this.UserStore.Menus, [
      "path",
      this.$root.$route.path
    ]);
    return path;
  }
  get selectedKeys() {
    const selectedKeys = [lodash.get(this.pageMenu, "key", "")];
    return selectedKeys;
  }
  get openKeys() {
    const openKeys = this.getDefaultOpenKeys(
      this.UserStore.Menus,
      this.pageMenu,
      []
    );
    return openKeys;
  }
  //  获取
  getDefaultOpenKeys(Menus, Menu, OpenKeys = []) {
    const ParentId = lodash.get(Menu, "ParentId");
    if (ParentId) {
      OpenKeys.push(ParentId);
      const Parent = lodash.find(Menus, ["Id", ParentId]);
      if (Parent.ParentId) {
        this.getDefaultOpenKeys(Menus, Parent, OpenKeys);
      }
    }
    return OpenKeys;
  }
  mounted() {}
  onCollapse(collapsed, type) {}
  onBreakpoint(broken) {}
}
</script>
<style lang="less">
.ant-layout-sider {
  user-select: none;
}
.layout-title {
  text-align: center;
  height: 32px;
  line-height: 32px;
  background: rgba(143, 57, 57, 0.2);
  color: #fff;
  margin: 16px;
}
</style>
