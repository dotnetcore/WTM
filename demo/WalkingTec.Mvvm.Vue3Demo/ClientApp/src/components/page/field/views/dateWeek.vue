<template>
  <template v-if="_readonly">
    <span v-text="value"></span>
  </template>
  <template v-else>
    <a-week-picker
      v-model:value="value"
      :placeholder="_placeholder"
      :disabled="disabled"
      v-bind="_fieldProps"
    />
  </template>
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
  async mounted() {
    // this.onRequest();
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
}
</script>
<style lang="less"></style>
