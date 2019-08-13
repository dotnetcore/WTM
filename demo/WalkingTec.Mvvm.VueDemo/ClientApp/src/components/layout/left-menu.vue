<template>
  <aside class="app-sidebar animated slideInLeft" :class="{ 'slide-toggle-left': !showLevelbar }">
    <el-menu class="el-menu-vertical-demo" :default-active="defaultPath" :router="true" :default-openeds="defaultOpen" :collapse="collapse">
      <div v-for="(item, index) in menuItems" v-bind:key="index">
        <el-submenu :index="item.path || index + ''" v-if="item.children && item.children.length">
          <template slot="title">
            <i :class="['fa', item.meta.icon]"></i>
            <span slot="title">{{item.name}}</span>
          </template>
          <el-menu-item-group>
            <el-menu-item v-for="(subItem, subIndex) in item.children" v-if="subItem.name || subItem.path" v-bind:key="subIndex" :index="subItem.path || index + '' + subIndex">
              {{subItem.name}}
            </el-menu-item>
          </el-menu-item-group>
        </el-submenu>
        <el-menu-item :index="item.path || index + ''" v-else>
          <i :class="['fa', item.meta.icon]"></i>
          <span slot="title">{{item.name}}</span>
        </el-menu-item>
      </div>
    </el-menu>
  </aside>
</template>

<script lang='ts'>
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import { State, Getter } from "vuex-class";
import dataMenuItems from "@/store/menu/menu-items";

@Component
export default class Menu extends Vue {
    @Prop({ default: false })
    isEmbed!: boolean;
    showLevelbar: boolean = true;
    defaultPath: string = "";
    defaultOpen: string[] = [];
    @Getter("isCollapse")
    collapse;
    @Getter("menuItems")
    menuItems;
    @State resourcesList;
    mounted() {
        // const menuItems = this.menuItems;
        // menuItems.forEach((item, index) => {
        //     if (item.meta.expanded) {
        //         this.defaultOpen.push(item.path || index + "");
        //     }
        // });
    }
    @Watch("$route")
    routeChange() {
        const matched = [this.$route];
        let meta = this.$route.meta;
        while (meta && meta.parentMenu) {
            matched.unshift(meta.parentMenu);
            meta = meta.parentMenu.meta;
        }
        this.defaultPath = (matched[1] && matched[1].path) || "/";
    }
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
