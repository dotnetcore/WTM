<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <a-row>
      <a-col :span="8">
        <span>readonly (只读) </span>
        <a-switch
          checked-children="开"
          un-checked-children="关"
          v-model:checked="readonly"
        />
      </a-col>
      <a-col :span="8">
        <span>disabled (禁用) </span>
        <a-switch
          checked-children="开"
          un-checked-children="关"
          v-model:checked="disabled"
        />
      </a-col>
      <a-col :span="8">
        <!-- <a-switch checked-children="开" un-checked-children="关" v-model:checked="checked1" /> -->
      </a-col>
    </a-row>
    <a-divider>WtmField</a-divider>
    <WtmField
      v-for="item in PageEntitys"
      :key="item.name"
      :entityKey="item.name"
      :fieldProps="item.fieldProps"
      :readonly="readonly"
      :disabled="disabled"
    />
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController, PageEntity } from "../controller";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    // image2: ["08d96fac-773a-4d18-8237-91d30145f392"]
  };
  PageEntity = PageEntity;
  PageEntitys = [];
  readonly = false;
  disabled = false;
  /**
   * 传递给 details 组件的 提交函数 返回一个 Promise
   * @param values
   * @returns
   */
  async onFinish(values) {
    console.log("LENG ~ extends ~ onFinish ~ values", values);
    throw values;
  }
  created() {}
  mounted() {
    this.PageEntitys = this.lodash.map(this.PageEntity, item => {
      return item;
    });
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
