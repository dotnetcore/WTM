<template>
  <aside class="app-sidebar animated slideInLeft" :class="{ 'slide-toggle-left': !showLevelbar }">
    <el-menu class="el-menu-vertical-demo" :default-active="defaultPath" :router="true" :default-openeds="defaultOpen" :collapse="collapse">
      <div v-for="(item, index) in menuItems" :key="index">
        <el-submenu v-if="item.children && item.children.length" :index="item.path || index + ''">
          <template slot="title">
            <i :class="['fa', item.meta.icon]" />
            <span slot="title">{{ item.name }}</span>
          </template>
          <el-menu-item-group>
            <el-menu-item v-for="(subItem, subIndex) in item.children" v-if="subItem.name || subItem.path" :key="subIndex" :index="subItem.path || index + '' + subIndex">
              {{ subItem.name }}
            </el-menu-item>
          </el-menu-item-group>
        </el-submenu>
        <el-menu-item v-else :index="item.path || index + ''">
          <i :class="['fa', item.meta.icon]" />
          <span slot="title">{{ item.name }}</span>
        </el-menu-item>
      </div>
    </el-menu>
  </aside>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class Menu extends Vue {
    @Prop()
    collapse;
    @Prop({ default: () => [] })
    menuItems;
    @Prop({ type: String, default: "" })
    defaultPath;
    @Prop({ type: Array, default: () => [] })
    defaultOpen;
    @Prop({ type: Boolean, default: true })
    showLevelbar;

    mounted() {}
}
</script>

<style lang="less" scoped>
@import "../../assets/css/variable";
.app-sidebar {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    // width: @sidebarWidth;
    margin-top: @navbarHeight;
    z-index: @maxZindex + 1;
    background: #fff;
    overflow-y: auto;
    overflow-x: hidden;
    padding: 0 0 15px;
    .el-menu-vertical-demo:not(.el-menu--collapse) {
        width: @sidebarWidth;
    }
    .sidebar-hd {
        color: #fff;
        font-size: 25px;
        height: @navbarHeight;
        background-color: #000;
    }
}
</style>
