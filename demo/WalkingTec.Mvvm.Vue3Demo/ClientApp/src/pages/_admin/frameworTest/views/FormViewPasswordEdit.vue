
<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <WtmField entityKey="Password"/>
      <div class="modal-footer table-modal-footer">
        <Button ButtonType="ButtonType.Submit" IsAsync="true"/>
        <Button OnClick="OnClose"/>
      </div>

  </WtmDetails>
</template>



<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
      Entity: {
        Password: "",  },
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

