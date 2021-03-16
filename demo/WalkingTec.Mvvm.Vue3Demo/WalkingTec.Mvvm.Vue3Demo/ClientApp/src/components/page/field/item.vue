<template>
  <a-col v-if="colProps?.colItem" :span="_span">
    <slot />
  </a-col>
  <slot v-else />
</template>
<script lang="ts">
import { Vue, Options, Prop, Inject } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject() colProps;
  get _span() {
    return this.colProps.colSpan ? this.colProps.colSpan : 24;
  }
  get parentElement(): HTMLDivElement {
    return this.$el.parentElement;
  }
  get containerWidth() {
    return this.parentElement.clientWidth;
  }
  mounted() {
  }
}
</script>
<style lang="less">
</style>
