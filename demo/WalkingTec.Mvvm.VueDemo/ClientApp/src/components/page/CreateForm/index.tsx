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

    return (
      <el-form ref="form" v-model={formData} {...{ props: options.formProps }}>
        <el-row>{components}</el-row>
      </el-form>
    );
  }
}
