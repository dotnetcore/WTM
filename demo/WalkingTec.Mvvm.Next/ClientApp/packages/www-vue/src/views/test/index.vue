<template>
  <div class="app-page">
    <a-card class="page-card">
      <ViewFilter :PageStore="PageStore" />
    </a-card>
     <a-divider class="page-divider"/>
    <a-card class="page-card">
      <ViewGrid />
    </a-card>
  </div>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ViewGrid from "./views/grid.vue";
import ViewFilter from "./views/filter.vue";
import PageStore from "./store";
@Component({
  components: { ViewGrid, ViewFilter }
})
export default class PageView extends Vue {
  PageStore = new PageStore();
}
</script>
<style scoped lang="less">

</style>