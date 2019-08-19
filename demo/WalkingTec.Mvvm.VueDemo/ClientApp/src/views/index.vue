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
import { Action } from "vuex-class";
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
    getResourcesList;
    @Action
    localMenus;

    created() {
        const uData = cache.getStorage(config.tokenKey, true);
        if (!uData) {
            location.href = "/login.html";
        }
        // this.localMenus().then(res => {
        //     this.$router["options"].routes = res;
        //     this.$router.addRoutes(res);
        // });
        // this.getResourcesList().then(res => {
        //     if (res.result_code !== "success") {
        //         this.$message.error(res.message);
        //         // this["onHref"]("/login.html");
        //     }
        // });
    }
}
</script>
