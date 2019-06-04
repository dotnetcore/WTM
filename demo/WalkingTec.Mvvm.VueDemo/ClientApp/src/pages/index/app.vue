<template>
    <div id="app" class="app-layout">
        <Header></Header>
        <leftMenu></leftMenu>
        <AppMain></AppMain>
        <Footer></Footer>
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

<script>
import { AppMain, Footer, Header, leftMenu } from "@/components/layout/index";
import { mapState, mapMutations, mapActions } from "vuex";

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
    mounted() {
        console.log("app", this);
    },
    methods: {}
};
</script>
