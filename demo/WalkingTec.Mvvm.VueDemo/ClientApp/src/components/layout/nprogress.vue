<template>
  <div class="app-nprogress" />
</template>
<!--  因后期要对app-nprogress 操作， 所以无法使用scopd -->
<style lang="less">
@import "../../assets/css/variable";
@import "~nprogress/nprogress.css";
.app-nprogress {
    position: fixed !important;
    width: 100%;
    height: 100%;
    z-index: @maxZindex*2;
    pointer-events: none;
    // 进度条
    .bar {
        height: 3px !important;
        background: #409eff !important;
    }
    // 进度条最右进度
    .peg {
        box-shadow: 0 0 20px #409eff, 0 0 10px #409eff !important;
    }
    // 最右圈
    .spinner-icon {
        border-top-color: #409eff !important;
        border-left-color: #409eff !important;
    }
}
</style>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
@Component
export default class Nprogress extends Vue {}
</script>
