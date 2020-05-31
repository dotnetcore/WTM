import { Component, Prop, Vue, Provide } from "vue-property-decorator";
/**
 *
 */
@Component({
  name: "wtm-search",
  components: {}
})
export default class CreateForm extends Vue {
  @Provide()
  componentName = "wtmSearch";
  // 请求表单
  @Prop({ type: Object, default: () => {} })
  formOptions;
  // source 数据
  @Prop() sourceFormData?: object;
  // 是否需要折叠面板
  @Prop({ type: Boolean, default: false })
  needCollapse!: boolean;
  // 是否展开折叠
  @Prop({ type: Boolean, default: false })
  isActive!: boolean;
  // 是否需要重置按钮
  @Prop({ default: true })
  needResetBtn!: boolean;
  // 禁用input，通常是在加载的时候禁用
  @Prop({ default: false })
  disabledInput!: boolean;
  // 执行事件集合
  @Prop({ type: Object, default: null })
  events!: object;

  refName: string = "searchRefName";
  /**
   * 异步验证, 失败组件集合(form-item类型是error需要)
   */
  asynValidateEl: Array<any> = [];
  /**
   * 返回表单组件, this.$refs，get监听不到，改为方法
   */
  FormComp() {
    return _.get(this.$refs, this.refName);
  }
  /**
   * 表单数据
   */
  public getFormData() {
    return this.FormComp().getFormData();
  }
  /**
   * 返回wtmformItem
   * @param key
   */
  public getFormItem(key): Vue | Element | Vue[] | Element[] {
    return this.FormComp().getFormItem(key);
  }
  /**
   * 展示接口 验证错误提示
   */
  public showResponseValidate(resForms: {}) {
    _.mapKeys(resForms, (value, key) => {
      const formItem = this.getFormItem(key);
      if (formItem) {
        formItem.showError(value);
        this.asynValidateEl.push(key);
      }
    });
  }
  onSearch() {
    this.asynValidateEl.forEach(key =>
      this.getFormItem(key).clearValidate()
    );
    if (this.events && this.events["onSearch"]) {
      this.events["onSearch"]();
    } else {
      this.$emit("onSearch");
    }
  }
  toggleCollapse() {
    this.$emit("update:isActive", !this.isActive);
  }
  onReset() {
    this.FormComp().resetFields();
    this.onSearch();
  }
  get slotList() {
    let arr: Array<any> = [];
    _.mapValues(this.formOptions.formItem, item => {
      if (item.slotKey) {
        arr.push(item.slotKey);
      }
    });
    return arr;
  }
  render(h) {
    let arr: Array<any> = [];
    _.mapValues(this.formOptions.formItem, item => {
      if (item.slotKey) {
        const fn = this.$scopedSlots[item.slotKey] || (() => {});
        arr.push({ key: item.slotKey, value: fn({}) });
      }
    });
    return (
      <el-card class="search-box" shadow="never">
        <wtm-create-form
          ref={this.refName}
          options={this.formOptions}
          sourceFormData={this.sourceFormData}
          elRowClass="flex-container"
        >
          {arr.map(item => {
            return <span slot={item.key}>{item.value}</span>;
          })}
          <wtm-form-item class="search-but-box">
            <el-button-group class="button-group">
              <el-button
                type="primary"
                class="btn-search"
                icon="el-icon-search"
                disabled={this.disabledInput}
                on-click={this.onSearch}
              >
                { this.$t("buttom.search") }
              </el-button>
              {this.needCollapse ? (
                <el-button
                  type="primary"
                  on-click={this.toggleCollapse}
                  style="padding-left: 7px; padding-right: 7px;"
                >
                  <i
                    class={{
                      "is-active": this.isActive,
                      fa: true,
                      "arrow-down": true,
                      "el-icon-arrow-down": true
                    }}
                  />
                </el-button>
              ) : (
                ""
              )}
            </el-button-group>
            {this.needResetBtn ? (
              <el-button
                style="position: relative;margin-left: 10px;"
                plain
                type="primary"
                icon="el-icon-refresh"
                on-click={this.onReset}
              >
                { this.$t("buttom.reset") }
              </el-button>
            ) : (
              ""
            )}
          </wtm-form-item>
        </wtm-create-form>
      </el-card>
    );
  }
}
