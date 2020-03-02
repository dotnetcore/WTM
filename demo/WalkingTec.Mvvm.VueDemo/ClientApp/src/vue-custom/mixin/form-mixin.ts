import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import { Action } from "vuex-class";

/**
 * 弹出框（详情/编辑/创建）
 * dialogData：被编辑数据
 * status：弹出框的状态
 * actionType: 弹出框的状态-枚举
 * formData：提交表单结构
 *
 * @param defaultFormData
 *
 * 注：
 *     个性化请求，可以在组件中重新定义
 *
 */
interface formdata {
  refName?: string; // 表单名称
  formData: object; // 表单数据
}
function mixinFunc(defaultFormData: formdata = { formData: {} }) {
  @Component({
    components: {}
  })
  class formMixins extends Vue {
    @Action("add") add; // 添加 》store
    @Action("edit") edit; // 修改 》store
    @Action("detail") detail; // 详情 》store

    @Prop({ type: Object, default: () => {} })
    dialogData; // 表单传入数据
    @Prop({ type: String, default: "" })
    status; // 表单类型
    @Prop({ type: Boolean, default: false })
    isShow; // 弹框是否显示

    // 表单数据
    formData = {
      ..._.cloneDeep(defaultFormData.formData)
    };
    // 表单ref name
    refName: string = defaultFormData.refName || "refName";
    // 异步验证, 失败组件集合
    asynValidateEl: Array<any> = [];
    // 校验
    rules: object = {};
    /**
     * wtm-dialog-box所需属性
     */
    get formAttrs() {
      return {
        rules: this.rules,
        model: this.formData,
        status: this.status
      };
    }
    /**
     * wtm-dialog-box所需方法
     */
    get formEvent() {
      return {
        close: this.onClose,
        open: this.onBindFormData,
        onSubmit: this.onSubmitForm
      };
    }

    // 关闭
    onClose() {
      this.$emit("update:isShow", false);
      this.onReset();
    }
    // 重置&清除验证
    onReset() {
      this.formData = _.cloneDeep(defaultFormData.formData);
      this.cleanValidate();
    }
    /**
     * 表单数据 赋值
     * @param params
     */
    setFormData(params) {
      Object.keys(defaultFormData.formData).forEach(key => {
        if (
          _.isPlainObject(this.formData[key]) ||
          _.isArray(this.formData[key])
        ) {
          // Entity
          Object.keys(this.formData[key]).forEach(item => {
            this.formData[key][item] = _.get(params, key + "." + item);
          });
        } else {
          this.formData[key] = params[key];
        }
      });
    }
    /**
     * 展示验证
     */
    showResponseValidate(resForms: {}) {
      Object.keys(resForms).forEach(key => {
        const formItem = this.$refs[key];
        if (formItem) {
          formItem.showError(resForms[key]);
          this.asynValidateEl.push(key);
        }
      });
    }
    /**
     * 清理验证
     */
    cleanValidate(refName?: string) {
      this.asynValidateEl.forEach(key => this.$refs[key].clearValidate());
      const refForm = _.get(this, `$refs[${refName || this.refName}]`);
      refForm && this.$nextTick(() => refForm.resetFields());
    }
    /**
     * 打开详情
     */
    onBindFormData() {
      if (!this["dialogData"]) {
        console.log(this["dialogData"]);
        console.error("dialogData 没有id数据");
      }
      if (this["status"] !== this["$actionType"].add) {
        const parameters = { ...this["dialogData"], id: this["dialogData"].ID };
        this["detail"](parameters).then(res => {
          // 判断是否 有Entity 属性，赋值全部
          if (this.formData.hasOwnProperty("Entity")) {
            this.setFormData(res);
          } else {
            this.setFormData(res.Entity);
          }
          this["afterBindFormData"](res);
        });
      } else {
        this.onReset();
      }
    }
    /**
     * 查询详情-绑定数据 之后
     */
    afterBindFormData(data?: object) {
      console.log("data:", data);
    }
    /**
     * 提交
     */
    onSubmitForm() {
      this.$refs[this.refName].validate(valid => {
        if (valid) {
          if (this["status"] === this["$actionType"].add) {
            this.onAdd();
          } else if (this["status"] === this["$actionType"].edit) {
            this.onEdit();
          }
        }
      });
    }
    /**
     * 添加
     */
    onAdd(delID: string = "ID") {
      let parameters = _.cloneDeep(this["formData"]);
      if (parameters.Entity) {
        delete parameters.Entity[delID];
      } else {
        delete parameters[delID];
        parameters = { Entity: parameters };
      }
      this["add"](parameters)
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
    onEdit() {
      let parameters = _.cloneDeep(this["formData"]);
      if (!parameters.Entity) {
        parameters = { Entity: parameters };
      }
      this["edit"](parameters)
        .then(res => {
          this["$notify"]({
            title: "修改成功",
            type: "success"
          });
          this.onClose();
          this.$emit("onSearch");
        })
        .catch(error => {
          this.showResponseValidate(error.response.data.Form);
        });
    }
  }
  return formMixins;
}

export default mixinFunc;
