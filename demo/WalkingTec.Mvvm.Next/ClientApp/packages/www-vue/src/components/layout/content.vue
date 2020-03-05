<template>
  <a-layout-content
    class="app-layout-content"
    :class="{ 'layout-content-tabs': $GlobalConfig.settings.tabsPage }"
  >
    <a-tabs
      v-if="$GlobalConfig.settings.tabsPage"
      class="layout-tabs"
      hideAdd
      size="small"
      type="editable-card"
      :activeKey="$route.fullPath"
      @edit="onEdit"
    >
      <a-tab-pane :key="'/'" :closable="false">
        <template #tab>
          <router-link to="/">
            <a-icon :type="'pic-right'" />
            <span>Home</span>
          </router-link>
        </template>
      </a-tab-pane>
      <a-tab-pane
        v-for="page in TabPages"
        :key="page.fullPath"
        :closable="true"
      >
        <template #tab>
          <router-link :to="page.path">
            <a-icon :type="page.icon || 'pic-right'" />
            <span>{{ page.path }}</span>
          </router-link>
        </template>
        <!-- <router-view v-else></router-view> -->
      </a-tab-pane>
    </a-tabs>
    <keep-alive>
      <router-view
        class="layout-page-view"
        :style="{
          minHeight: height + 'px'
        }"
      ></router-view>
    </keep-alive>
  </a-layout-content>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Subscription, fromEvent } from "rxjs";
import { Debounce } from "lodash-decorators";
import lodash from "lodash";
@Component({
  components: {}
})
export default class extends Vue {
  TabPages = [];
  mounted() {
    lodash.delay(() => {
      this.onPushTabPages();
    }, 300);
    this.onSetHeight();
    this.ResizeEvent = fromEvent(window, "resize").subscribe(e => {
      this.onSetHeight();
    });
  }
  updated() {
    this.onPushTabPages();
  }
  onPushTabPages() {
    if (
      this.$route.fullPath === "/" ||
      lodash.some(this.TabPages, ["fullPath", this.$route.fullPath])
    ) {
      return;
    }

    this.TabPages.push(this.$route);
    console.log("extends -> onPushTabPages -> this.TabPages", this.TabPages)
  }
  onEdit(targetKey, action) {
    // this.$router.back();
    if (this.$route.fullPath === targetKey) {
      let index = lodash.findIndex(this.TabPages, ["fullPath", targetKey]);
      let fullPath = "/";
      if (index !== 0) {
        fullPath = lodash.get(this.TabPages, `[${index - 1}].fullPath`);
      }
      this.$router.replace(fullPath);
    }
    lodash.remove(this.TabPages, ["fullPath", targetKey]);
    this.TabPages = [...this.TabPages];
  }
  // 事件对象
  ResizeEvent: Subscription;
  height = 400;
  @Debounce(200)
  onSetHeight() {
    try {
      const offsetTop = lodash.get(this, "$el.firstChild.offsetTop", 0) + 5,
        innerHeight = window.innerHeight,
        height = innerHeight - offsetTop;
      this.height = height;
    } catch (error) {
      this.height = 400;
    }
  }
}
</script>
<style lang="less">
.layout-tabs {
  &.ant-tabs {
    position: initial;
  }
  .ant-tabs-nav-container {
    background: #fff;
  }
  .ant-tabs-bar {
    margin: 0;
  }
  .ant-tabs-tabpane {
    padding: 0;
  }
  .ant-tabs-nav .ant-tabs-tab-active{
    font-weight: 400;
  }
}
.app-layout-content {
  margin: 8px;
  position: relative;
  &.layout-content-tabs {
    margin: 0;
  }
  a {
    text-decoration: none;
  }
}
.layout-page-view {
  padding: 6px;
}
iframe.layout-page-view {
  padding: 0;
}
</style>
