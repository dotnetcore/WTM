<template>
  <div ref="content" :style="style">
    <iframe
      v-if="src"
      class="w-iframe"
      v-resize="{
        log: false,
        scrolling: true
        // interval: 3000,
        // heightCalculationMethod: 'max'
      }"
      width="100%"
      :src="src"
      frameborder="0"
      scrolling="y"
    />
    <Error v-else />
  </div>
</template>
<script lang="ts">
import { Inject, Options, Vue, Ref } from "vue-property-decorator";
import iframeResize from "iframe-resizer/js/iframeResizer";
import Error from "../error/index.vue";
@Options({
  components: { Error },
  directives: {
    resize: {
      mounted: function(el, { value = {} }) {
        el.addEventListener("load", () => iframeResize(value, el));
      },
      unmounted: function(el) {
        el?.iFrameResizer?.removeListeners();
      }
    }
  }
})
export default class extends Vue {
  @Ref("content") readonly content: HTMLDivElement;
  style = { height: "" };
  get src() {
    return decodeURIComponent(
      this.lodash.get(this.$route.query, "src", "").toString()
    );
  }
  /**
   * 计算 表格高度
   */
  onReckon() {
    let height = 500;
    height = window.innerHeight - this.content.offsetTop - 80;
    this.style.height = height + "px";
  }
  created() {}
  mounted() {
    this.onReckon();
  }
}
</script>
<style lang="less" scoped>
.w-iframe {
  height: 100%;
}
</style>
