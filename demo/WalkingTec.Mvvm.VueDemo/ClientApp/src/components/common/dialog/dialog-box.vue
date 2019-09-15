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
import cache from "@/util/cache";
import config from "@/config/index";
@Component
export default class DialogBox extends Vue {
    @Prop({ type: Boolean, default: false })
    isShow;

    dialog: string = "弹框";
    /**
     * 用法调用 携带sync
     * :is-show.sync="XXX"
     */
    onClose() {
        this.$emit("update:isShow", false);
    }
    created() {
        const userGlobal = cache.getCookieJson(config.globalKey) || {};
        this.dialog = userGlobal.dialog;
    }
}
</script>
<style lang="less">
.dialog-box {
}
.el-drawer-box {
    .el-drawer__body {
        padding: 0 15px;
    }
}
</style>
