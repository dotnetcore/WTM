<template>
    <div class="flexbox" :class="['dir-' + dir, 'justify-' + justify, 'align-' + align, 'wrap-'+ wrap]">
        <slot />
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class FlexBox extends Vue {
    @Prop({ default: "row" })
    dir!: string;
    @Prop({ default: "center" })
    justify!: string;
    @Prop({ default: "center" })
    align!: string;
    @Prop({ default: "nowrap" })
    wrap!: string;
}
</script>
<style lang="less" scoped>
@import "../../assets/css/mixin";
.flexbox {
    &.dir-row {
        .flexbox(row);
    }
    &.dir-row-reverse {
        .flexbox(row-reverse);
    }
    &.dir-column {
        .flexbox(column);
    }
    &.dir-column-reverse {
        .flexbox(column-reverse);
    }
    // 水平方向
    &.justify-center {
        .flexjustify(center);
    }
    &.justify-flex-start {
        .flexjustify(flex-start);
    }
    &.justify-flex-end {
        .flexjustify(flex-end);
    }
    &.justify-space-between {
        .flexjustify(space-between);
    }
    &.justify-space-around {
        .flexjustify(space-around);
    }
    // 垂直方向
    &.align-center {
        .flexalign(center);
    }
    &.align-flex-start {
        .flexalign(flex-start);
    }
    &.align-flex-end {
        .flexalign(flex-end);
    }
    &.align-baseline {
        .flexalign(baseline);
    }
    &.align-stretch {
        .flexalign(stretch);
    }
    // 换行
    &.wrap-wrap {
        .flexwrap(wrap);
    }
}
</style>
