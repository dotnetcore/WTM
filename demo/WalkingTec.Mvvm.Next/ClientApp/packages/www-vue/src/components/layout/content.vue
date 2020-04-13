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
      <a-tab-pane v-for="page in TabPages" :key="page.fullPath" :closable="true">
        <template #tab>
          <router-link :to="page.path">
            <a-icon :type="page.icon || 'pic-right'" />
            <span v-text="page.name"></span>
          </router-link>
        </template>
        <!-- <router-view v-else></router-view> -->
      </a-tab-pane>
    </a-tabs>
    <div
      ref="pageView"
      class="layout-page-view"
      :style="$GlobalConfig.settings.tabsPage&&{
        height: height + 'px'
      }"
    >
      <keep-alive>
        <router-view />
      </keep-alive>
      <a-back-top :target="target" />
    </div>
  </a-layout-content>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Subscription, fromEvent } from "rxjs";
import { Debounce } from "lodash-decorators";
import { computed, observable, toJS, action } from "mobx";
import { create, persist } from "mobx-persist";
import rootStore from "../../rootStore";
import lodash from "lodash";
@Component({
  components: {}
})
export default class extends Vue {
  UserStore = rootStore.UserStore;
  target() {
    return this.$GlobalConfig.settings.tabsPage ? this.$refs.pageView : window;
  }
  hydrate = create({
    storage: window.localStorage, // 存储的对象
    jsonify: true, // 格式化 json
    debounce: 1000
  });
  // @persist("list")
  // @observable
  TabPages = [];
  onGetName(path) {
    return lodash.get(
      lodash.find(this.UserStore.Menus, ["path", path]),
      "name",
      path
    );
  }
  beforeMount() {
    // this.hydrate("layout", this);
  }
  mounted() {
    console.log(this.$refs);
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
  @action
  onPushTabPages() {
    if (
      this.$route.fullPath === "/" ||
      lodash.some(this.TabPages, ["fullPath", this.$route.fullPath])
    ) {
      return;
    }
    this.TabPages.push(
      lodash.merge({}, this.$route, { name: this.onGetName(this.$route.path) })
    );
    // console.log("extends -> onPushTabPages -> this.TabPages", this.TabPages)
  }
  @action
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
      const offsetTop = lodash.get(this, "$el.firstChild.offsetTop", 0), //+ 5,
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
    box-shadow: 0 1px 10px rgba(0, 21, 41, 0.08);
  }
  .ant-tabs-tabpane {
    padding: 0;
  }
  .ant-tabs-nav .ant-tabs-tab-active {
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
  overflow: auto;
  .ant-back-top {
    right: 50px;
    bottom: 30px;
  }
}
.layout-page-view > div.app-page {
  margin: 0 6px;
  padding-top: 6px;
}
// iframe.layout-page-view {
//   padding: 0;
// }
</style>
