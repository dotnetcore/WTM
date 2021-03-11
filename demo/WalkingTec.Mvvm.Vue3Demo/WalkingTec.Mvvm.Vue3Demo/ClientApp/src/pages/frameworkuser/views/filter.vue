<template>
  <div>filter</div>
</template>
<script lang="ts">
import { Vue, Options, Inject } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends Vue {
  @Inject() PageController: PageController;
  created() {}
}
</script>
<style lang="less">
</style>
