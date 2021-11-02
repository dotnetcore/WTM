
<template>
  <div>
    <FilterViewIndexSearcher />

  <ActionViewIndex />
  <a-divider />
  <GridViewIndex />


  <WtmView queryKey="Create">
    <Create />
  </WtmView>

  <WtmView queryKey="Edit">
    <Edit />
  </WtmView>

  <WtmView queryKey="Details">
    <Details />
  </WtmView>

  <WtmView queryKey="Import">
    <Import />
  </WtmView>

  <WtmView queryKey="BatchEdit">
    <BatchEdit />
  </WtmView>

  <WtmView queryKey="Password">
    <Password />
  </WtmView>
  </div>
</template>


<script lang="ts">
import { Options,Provide,Vue } from "vue-property-decorator";
import PageController,{ PageEntity } from "./controller";
import FilterViewIndexSearcher from "./views/FilterViewIndexSearcher.vue";
import ActionViewIndex from "./views/ActionViewIndex.vue";
import GridViewIndex from "./views/GridViewIndex.vue";
import Create from "./views/Create.vue";
import Edit from "./views/Edit.vue";
import Details from "./views/Details.vue";
import Import from "./views/Import.vue";
import BatchEdit from "./views/BatchEdit.vue";
import Password from "./views/Password.vue";
@Options({
  components: {
    FilterViewIndexSearcher,
    ActionViewIndex,
    GridViewIndex,
    Create,
    Edit,
    Details,
    Import,
    BatchEdit,
    Password,
  },
})
export default class extends Vue {
  get queryKey() {
    return "_AdminFrameworkUserIndex";
  }
  /**
   * 后端控制器标识
   */
   static controller = "vuepro._Admin.Controllers,FrameworkUser"
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
  created() {}
  mounted() {
  }
}
</script>
<style lang="less">
</style>

