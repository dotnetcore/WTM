<template>
  <ViewInfo v-if="IsInfo" />
  <ViewBatch v-else-if="IsBatch" />
  <ViewUpdate v-else-if="IsEdit" />
  <ViewInsert v-else />
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import ViewInfo from "./details_info.vue";
import ViewInsert from "./details_insert.vue";
import ViewUpdate from "./details_update.vue";
import ViewBatch from "./details_batch.vue";
@Options({ components: { ViewInfo, ViewInsert, ViewUpdate, ViewBatch } })
export default class extends mixins(PageDetailsBasics) {
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
