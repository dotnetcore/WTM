<template>
    <div class="wrap">
        <div class="wrap-title">{{title}}</div>
        <slot />
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class Wrap extends Vue {
    @Prop({ default: "" })
    title!: string;
}
</script>
<style lang="less" scoped>
@import "~@/assets/css/variable.less";
.wrap {
    background: rgba(38, 93, 255, 1) rgba(255, 255, 255, 1);
    box-shadow: -1px 4px 6px 0px rgba(209, 209, 209, 0.53);
    padding: 16vw * @vwUnit 24vw * @vwUnit;
    background: #ffffff;
    .wrap-title {
        color: #303133;
        font-size: 24px;
        // line-height: 33vw * @vwUnit;
        padding-bottom: 12vw * @vwUnit;
        border-bottom: 1px solid #dcdfe6;
    }
}
</style>
