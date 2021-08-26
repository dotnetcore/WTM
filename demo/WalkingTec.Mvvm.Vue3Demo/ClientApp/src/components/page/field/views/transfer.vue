<template>
  <a-spin :spinning="spinning">
    <template v-if="_readonly">
      <span v-text="readonlyText"></span>
    </template>
    <template v-else>
      <a-transfer
        :dataSource="dataSource"
        :target-keys="targetKeys"
        :selected-keys="selectedKeys"
        :render="renderTitle"
        :placeholder="_placeholder"
        :disabled="disabled"
        @change="onChange"
        @selectChange="onSelectChange"
        v-bind="_fieldProps"
      />
    </template>
  </a-spin>
</template>
<script lang="ts">
import { Vue, Options, Prop, mixins, Inject } from "vue-property-decorator";
import { FieldBasics } from "../script";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  // 表单类型
  @Inject({ default: "" }) readonly formType;
  get readonlyText() {
    const filters = this.lodash.filter(this.dataSource, item =>
      this.lodash.includes(this.value, String(item.value))
    );
    return this.lodash.map(filters, "label").join(" / ");
  }
  get targetKeys() {
    return this.value;
  }
  get selectedKeys() {
    return;
  }
  get titles() {
    return [];
  }
  renderTitle(item) {
    return item.label;
  }
  onChange(nextTargetKeys: string[], direction: string, moveKeys: string[]) {
    this.value = nextTargetKeys;
  }
  onSelectChange(sourceSelectedKeys: string[], targetSelectedKeys: string[]) {
    console.log(
      "🚀 ~ file: transfer.vue",
      sourceSelectedKeys,
      targetSelectedKeys,
      this
    );
  }
  // 加载数据源
  async onRequest() {
    this.spinning = true;
    try {
      const res = await this.lodash.invoke(
        this,
        "_request",
        this.lodash.cloneDeep(this.formState)
      );
      this.dataSource = this.lodash.map(res, item => {
        return this.lodash.assign(
          { title: item.label, description: item.label, disabled: false },
          item
        );
      });
    } catch (error) {
      console.error("LENG ~ onRequest", error);
    }
    this.spinning = false;
  }
  async mounted() {
    this.onRequest();
    this.onLinkage();
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
  beforeUnmount() {
    // this.dataSource = []
  }
}
</script>
<style lang="less"></style>
