<template>
  <a-pagination
    :current="PageStore.Current"
    :pageSize="PageStore.PageSize"
    :total="PageStore.Total"
    :showSizeChanger="Props.showSizeChanger"
    :hideOnSinglePage="Props.hideOnSinglePage"
    :size="Props.size"
    :pageSizeOptions="pageSizeOptions"
    @change="onChange"
    @showSizeChange="onChange"
  />
</template>

<script lang="ts">
import { EntitiesPageStore } from "@leng/public/src";
import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
import { toJS } from "mobx";
@Component
export default class Pagination extends Vue {
  @Prop() PageStore: EntitiesPageStore;
  @Prop() Pagination: any;
  pageSizeOptions = ["10", "20", "50", "100", "200"];
  Props = lodash.merge(
    {
      // hideOnSinglePage: true,
      showSizeChanger: true,
      size: "small"
    },
    this.Pagination
  );
  onChange(current, size) {
    this.PageStore.onSearch({
      body: lodash.merge({}, toJS(this.PageStore.SearchParams), {
        Page: current,
        Limit: size
      })
    });
  }
  mounted() {}
  destroyed() {}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
