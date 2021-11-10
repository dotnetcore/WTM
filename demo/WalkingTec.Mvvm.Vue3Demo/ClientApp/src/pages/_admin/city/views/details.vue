<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <WtmField entityKey="ParentId" />
    <WtmField entityKey="Name" />
    <WtmField entityKey="Level" />
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    Entity: {
      ID: ""
    }
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
