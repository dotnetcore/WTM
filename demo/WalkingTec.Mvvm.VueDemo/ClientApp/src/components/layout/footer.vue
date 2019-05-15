<template>
    <div class="footerbg">
        <div class="white_W1200">
            <div class="copyright"> Copyright 2019 www.123.com AII Rights Reserved. ICP备 123号 123有限公司版权所有
            </div>
        </div>
    </div>
</template>
<script>
import Filing from './footer-filing';
export default {
    props: {
        pageNmae: {
            type: String,
            default: ''
        }
    },
    data() {
        return {};
    },
    mounted() {
        // console.log(config);
    },
    methods: {
        goPath(path, param) {
            this.$router.push({
                path: path,
                treePath: param
            });
        }
    },
    components: {
        Filing
    }
};
</script>

<style lang="less">
@import '../../assets/css/mixin.less';
.footerbg {
}
</style>


