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
    }
}
</style>

<script lang="ts">
import {
    AppMain,
    Footer,
    Header,
    LeftMenu,
    Nprogress
} from "@/components/layout/index";
import { Component, Vue, Watch } from "vue-property-decorator";
import { Action, Mutation, Getter } from "vuex-class";
// import cache from "@/util/cache";
// import config from "@/config/index";

@Component({
    components: {
        AppMain,
        "app-header": Header,
        "app-footer": Footer,
        LeftMenu,
        Nprogress
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

    defaultPath: string = "";

    @Getter("isCollapse")
    collapse;
    @Getter("menuItems")
    menuItems;

    @Watch("$route")
    routeChange() {
        const matched = this["$route"];
        const menuAll = this["$router"].options.routes;
        const ps = _.filter(menuAll, ["meta.Id", matched.meta.ParentId]);
        if (ps.length > 0) {
            this.defaultPath = matched.path || ps[0].path;
        }
    }

    created() {}
}
</script>
