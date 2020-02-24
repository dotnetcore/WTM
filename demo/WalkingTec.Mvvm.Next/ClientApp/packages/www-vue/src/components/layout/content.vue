<template>
  <a-tabs
    v-if="$GlobalConfig.settings.tabsPage"
    class="layout-tabs"
    hideAdd
    size="small"
    type="editable-card"
    :activeKey="$route.fullPath"
  >
    <a-tab-pane v-for="page in TabPages" :key="page.fullPath" :closable="true">
      <template #tab>
        <router-link :to="page.path">
          <a-icon :type="page.icon || 'pie-chart'" />
          <span>{{ page.name }}</span>
        </router-link>
      </template>
      <router-view :name="page.meta.pageKey"></router-view>
    </a-tab-pane>
  </a-tabs>
  <router-view v-else></router-view>
</template>
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
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
  }
  updated() {
    this.onPushTabPages();
  }
  onPushTabPages() {
    if (lodash.some(this.TabPages, ["fullPath", this.$route.fullPath])) {
      return;
    }
    this.TabPages.push(this.$route);
  }
}
</script>
<style lang="less">
.layout-tabs {
  .ant-tabs-bar {
    margin: 0;
  }
}
</style>
