
<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <a-tabs>
      <a-tab-pane>
        <div>
          <WtmField entityKey="ITCode"/>
          <WtmField entityKey="Name"/>
          <WtmField entityKey="IsValid"/>
          <WtmField entityKey="Photo"/>
        </div>

        <div>
          <WtmField entityKey="Role"/>
          <WtmField entityKey="Group"/>
        </div>

      </a-tab-pane>

      <a-tab-pane>
        <div>
          <WtmField entityKey="Email"/>
          <WtmField entityKey="Gender"/>
          <WtmField entityKey="CellPhone"/>
          <WtmField entityKey="HomePhone"/>
          <WtmField entityKey="ZipCode"/>
        </div>

        <div>
          <WtmField entityKey="Address"/>
        </div>

      </a-tab-pane>

    </a-tabs>

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
        ITCode: "",
        Name: "",
        IsValid: "true",
        Photo: "",
        Email: "",
        Gender: "",
        CellPhone: "",
        HomePhone: "",
        ZipCode: "",
        Address: "",  },
    SelectedFrameworkRoleFrameworkUsersIDs: [],
    SelectedFrameworkGroupFrameworkUsersIDs: [],
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

