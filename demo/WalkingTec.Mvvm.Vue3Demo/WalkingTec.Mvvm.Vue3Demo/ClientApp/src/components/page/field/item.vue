<template>
  <a-col v-if="colItem" :span="span">
    <slot />
  </a-col>
  <slot v-else />
</template>
<script lang="ts">
import { Vue, Options, Prop, Inject } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject() colItem;
  @Prop({ default: 6 }) span;
  mounted() {}
}
</script>
<style lang="less">
</style>
