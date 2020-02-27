<template>
  <el-col :lg="size.lg" :sm="size.sm" :xs="size.xs">
    <el-form-item class="form-item" ref="elItem" v-bind="$attrs">
      <slot v-if="isEdit" />
      <template v-else>
        <img v-if="isImg" :src="value">
        <slot v-else name="editValue">
          <!--后备内容 -->
          {{ value }}
        </slot>
      </template>
    </el-form-item>
  </el-col>
</template>

<script lang='ts'>
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
        const span = parseInt(this.$attrs.span) || 6;
        return {
            lg: span,
            sm: span + 4,
            xs: 24
        };
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
<style lang='less'>
.form-item {
    margin-bottom: 0;
    padding: 10px 0;
    &.el-form-item--small.el-form-item {
        margin-bottom: 0;
    }
    // width: 90%;
    .el-form-item__label::after {
        content: ":";
    }
}
</style>
