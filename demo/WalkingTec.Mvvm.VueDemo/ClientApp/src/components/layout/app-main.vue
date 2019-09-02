<template>
  <section :class="[collapse?'folder':'','app-main']" :style="[]">
    <Breadcrumd />
    <div class="content-com">
      <router-view class="animated fadeIn" />
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
    min-height: 650px;
    right: 0;
    top: 60px;
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
