<template>
  <span v-html="valueText"></span>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component({ components: {} })
export default class display extends Vue {
  @Prop() private value: any;
  @Prop() private Entitie: any;
  @Prop() private dataSource: any[];
  get valueText() {
    if (
      lodash.isArray(this.value) &&
      lodash.isArray(this.dataSource) &&
      this.dataSource.length
    ) {
      return lodash
        .map(this.value, val => {
          const data = lodash.find(this.dataSource, ["key", val]);
          return `
        <div class="ant-tag ant-tag-blue">${data.label}</div>
        `;
        })
        .join(" ");
    }
    return this.value || "- - - ";
  }
  mounted() {
    // console.log(this);
  }
}
</script>
<style scoped lang="less">
</style>
