<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <template v-show="false">
      <WtmField entityKey="ID" />
    </template>
    <WtmField entityKey="SchoolCode" />
    <WtmField entityKey="SchoolName" />
    <WtmField entityKey="SchoolType" />
    <WtmField entityKey="Remark" />
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
      Gender: "Male",
      CellPhone: "",
      HomePhone: "",
      Address: "",
      ZipCode: "",
      PhotoId: undefined,
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
