<template>
  <div>
    <span :class="params.value"></span>
  </div>
</template>

<script lang="ts">
import { Options, Prop, Vue } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { $System } from "@/client";
@Options({
  components: {}
})
export default class extends Vue {
  params: ICellRendererParams;
  get value() {
    return this.params.getValue();
  }
  created() {}
  mounted() {}
  updated() {}
  destroyed() {}
}
</script>

<style lang="less"></style>
