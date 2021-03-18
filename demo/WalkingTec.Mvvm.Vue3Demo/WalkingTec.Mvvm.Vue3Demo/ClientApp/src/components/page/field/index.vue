<template>
  <Item>
    <a-form-item v-bind="bind" hasFeedback>
      <slot>
        <a-input
          v-model:value="value"
          :placeholder="_placeholder"
          autocomplete="off"
        />
      </slot>
    </a-form-item>
  </Item>
</template>
<script lang="ts">
import { Vue, Options, Prop, Inject } from "vue-property-decorator";
import Item from "./item.vue";
@Options({ components: { Item } })
export default class extends Vue {
  @Prop({ type: String }) label;
  @Prop({ type: String }) name;
  @Prop({ type: String }) placeholder;
  @Prop({ type: String }) entityKey;
  @Prop({ type: Boolean, default: false }) readonly;
  @Prop({ type: Boolean, default: false }) disabled;
  @Inject() formState;
  @Inject() PageEntity;
  get bind() {
    // if (this.entityKey) {
    return {
      label: this._label,
      name: this._name,
      rules: this._rules,
    };
    // }
  }
  get _label() {
    const label =
      this.lodash.get(this.PageEntity, `${this.entityKey}.label`, this.label) || this._name;
    try {
      return label ? this.$t(label) : label;
    } catch (error) {
      return label;
    }
  }
  get _name() {
    return this.lodash.get(
      this.PageEntity,
      `${this.entityKey}.name`,
      this.name
    );
  }
  get _placeholder() {
    return this.lodash.get(
      this.PageEntity,
      `${this.entityKey}.placeholder`,
      this.placeholder
    );
  }
  get _rules() {
    return this.lodash.get(this.PageEntity, `${this.entityKey}.rules`);
  }
  get value() {
    return this.lodash.get(this.formState, this._name);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  mounted() {
    // console.log("");
    // console.group(`Field ~ ${this.entityKey} ${this._name} `);
    // console.log(this);
    // console.groupEnd();
  }
}
</script>
<style lang="less">
</style>
