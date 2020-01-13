<template>
  <div v-loading="loading" class="table-card">
    <div class="table-box">
      <el-table class="list-table" v-bind="$attrs" stripe border element-loading-text="拼命加载中" v-on="tableListeners">
        <el-table-column v-if="isSelection" type="selection" align="center" width="55" />
        <!-- 判断是否需要插槽,自定义列内容 -->
        <template v-for="(item, index) of Cols">
          <el-table-column v-if="item.isSlot" :key="index" :label="item.label" :width="item.width" :align="item.align || 'center'" :fixed="item.fixed">
            <template slot-scope="scope">
              <slot :name="item.key" :row="{...scope.row}" />
            </template>
          </el-table-column>
          <el-table-column v-else :key="index" :sortable="item.sortable" :prop="item.key" :label="item.label" :width="item.width" :align="item.align || 'center'" />
        </template>
      </el-table>
      <!-- table 右侧展示 -->
      <div v-if="false" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selCols">
          <el-checkbox v-for="item in tbColumn" :key="item.key" :label="item.key" :checked="true">
            {{ item.label }}
          </el-checkbox>
        </el-checkbox-group>
        <div class="col-but" @click="onOpenCol">
          <i :class="[isColBox?'el-icon-s-unfold':'el-icon-s-fold']" />
          自定义列
        </div>
      </div>
      <!-- Popover 弹出框 -->
      <el-popover v-else placement="top" trigger="click" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selCols">
          <el-checkbox v-for="item in tbColumn" :key="item.key" :label="item.key" :checked="true">
            {{ item.label }}
          </el-checkbox>
        </el-checkbox-group>
        <div slot="reference" class="col-but" @click="onOpenCol">
          <i :class="[isColBox?'el-icon-s-unfold':'el-icon-s-fold']" />
          自定义列
        </div>
      </el-popover>
    </div>
    <el-pagination v-if="isPagination" class="page-box" :current-page="parseInt(pageDate.currentPage)" :page-sizes="pageDate.pageSizes" :page-size="pageDate.pageSize" :layout="pageDate.layout || layout" :total="pageDate.pageTotal" v-on="$listeners" />
  </div>
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
    @Prop({ type: String, default: "total, sizes, prev, pager, next, jumper" })
    layout; // 分页参数
    // 选中列
    selCols: string[] = [];
    // 展行选择
    isColBox: boolean = false;
    // 剔除table重复事件
    get tableListeners() {
        const envList = Object.assign({}, this.$listeners);
        delete envList["current-change"];
        return envList;
    }
    // 现实分页
    get isPagination() {
        return !this.$attrs["tree-props"];
    }
    /**
     * 自定义列
     */
    get Cols() {
        return _.filter(this.tbColumn, item => {
            return _.includes(this.selCols, item.key);
        });
    }
    /**
     * 打开行
     */
    onOpenCol() {
        this.isColBox = !this.isColBox;
    }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "~@/assets/css/mixin.less";
.table-box {
    .flexbox(row);
    .col-box {
        border: 1px solid #ebeef5;
        border-left: 0px solid #ebeef5;
        .flexbox(row);
        .col-but {
            padding-top: 10px;
            width: 2em;
            writing-mode: tb-rl;
            -webkit-writing-mode: vertical-rl;
            writing-mode: vertical-rl;
            *writing-mode: tb-rl;
            background: #f3f8ff;
            font-size: 14px;
            color: #606266;
            cursor: pointer;
            .flexbox();
            .flexalign(center);
        }
        .el-checkbox-group {
            .flexbox(column);
            padding: 10px 5px 0;
            .el-checkbox {
                margin-right: 0;
            }
        }
    }
}
.page-box {
    margin-top: 15px;
}
</style>
