<template>
  <template v-if="_readonly">
    <span v-text="value"></span>
  </template>
  <template v-else>
    <a-date-picker
      v-model:value="value"
      style="width:100%;"
      format="YYYY-MM-DD HH:mm:ss"
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
/*  get value(){
     return ref<Dayjs>(dayjs('2015-06-06', 'YYYY-MM-DD'))
  }*/
  get value() {
      return this.lodash.get(this.formState, this._name)
  }
  set value(value) {
      this.lodash.set(this.formState, this._name, value);
  }
  async mounted() {
    this.onValueChange(this.value, undefined);
  }
  @Watch("value")
  onValueChange(val, old) {
      if(val){
        this.value = ref<Dayjs>(dayjs(val, 'YYYY-MM-DD'))
      }
  }
}
</script>
<style lang="less">
input::-webkit-input-placeholder{
  color:rgba(0, 0, 0, 0.85)!important;
}

input::-moz-placeholder{   /* Mozilla Firefox 19+ */
  color:rgba(0, 0, 0, 0.85)!important;
}

input:-moz-placeholder{    /* Mozilla Firefox 4 to 18 */
  color:rgba(0, 0, 0, 0.85)!important;
}

input:-ms-input-placeholder{  /* Internet Explorer 10-11 */
  color:rgba(0, 0, 0, 0.85)!important;
}
</style>