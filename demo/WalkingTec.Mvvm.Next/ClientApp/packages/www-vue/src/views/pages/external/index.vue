<template>
  <iframe
    v-resize="{ log: false, scrolling: true, warningTimeout: 0 }"
    width="100%"
    src="/pages/test.html"
    frameborder="0"
    class="iframe"
  ></iframe>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component
export default class External extends Vue {
  mounted() {
    lodash.set(this.$el.parentNode, "style.position", "relative");
  }
}
</script>
<style scoped lang="less">
.iframe {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  height: 100%;
}
</style>
