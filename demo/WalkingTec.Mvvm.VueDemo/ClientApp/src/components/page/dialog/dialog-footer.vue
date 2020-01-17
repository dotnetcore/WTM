<template>
  <div slot="footer" class="dialog-footer">
    <el-button @click="onClear">
      关 闭
    </el-button>
    <el-button v-if="status !== detail" type="primary" @click="onSubmit">
      提 交
    </el-button>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class DialogFooter extends Vue {
    @Prop({ type: String, default: "" })
    status;
    onClear() {
        this.$emit("onClear");
    }
    onSubmit() {
        this.$emit("onSubmit");
    }

    get detail() {
        return this.$dialogType.detail;
    }
}
</script>
<style lang="less" >
@import "~@/assets/css/mixin.less";
.dialog-footer {
    .flexbox(row, flex-end);
}
</style>
