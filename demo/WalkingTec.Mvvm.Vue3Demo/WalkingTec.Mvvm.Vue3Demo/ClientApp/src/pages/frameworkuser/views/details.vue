<template>
  <WtmDetails :onFinish="onFinish">
    <WtmField entityKey="ITCode" />
    <WtmField entityKey="Password" />
    <WtmField entityKey="Email" />
    <WtmField entityKey="Name" />
    <WtmField entityKey="Gender" />
  </WtmDetails>
</template>
<script lang="ts">
import { Vue, Options, Inject, Provide } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  @Inject() PageController;
  @Provide() formState = {
    Entity: {
      ITCode: "1",
      Password: "2",
      Email: "3",
      Name: "4",
      Gender: "5",
    },
  };
  async onFinish(values) {
    console.log("LENG ~ extends ~ onFinish ~ values", values);
    return new Promise((res, rej) => {
      this.lodash.delay(rej, 2000);
    });
  }
  created() {}
}
</script>
<style lang="less">
</style>
