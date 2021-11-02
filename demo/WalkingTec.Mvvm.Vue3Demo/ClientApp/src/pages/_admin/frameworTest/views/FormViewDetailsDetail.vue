
<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <div>
      <WtmField entityKey="Email"/>
      <WtmField entityKey="Gender"/>
      <WtmField entityKey="CellPhone"/>
      <WtmField entityKey="HomePhone"/>
      <WtmField entityKey="Address"/>
      <WtmField entityKey="ZipCode"/>
      <WtmField entityKey="ITCode"/>
      <WtmField entityKey="Name"/>
      <WtmField entityKey="IsValid"/>
      <WtmField entityKey="Photo" width="300"/>
      <WtmField entityKey="CreateTime"/>
      <WtmField entityKey="UpdateTime"/>
      <WtmField entityKey="CreateBy"/>
      <WtmField entityKey="UpdateBy"/>
    </div>

      <div class="modal-footer table-modal-footer">
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
        Email: "",
        Gender: "",
        CellPhone: "",
        HomePhone: "",
        Address: "",
        ZipCode: "",
        ITCode: "",
        Name: "",
        IsValid: "",
        Photo: "",
        CreateTime: "",
        UpdateTime: "",
        CreateBy: "",
        UpdateBy: "",  },
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

