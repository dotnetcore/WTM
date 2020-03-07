<template>
    <el-col v-show="isShow" :lg="size.lg" :sm="size.sm" :xs="size.xs">
        <el-form-item ref="elItem" :class="[parentName === 'wtmSearch' ? 'form-item-search' : 'form-item']" v-bind="$attrs">
            <slot v-if="isEdit" />
            <template v-else>
                <img v-if="isImg" :src="value" />
                <slot v-else name="editValue">
                    <!--后备内容 -->
                    {{ value }}
                </slot>
            </template>
        </el-form-item>
    </el-col>
</template>

<script lang="ts">
import { Component, Vue, Prop, Inject, Ref } from "vue-property-decorator";
/**
 *
 * Prop:
 * { span：0～24 }
 */
@Component
export default class WtmFormItem extends Vue {
    @Inject({ from: "componentName", default: "" })
    parentName; // 父级组件name
    @Ref("elItem")
    readonly elItem; // el-form-item
    @Prop({ type: Boolean, default: true })
    isShow;
    @Prop({ type: String }) // required
    status;
    @Prop({ type: String, default: "" })
    value;
    @Prop({ type: Boolean, default: false })
    isImg;
    /**
     * 是否修改
     */
    get isEdit() {
        return this.status !== this.$actionType.detail;
    }
    /**
     * span大小
     */
    get size() {
        const span = parseInt(this.$attrs.span) || this.parentCmpt.span;
        return {
            lg: span,
            md: span + 2,
            sm: span + 4,
            xs: 24
        };
    }
    /**
     * 上级组件
     * 判断应用父级组件
     *      wtmSearch： 默认6
     *      wtmDialog： 默认12
     * 注：
     *      当前只有span
     */
    get parentCmpt() {
        let span: number = 6;
        switch (this.parentName) {
            case "wtmDialog":
                span = 12;
                break;
            case "wtmSearch":
            default:
                span = 6;
                break;
        }
        return { span };
    }

    mounted() {
        // console.log("formitem $attrs", this.$attrs);
    }

    /**
     * 指定错误
     */
    showError(msg) {
        this.elItem.clearValidate();
        if (msg) {
            this.elItem.validateMessage = msg;
            this.elItem.validateState = "error";
        }
    }
    /**
     * 清理错误
     */
    clearValidate() {
        this.elItem.clearValidate();
    }
}
</script>
<style lang="less" scoped>
.form-item-search {
    display: flex;
    margin-bottom: 0;
    padding: 10px 0;
}
.form-item {
    // &.el-form-item--small.el-form-item {
    //     margin-bottom: 0;
    // }
    .el-form-item__label::after {
        content: ":";
    }
}
</style>
