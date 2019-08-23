import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import { dialogType } from "@/config/enum";
import DialogFooter from "@/components/common/dialog/dialog-footer.vue";
import EditBox from "@/components/common/edit-box.vue";

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
    refName?: string;
    formData: object;
}
function mixinFunc(defaultFormData: formdata = { formData: {} }) {
    @Component({
        components: { DialogFooter, EditBox }
        })
    class editMixins extends Vue {
        @Prop({ type: Object, default: {} })
        dialogData; // 表单传入数据
        @Prop({ type: String, default: "" })
        status; // 表单类型
        @Prop({ type: Boolean, default: false })
        isShow; // 弹框是否显示
        dialogType = dialogType; // 弹框类型 add/edit/detail
        // 表单数据
        formData = {
            ...defaultFormData.formData
        };
        // 表单ref name
        refName = defaultFormData.refName;
        // 关闭
        onClear() {
            this.$emit("update:isShow", false);
            this.onReset();
        }
        // 重置&清除验证
        onReset() {
            this.setFormData(defaultFormData.formData);
            // if (key !== "SelectedItemsID") {}
            if (this.refName) {
                // 去除搜索中的error信息
                _.get(this, `$refs[${this.refName}]`).resetFields();
            }
        }
        // 表单数据 赋值
        setFormData(params) {
            Object.keys(defaultFormData.formData).forEach(key => {
                // if (key in params) {}
                this.formData[key] = params[key];
            });
        }
    }
    return editMixins;
}

export default mixinFunc;
