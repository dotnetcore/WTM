<template>
  <a-tabs v-model:activeKey="activeKey" type="editable-card" hide-add @edit="onEdit">
    <a-tab-pane
      v-for="pane in PagesCache"
      :key="pane.path"
      :tab="getTab(pane)"
      :closable="pane.closable"
    ></a-tab-pane>
  </a-tabs>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import AppRouter from "@/router";
import { action, observable } from "mobx";
@Options({ components: {} })
export default class extends Vue {
  keep = true;
  PagesCache = [];
  get activeKey() {
    return this.$route.path;
  }
  set activeKey(value) {
    this.$router.replace(this.lodash.find(this.PagesCache, ["path", value]));
  }
  getTab(pane) {
    const name = this.lodash.get(pane, 'name')
    if (name && !this.lodash.eq(name, "NotFound")) {
      return this.$t(`PageName.${name}`)
    }
    return this.lodash.get(pane, 'path', 'NotFound')
  }
  onEdit() { }
  @action
  created() {
    // console.log("LENG ~ extends ~ created ~ this", this);
  }
  mounted() {
    AppRouter.RouterBehaviorSubject.subscribe((obs) => {
      this.PagesCache = obs;
    });
  }
}
</script>
<style lang="less">
</style>
