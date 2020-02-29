<template>
    <el-col :lg="size.lg" :sm="size.sm" :xs="size.xs">
        <el-form-item ref="elItem" :class="[parentCmpt.componentName === 'wtmSearch' ? 'form-item-search' : 'form-item']" v-bind="$attrs">
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
import { Component, Vue, Prop } from "vue-property-decorator";
/**
 *
 * Prop:
 * { span：0～24 }
 */
@Component
export default class WtmFormItem extends Vue {
    @Prop({ type: String }) // required
    status;
    @Prop({ type: String, default: "" })
    value;
    @Prop({ type: Boolean, default: false })
    isImg;

    get isEdit() {
        return this.status !== this.$actionType.detail;
    }

    get size() {
        const dfltSpan = this.parentCmpt["span"];
        const span = parseInt(this.$attrs.span) || dfltSpan;
        return {
            lg: span,
            sm: span + 4,
            xs: 24
        };
    }
    // this.$parent.$options.
    /**
     * 上级组件name
     * 判断应用父级组件
     * wtmSearch： 默认6
     * wtmDialog： 默认12
     */
    get parentCmpt() {
        let parent = this.$parent;
        let parentName: string = parent.componentName;
        while (!["wtmSearch", "wtmDialog"].includes(parentName)) {
            parent = parent.$parent;
            parentName = parent.componentName;
        }
        let span: number = 6;
        switch (parentName) {
            case "wtmDialog":
                span = (parent || {}).span || 12;
                break;
            case "wtmSearch":
            default:
                span = (parent || {}).span || 6;
                break;
        }
        return Object.assign(parent, { span });
    }
    showError(msg) {
        this.$refs["elItem"].clearValidate();
        if (msg) {
            this.$refs["elItem"].validateMessage = msg;
            this.$refs["elItem"].validateState = "error";
        }
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
