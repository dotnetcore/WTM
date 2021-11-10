<template>
  <a-result
    status="404"
    title="404"
    sub-title="Sorry, the page you visited does not exist."
  >
    <template #extra>
      <a-button type="primary" @click="click">Back Home</a-button>
    </template>
  </a-result>
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  created() {}
  click() {
    this.$router.replace({ name: "home" });
  }
}
</script>
<style lang="less" scoped></style>
