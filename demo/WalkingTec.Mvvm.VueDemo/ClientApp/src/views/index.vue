<template>
  <div id="app" class="app-layout">
    <!--顶部导航-->
    <Header />
    <LeftMenu />
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
import { Component, Vue } from "vue-property-decorator";
import { Action, Mutation } from "vuex-class";
import cache from "@/util/cache";
import config from "@/config/index";

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
    created() {
        const uData = cache.getStorage(config.tokenKey, true);
        this.loginCheckLogin({ ID: uData.Id })
            .then(res => {
                this.setActions(_.get(res, "Attributes.Actions", []));
                this.setMenus(_.get(res, "Attributes.Menus", []));
                return res;
            })
            .catch(err => {
                console.log(err);
                location.href = "/login.html";
            });
    }
}
</script>
