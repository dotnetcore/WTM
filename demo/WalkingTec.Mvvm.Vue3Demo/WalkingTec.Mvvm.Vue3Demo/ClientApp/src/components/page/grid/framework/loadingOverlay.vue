<template>
  <div>
    <a-spin />
  </div>
</template>

<script lang="ts">
import { Options, Prop, Vue } from "vue-property-decorator";
import { defineComponent } from "vue";
@Options({
  components: {},
})
export default class extends Vue {
  created() {}
  mounted() {
  }
  updated() {}
  destroyed() {}
}
// export default defineComponent({
//   created() {
//   },
//   mounted() {
//   },
// });
</script>

<style lang="less">
</style>
