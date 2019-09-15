<template>
  <el-container id="app" class="app-layout">
    <el-header>
      <app-header />
    </el-header>
    <el-container>
      <el-aside width="220px">
        <LeftMenu :default-path="defaultPath" :menu-items="menuItems" :collapse="collapse" />
      </el-aside>
      <el-main>
        <Tabs v-if="isTab" />
        <BreadCrumb v-else />
        <AppMain />
      </el-main>
    </el-container>
  </el-container>
</template>

<style lang="less">
.app-layout {
    .el-header {
        background: #000;
        position: fixed;
        width: 100%;
        z-index: 100;
    }
    .el-main {
        padding: 60px 0;
    }
}
</style>

<script lang="ts">
import {
    AppMain,
    Footer,
    Header,
    LeftMenu,
    Nprogress,
    BreadCrumb,
    Tabs
} from "@/components/layout/index";
import { Component, Vue } from "vue-property-decorator";
import { Action, Mutation, Getter } from "vuex-class";
import cache from "@/util/cache";
import config from "@/config/index";

@Component({
    components: {
        AppMain,
        "app-header": Header,
        "app-footer": Footer,
        LeftMenu,
        Nprogress,
        BreadCrumb,
        Tabs
    }
})
export default class App extends Vue {
    // 菜单
    @Action
    loginCheckLogin;
    @Action
    localMenus;
    @Mutation
    setActions;
    @Mutation
    setMenus;

    @Getter("isCollapse")
    collapse;
    @Getter("menuItems")
    menuItems;

    isTab: Boolean = true;
    get defaultPath() {
        return this["$route"].path;
    }

    created() {
        const global = cache.getCookieJson(config.globalKey) || {};
        this.isTab = global["tabs"];
    }
}
</script>
