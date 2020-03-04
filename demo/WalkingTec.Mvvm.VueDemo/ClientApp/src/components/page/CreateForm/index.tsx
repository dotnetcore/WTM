import { Component, Prop, Vue } from "vue-property-decorator";
import componentObj from "./utils";
@Component
export default class CreateForm extends Vue {
  @Prop({ default: false }) show!: boolean;
  @Prop() status;
  @Prop({ type: Object, required: true }) options;

  render(h) {
    const options = this.options;
    const formData = options.formData;
    const components = options.formItem.map(item => {
      let compItem = componentObj[item.type];
      let contentComp = compItem
        ? compItem.call(this, h, formData, item, this)
        : null;
      return componentObj.wtmformItem(h, item, contentComp);
    });
    const testSubmit = () => {
      console.log("formData", JSON.stringify(formData));
    };
    return (
      <el-form
        ref="form"
        {...{ props: options.formProps, attrs: { model: formData } }}
      >
        <el-row>{components}</el-row>
        <el-button onClick={testSubmit}>提交</el-button>
      </el-form>
    );
  }
}
