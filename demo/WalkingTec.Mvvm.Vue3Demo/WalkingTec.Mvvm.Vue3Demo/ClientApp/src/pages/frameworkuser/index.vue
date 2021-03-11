<template>
  <ViewFilter />
  <ViewAction />
  <a-divider />
  <ViewGrid />
  <ViewDetails />
</template>
<script lang="ts">
import { Vue, Options, Provide } from "vue-property-decorator";
import ViewAction from "./views/action.vue";
import ViewDetails from "./views/details.vue";
import ViewFilter from "./views/filter.vue";
import ViewGrid from "./views/grid.vue";
import PageController from "./controller";
@Options({ components: { ViewAction, ViewFilter, ViewGrid, ViewDetails } })
export default class extends Vue {
  @Provide() PageController = PageController;
  created() {}
  mounted(){
    console.log('')
    console.group(this.$route.name)
    console.log(this)
    console.groupEnd()
  }
}
</script>
<style lang="less">
</style>
