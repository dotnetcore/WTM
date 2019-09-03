<template>
  <div class="dialog-box">
    <el-dialog v-bind="$attrs" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" @close="onClose" v-on="$listeners">
      <slot />
    </el-dialog>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class DialogBox extends Vue {
    @Prop({ type: Boolean, default: false })
    isShow;
    /**
     * 用法调用 携带sync
     * :is-show.sync="XXX"
     */
    onClose() {
        this.$emit("update:isShow", false);
    }
}
</script>
<style lang="less" scoped>
.dialog-box {
}
</style>
