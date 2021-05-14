<template>
  <a-tabs v-model:activeKey="activeKey" type="editable-card" hide-add @edit="onEdit">
    <a-tab-pane
      v-for="pane in PagesCache"
      :key="pane.path"
      :tab="getTab(pane)"
      :closable="isClosable(pane)"
    ></a-tab-pane>
  </a-tabs>
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
    return this.$route.path;
  }
  set activeKey(value) {
    this.$router.replace(this.lodash.find(this.PagesCache, ["path", value]));
  }
  isClosable(pane: _RouteRecordBase) {
    return !this.lodash.includes(['/'], pane.path)
  }
  getTab(pane: _RouteRecordBase) {
    const name = this.lodash.get<any, any>(pane, 'name')
    if (name && !this.lodash.eq(name, "NotFound")) {
      return this.$t(`PageName.${name}`)
    }
    return this.lodash.get(pane, 'path', 'NotFound')
  }
  onEdit(event) {
    const index = this.lodash.findIndex(this.PagesCache, ['path', event])
    this.lodash.remove(this.PagesCache, ['path', event])
    AppRouter.PagesCache.delete(event)
    // 停留在被关闭的页面
    if (this.lodash.eq(this.$route.path, event)) {
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
</style>
