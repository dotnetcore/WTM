<template>
  <ViewFilter />
  <ViewAction />
  <a-divider />
  <ViewGrid />
  <WtmView>
    <ViewDetails />
  </WtmView>
  <WtmView queryKey="privilege" title="分配权限">
    <ViewPrivilege />
  </WtmView>
</template>
<script lang="ts">
import { Options, Provide, Vue } from "vue-property-decorator";
import PageController, { PageEntity } from "./controller";
import ViewAction from "./views/action.vue";
import ViewDetails from "./views/details.vue";
import ViewFilter from "./views/filter.vue";
import ViewGrid from "./views/grid.vue";
import ViewPrivilege from "./views/privilege.vue";
@Options({
  name: "WalkingTec.Mvvm.Admin.Api,FrameworkRole",
  components: {
    ViewAction,
    ViewFilter,
    ViewGrid,
    ViewDetails,
    ViewPrivilege,
  },
})
export default class extends Vue {
  /**
   * 后端控制器标识
   */
   static controller = "WalkingTec.Mvvm.Admin.Api,FrameworkRole"
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
