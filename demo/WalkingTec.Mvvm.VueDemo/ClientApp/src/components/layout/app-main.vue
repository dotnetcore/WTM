<template>
  <section :class="[collapse?'folder':'','app-main']" :style="[]">
    <div class="app-content">
      <Breadcrumd />
      <div class="content-com">
        <router-view class="animated fadeIn" />
      </div>
    </div>
  </section>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import Breadcrumd from "./bread-crumb.vue";
import { Getter } from "vuex-class";
@Component({
    components: { Breadcrumd }
})
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
    // height: 100%;
    min-height: 650px;
    right: 0;
    top: 60px;
    // bottom: 44px;
    // z-index: 998;
    width: auto;
    overflow: hidden;
    overflow-y: auto;
    box-sizing: border-box;
    transition: left 400ms ease;
    padding: 20px;
    padding-bottom: 0;
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
