<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <WtmField entityKey="LinkedVM_SelectedRolesCodes" />
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
    LinkedVM: {
      SelectedRolesCodes: []
    }
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
