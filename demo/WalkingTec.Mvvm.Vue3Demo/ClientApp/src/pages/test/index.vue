<template>
  <!-- <ViewFilter /> -->
  <!-- <ViewAction /> -->
  <!-- <ViewGrid /> -->
  <!-- <WtmView>
    <ViewDetails />
  </WtmView>-->
  <a-tabs v-model:activeKey="activeKey" tab-position="left">
    <a-tab-pane key="1" tab="表单">
      <a-divider>表单测试</a-divider>
      <ViewDetails />
    </a-tab-pane>
    <a-tab-pane key="2" tab="树结构">Content of Tab 2</a-tab-pane>
    <a-tab-pane key="3" tab="Tab 3">Content of Tab 3</a-tab-pane>
  </a-tabs>
</template>
<script lang="ts">
import { Options, Provide, Vue } from "vue-property-decorator";
import PageController, { PageEntity } from "./controller";
import ViewAction from "./views/action.vue";
import ViewDetails from "./views/details.vue";
import ViewFilter from "./views/filter.vue";
import ViewGrid from "./views/grid.vue";
@Options({
  components: {
    ViewAction,
    ViewFilter,
    ViewGrid,
    ViewDetails,
  },
})
export default class extends Vue {
  /**
   * 当前页面控制器
   * 子组件 通过 Inject 均可访问
   */
  @Provide({ reactive: true }) readonly PageController = PageController;
  /**
   * 当前实体配置
   * 子组件 通过 Inject 均可访问
   */
  @Provide({ reactive: true }) readonly PageEntity = PageEntity;
  activeKey = "1"
  created() { }
  mounted() {
    console.log("");
    console.group(this.$route.name);
    console.log(this);
    console.groupEnd();
  }
}
</script>
<style lang="less">
</style>
