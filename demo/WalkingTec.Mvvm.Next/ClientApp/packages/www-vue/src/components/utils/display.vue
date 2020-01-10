<template>
  <span v-text="valueText"></span>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component({ components: {} })
export default class display extends Vue {
  @Prop() private value: any;
  get valueText() {
    return lodash.get(this, "value", "- - -");
  }
}
</script>
<style scoped lang="less">
</style>
