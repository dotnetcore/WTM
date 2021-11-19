<template>
  <div>
    <a-switch :checked="this.lodash.includes(['true', true], params.getValue())" disabled>
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

  created() { 
  }
  mounted() {
  }
  updated() { }
  destroyed() { }
}
</script>

<style lang="less">
</style>
