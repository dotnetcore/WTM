<template>
    <el-card class="search-box" shadow="never">
        <wtm-create-form :ref="refName" :options="formOptions" elRowClass="flex-container">
            <wtm-form-item class="search-but-box">
                <el-button-group class="button-group">
                    <el-button type="primary" class="btn-search" icon="el-icon-search" :disabled="disabledInput" @click="onSearch">
                        {{ $t("buttom.search") }}
                    </el-button>
                    <el-button v-if="needCollapse" class="toggle-class" type="primary" @click="toggleCollapse">
                        <i class="fa arrow-down el-icon-arrow-down" :class="{ 'is-active': isActive }" />
                    </el-button>
                </el-button-group>
                <el-button v-if="needResetBtn" class="reset-btn" plain type="primary" icon="el-icon-refresh" @click="onReset">
                    {{ $t("buttom.reset") }}
                </el-button>
            </wtm-form-item>
        </wtm-create-form>
    </el-card>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch, Provide } from "vue-property-decorator";
@Component({
    name: "wtm-search"
})
export default class WtmSearch extends Vue {
    @Provide()
    componentName = "wtmSearch";
    // 请求表单
    @Prop({ type: Object, default: () => {} })
    formOptions;
    // 是否需要折叠面板
    @Prop({ type: Boolean, default: false })
    needCollapse!: boolean;
    // 是否展开折叠
    @Prop({ type: Boolean, default: false })
    isActive!: boolean;
    // 是否需要重置按钮
    @Prop({ default: true })
    needResetBtn!: boolean;
    // 禁用input，通常是在加载的时候禁用
    @Prop({ default: false })
    disabledInput!: boolean;
    // 执行事件集合
    @Prop({ type: Object, default: null })
    events!: object;

    refName: string = "searchRefName";
    /**
     * 返回表单组件, this.$refs，get监听不到，改为方法
     */
    FormComp() {
        return _.get(this.$refs, this.refName);
    }
    /**
     * 表单数据
     */
    getFormData() {
        return this.FormComp().getFormData();
    }
    onSearch() {
        if (this.events && this.events["onSearch"]) {
            this.events["onSearch"]();
        } else {
            this.$emit("onSearch");
        }
    }
    toggleCollapse() {
        this.$emit("update:isActive", !this.isActive);
    }
    onReset() {
        this.FormComp().resetFields();
        this.onSearch();
    }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "~@/assets/css/animate.less";
.fuzzy-card {
    padding: 6px;
}
.search-box {
    background-color: #f5f5f5;
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
        float: left;
    }
    .search-but {
        height: 100%;
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
        vertical-align: text-top;
    }
    .toggle-class {
        padding-left: 7px;
        padding-right: 7px;
    }
    .wtm-collapse {
        will-change: max-height;
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
            // margin: 10px 0;
            max-height: 500px;
        }
    }
    .el-form-item--small .el-form-item__label {
        font-size: 12px;
        line-height: 32px;
    }
    .flex-container {
        overflow: hidden;
        .search-but-box {
            .el-form-item__content {
                line-height: 0;
            }
        }
    }
}
</style>
