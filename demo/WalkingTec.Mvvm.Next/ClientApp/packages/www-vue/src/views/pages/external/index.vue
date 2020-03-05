<template>
    <iframe
      v-resize="{ log: false,scrolling:true,warningTimeout:0 }"
      width="100%"
      src="/pages/test.html"
      frameborder="0"
    ></iframe>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
@Component
export default class External extends Vue {}
</script>
<style scoped lang="less"></style>
