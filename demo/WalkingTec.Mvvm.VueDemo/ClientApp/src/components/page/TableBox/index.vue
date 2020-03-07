<template>
  <div v-loading="tableAttrs.loading" class="table-card">
    <div class="table-box">
      <el-table class="list-table" v-bind="tableAttrs" stripe border element-loading-text="拼命加载中" v-on="tableEvents">
        <el-table-column v-if="tableAttrs.isSelection" type="selection" align="center" width="55" />
        <!-- 判断是否需要插槽,自定义列内容 -->
        <template v-for="(item, index) of Cols">
          <el-table-column v-if="item.isSlot" :key="index" :label="item.label" :width="item.width" :align="item.align || 'center'" :fixed="item.fixed">
            <template slot-scope="scope">
              <slot :name="item.key" :row="{ ...scope.row }" />
            </template>
          </el-table-column>
          <el-table-column v-else-if="item.isOperate" :key="index" :label="item.label" :width="item.width" :align="item.align || 'center'" :fixed="item.fixed">
            <template slot-scope="scope">
              <el-button v-if="item.actions.includes('detail')" v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="colEvents.onDetail(scope.row)">
                详情
              </el-button>
              <el-button v-if="item.actions.includes('edit')" v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="colEvents.onEdit(scope.row)">
                修改
              </el-button>
              <el-button v-if="item.actions.includes('deleted')" v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="colEvents.onDelete(scope.row)">
                删除
              </el-button>
            </template>
          </el-table-column>

          <el-table-column v-else :key="index" :sortable="item.sortable" :prop="item.key" :label="item.label" :width="item.width" :align="item.align || 'center'" />
        </template>
      </el-table>
      <!-- table 右侧展示 -->
      <div v-if="false" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selCols">
          <el-checkbox v-for="item in tableAttrs.tbHeader" :key="item.key" :label="item.key" :checked="true">
            {{ item.label }}
          </el-checkbox>
        </el-checkbox-group>
        <div class="col-but" @click="onOpenCol">
          <i :class="[isColBox ? 'el-icon-s-unfold' : 'el-icon-s-fold']" />
          自定义列
        </div>
      </div>
      <!-- Popover 弹出框 -->
      <el-popover v-else placement="top" trigger="click" @hide="isColBox = false" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selCols">
          <el-checkbox v-for="item in tableAttrs.tbHeader" :key="item.key" :label="item.key" :checked="true">
            {{ item.label }}
          </el-checkbox>
        </el-checkbox-group>
        <div slot="reference" class="col-but" @click="onOpenCol">
          <i :class="[isColBox ? 'el-icon-s-unfold' : 'el-icon-s-fold']" />
          自定义列
        </div>
      </el-popover>
    </div>
    <el-pagination v-if="isPagination" class="page-box" v-bind="pageAttrs" :current-page="pageAttrs.currentPage" :page-sizes="pageAttrs.pageSizes" :page-size="pageAttrs.pageSize" :layout="pageAttrs.layout" :total="pageAttrs.pageTotal" v-on="pageEvents" />
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
/**
 * table列表&分页 组件组合
 *
 * $listeners：(events)
 * el-table，el-pagination 添加统一的 v-on="$listeners"
 * 存在问题：如果el-table，el-pagination组件事件名相同时，需要自定义事件区分执行
 * 解决问题：在tableEvents 中删除命名相同的事件
 * $attrs：(attrs)
 * $attrs作用域赋给el-table，el-pagination需要通过prop自定义
 */
@Component
export default class TableBox extends Vue {
    /* ---------table 属性--------- */
    @Prop(Boolean)
    loading;
    @Prop({ type: Boolean, default: false })
    isSelection; // 是否多选
    @Prop({ type: Array, default: () => [] })
    tbHeader; // 列字段数据
    /* ---------page 属性--------- */
    @Prop({ type: Number, default: 0 })
    currentPage; //当前页
    @Prop({ type: Array, default: () => [10, 25, 50, 100] })
    pageSizes; // 页码集合
    @Prop({ type: Number, default: 0 })
    pageSize; // 页码大小
    @Prop({ type: Number, default: 0 })
    pageTotal; // 总量
    @Prop({ type: String, default: "total, sizes, prev, pager, next, jumper" })
    layout; // 分页参数
    @Prop({ type: Object, default: {} })
    events!: object; // 事件集合
    @Prop({ type: Object, default: {} })
    attrs!: object; // 属性集合
    // 选中列
    selCols: string[] = [];
    // 展行选择
    isColBox: boolean = false;
    /**
     * table 事件
     */
    get tableEvents() {
        const envObj = Object.assign({}, this.events, this.$listeners);
        delete envObj["current-change"]; // 剔除重复 与page组件重复事件
        return envObj;
    }
    /**
     * column 事件
     */
    get colEvents() {
        return {
            onDetail: this.tableEvents.onDetail,
            onEdit: this.tableEvents.onEdit,
            onDelete: this.tableEvents.onDelete
        };
    }
    /**
     * page 事件
     */
    get pageEvents() {
        const envObj = Object.assign({}, this.events, this.$listeners);
        return envObj;
    }
    /**
     * table 属性
     */
    get tableAttrs() {
        const attrsObj = Object.assign({}, this.attrs, this.$attrs);
        return {
            ...attrsObj,
            isSelection: this.isSelection || attrsObj.isSelection,
            tbHeader:
                this.tbHeader.length > 0 ? this.tbHeader : attrsObj.tbHeader
        };
    }
    /**
     * page 属性
     */
    get pageAttrs() {
        const attrsObj = Object.assign({}, this.attrs, this.$attrs);
        return {
            ...attrsObj,
            layout: this.layout || attrsObj.attrsObj
        };
    }
    /**
     *  动作列表
     */
    get actionList() {
        return this.tableAttrs.actionList;
    }
    // 现实分页
    get isPagination() {
        return !this.$attrs["tree-props"];
    }
    /**
     * 自定义列
     */
    get Cols() {
        return _.filter(this.tableAttrs.tbHeader, item => {
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
