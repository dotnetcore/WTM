<template>
  <Item>
    <a-spin :spinning="spinning">
      <a-form-item v-bind="itemBind">
        <slot>
          <keep-alive v-if="includeComponent">
            <component :is="componentIs" v-bind="$props" @change="onChangeValue"></component>
          </keep-alive>
          <!-- 未配置 -->
          <template v-else>
            <span>
              未配置【
              <span v-text="_valueType"></span>
              】：
              <span v-text="value"></span>
            </span>
          </template>
        </slot>
      </a-form-item>
    </a-spin>
  </Item>
</template>
<script lang="ts">
import { Inject, mixins, Options } from "vue-property-decorator";
import { FieldBasics } from "./script";
import Item from "./item.vue";
import Field_text from "./views/text.vue";
import Field_password from "./views/password.vue";
import Field_textarea from "./views/textarea.vue";
import Field_rate from "./views/rate.vue";
import Field_radio from "./views/radio.vue";
import Field_checkbox from "./views/checkbox.vue";
import Field_date from "./views/date.vue";
import Field_dateWeek from "./views/dateWeek.vue";
import Field_dateMonth from "./views/dateMonth.vue";
import Field_dateRange from "./views/dateRange.vue";
import Field_select from "./views/select.vue";
import Field_slider from "./views/slider.vue";
import Field_switch from "./views/switch.vue";
import Field_transfer from "./views/transfer.vue";
import Field_image from "./views/image.vue";
import Field_upload from "./views/upload.vue";
import Field_grid from "./views/grid/index.vue";
import Field_editor from "./views/editor.vue";
import Field_icons from "./views/icons.vue";
@Options({
  components: {
    Item,
    Field_text,
    Field_password,
    Field_textarea,
    Field_rate,
    Field_radio,
    Field_checkbox,
    Field_date,
    Field_dateWeek,
    Field_dateMonth,
    Field_dateRange,
    Field_select,
    Field_slider,
    Field_switch,
    Field_transfer,
    Field_image,
    Field_upload,
    Field_grid,
    Field_editor,
    Field_icons
  }
})
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  // 表单类型
  @Inject({ default: '' }) readonly formType;
  /**
   * 包含注册的组件
   */
  get includeComponent() {
    return this.lodash.has(this.$options.components, this.componentIs)
  }
  /**
   * 动态组件 is
   */
  get componentIs() {
    return `Field_${this._valueType}`
  }
  async mounted() {
  }
}
</script>
<style lang="less">
</style>
