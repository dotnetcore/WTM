<template>
  <WtmDetails :onFinish="onFinish">
    <a-space>
      <WtmField entityKey="ITCode" />
      <WtmField entityKey="Password" />
    </a-space>
    <a-space>
      <WtmField entityKey="Email" />
      <WtmField entityKey="Name" />
    </a-space>

    <WtmField entityKey="Gender" />
    <WtmField entityKey="SelectedGroupCodes" />
  </WtmDetails>
</template>
<script lang="ts">
import { Vue, Options, Inject, Provide } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController;
  @Provide({ reactive: true }) readonly formState = {
    Entity: {
      ITCode: "1",
      Password: "2",
      Email: "3",
      Name: "4",
      Gender: "Male",
    },
    SelectedGroupCodes: [],
  };
  async onFinish(values) {
    console.log("LENG ~ extends ~ onFinish ~ values", values);
    return new Promise((res, rej) => {
      this.lodash.delay(() => rej("出错了"), 2000);
    });
  }
  created() {}
}
</script>
<style lang="less">
</style>
