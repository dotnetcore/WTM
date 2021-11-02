
<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <div  style="margin-bottom:10px"> @WtmBlazor.Localizer["Sys.BatchEditConfirm"]</div>
    <div>
      <WtmField entityKey="Email"/>
      <WtmField entityKey="Gender" Id="g29c27c8dc138403981d640ed7ff05fc2"/>
      <WtmField entityKey="CellPhone"/>
      <WtmField entityKey="HomePhone"/>
      <WtmField entityKey="Address"/>
      <WtmField entityKey="ZipCode"/>
      <WtmField entityKey="Role" Id="gcab636799baf439186c8129d72a19ca3"/>
      <WtmField entityKey="Group" Id="g6c94bd1aad434ba085fe42617e2156ef"/>
    </div>

    <div class="modal-footer table-modal-footer">
      <Button ButtonType="ButtonType.Submit" IsAsync="true"/>
      <Button OnClick="OnClose"/>
    </div>

  </WtmDetails>

</template>


<script lang="ts">
import { Options,Provide,Vue } from "vue-property-decorator";
import PageController,{ PageEntity } from "../controller";
@Options({
  components: {
  },
})
export default class extends Vue {
  get queryKey() {
    return "_AdminFrameworkUserBatchEdit";
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

