<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="IsInside" />
      <WtmField entityKey="PageName" />
    </a-space>
    <a-space>
      <WtmField entityKey="SelectedModule" v-if="IsInside" @change="onModuleChange" />
      <WtmField entityKey="Url" :disabled="IsInside" />
    </a-space>
    <a-space>
      <WtmField
        entityKey="SelectedActionIDs"
        :disabled="!formState.SelectedModule"
      />
    </a-space>
    <a-space>
      <WtmField entityKey="FolderOnly" />
      <WtmField entityKey="ShowOnMenu" />
      <WtmField entityKey="IsPublic" />
    </a-space>
    <a-space>
      <WtmField entityKey="ParentId" />
      <WtmField entityKey="DisplayOrder" />
    </a-space>
    <a-space>
      <WtmField entityKey="Icon" />
    </a-space>
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
import router from "@/router";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    Entity: {
      IsInside: true,
      Url: undefined
    },
    SelectedModule: undefined
  };
  get IsInside() {
    const IsInside = this.lodash.get(this.formState, 'Entity.IsInside', true)
    return IsInside
  }
  async onModuleChange(value) {
    const pages = await router.onGetRequest()
    const find = this.lodash.find(pages, ['value', value])
    this.formState.Entity.Url = find.path;
  }
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
