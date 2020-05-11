<template>
    <!-- table 右侧展示 -->
    <div v-if="false" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selects" @change="onChange">
            <el-checkbox v-for="item in headerList" :key="item.key" :label="item.key">
                {{ getColsRowItemLabel(item) }}
            </el-checkbox>
        </el-checkbox-group>
        <div class="col-but" @click="onOpenCol">
            <i :class="[isColBox ? 'el-icon-s-unfold' : 'el-icon-s-fold']" />
            {{ $t("table.custom") }}
        </div>
    </div>
    <!-- Popover 弹出框 -->
    <el-popover v-else placement="top" trigger="click" @hide="isColBox = false" class="col-box">
        <el-checkbox-group v-show="isColBox" v-model="selects" @change="onChange">
            <el-checkbox v-for="item in headerList" :key="item.key" :label="item.key">
                {{ getColsRowItemLabel(item) }}
            </el-checkbox>
        </el-checkbox-group>
        <div slot="reference" class="col-but" @click="onOpenCol">
            <i :class="[isColBox ? 'el-icon-s-unfold' : 'el-icon-s-fold']" />
            {{ $t("table.custom") }}
        </div>
    </el-popover>
</template>

<script lang="ts">
import { Component, Vue, Prop, Emit } from "vue-property-decorator";
@Component
export default class CustomColumn extends Vue {
    /**
     * 列字段数据
     */
    @Prop({ type: Array, default: () => [] })
    tableHeader;
    /**
     * 默认列字段
     */
    @Prop({ type: Array })
    defaultHeaderKeys?;
    /**
     * 选中
     */
    selects: string[] = [];
    /**
     * 展行选择
     */
    isColBox: boolean = false;
    /**
     * 打开行
     */
    onOpenCol(): void {
        this.isColBox = !this.isColBox;
    }
    /**
     * 多语言key
     */
    @Prop({ type: String })
    languageKey?;
    /**
     * 操作 header
     */
    @Emit("onSetHeader")
    onChange(data) {
        return data;
    }
    get headerList() {
        return this.tableHeader.filter(item => !item.isOperate);
    }
    /**
     * 列文案
     */
    getColsRowItemLabel(item: Object) {
        return this.$getLanguageByKey({
            languageKey: this.languageKey,
            label: item.label,
            key: item.key
        });
    }
    created() {
        if (this.defaultHeaderKeys) {
            this.selects = _.clone(this.defaultHeaderKeys);
        } else {
            this.selects = _.map(this.tableHeader, "key");
        }
        this.onChange(this.selects);
    }
}
</script>
<style lang="less" scoped>
@import "~@/assets/css/mixin.less";
.col-box {
    border: 1px solid #ebeef5;
    border-left: 0px solid #ebeef5;
    .flexbox(row);
    white-space: nowrap;
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
</style>
