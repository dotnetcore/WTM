<template>
    <div class="loadmore">
        <div v-if="isLoad" class="weui-loadmore">
            <i class="weui-loading" />
            <span class="weui-loadmore__tips">正在加载</span>
        </div>
        <div v-else class="weui-loadmore weui-loadmore_line">
            <span class="weui-loadmore__tips">{{ loadmoreText }}</span>
        </div>
    </div>
</template>
<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { State, Getter, Action, Mutation, namespace } from "vuex-class";
@Component
export default class LoadMore extends Vue {
    @Prop({ default: false })
    isLoad!: boolean;
    @Prop({ default: "无数据" })
    loadmoreText: string | undefined;
}
</script>

<style lang="less">
@import "../../assets/css/variable.less";
.loadmore {
    .weui-loadmore {
        font-size: 14px;
        .weui-loadmore__tips {
            background-color: @backgrouindColor;
        }
    }
}
</style>


