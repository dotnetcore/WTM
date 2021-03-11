<template>
  <a-pagination v-model:current="current" :total="999" show-less-items />
</template>

<script lang="ts">
import { Options, Prop, Vue, Watch } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  current = 1;
  created() {}
}
</script>
<style  lang="less">
</style>
