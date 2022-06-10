<template>
  <template v-if="_readonly">
    <span v-text="value"></span>
  </template>
  <template v-else>
    <a-date-picker
      v-model="value"
      style="width:100%;"
      format="YYYY-MM-DD HH:mm:ss"
      :placeholder="value ? value : _placeholder"
      :disabled="disabled"
      v-bind="_fieldProps"
    />
  </template>
</template>
<script lang="ts">
import { $System, globalProperties } from "@/client";
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
  @Inject({ default: "" }) readonly formType;
  async mounted() {
  }
  methods: {
    //Moment,
  }
  created(){
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