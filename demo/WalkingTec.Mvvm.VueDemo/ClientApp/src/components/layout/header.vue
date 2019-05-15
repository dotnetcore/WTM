<template>
    <div class="header">
        header - hot
    </div>
</template>
<script>
// import cookie from '@/util/cookie.js';
// import config from '@/config/index.js';

const logo = require('@/assets/images/logo.png');

export default {
    props: {},
    data() {
        return {
            logo: logo,
            isLogin: false,
            userName: ''
        };
    },
    mounted() {},
    methods: {},
    computed: {}
};
</script>

<style lang="less">
.header {
    background-color: #fff;
    width: 100%;
    position: relative;
    z-index: 20;
    color: #333;
}
</style>



