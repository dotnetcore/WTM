<template>
  <a-row :gutter="[16, 16]">
    <a-col :span="8">
      <ViewInsert />
    </a-col>
    <a-col :span="16">
      <ViewFilter />
      <a-row :gutter="[16, 16]">
        <a-col :span="12">
          <ViewGrid />
        </a-col>
        <a-col :span="12">
          <ViewGrid />
        </a-col>
      </a-row>
    </a-col>
  </a-row>
</template>
<script lang="ts">
import { Options, Provide, Vue } from "vue-property-decorator";
import PageController, { PageEntity } from "./controller";
import ViewAction from "./views/action.vue";
import ViewInsert from "./views/details_insert.vue";
import ViewFilter from "./views/filter.vue";
import ViewGrid from "./views/grid.vue";
@Options({
  components: {
    ViewAction,
    ViewFilter,
    ViewGrid,
    ViewInsert
  }
})
export default class extends Vue {
  /**
   * 后端控制器标识
   */
  static controller = "WalkingTec.Mvvm.Admin.Api,FrameworkUser";
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
    console.log("");
    console.group(this.$route.name);
    console.log(this);
    console.groupEnd();
  }
}
</script>
<style lang="less"></style>
