<template>
  <div class="dialog-box">
    <el-dialog v-if="dialog==='弹框'" v-bind="$attrs" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" @close="onClose" v-on="$listeners">
      <slot />
    </el-dialog>
    <el-drawer v-else class="el-drawer-box" v-bind="$attrs" :visible="isShow" direction="rtl" size="50%" @close="onClose" v-on="$listeners">
      <slot />
    </el-drawer>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { globalConfig } from "@/config/global";

@Component
export default class DialogBox extends Vue {
    @Prop({ type: Boolean, default: false })
    isShow;

    get dialog() {
        return globalConfig.dialog;
    }

    /**
     * 用法调用 携带sync
     * :is-show.sync="XXX"
     */
    onClose() {
        this.$emit("update:isShow", false);
    }
}
</script>
<style lang="less">
.dialog-box {
}
.el-drawer-box {
    .el-drawer {
        overflow: auto;
    }
    .el-drawer__body {
        padding: 0 15px 15px;
    }
}
</style>
