<template>
  <div id="app" class="app-layout">
    <Header />
    <LeftMenu />
    <AppMain />
    <Footer />
  </div>
</template>

<style lang="less">
.app-layout {
    background-repeat: no-repeat;
    background-position: center 110px;
    background-size: 100%;
    min-height: 100vh;
}
</style>

<script lang="ts">
import { AppMain, Footer, Header, LeftMenu } from "@/components/layout/index";
import { Component, Vue } from "vue-property-decorator";

@Component({
    components: {
        AppMain,
        Header,
        Footer,
        LeftMenu
    }
})
export default class App extends Vue {
    mounted() {
        console.log("app", this);
    }
}
</script>
