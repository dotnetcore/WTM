<template>
  <a-avatar size="small">
    <template #icon><UserOutlined /></template>
  </a-avatar>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
// Component definition
@Options({ components: {} })
export default class extends Vue {
  created() {}
  mounted() {}
}
</script>
<style lang="less">
</style>
