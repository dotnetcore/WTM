<template>
  <el-card v-loading="loading" class="table-card">
    <el-table class="list-table" v-bind="$attrs" stripe border element-loading-text="拼命加载中" v-on="tableListeners">
      <el-table-column v-if="isSelection" type="selection" align="center" width="55" />
      <!-- 判断是否需要插槽,自定义列内容 -->
      <template v-for="(item, index) of tbColumn">
        <el-table-column v-if="item.isSlot" :key="index" :label="item.label" :width="item.width" :align="item.align || 'center'">
          <template slot-scope="scope">
            <slot :name="item.key" :row="{...scope.row}" />
          </template>
        </el-table-column>
        <el-table-column v-else :key="index" :sortable="item.sortable" :prop="item.key" :label="item.label" :width="item.width" :align="item.align || 'center'" />
      </template>
    </el-table>
    <el-pagination v-if="isPagination" :current-page="parseInt(pageDate.currentPage)" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" :layout="pageDate.layout || layout" :total="pageDate.pageTotal" v-on="$listeners" />
  </el-card>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
/**
 * table列表&分页 组件组合
 * $listeners：
 * el-table，el-pagination 添加统一的 v-on="$listeners"
 * 存在问题：如果el-table，el-pagination组件事件名相同时，需要自定义事件区分执行
 * 解决问题：在tableListeners 中删除命名相同的事件
 * $attrs：
 * $attrs作用域赋给el-table，el-pagination需要通过prop自定义
 */
@Component
export default class FuzzySearch extends Vue {
    @Prop(Boolean)
    loading;
    @Prop({ type: Boolean, default: false })
    isSelection; // 是否多选
    @Prop({ type: Array, default: () => [] })
    tbColumn; // 列字段数据
    @Prop({ type: Object, default: () => {} })
    pageDate; // 分页数据
    // @Prop({ type: Object, default: () => "" })
    // treeProps; // 树形数据
    // @Prop(Array)
    // tableData; // 列表数据

    // pageDate.layout默认
    @Prop({ type: String, default: "total, sizes, prev, pager, next, jumper" })
    layout;

    // 剔除table重复事件
    get tableListeners() {
        const envList = Object.assign({}, this.$listeners);
        delete envList["current-change"];
        return envList;
    }
    get isPagination() {
        return !this.$attrs["tree-props"];
    }
}
</script>
<style lang="less" rel="stylesheet/less">
</style>
