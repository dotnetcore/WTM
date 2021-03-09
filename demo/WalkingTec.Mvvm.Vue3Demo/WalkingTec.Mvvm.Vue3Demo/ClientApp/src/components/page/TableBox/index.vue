<template>
    <div v-loading="tableAttrs.loading" class="table-card">
        <div class="table-box">
            <!-- v-el-height-full-auto-table="height"  -->
            <el-table ref="table" v-el-height-full-auto-table="!!height" class="list-table" v-bind="tableAttrs" stripe border element-loading-text="拼命加载中" v-on="tableEvents" :height="autoHeight">
                <el-table-column v-if="tableAttrs.isSelection" type="selection" align="center" width="55" />
                <!-- 判断是否需要插槽,自定义列内容 -->
                <template v-for="(item, index) of Cols">
                    <el-table-column v-if="item.isSlot" :key="index" :label="getColsRowItemLabel(item)" :width="item.width" :align="item.align || 'center'" :fixed="item.fixed">
                        <template slot-scope="scope">
                            <slot :name="item.key" :row="{ ...scope.row }" />
                        </template>
                    </el-table-column>
                    <el-table-column v-else-if="item.isOperate" :key="index" :label="getColsRowItemLabel(item, $t('table.actions'))" :width="item.width || actionsColWidth" :align="item.align || 'center'" :fixed="item.fixed">
                        <template slot-scope="scope">
                            <el-button v-if="item.actions.includes('detail')" v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="colEvents.onDetail(scope.row)">
                                {{ $t("table.detail") }}
                            </el-button>
                            <el-button v-if="item.actions.includes('edit')" v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="colEvents.onEdit(scope.row)">
                                {{ $t("table.edit") }}
                            </el-button>
                            <el-button v-if="item.actions.includes('deleted')" v-visible="actionList.batchDelete" type="text" size="small" class="view-btn" @click="colEvents.onDelete(scope.row)">
                                {{ $t("table.delete") }}
                            </el-button>
                        </template>
                    </el-table-column>
                    <el-table-column v-else :key="index" :sortable="item.sortable" :prop="item.key" :label="getColsRowItemLabel(item)" :width="item.width" :align="item.align || 'center'" />
                </template>
            </el-table>
            <custom-column :table-header="tableAttrs.tbHeader" :default-header-keys="tableAttrs.defaultHeaderKeys" @onSetHeader="onSetHeader" :languageKey="languageKey"></custom-column>
        </div>
        <el-pagination v-if="isPagination" class="page-box" v-bind="pageAttrs" :current-page="pageAttrs.currentPage" :page-sizes="pageAttrs.pageSizes" :page-size="pageAttrs.pageSize" :layout="pageAttrs.layout" :total="pageAttrs.pageTotal" v-on="pageEvents" />
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop, Ref } from "vue-property-decorator";
import CustomColumn from "./CustomColumn.vue";
import { AppModule } from "@/store/modules/app";
import { debounce } from "@/util/throttle-debounce.ts";
const PAUSE = 300;
const BOTTOM_OFFSET = 68;
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
@Component({
    components: { CustomColumn }
})
export default class TableBox extends Vue {
    /* ---------table 属性--------- */
    @Ref() readonly table;
    /**
     * loading
     */
    @Prop(Boolean)
    loading;
    /**
     * 是否多选
     */
    @Prop({ type: Boolean, default: false })
    isSelection;
    /**
     * 列字段数据
     */
    @Prop({ type: Array, default: () => [] })
    tbHeader;
    /**
     * 默认列字段
     */
    @Prop({ type: Array })
    defaultHeaderKeys?;
    /**
     * table height，不设置自动算高度
     */
    @Prop({ type: String })
    height?;

    /* ---------page 属性--------- */
    /**
     * 当前页
     */
    @Prop({ type: Number, default: 0 })
    currentPage;
    /**
     * 页码集合
     */
    @Prop({ type: Array, default: () => [10, 25, 50, 100] })
    pageSizes;
    /**
     * 页码大小
     */
    @Prop({ type: Number, default: 0 })
    pageSize;
    /**
     * 总量
     */
    @Prop({ type: Number, default: 0 })
    pageTotal;
    /**
     * 分页参数
     */
    @Prop({ type: String, default: "total, sizes, prev, pager, next, jumper" })
    layout;
    /**
     * 多语言key
     */
    @Prop({ type: String })
    languageKey?;
    /**
     * 事件集合
     */
    @Prop({ type: Object, default: {} })
    events!: object;
    /**
     * 属性集合
     */
    @Prop({ type: Object, default: {} })
    attrs!: object;

    // 选中列
    selCols: string[] = [];

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
            defaultHeaderKeys:
                this.defaultHeaderKeys || attrsObj.defaultHeaderKeys,
            tbHeader:
                this.tbHeader.length > 0 ? this.tbHeader : attrsObj.tbHeader,
            isSelection: this.isSelection || attrsObj.isSelection,
            loading: this.loading || attrsObj.loading,
            languageKey: this.languageKey || attrsObj.languageKey
        };
    }
    /**
     * page 属性
     */
    get pageAttrs() {
        const attrsObj = Object.assign({}, this.attrs, this.$attrs);
        return {
            ...attrsObj,
            layout: this.layout || attrsObj.layout,
            pageSizes: this.pageSizes || attrsObj.pageSizes,
            pageSize: this.pageSize || attrsObj.pageSize,
            currentPage: this.currentPage || attrsObj.currentPage,
            pageTotal: this.pageTotal || attrsObj.pageTotal
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

    get actionsColWidth() {
        if (AppModule.language === "en") {
            return 180;
        }
        return 150;
    }

    get autoHeight() {
        if (!!this.height && this.height !== 0) {
            return this.height;
        }
        return 200;
    }
    /**
     * 列文案
     */
    getColsRowItemLabel(item: Object, defaultLabel: string = "") {
        return this.$getLanguageByKey(
            {
                languageKey: this.tableAttrs.languageKey,
                label: item.label,
                key: item.key
            },
            defaultLabel
        );
    }
    /**
     *  选中列
     */
    onSetHeader(data) {
        this.selCols = data;
    }
}
</script>
<style lang="less" scoped>
@import "~@/assets/css/mixin.less";
.table-box {
    .flexbox(row);
}
.page-box {
    margin-top: 15px;
}
</style>
