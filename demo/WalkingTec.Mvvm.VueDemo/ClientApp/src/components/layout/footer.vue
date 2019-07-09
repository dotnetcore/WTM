<template>
  <div class="footerbg">
    <div class="copyright">
      Copyright 2019 www.123.com AII Rights Reserved. ICP备 123号 123有限公司版权所有
    </div>
  </div>
</template>
<script>
export default {
    props: {
        pageNmae: {
            type: String,
            default: ""
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
    }
};
</script>

<style lang="less">
@import "../../assets/css/mixin.less";
.footerbg {
    position: fixed;
    left: 200px;
    right: 0;
    bottom: 0;
    height: 44px;
    line-height: 44px;
    padding: 0 15px;
    background-color: #eee;
}
</style>
