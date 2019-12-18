<template>
  <Grid />
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Grid from "@/components/grid/index.vue";
@Component({
  components: {
    Grid
  }
})
export default class ViewGrid extends Vue {}
</script>
<style scoped lang="less">
</style>