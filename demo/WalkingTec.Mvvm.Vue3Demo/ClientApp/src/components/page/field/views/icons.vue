<template>
  <template v-if="_readonly">
    <span v-text="value"></span>
  </template>
  <template v-else>
    <a-select
      v-model:value="value"
      :placeholder="_placeholder"
      :disabled="disabled"
      allowClear
      v-bind="_fieldProps"
    >
      <a-select-opt-group
        v-for="icons in dataSource"
        :key="icons.name"
        :label="icons.name"
      >
        <a-select-option v-for="item in icons.icons" :key="item" :value="icons.class + ' ' + item">
          <a-row>
            <a-col :span="4">
              <span :class="icons.class + ' ' + item"></span
            ></a-col>
            <a-col :span="20"> <span v-text="item"></span></a-col>
          </a-row>
        </a-select-option>
      </a-select-opt-group>
    </a-select>
  </template>
</template>
<script lang="ts">
import { Vue, Options, Prop, mixins, Inject } from "vue-property-decorator";
import { FieldBasics } from "../script";
import fonts from "@/assets/font/font";
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
    this.dataSource = fonts;
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
