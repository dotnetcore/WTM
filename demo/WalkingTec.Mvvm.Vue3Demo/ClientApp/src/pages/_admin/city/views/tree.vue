<template>
  <a-tree :tree-data="treeData" @select="onSelect" />
</template>
<script lang="ts">
import { Vue, Options, Provide, Inject, Emit } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  treeData = [];
  async mounted() {
    const res = await this.PageController.onGetTree();
    this.treeData = this.$Encryption.toTree(res);
  }
  @Emit("select")
  onSelect(event) {
    return this.lodash.head(event) || null;
  }
}
</script>
<style lang="less"></style>
