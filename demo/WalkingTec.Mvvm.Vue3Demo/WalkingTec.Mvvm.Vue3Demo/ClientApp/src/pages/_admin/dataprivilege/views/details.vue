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
  get body() {
    return this.lodash.assign({ ID: this.ID }, this.lodash.pick(this.$route.query, ['TableName', 'TargetId']));
  }
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
