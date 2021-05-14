<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <WtmField entityKey="DpType" />
    <WtmField entityKey="TableName" />
    <WtmField entityKey="SelectedItemsID" />
    <WtmField entityKey="IsAll" />
    <WtmField entityKey="UserCode" />
    <WtmField entityKey="GroupCode" />
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

    },
  };
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
