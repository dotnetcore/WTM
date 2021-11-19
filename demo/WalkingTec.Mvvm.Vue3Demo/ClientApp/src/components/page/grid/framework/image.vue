<template>
  <div>
    <a-image :width="40" :src="'/api/_file/downloadFile/'+params.value" :fallback="imagefallback" />
  </div>
</template>

<script lang="ts">
import { Options, Prop, Vue } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { $System } from "@/client";
@Options({
  components: {},
})
export default class extends Vue {
  params: ICellRendererParams;
  get value() {
    return this.params.getValue()
  }
  get src() {
    return $System.FilesController.getDownloadUrl(this.value)
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
