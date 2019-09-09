<template>
  <section :class="[collapse?'folder':'','app-main']" :style="[]">
    <div class="content-com">
      <keep-alive>
        <router-view class="animated fadeIn" />
      </keep-alive>
    </div>
  </section>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Getter } from "vuex-class";
@Component
export default class AppMain extends Vue {
    @Getter("isCollapse")
    collapse;
}
</script>

<style lang="less" scoped>
@import "../../assets/css/variable";
.app-main {
    position: absolute;
    left: 220px;
    width: 100%;
    min-height: 650px;
    right: 0;
    top: 80px;
    width: auto;
    overflow: hidden;
    overflow-y: auto;
    box-sizing: border-box;
    transition: left 200ms ease;
    padding: 16px;
    .content-com {
        margin-top: 20px;
    }
    &.folder {
        left: @foldWith;
    }
    .app-content {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
    }
}
</style>
