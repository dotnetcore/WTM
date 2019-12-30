<template>
  <ViewFilterBasics :PageStore="PageStore" :Entities="Entities" />
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { ViewFilterBasics } from "../../../components/pages/filterForm";
import PageStore from "../store";
import Entities from "./entities";
@Component({
  components: {
    ViewFilterBasics
  }
})
export default class ViewGrid extends Vue {
  @Prop() PageStore: PageStore;
  /**
   * 实体模型
   */
  Entities = Entities;
}
</script>
<style scoped lang="less">
</style>