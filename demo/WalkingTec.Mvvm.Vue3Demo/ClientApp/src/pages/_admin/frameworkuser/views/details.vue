<template>
  <ViewUpdate />
</template>
<script lang="ts">
import { Options, Vue } from "vue-property-decorator";
import ViewUpdate from "./details_update.vue";
@Options({ components: { ViewUpdate } })
export default class extends Vue {
  created() { }
  mounted() {
  }
}
</script>
<style lang="less">
</style>
