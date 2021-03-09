<template>
    <div slot="footer" class="dialog-footer">
        <el-button @click="onClose">
            {{ $t("buttom.closed")}}
        </el-button>
        <el-button v-if="status !== detail" type="primary" @click="onSubmit">
            {{ $t("buttom.submit")}}
        </el-button>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class DialogFooter extends Vue {
    @Prop({ type: String, default: "" })
    status;

    get detail() {
        return this.$actionType.detail;
    }
    onClose() {
        this.$emit("onClose");
    }
    onSubmit() {
        this.$emit("onSubmit");
    }
}
</script>
<style lang="less" >
@import "~@/assets/css/mixin.less";
.dialog-footer {
    .flexbox(row, flex-end);
}
</style>
