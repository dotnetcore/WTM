<template>
    <div class="dialog-wrap">
        <el-dialog v-el-draggable-dialog v-if="isDialog" :class="[componentClass]" class="el-dialog-wrap" v-bind="dialogAttrs" :visible="isShow" :modal-append-to-body="true" :append-to-body="true" v-on="dialogEvent">
            <el-form ref="ref_name" :model="modelPvt" :rules="dialogAttrs.rules" :label-width="dialogAttrs.labelWidth">
                <el-row>
                    <slot />
                </el-row>
            </el-form>
            <dialog-footer :status="dialogAttrs.status" @onClose="onClose" @onSubmit="onSubmit" />
        </el-dialog>
        <el-drawer v-else :class="[componentClass]" class="el-drawer-wrap" v-bind="dialogAttrs" :visible="isShow" direction="rtl" size="50%" v-on="dialogEvent">
            <el-form ref="ref_name" :model="modelPvt" :rules="dialogAttrs.rules" :label-width="dialogAttrs.labelWidth">
                <el-row>
                    <slot />
                </el-row>
            </el-form>
            <dialog-footer :status="dialogAttrs.status" @onClose="onClose" @onSubmit="onSubmit" />
        </el-drawer>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop, Provide } from "vue-property-decorator";
import { SettingsModule } from "@/store/modules/settings";
import DialogFooter from "./dialog-footer.vue";
/**
 * 弹出框
 * 包含表单（el-form），并透传（el-form）组件的方法/对象  ：
 *    resetFields： 清空方法
 *    validate：验证方法
 *    elForm：el-form组件对象
 *
 */
@Component({
    name: "wtm-dialog",
    components: { DialogFooter }
})
export default class DialogBox extends Vue {
    @Provide()
    componentName = "wtmDialog";
    @Prop({ type: Boolean, default: false })
    isShow;
    @Prop({ type: String, default: "" })
    componentClass; // 添加样式 el-dialog/el-drawer 样式控制
    @Prop({ type: String, default: "" })
    status; // 打开后的状态，新增/详情/编辑
    @Prop({ type: String, default: "100px" })
    labelWidth;
    @Prop({ type: Object, default: () => {} })
    rules; // 验证
    @Prop({ type: Object })
    model; // formData
    @Prop({ type: String, default: "" })
    title; // title

    @Prop({ type: Object, default: () => {} })
    events; // 事件集合
    @Prop({ type: Object, default: () => {} })
    attrs; // 属性集合

    // 事件
    get dialogEvent() {
        const envObj = Object.assign({}, this.events, this.$listeners);
        return envObj;
    }
    // 属性
    get dialogAttrs() {
        const attrsObj = Object.assign(
            { labelWidth: this.labelWidth },
            this.attrs,
            this.$attrs
        );
        return {
            ...attrsObj,
            title: this.titlePvt
        };
    }
    /**
     * 弹出模式
     */
    get isDialog() {
        return SettingsModule.isDialog;
    }
    // title
    get titlePvt() {
        return this.title || this.$t(`table.${this.status}`);
    }
    get modelPvt() {
        if (!this.model || this.model === {}) {
            return this.dialogAttrs.model || {};
        }
        return this.model;
    }
    /**
     * 关闭事件
     */
    get onClose() {
        return this.dialogEvent.close;
    }
    /**
     * 提交事件
     */
    get onSubmit() {
        if (!!this.dialogEvent.onSubmit) {
            return this.dialogEvent.onSubmit;
        } else {
            return () => {};
        }
    }
    /**
     * 透传el-form组件
     */
    get elForm() {
        const refForm = _.get(this, `$refs.ref_name`);
        return refForm;
    }
    /**
     * 透传el-form，validate事件
     */
    get validate() {
        const refForm = _.get(this, `$refs.ref_name`);
        return refForm.validate;
    }
    /**
     * 清空el-form验证
     */
    resetFields() {
        const refForm = _.get(this, `$refs.ref_name`);
        refForm.resetFields();
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
@media only screen and (min-width: 1200px) {
    .el-dialog-wrap {
        .el-dialog {
            width: 60%;
        }
    }
}
@media only screen and (max-width: 1119px) {
    .el-dialog-wrap {
        .el-dialog {
            width: 70%;
        }
    }
}
@media only screen and (max-width: 992px) {
    .el-dialog-wrap {
        .el-dialog {
            width: 80%;
        }
    }
}
@media only screen and (max-width: 768px) {
    .el-dialog-wrap {
        .el-dialog {
            width: 100%;
            height: 100%;
        }
    }
}
</style>
