import { Component, Prop, Vue } from "vue-property-decorator";
import Utils from "./utils";
import WtmUploadImg from "@/components/page/UploadImg.vue";
import { ICreateFormOptions } from "./interface";
// 组件集合
const componentObj: any = new Utils();
/**
 * 创建form
 *
 * 注：
 *    formData虽然是prop参数，但多层级，不符合vue规范，但可以实现父级数据同步；
 *
 */
@Component({
  components: {
    "wtm-upload-img": WtmUploadImg,
  },
})
export default class CreateForm extends Vue {
  /**
   * 表单状态
   */
  @Prop({ type: String, default: "add" }) status;
  /**
   * 表单
   */
  @Prop() options!: ICreateFormOptions;
  /**
   * row class
   */
  @Prop({ type: String, default: "" }) elRowClass;
  /**
   * source 数据
   */
  @Prop() sourceFormData?: object;
  /**
   * 组件display
   */
  @Prop({ type: Boolean, default: false }) elDisabled;
  /**
   * 多语言key
   */
  @Prop({ type: String })
  languageKey?;
  /**
   * key替换'.'之后的数据
   */
  private formData: object = {};
  /**
   * 事件集合 @Prop({ type: Object, default: () => {} }) events;
   */
  private elFormRefKey: string = "ref_name";
  /**
   * 透传el-form组件
   */
  get elForm() {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    return refForm;
  }
  /**
   * 透传el-form，validate事件
   * 重定义callback，验证通过，data参数 改为表单数据
   */
  public validate(callback) {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    refForm.validate((valid, data) => {
      if (valid) {
        callback(true, this.getFormData());
      } else {
        callback(valid, data);
      }
    });
  }
  /**
   * 清空el-form验证
   */
  public resetFields() {
    const refForm = _.get(this.$refs, this.elFormRefKey);
    return refForm.resetFields();
  }
  /**
   * 'Entity.ID' => Entity: { ID }
   */
  public getFormData(): object {
    return this.sourceFormData || this.formData;
  }
  /**
   *  Entity: { ID } => 'Entity.ID'
   */
  public setFormData(data): object {
    let pointData: object = {};
    _.mapKeys(this.options.formItem, (value, key) => {
      this.setFormDataItem(key, _.get(data, key));
      pointData[key] = _.get(data, key);
    });
    return pointData;
  }

  /**
   * 赋值formData
   * @param path 字段对应路径 a: { b: { c: 1}} , path = a.b.c
   * @param value
   */
  public setFormDataItem(path: string, value: any) {
    _.set(this.sourceFormData || this.formData, path, value);
  }
  /**
   * 返回wtmformItem
   * @param key
   */
  public getFormItem(key): Vue | Element | Vue[] | Element[] {
    return this.$refs[key];
  }
  /**
   * 多语言
   * @param option
   */
  private getLanguageByKey({ label, key }) {
      return this.$getLanguageByKey({
          languageKey: this.languageKey,
          label,
          key
      });
  }
  private createFormData() {
    let newFormData = {};
    const formItem = this.options.formItem;
    _.mapKeys(formItem, (item, key) => {
      let value: any = "";
      if (_.isNil(item.defaultValue)) {
        value = ["switch"].includes(item.type) ? true : "";
      } else {
        value = item.defaultValue;
      }
      _.setWith(newFormData, key, value);
    });
    return newFormData;
  }

  created() {
    this.formData = this.createFormData();
  }
  render(h) {
    const components = _.keys(this.options.formItem).map((key) => {
      const item = this.options.formItem[key];
      if (_.isFunction(item.isHidden)) {
        if (item.isHidden(this.getFormData(), this.status)) {
          return;
        }
      }
      if ((_.isBoolean(item.isHidden) && item.isHidden) || !item.type) {
        return;
      }
      const itemComp = componentObj[item.type];
      const option = { ...item, key };
      const contentComp = itemComp ? itemComp.call(this, h, option) : null;
      return componentObj.wtmFormItem.call(this, h, option, contentComp);
    });
    const props = {
      ...this.options.formProps,
      disabled: this.status === "detail",
      model: this.sourceFormData || this.formData,
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

// private createFormData() {
// let formData = {};
// const formItem = this.options.formItem;
// _.mapKeys(formItem, (valule, key) => {
//   const newKey = this.KeyByString(key);
//   if (_.isNil(valule.defaultValue)) {
//     formData[newKey] = ["switch"].includes(valule.type) ? true : "";
//   } else {
//     formData[newKey] = valule.defaultValue;
//   }
// });
// return formData;
// }
// /**
//  * e.key => e_partition_key
//  */
// private KeyByPoint(key): string {
//   return key.replace(/_partition_/g, ".");
// }
// /**
//  * e_partition_key => e.key
//  */
// private KeyByString(key): string {
//   return key.replace(/(\.)/g, "_partition_");
// }
