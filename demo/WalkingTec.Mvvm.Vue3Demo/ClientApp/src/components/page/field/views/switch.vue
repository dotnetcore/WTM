<template>
  <template v-if="_readonly">
    <a-switch :checked="value" disabled>
      <template #checkedChildren>
        <check-outlined />
      </template>
      <template #unCheckedChildren>
        <close-outlined />
      </template>
    </a-switch>
  </template>
  <template v-else>
    <a-switch v-model:checked="value" :disabled="disabled">
      <template #checkedChildren>
        <check-outlined />
      </template>
      <template #unCheckedChildren>
        <close-outlined />
      </template>
    </a-switch>
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
  @Inject({ default: '' }) readonly formType;
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
<style lang="less">
</style>
