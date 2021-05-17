<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <WtmField entityKey="DpType" debug />
    <a-space>
      <WtmField entityKey="TableName" />
      <WtmField entityKey="SelectedItemsID" :disabled="!formState.Entity.TableName" />
    </a-space>
    <WtmField entityKey="IsAll" />
    <WtmField entityKey="GroupCode" v-if="IsGroupCode" />
    <WtmField entityKey="UserCode" v-else />
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
      TableName: undefined,
    },
    IsAll: true,
    DpType: 'UserGroup',
  };
  get IsGroupCode() {
    return this.formState.DpType == 'UserGroup'
  }
  async onLoading() {
    if (this.ID) {
      await this.Entities.onLoading(this.PageController.Pagination.onFind(this.ID));
      this.formState = this.lodash.assign({}, this.formState, this.Entities.dataSource)
    }
  }
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
