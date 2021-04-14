<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <a-space>
      <WtmField entityKey="ITCode" :disabled="IsEdit" />
      <WtmField entityKey="Password" />
    </a-space>
    <a-space>
      <WtmField entityKey="Email" />
      <WtmField entityKey="Name" />
    </a-space>

    <WtmField entityKey="Gender" />
    <WtmField entityKey="SelectedRolesCodes" />
    <WtmField entityKey="SelectedGroupCodes" />
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
      ID: "",
      ITCode: "",
      Password: "",
      Email: "",
      Name: "",
      Gender: "",
    },
    SelectedRolesCodes: [],
    SelectedGroupCodes: [],
  };
  created() { }
  mounted() {
    this.onLoading()
  }
}
</script>
<style lang="less">
</style>
