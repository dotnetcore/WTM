import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import { dialogType } from "@/config/enum";
import DialogFooter from "@/components/common/dialog/dialog-footer.vue";

/**
 * 弹出框（详情/编辑/创建）
 * dialogData：被编辑数据
 * status：弹出框的状态
 * dialogType： 弹出框的状态-枚举
 * formData：提交表单结构
 *
 * @param defaultFormData
 * defaultFormData 结构
 * {
 * refName: 表单名称
 * formData: 表单数据
 * }
 */
interface formdata {
    refName?: String;
    formData: Object;
}
function mixinFunc(defaultFormData: formdata = { formData: {} }) {
    @Component({
        directives: {
        display: {
        inserted: (el, binding) => {
        if (binding.modifiers["status"] === dialogType.add) {
        el.innerHTML = binding.value;
        }
        }
        }
        },
        components: { DialogFooter }
        })
    class editMixins extends Vue {
        @Prop({ type: Object, default: {} })
        dialogData;
        @Prop({ type: String, default: "" })
        status;
        dialogType = dialogType;
        // 表单
        formData = {
            ...defaultFormData.formData
        };
        refName = defaultFormData.refName;
        @Watch("dialogData")
        setDialogData(val) {
            this.formData = { ...val };
            if (this.status === dialogType.detail) {
            } else if (this.status === dialogType.add) {
                this.onReset();
            } else {
            }
        }
        // 关闭
        onClear() {
            this.$emit("update:isShow", false);
            this.onReset();
        }
        // 重置&清除验证
        onReset() {
            Object.keys(defaultFormData.formData).forEach(key => {
                if (key !== "SelectedItemsID") {
                    this.formData[key] = defaultFormData.formData[key];
                }
            });
            if (defaultFormData.refName) {
                //去除搜索中的error信息
                _.get(this, `$refs[${defaultFormData.refName}]`).resetFields();
            }
        }
    }
    return editMixins;
}

export default mixinFunc;
