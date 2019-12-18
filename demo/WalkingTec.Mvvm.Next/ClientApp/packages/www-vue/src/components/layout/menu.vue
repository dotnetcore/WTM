<template>
  <a-layout-sider
    breakpoint="lg"
    collapsedWidth="0"
    @collapse="onCollapse"
    @breakpoint="onBreakpoint"
  >
    <router-link to="/">
      <div class="logo" />
    </router-link>
    <a-menu :defaultSelectedKeys="['1']" :defaultOpenKeys="['2']" mode="inline" theme="dark">
      <template v-for="item in UserStore.MenuTrees">
        <a-menu-item v-if="!item.children" :key="item.key">
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
import rootStore from "../../rootStore";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
@observer
@Component({
  components: {
    "sub-menu": SubMenu
  }
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  onCollapse(collapsed, type) {
    console.log(collapsed, type);
  }
  onBreakpoint(broken) {
    console.log(broken);
  }
}
</script>
<style lang="less">
.ant-layout-sider {
  user-select: none;
}
</style>
