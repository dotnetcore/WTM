<template>
  <ViewFilter />
  <ViewAction />
  <ViewGrid />
  <ViewDetails />
</template>
<script lang="ts">
import { Vue, Options } from "vue-class-component";
import ViewAction from "./views/action.vue";
import ViewDetails from "./views/details.vue";
import ViewFilter from "./views/filter.vue";
import ViewGrid from "./views/grid.vue";
@Options({ components: { ViewAction, ViewFilter, ViewGrid, ViewDetails } })
export default class extends Vue {
  created() {}
}
</script>
<style lang="less">
</style>
