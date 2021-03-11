<template>
  <Layout />
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import Layout from "./layouts/layout.vue";
@Options({ components: { Layout } })
export default class extends Vue {
  created() {
    console.log("LENG ~ extends ~ created ~ this", this)
  }
}
</script>
<style lang="less">
#app {
  height: 100%;
}
</style>
