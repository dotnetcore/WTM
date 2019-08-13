<template>
  <section class="app-main" :style="[]">
    <router-view />
  </section>
</template>

<script>
import { mapGetters } from "vuex";

export default {
    props: {
        isEmbed: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            showLevelbar: true
        };
    },
    computed: {
        ...mapGetters({
            layout: "layout"
        })
    },
    methods: {}
};
</script>

<style lang="less" scoped>
@import "../../assets/css/variable";
.app-main {
    // margin: 10px;
    // font-family: 'segoe-ui_normal,Segoe UI,Segoe,Segoe WP,Helvetica Neue,Helvetica,sans-serif';
    // line-height: 160%;
    position: absolute;
    left: 200px;
    right: 0;
    top: 60px;
    bottom: 44px;
    z-index: 998;
    width: auto;
    overflow: hidden;
    overflow-y: auto;
    box-sizing: border-box;
}
</style>
