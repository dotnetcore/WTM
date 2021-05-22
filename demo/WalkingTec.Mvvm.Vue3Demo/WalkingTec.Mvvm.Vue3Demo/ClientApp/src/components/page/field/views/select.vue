<template>
  <a-spin :spinning="spinning">
    <template v-if="_readonly">
      <span v-text="readonlyText"></span>
    </template>
    <template v-else>
      <a-select
        v-model:value="value"
        :placeholder="_placeholder"
        :disabled="disabled"
        :options="dataSource"
        allowClear
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
  @Inject({ default: '' }) readonly formType;
  get readonlyText() {
    // if (this.lodash.isArray(this.value)) {
    //   const filters = this.lodash.filter(this.dataSource, item => this.lodash.includes(this.value, String(item.value)));
    //   return this.lodash.map(filters, 'label').join(' / ')
    // }
    const filters = this.lodash.filter(this.dataSource, item => this.lodash.includes(this.value, item.value));
    return this.lodash.map(filters, 'label').join(' / ')
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
}
</script>
<style lang="less">
</style>
