<template>
    <div class="loadmore">
        <div class="weui-loadmore" v-if="isLoad">
            <i class="weui-loading"></i>
            <span class="weui-loadmore__tips">正在加载</span>
        </div>
        <div class="weui-loadmore weui-loadmore_line" v-else>
            <span class="weui-loadmore__tips">{{loadmoreText}}</span>
        </div>
    </div>
</template>
<script>
export default {
    props: {
        isLoad: {
            type: Boolean,
            default: false
        },
        loadmoreText: {
            type: String,
            default: () => {
                return '无数据';
            }
        }
    },
    data() {
        return {};
    },
    mounted() {},
    methods: {}
};
</script>

<style lang="less">
@import '../../assets/css/variable.less';
.loadmore {
    .weui-loadmore {
        font-size: 14px;
        .weui-loadmore__tips {
            background-color: @backgrouindColor;
        }
    }
}
</style>


