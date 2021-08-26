<template>
  <a-col v-if="isCol" :span="colSpan">
    <slot />
  </a-col>
  <slot v-else />
</template>
<script lang="ts">
import { Inject,Options,Vue } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject({ default: { colItem: false, colSpan: 24 } }) readonly colProps;
  get colSpan() {
    return this.colProps.colSpan;
  }
  get isCol() {
    return this.colProps.colItem;
  }
  // get parentElement(): HTMLDivElement {
  //   return this.$el.parentElement;
  // }
  // get containerWidth() {
  //   return this.parentElement.clientWidth;
  // }
  mounted() {}
}
</script>
<style lang="less">
</style>
