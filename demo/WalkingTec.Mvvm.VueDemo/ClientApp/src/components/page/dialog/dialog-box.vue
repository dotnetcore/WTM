<template>
  <div class="dialog-wrap">
    <el-dialog v-el-draggable-dialog v-if="isDialog" :class="[componentClass]" v-bind="$attrs" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" v-on="$listeners">
      <slot />
    </el-dialog>
    <el-drawer v-else :class="[componentClass]" class="el-drawer-wrap" v-bind="$attrs" :visible="isShow" direction="rtl" size="50%" v-on="$listeners">
      <slot />
    </el-drawer>
  </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { SettingsModule } from "@/store/modules/settings";

@Component
export default class DialogBox extends Vue {
    @Prop({ type: Boolean, default: false })
    isShow;
    @Prop({ type: String, default: "" })
    componentClass;
    get isDialog() {
        return SettingsModule.isDialog;
    }
    // /**
    //  * 用法调用 携带sync
    //  * :is-show.sync="XXX"
    //  */
    // onClose() {
    //     // this.$emit("update:isShow", false);
    //     this.$emit("close", false);
    // }
}
</script>
<style lang="less">
.dialog-wrap {
}
.el-drawer-wrap {
    .el-drawer {
        overflow: auto;
    }
    .el-drawer__body {
        padding: 0 15px 15px;
    }
}
</style>
