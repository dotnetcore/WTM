<template>
  <div id="app" class="app-layout">
    <!--顶部导航-->
    <Header />
    <LeftMenu :default-path="defaultPath" :menu-items="menuItems" :collapse="collapse" />
    <article class="main-container">
      <!--页面内容-->
      <AppMain />
      <!-- copyright -->
      <!-- <Footer class="copyright" :style="[]"></Footer> -->
    </article>
  </div>
</template>

<style lang="less">
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
        Header,
        Footer,
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
