<template>
  <Grid />
  <a-divider />
</template>

<script lang="ts">
import { defineAsyncComponent } from "vue";
import { Options, Prop, Vue, Watch } from "vue-property-decorator";
import { GridOptions, SelectionChangedEvent, ColDef } from "ag-grid-community";
import framework from "./framework";
@Options({
  components: {
    Grid: defineAsyncComponent({
      loader: () => import("./grid.vue"),
      loadingComponent: framework.loadingOverlayComponent,
    }),
  },
})
export default class extends Vue {
  created() {}
}
</script>
<style  lang="less">
</style>
