<template>
    <div id="app" class="app-main">
        <Header></Header>
        <leftMenu></leftMenu>
        <AppMain></AppMain>
        <Footer></Footer>
    </div>
</template>

<style lang="less">
// @import '../../assets/css/index.less';
.app-main {
    // height: 100%;
}
</style>

<script>
import { AppMain, Footer, Header, leftMenu } from "@/components/layout/index";
import { mapState, mapMutations, mapActions } from "vuex";
// import AppMain from "@/components/layout/app-main";
// import Header from "@/components/layout/header";
// import Footer from "@/components/layout/footer";
// import leftMenu from "@/components/layout/left-menu";

const mixin = {
    computed: {
        ...mapState({})
    },
    methods: {
        ...mapMutations({ setIsLogin: "setIsLogin" }),
        ...mapActions({})
    }
};

export default {
    components: {
        AppMain,
        Header,
        Footer,
        leftMenu
    },
    mixins: [mixin],
    data() {
        return {};
    },
    computed: {},
    mounted() {},
    methods: {}
};
</script>
