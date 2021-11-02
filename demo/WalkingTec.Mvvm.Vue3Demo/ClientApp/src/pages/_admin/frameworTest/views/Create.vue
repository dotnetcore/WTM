
<template>
  <FormViewCreateAdd />

</template>


<script lang="ts">
import { Options,Provide,Vue } from "vue-property-decorator";
import PageController,{ PageEntity } from "../controller";
import FormViewCreateAdd from "./FormViewCreateAdd.vue";
@Options({
  components: {
    FormViewCreateAdd
  },
})
export default class extends Vue {
  get queryKey() {
    return "_AdminFrameworkUserCreate";
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

