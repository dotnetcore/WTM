<template>
  <Item>
    <a-spin :spinning="spinning">
      <a-form-item v-bind="itemBind">
        <slot>
          <!-- 只读 -->
          <template v-if="readonly">
            <span v-text="value"></span>
          </template>
          <!-- 可编辑 -->
          <template v-else>
            <!-- 文本 text -->
            <template v-if="_valueType === 'text'">
              <a-input
                v-model:value="value"
                :placeholder="_placeholder"
                :disabled="disabled"
                autocomplete="off"
              />
            </template>
            <!-- 文本 textarea -->
            <template v-else-if="_valueType === 'textarea'">
              <a-textarea
                v-model:value="value"
                :placeholder="_placeholder"
                :auto-size="{ minRows: 2, maxRows: 5 }"
              />
            </template>
            <!-- 单选 radio -->
            <template v-else-if="_valueType === 'radio'">
              <a-radio-group v-model:value="value">
                <a-radio v-for="item in dataSource" :key="item.value" :value="item.value">
                  <span v-text="item.label"></span>
                </a-radio>
              </a-radio-group>
            </template>
            <!-- 多选 checkbox -->
            <template v-else-if="_valueType === 'checkbox'">
              <a-checkbox-group v-model:value="value" :options="dataSource" />
            </template>
            <!-- 未配置 -->
            <template v-else>
              <span>
                没有找到类型【
                <span v-text="_valueType"></span>】：
                <span v-text="value"></span>
              </span>
            </template>
          </template>
        </slot>
      </a-form-item>
    </a-spin>
  </Item>
</template>
<script lang="ts">
import { WTM_ValueType } from "@/client";
import { Inject, Options, Prop, Vue } from "vue-property-decorator";
import Item from "./item.vue";
import Text from "./views/text.vue";
@Options({ components: { Item, Text } })
export default class extends Vue {
  // form label 没有 取 name 数据
  @Prop({ type: String }) readonly label;
  // form name
  @Prop({ type: String }) readonly name;
  // 输入提示
  @Prop({ type: String }) readonly placeholder;
  // 当前实体对应的 属性key
  @Prop({ type: String }) readonly entityKey;
  // 值类型
  @Prop({ type: String, default: "text" }) readonly valueType: WTM_ValueType;
  // 只读
  @Prop({ type: Boolean, default: false }) readonly readonly;
  // 禁用
  @Prop({ type: Boolean, default: false }) readonly disabled;
  // 测试日志
  @Prop({ type: Boolean, default: false }) readonly debug;
  // 数据源
  @Prop({ type: Function, default: () => [] }) readonly request;
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  // 数据加载
  spinning = false;
  // 数据源
  dataSource: Array<{ label: any; value: any }> = [];
  /**  form-item 属性 */
  get itemBind() {
    const label = this.$t(this._label),
      name = this._name,
      valueType = this._valueType;
    const formValidate = this.lodash.get(this.formValidate, this.lodash.isArray(name) ? name.join('.') : name, {})
    return {
      label: label || name,
      name: name,
      rules: this._rules,
      hasFeedback: this.lodash.includes(["text"], valueType),
      ...formValidate
    };
  }
  // form-item lable
  get _label() {
    const label = this.lodash.head(this.lodash.compact([
      // 优先获取 Props 配置
      this.label,
      // 获取 Entity 配置
      this.lodash.get(this.PageEntity, `${this.entityKey}.label`),
      'label 未配置'
    ]))
    return label;
  }
  // form-item name
  get _name() {
    const name = this.lodash.head(this.lodash.compact([
      // 优先获取 Props 配置
      this.name,
      // 获取 Entity 配置
      this.lodash.get(this.PageEntity, `${this.entityKey}.name`),
      '__name'
    ]));
    if (name === '__name') {
      console.warn('name 未配置', this)
    }
    return name;
  }
  // 输入提示
  get _placeholder() {
    const placeholder = this.lodash.head(this.lodash.compact([
      // 优先获取 Props 配置
      this.placeholder,
      // 获取 Entity 配置
      this.lodash.get(this.PageEntity, `${this.entityKey}.placeholder`),
      // this.$t(localesKey, { label: $i18n.t(label) }),
      // 转换 执行 toPlaceholder 
      this.lodash.invoke(this.$i18n, 'toPlaceholder', this._label),
      'placeholder'
    ]));
    return placeholder;
  }
  // form 校验规则
  get _rules() {
    return this.lodash.get(this.PageEntity, `${this.entityKey}.rules`);
  }
  // 属性值 v-model:value
  get value() {
    return this.lodash.get(this.formState, this._name);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  // 属性值类型
  get _valueType(): WTM_ValueType {
    return this.lodash.get(
      this.PageEntity,
      `${this.entityKey}.valueType`,
      this.valueType
    );
  }
  // 数据源
  get _request() {
    return this.lodash.get(
      this.PageEntity,
      `${this.entityKey}.request`,
      this.request
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
      this.dataSource = res;
    } catch (error) {
      console.error("LENG ~ onRequest", error)
    }
    this.spinning = false;
  }
  async mounted() {
    this.onRequest();
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
