<template>
  <WtmDetails @finish="onFinish">
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
      ITCode: "",
      Password: "",
    },
  };
  onFinish(values) {
    console.log("LENG ~ extends ~ onFinish ~ values", values);
  }
  created() {}
}
</script>
<style lang="less">
</style>
