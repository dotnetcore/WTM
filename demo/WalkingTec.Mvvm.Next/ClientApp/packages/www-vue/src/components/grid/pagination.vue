<template>
  <a-pagination
    showSizeChanger
    :defaultCurrent="3"
    :total="500"
  />
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
@Component
export default class HelloWorld extends Vue {
  @Prop() private msg!: string;
  mounted() {
  }
  destroyed() {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
