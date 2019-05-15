<template>
    <div id="app" class="wx-urm">
        <Header pageNmae="index"></Header>
        <AppMain></AppMain>
        <!-- copyright -->
        <Footer class="copyright" pageNmae="index"></Footer>
        <RightFloat></RightFloat>
    </div>
</template>

<style lang="less">
@import '../../assets/css/index.less';
.wx-urm {
    height: 100%;
}
</style>

<script>
import { AppMain, Footer, Header } from '@/components/layout/index';
import cookie from '@/util/cookie.js';
import config from '@/config/index.js';
import { mapState, mapMutations, mapActions } from 'vuex';
const mixin = {
    computed: {
        ...mapState({})
    },
    methods: {
        ...mapMutations({ setIsLogin: 'setIsLogin' }),
        ...mapActions({})
    }
};

export default {
    components: {
        AppMain,
        Header,
        Footer
    },
    mixins: [mixin],
    data() {
        return {};
    },
    computed: {},
    mounted() {
        const token = cookie.getCookie(config.tokenKey);
        console.log('app login');
        if (token) {
            this.setIsLogin(true);
        } else {
            this.setIsLogin(false);
        }
    },
    methods: {}
};
</script>
