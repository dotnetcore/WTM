<template>
  <WtmDetails> 哈哈哈哈哈 </WtmDetails>
</template>
<script lang="ts">
import { Vue, Options, Inject } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject() PageController;
  created() {}
}
</script>
<style lang="less">
</style>
