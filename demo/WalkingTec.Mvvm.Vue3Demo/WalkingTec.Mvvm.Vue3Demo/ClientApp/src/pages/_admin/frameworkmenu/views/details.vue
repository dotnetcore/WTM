<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="IsInside" />
    </a-space>
    <a-space>
      <WtmField entityKey="PageName" />
      <WtmField entityKey="SelectedModule" />
    </a-space>
    <a-space>
      <WtmField
        entityKey="SelectedActionIDs"
        :fieldProps="{ mode: 'tags' }"
        :disabled="!formState.SelectedModule"
      />
    </a-space>
    <a-space>
      <WtmField entityKey="FolderOnly" />
      <WtmField entityKey="ShowOnMenu" />
      <WtmField entityKey="IsPublic" />
    </a-space>
    <a-space>
      <WtmField entityKey="DisplayOrder" />
      <WtmField entityKey="ParentId" />
    </a-space>
    <a-space>
      <WtmField entityKey="Url" />
      <WtmField entityKey="Icon" />
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
    Entity: {

    },
    SelectedModule: undefined
  };
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
