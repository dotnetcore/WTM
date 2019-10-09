<template>
    <!-- <el-card class="fuzzy-card">
    </el-card> -->
    <div class="fuzzy-search">
        <div class="flex-container">
            <div v-if="isFuzzy" class="search-input el-input el-input--small">
                <!-- <span class="search-text" :style="{width:searchLabelWidth?(searchLabelWidth+labelPadding)+'px':'auto'}">搜索</span>
                    <input v-model.trim="searchKeyWord" :disabled="disabledInput" class="search-input el-input__inner" :style="{width:inputWidth+'px'}" :placeholder="placeholder" @keyup.enter="onSearch"> -->
                <slot name="search-content" />
            </div>
            <el-button-group v-if="isFuzzy" class="button-group">
                <el-button type="primary" class="btn-search" :disabled="disabledInput" @click="onSearch">
                    模糊查询
                </el-button>
                <el-button v-if="needCollapse&&$slots['collapse-content']" class="toggle-class" type="primary" @click="toggleCollapse">
                    <!-- <i class="fa fa-angle-double-down arrow-down" :class="{'is-active': isActive}"></i> -->
                    <i class="fa fa-angle-down arrow-down" :class="{'is-active': isActive}" />
                </el-button>
            </el-button-group>
            <el-button v-if="isFuzzy && needResetBtn" class="reset-btn" plain type="primary" @click="onReset">
                重置
            </el-button>
            <div :class="{'fr': isFuzzy,'tar': !isFuzzy}">
                <slot name="operation" />
            </div>
        </div>
        <div class="uds-collapse" :class="{'is-active':!isFuzzy || isActive}" v-show="$slots['collapse-content']">
            <div class="collapse-content">
                <slot name="collapse-content" />
            </div>
        </div>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
@Component
export default class FuzzySearch extends Vue {
    @Prop({ default: 200 })
    inputWidth!: number;
    // @Prop({ default: "" })
    // placeholder!: string;
    @Prop({ default: true })
    isFuzzy!: boolean;
    @Prop({ default: "" })
    fuzzyKeyWord!: string;
    @Prop({ default: true }) // 是否需要折叠面板
    needCollapse!: boolean;
    @Prop({ default: true }) // 是否需要重置按钮
    needResetBtn!: boolean;
    @Prop({ default: false }) // 禁用input，通常是在加载的时候禁用
    disabledInput!: boolean;
    @Prop({ default: 0 })
    searchLabelWidth!: number;
    @Prop({ default: "fuzzy" })
    fuzzyField!: string;
    labelPadding: number = -30;
    isActive: boolean = false;
    onSearch() {
        this.$emit("onSearch");
    }
    toggleCollapse() {
        this.isActive = !this.isActive;
    }
    onReset() {
        this.$emit("onReset");
    }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "~@/assets/css/animate.less";
.fuzzy-card {
    padding: 6px;
}
.fuzzy-search {
    background-color: #f5f5f5;
    border: 1px solid #e3e3e3;
    border-radius: 4px;
    padding: 15px 0 10px;
    .search-text {
        display: inline-block;
        margin-left: 10px;
        margin-right: 20px;
    }
    .fr {
        float: right;
    }
    .tar {
        text-align: right;
    }
    .search-input {
        width: auto;
        display: inline-block;
        height: 40px;
        padding-right: 20px;
    }
    .button-group {
        margin-left: 24px;
        vertical-align: text-top;
        .transition(all 0.5s ease 0s);
        .arrow-down {
            &.is-active {
                .rotate(@deg:180deg);
            }
        }
        .btn-search {
            -webkit-border-radius: 0;
            -moz-border-radius: 0;
            border-radius: 0;
        }
    }
    .reset-btn {
        position: relative;
        margin-left: 10px;
        width: 95px;
        vertical-align: text-top;
    }
    .toggle-class {
        width: 34px;
    }
    .uds-collapse {
        will-change: max-height;

        // background-color: #f5f5f5;
        .transition(all 0.4s ease 0s);
        max-height: 0;
        opacity: 0;
        overflow: hidden;
        .collapse-content {
            padding: 18px 0;
            border-radius: 4px;
            // border: 1px solid #e3e3e3;
        }
        &.is-active {
            opacity: 1;
            margin: 10px 0;
            max-height: 500px;
        }
    }
    .el-form-item--small .el-form-item__label {
        font-size: 12px;
        line-height: 32px;
    }
    .flex-container {
        overflow: hidden;
    }
}
</style>