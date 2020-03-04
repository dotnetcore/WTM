import { Component, Prop, Vue } from "vue-property-decorator";
import componentObj from "./utils";
@Component
export default class test extends Vue {
  @Prop({ default: false }) show!: boolean;
  UserItCode: string = "";
  formData = { UserItCode: "" };
  @Prop() status;

  onTest() {
    console.log("onTest", this.formData);
  }
  onTestBut(e) {
    console.log("onTestBut", this.formData, e);
  }
  render(h) {
    const comp = componentObj.input.call(
      this,
      h,
      this.formData,
      { key: "UserItCode" },
      this
    );
    // 测试
    const but = componentObj.button(h, this.formData, {
      events: { click: this.onTestBut },
      props: { type: "warning" },
      text: "测试but"
    });
    return (
      <el-form ref="form" v-model={this.formData} label-width="80px">
        <el-form-item label="用户Id">{comp}</el-form-item>
        {but}
      </el-form>
    );
  }
}
