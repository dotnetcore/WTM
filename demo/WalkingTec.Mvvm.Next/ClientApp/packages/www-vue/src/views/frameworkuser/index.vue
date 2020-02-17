<template>
  <div class="app-page">
    <a-card class="page-card">
      <ViewFilter :PageStore="PageStore" />
    </a-card>
    <a-divider class="page-divider" />
    <a-card class="page-card">
      <ViewAction :PageStore="PageStore" />
      <a-divider class="page-divider" />
      <ViewGrid :PageStore="PageStore" />
    </a-card>
  </div>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ViewGrid from "./views/grid.vue";
import ViewAction from "./views/action.vue";
import ViewFilter from "./views/filter.vue";
import PageStore from "./store";
import { Subscription } from "rxjs";
@Component({
  components: { ViewGrid, ViewAction, ViewFilter }
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