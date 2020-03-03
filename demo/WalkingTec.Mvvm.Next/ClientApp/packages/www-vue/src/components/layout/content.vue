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
      <a-tab-pane
        v-for="page in TabPages"
        :key="page.fullPath"
        :closable="true"
      >
        <template #tab>
          <router-link :to="page.path">
            <a-icon :type="page.icon || 'pic-right'" />
            <span>{{ page.name || "404" }}</span>
          </router-link>
        </template>
        <router-view
          :name="page.meta.pageKey || '404'"
        ></router-view>
        <!-- <router-view v-else></router-view> -->
      </a-tab-pane>
    </a-tabs>
    <router-view v-else></router-view>
  </a-layout-content>
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
  onEdit(targetKey, action) {
    this.$router.back();
    lodash.remove(this.TabPages, ["fullPath", targetKey]);
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
    padding: 8px;
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
</style>
