<template>
  <a-space>
    <a-icon :type="icon" />
    <span v-text="value"></span>
  </a-space>
</template>

<script lang="ts">
import { Options, Prop, Vue } from "vue-property-decorator";
import lodash from 'lodash';
import { IHeaderParams } from "ag-grid-community";

@Options({
  components: {},
})
export default class extends Vue {
  params: IHeaderParams;
  get icon() {
    return lodash.get(this.params, "icon");
  }
  get value() {
    return this.params.displayName;
  }
  created() {}
  mounted() {}
  updated() {}
  destroyed() {}
}
</script>

<style lang="less"></style>
