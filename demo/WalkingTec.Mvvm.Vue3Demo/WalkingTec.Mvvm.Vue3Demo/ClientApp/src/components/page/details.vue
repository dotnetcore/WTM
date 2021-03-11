<template>
  <a-modal @cancel="onCancel" :visible="visible" title="Basic Modal">
    <slot />
  </a-modal>
</template>

<script lang="ts">
import { Options, Prop, Vue, Watch } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  @Prop() queryKey: string;
  get visibleKey() {
    return this.queryKey || this.$WtmConfig.detailsVisible;
  }
  get query() {
    return this.lodash.clone(this.$route.query);
  }
  get visible() {
    return this.lodash.has(this.query, this.visibleKey);
  }
  get title() {
    return this.lodash.get(this.query, this.visibleKey);
  }
  onCancel() {
    this.__wtmBackDetails();
  }
  created() {}
}
</script>
<style  lang="less">
</style>
