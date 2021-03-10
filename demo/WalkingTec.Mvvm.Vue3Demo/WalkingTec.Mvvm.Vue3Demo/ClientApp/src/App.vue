<template>
  <Layout />
</template>
<script lang="ts">
import { Vue, Options } from "vue-class-component";
import Layout from "./layouts/layout.vue";
@Options({ components: { Layout } })
export default class extends Vue {
  created() {
  }
}
</script>
<style lang="less">
#app {
  height: 100%;
}
</style>
