<template>
  <div style="position:relative;height:100%">
    <iframe
      v-resize="{ log: false, scrolling: true, warningTimeout: 0 }"
      width="100%"
      :src="iframeSrc"
      frameborder="0"
      class="iframe"
    ></iframe>
  </div>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component
export default class External extends Vue {
  get iframeSrc() {
    // console.log(
    //   "External -> url -> this.$route.params.url",
    //   this.$route.params.url
    // );
    return this.$route.params.url;
  }
  mounted() {
    // console.log(this.$route.params.url);
    // lodash.set(this.$el.parentNode, "style.position", "relative");
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
