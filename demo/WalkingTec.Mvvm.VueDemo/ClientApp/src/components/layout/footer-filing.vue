<template>
    <div class="footer-filing w1200">Copyright 2019 www.123.com AII Rights Reserved. ICP备 123号 123科技有限公司 版权所有</div>
</template>
<script>
import config from '@/config/index';
export default {
    data() {
        return {};
    },
    mounted() {
        console.log(config);
    },
    methods: {}
};
</script>

<style lang="less">
.footer-filing {
    height: 38px;
    background: #333333;
    text-align: center;
    color: #fff;
    line-height: 38px;
    font-size: 12px;
}
</style>


