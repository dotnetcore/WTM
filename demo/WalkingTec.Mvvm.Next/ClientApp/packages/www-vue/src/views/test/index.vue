<template>
  <div class="app-page">
    <a-card class="page-card">
      <ViewFilter :PageStore="PageStore" />
    </a-card>
    <a-divider class="page-divider" />
    <a-card class="page-card">
      <ViewGrid :PageStore="PageStore" />
    </a-card>
  </div>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ViewGrid from "./views/grid.vue";
import { FormFilter } from "./views/forms";
import PageStore from "./store";
import { Subscription } from "rxjs";
@Component({
  components: { ViewGrid, ViewFilter: FormFilter }
})
export default class PageView extends Vue {
  PageStore = new PageStore();
  mounted() {
    console.warn("TCL: PageView ", this.PageStore);
  }
  destroyed() {
    // 销毁订阅
    this.PageStore.onUnsubscribe();
  }
}
</script>
<style scoped lang="less">
</style>