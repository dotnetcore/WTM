<template>
  <a-pagination
    class="w-pagination"
    :current="Pagination.current"
    :pageSize="Pagination.pageSize"
    :total="999"
    size="small"
    show-less-items
    show-size-changer
    show-quick-jumper
    @change="change"
    @showSizeChange="change"
  />
</template>

<script lang="ts">
import { Options, Prop, Vue, Watch } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
@Options({
  components: {},
})
export default class extends Vue {
  @Prop({ required: true }) PageController: ControllerBasics;
  get Pagination() {
    return this.PageController.Pagination;
  }
  get currentKey() {
    return this.Pagination.options.currentKey;
  }
  get pageSizeKey() {
    return this.Pagination.options.pageSizeKey;
  }
  get defaultPageSize() {
    return this.Pagination.options.defaultPageSize;
  }
  change(current, pageSize) {
    const values = {
      [this.currentKey]: current,
      [this.pageSizeKey]: pageSize,
    };
    if (this.defaultPageSize === pageSize) {
      this.lodash.unset(values, this.pageSizeKey);
    }
    this.__wtmToQuery(values);
    this.Pagination.onCurrentChange({ current, pageSize });
  }
  created() {}
}
</script>
<style  lang="less">
.w-pagination.ant-pagination {
  margin-top: 8px;
  text-align: right;
}
</style>
