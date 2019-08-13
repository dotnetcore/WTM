<template>
  <div class="header">
    WalkingTec.Mvvm Framework
  </div>
</template>
<script>
// import cookie from '@/util/cookie.js';
// import config from '@/config/index.js';

const logo = require("static/img/logo.png");

export default {
    props: {},
    data() {
        return {
            logo: logo,
            isLogin: false,
            userName: ""
        };
    },
    mounted() { },
    methods: {}
};
</script>

<style lang="less">
.header {
    z-index: 1000;
    height: 60px;
    background-color: #23262e;
    width: 100%;
    position: relative;
    color: #009688;
}
</style>


