import { Component, Prop, Vue } from "vue-property-decorator";
import Utils from "./utils";
import WtmUploadImg from "@/components/page/UploadImg.vue";
/**
 * 创建form
 *
 * 注：
 *    formData虽然是prop参数，但多层级，不符合vue规范，但可以实现父级数据同步；
 *
 */
@Component({
  components: { "wtm-upload-img": WtmUploadImg }
})
export default class CreateForm extends Vue {
  // 表单状态
  @Prop({ type: String, default: "add" }) status;
  // 表单
  @Prop({ type: Object, required: true }) options;
  // row class
  @Prop({ type: String, default: "" }) elRowClass;

  // 事件集合 @Prop({ type: Object, default: () => {} }) events;
  elFormRefKey: string = "ref_name";
  componentObj: any = new Utils();
  // key替换'.'之后的数据
  formData: object = {};
  // key包含'.'的数据
  originData: object = {};
  /**
   * 透传el-form，validate事件
   */
  get validate() {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    return refForm.validate;
  }
  /**
   * 透传el-form组件
   */
  get elForm() {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    return refForm;
  }
  /**
   * 清空el-form验证
   */
  resetFields() {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    return refForm.resetFields();
  }
  /**
   * 'Entity.ID' => Entity: { ID }
   */
  getFormData() {
    let newFormData = {};
    _.mapKeys(this.formData, (value, key) => {
      const oldKey = key.replace(/_partition_/g, ".");
      _.setWith(newFormData, oldKey, value);
    });
    return newFormData;
  }
  /**
   *  Entity: { ID } => 'Entity.ID'
   */
  setFormData(data) {
    _.mapKeys(this.options.formItem, (value, key) => {
      const newKey = key.replace(/(\.)/g, "_partition_");
      this.formData[newKey] = _.get(data, key);
      this.originData[key] = _.get(data, key);
    });
    return this.originData;
  }
  /**
   * 返回wtmformItem
   * @param key
   */
  getFormItem(key) {
    const newKey = key.replace(/(\.)/g, "_partition_");
    return this.$refs[newKey];
  }

  createFormData() {
    let formData = {};
    const formItem = this.options.formItem;
    _.mapKeys(formItem, (valule, key) => {
      const newKey = key.replace(/(\.)/g, "_partition_");
      formData[newKey] = _.isNil(valule.defaultValue)
        ? ""
        : valule.defaultValue;
    });
    return formData;
  }

  created() {
    this.formData = this.createFormData();
  }
  render(h) {
    const components = _.keys(this.formData).map(key => {
      const oldKey = key.replace(/_partition_/g, ".");
      const item = this.options.formItem[oldKey];
      if (_.isFunction(item.isHidden)) {
        if (item.isHidden(this.getFormData(), status)) {
          return;
        }
      }
      if ((_.isBoolean(item.isHidden) && item.isHidden) || !item.type) {
        return;
      }
      let compItem = this.componentObj[item.type];
      let contentComp = compItem
        ? compItem.call(this, h, { ...item, key })
        : null;
      return this.componentObj.wtmFormItem.call(
        this,
        h,
        { ...item, key },
        contentComp
      );
    });
    const props = {
      ...this.options.formProps,
      model: this.formData
    };
    const slots = this.$scopedSlots["default"];
    return (
      <el-form ref={this.elFormRefKey} {...{ props }}>
        <el-row class={this.elRowClass}>
          {components}
          {slots && slots({})}
        </el-row>
      </el-form>
    );
  }
}
