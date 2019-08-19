<template>
  <div class="dialog-box">
    <el-dialog :title="title" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" @close="onClose" @open="onOpen">
      <slot />
    </el-dialog>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class DialogBox extends Vue {
    @Prop({ default: "" })
    title;
    @Prop({ type: Boolean, default: false })
    isShow;
    onClose() {
        this.$emit("update:isShow", false);
    }
    onOpen() {}
}
</script>
<style lang="less" scoped>
.dialog-box {
}
</style>
