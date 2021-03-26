<template>
  <template v-if="readonly">
    <span v-text="$attrs.value"></span>
  </template>
  <template v-else>
    <a-input
      v-model="value"
      :placeholder="placeholder"
      :disabled="disabled"
      autocomplete="off"
    />
  </template>
</template>
<script lang="ts">
import { Vue, Options, Prop, Model } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Model("modelValue", {})
  readonly value: string;
  @Prop({ type: String }) placeholder;
  @Prop({ type: Boolean, default: false }) readonly;
  @Prop({ type: Boolean, default: false }) disabled;
  mounted() {}
}
</script>
<style lang="less">
</style>
