<template>
  <div ref="content" class="w-webview" :style="style">
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
    <div class="w-webview-open">
      <ArrowsAltOutlined @click="onOpne" />
    </div>
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
  onOpne() {
    window.open(this.src);
  }
  created() {}
  mounted() {
    this.onReckon();
  }
}
</script>
<style lang="less" scoped>
.w-webview {
  position: relative;
  &-open {
    position: absolute;
    right: 18px;
    top: 3px;
    font-size: 16px;
    background: rgba(255, 255, 255, 0.493);
    width: 20px;
    height: 20px;
    border-radius: 5px;
    display: flex;
    justify-content: center;
    align-items: center;
    cursor: pointer;
  }
}
.w-iframe {
  height: 100%;
}
</style>
