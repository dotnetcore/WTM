<template>
  <a-affix :offsetTop="48">
    <!-- <div > -->
    <a-tabs
      class="w-affix-tabs"
      v-model:activeKey="activeKey"
      type="editable-card"
      hide-add
      @edit="onEdit"
    >
      <a-tab-pane
        v-for="pane in PagesCache"
        :key="pane.pageKey"
        :tab="getTab(pane)"
        :closable="isClosable(pane)"
      ></a-tab-pane>
    </a-tabs>
    <!-- </div> -->
  </a-affix>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import AppRouter from "@/router";
import { _RouteRecordBase } from "vue-router";
@Options({ components: {} })
export default class extends Vue {
  keep = true;
  PagesCache = [];
  get activeKey() {
    if (this.lodash.eq(this.$route.name, 'webview')) {
      return `${this.$route.path}_${this.lodash.get(this.$route, 'query.src')}`
    }
    return this.$route.path;
  }
  set activeKey(value) {
    this.$router.replace(AppRouter.PagesCache.get(value));
  }
  isClosable(pane: _RouteRecordBase) {
    return !this.lodash.includes(['/'], pane.path)
  }
  getTab(pane: _RouteRecordBase) {
    const name = this.lodash.get<any, any>(pane, 'name')
    if (this.lodash.eq(name, 'webview')) {
      return this.$t(this.lodash.get(pane, 'query.name', this.lodash.get(pane, 'query.src', 'webview')))
    }
    if (name && !this.lodash.eq(name, "NotFound")) {
      return this.$t(`PageName.${name}`)
    }
    return this.lodash.get(pane, 'path', 'NotFound')
  }
  onEdit(event) {
    const predicate = ['pageKey', event]
    const index = this.lodash.findIndex(this.PagesCache, predicate)
    this.lodash.remove(this.PagesCache, predicate)
    AppRouter.PagesCache.delete(event)
    // 停留在被关闭的页面
    if (this.lodash.eq(this.activeKey, event)) {
      const to = this.lodash.nth(this.PagesCache, index) || this.lodash.last(this.PagesCache);
      this.$router.replace(to)
    }
  }
  created() {
    // console.log("LENG ~ extends ~ created ~ this", this);
  }
  mounted() {
    AppRouter.RouterBehaviorSubject.subscribe((obs) => {
      this.PagesCache = obs;
    });
  }
}
</script>
<style lang="less">
.w-affix-tabs {
  .ant-tabs-bar {
    background: white;
  }
}
</style>
