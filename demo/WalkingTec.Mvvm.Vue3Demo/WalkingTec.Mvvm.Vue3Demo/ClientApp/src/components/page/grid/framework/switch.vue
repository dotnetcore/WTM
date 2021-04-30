<template>
  <div>
    <a-switch :checked="value" disabled>
      <template #checkedChildren>
        <check-outlined />
      </template>
      <template #unCheckedChildren>
        <close-outlined />
      </template>
    </a-switch>
  </div>
</template>

<script lang="ts">
import { ICellRendererParams } from "ag-grid-community";
import { Options, Vue } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  params: ICellRendererParams;
  get value() {
    return this.lodash.includes(['true', true], this.params.getValue())
  }
  created() { }
  mounted() {
  }
  updated() { }
  destroyed() { }
}
</script>

<style lang="less">
</style>
