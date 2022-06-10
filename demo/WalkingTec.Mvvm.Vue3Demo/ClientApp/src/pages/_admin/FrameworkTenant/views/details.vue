<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template >
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="TCode" />
      <WtmField entityKey="TName" />
    </a-space>
    <a-space>
      <WtmField entityKey="TDb" />
      <WtmField entityKey="TDbType" />
    </a-space>
    <a-space>
      <WtmField entityKey="DbContext" />
      <WtmField entityKey="TDomain" />
    </a-space>
    <a-space>
      <WtmField entityKey="EnableSub" />
      <WtmField entityKey="Enabled" />
    </a-space>
    <a-space>
      <WtmField entityKey="AdminRoleCode" />
      <span style="font-size:12px; color:#999;"><i18n-t :keypath="$locales.FrameworkTenantAdminRoleCodeTip"  /></span>
    </a-space>
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
import EnumLocaleLabel from "../locales";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    Entity: {

    },
  };
  EnumLocaleLabel = EnumLocaleLabel
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
