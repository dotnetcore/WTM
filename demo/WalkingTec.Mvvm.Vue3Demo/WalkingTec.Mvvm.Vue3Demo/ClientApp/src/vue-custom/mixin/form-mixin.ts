import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import { Action } from "vuex-class";

/**
 * 弹出框（详情/编辑/创建）
 * dialogData：被编辑数据
 * status：弹出框的状态
 * actionType: 弹出框的状态-枚举
 * 注：
 *     目前与CreateForm组件高度依赖，做一需要配合CreateForm组件
 */
function mixinFunc(defaultRefName: string = "el_form_name") {
  @Component
  class formMixins extends Vue {
    @Action("add") add; // 添加 》store
    @Action("edit") edit; // 修改 》store
    @Action("detail") detail; // 详情 》store
    // 表单传入数据
    @Prop({ type: Object, default: () => {} })
    dialogData;
    // 表单状态
    @Prop({ type: String, default: "" })
    status;
    // 弹框是否显示
    @Prop({ type: Boolean, default: false })
    isShow;
    /**
     * 补充表单数据,onAdd|onEdit会把当前数据合并到提交表单（formData）中
     * 例如：
     *    创建表单（CreateForm）时，需要用到自定义组件(type: wtmSlot),
     *    可以将自定义组件的数据 放到mergeFormData中操作，查询/编辑/新增 动作都会填充数据项
     */
    mergeFormData: any = {};
    /**
     * 表单ref name
     */
    refName: string = defaultRefName;
    /**
     * 异步验证, 失败组件集合(form-item类型是error需要)
     */
    asynValidateEl: Array<any> = [];
    /**
     * wtm-dialog-box所需方法
     */
    get formEvent() {
      return {
        close: this.onClose,
        opened: this.onOpen,
        onSubmit: this.onSubmit
      };
    }
    /**
     * 返回表单组件, this.$refs，get监听不到，改为方法
     */
    FormComp() {
      return _.get(this.$refs, this.refName);
    }
    /**
     * 关闭
     */
    onClose() {
      this.$emit("update:isShow", false);
      this.onReset();
    }
    /**
     * 重置&清除验证
     */
    onReset() {
      const comp = this.FormComp();
      if (comp) {
        this.asynValidateEl.forEach(key =>
          comp.getFormItem(key).clearValidate()
        );
        comp.resetFields();
        comp.setFormDataItem('Entity.ID', '');
      }
    }
    /**
     * 打开详情
     */
    onOpen() {
      if (!this.dialogData) {
        console.warn("dialogData 没有id数据", this.dialogData);
      }
      const params = this.beforeOpen(this.dialogData) || this.dialogData;
      if (this["status"] !== this["$actionType"].add) {
        this["detail"](params).then(res => {
          this.setFormData(res);
          this.afterOpen(res);
        });
      } else {
        this.onReset();
        this.afterOpen();
      }
    }
    /**
     * 查询详情-绑定数据 之前
     */
    beforeOpen(data?: object): object | void {
      console.log("data:", data);
      return data;
    }
    /**
     * 查询详情-绑定数据 之后
     */
    afterOpen(data?: object) {
      console.log("data:", data);
    }
    /**
     * 提交
     */
    onSubmit() {
      this.FormComp().validate((valid, data) => {
        if (valid) {
          if (this.status === this["$actionType"].add) {
            this.onAdd(data);
          } else if (this.status === this["$actionType"].edit) {
            this.onEdit(data);
          }
        }
      });
    }
    /**
     * 添加
     */
    onAdd(data: object | null = null) {
      let formData = this.getFormData(data);
      if (!formData.Entity["ID"]) {
        delete formData.Entity["ID"];
      }
      formData = this.beforeRequest(formData) || formData;
      return this.add(formData)
        .then(res => {
          this["$notify"]({
            title: "添加成功",
            type: "success"
          });
          this.onClose();
          this.$emit("onSearch");
        })
        .catch(error => {
          this.showResponseValidate(error.response.data.Form);
        });
    }
    /**
     * 编辑
     */
    onEdit(data: object | null = null) {
      let formData = this.getFormData(data);
      formData = this.beforeRequest(formData) || formData;
      return this.edit(formData)
        .then(res => {
          this["$notify"]({
            title: this.$t("form.SuccessfullyModified"),
            type: "success"
          });
          this.onClose();
          this.$emit("onSearch");
        })
        .catch(error => {
          this.showResponseValidate(error.response.data.Form);
        });
    }
    /**
     * submit请求 之前
     */
    beforeRequest(formData?: object): object | void {
      console.log("beforeRequest:", formData);
      return formData;
    }
    /**
     * get merge formdata
     */
    private getFormData(data: object | null = null) {
      let formData = data || this.FormComp().getFormData();
      // 处理 array
      const customizer = (objValue, srcValue) => {
        if (_.isArray(objValue)) {
          return srcValue;
        }
      };
      formData = _.mergeWith(formData, this.mergeFormData, customizer);
      return formData;
    }
    /**
     * set
     */
    private setFormData(data: Object) {
      // 填充表单数据
      const originData = this.FormComp().setFormData(data);
      // 填充补充表单数据
      _.mapKeys(originData, (value, key) => {
        if (_.get(this.mergeFormData, key) !== undefined) {
          _.set(this.mergeFormData, key, _.cloneDeep(value));
        }
      });
    }
    /**
     * 展示接口 验证错误提示
     */
    private showResponseValidate(resForms: {}) {
      _.mapKeys(resForms, (value, key) => {
        const formItem = this.FormComp().getFormItem(key);
        if (formItem) {
          formItem.showError(value);
          this.asynValidateEl.push(key);
        }
      });
    }
  }
  return formMixins;
}

export default mixinFunc;
