<template>
  <div class="dialog-wrap">
    <el-dialog v-el-draggable-dialog v-if="isDialog" :class="[componentClass]" class="el-dialog-wrap" v-bind="$attrs" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" :title="titleValue" v-on="$listeners">
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
    @Prop({ type: String, default: "" })
    status;

    get isDialog() {
        return SettingsModule.isDialog;
    }
    get titleValue() {
        return this.$t(`table.${this.status}`);
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
.el-dialog-wrap {
    .el-dialog {
        max-height: 80%;
        overflow: auto;
        // .el-dialog__body {
        //     max-height: calc(100% - 60px);
        // }
    }
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
