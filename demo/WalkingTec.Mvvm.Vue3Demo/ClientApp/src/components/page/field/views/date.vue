<template>
  <template v-if="_readonly">
    <span v-text="value"></span>
  </template>
  <template v-else>
    <a-date-picker
      v-model:value="value"
      style="width:100%;"
      format="YYYY-MM-DD"
      :placeholder="_placeholder"
      :disabled="disabled"
      v-bind="_fieldProps"
    />
  </template>
</template>
<script lang="ts">
import { $System, globalProperties } from "@/client";
import { Vue, Options, Watch, mixins, Inject } from "vue-property-decorator";
import dayjs, { Dayjs } from 'dayjs';
import { defineComponent, ref } from 'vue';
import { FieldBasics } from "../script";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  @Inject({ default: "" }) readonly formType;
  get value() {
       let data = this.lodash.get(this.formState, this._name)
       return data ? dayjs(data, 'YYYY-MM-DD') : ''
  }
  set value(value) {
      this.lodash.set(this.formState, this._name, value);
  }
}
</script>
<style lang="less">

</style>