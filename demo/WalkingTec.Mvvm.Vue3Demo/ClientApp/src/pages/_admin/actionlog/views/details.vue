<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="ITCode" />
      <WtmField entityKey="LogType" />
    </a-space>
    <a-space>
      <WtmField entityKey="ActionUrl" />
      <WtmField entityKey="ActionTime" />
    </a-space>
    <a-space>
      <WtmField entityKey="ActionName" />
      <WtmField entityKey="Duration" />
    </a-space>
    <a-space>
      <WtmField entityKey="ModuleName" />
      <WtmField entityKey="IP" />
    </a-space>
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
    Entity: {}
  };
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
