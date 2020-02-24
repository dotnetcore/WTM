<template>
  <span v-if="childrenType==='span'" v-html="valueText"></span>
  <w-avatar v-else-if="childrenType==='avatar'" :value="value" disabled />
  <a-switch v-else-if="childrenType==='switch'" :checked="value" disabled>
    <a-icon type="check" slot="checkedChildren" />
    <a-icon type="close" slot="unCheckedChildren" />
  </a-switch>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component({ components: {} })
export default class display extends Vue {
  @Prop() private value: any;
  @Prop() private Entitie: any;
  @Prop() private dataSource: any[];
  get childrenType() {
    if (lodash.startsWith(this.Entitie.children, "<w-avatar")) {
      return "avatar";
    }
    if (lodash.startsWith(this.Entitie.children, "<a-switch")) {
      return "switch";
    }
    return "span";
  }
  get valueText() {
    if (
      // lodash.isArray(this.value) &&
      lodash.isArray(this.dataSource) &&
      this.dataSource.length
    ) {
      let { value } = this;
      if (!lodash.isArray(value)) {
        value = [value];
      }
      return lodash
        .map(value, val => {
          const data = lodash.find(this.dataSource, ["value", val]);
          if (data && data.label) {
            return `<div class="ant-tag ant-tag-blue">${data.label}</div>`;
          }
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
