<template>
    <div class="but-box">
        <slot />
        <el-button v-assembly:[assembly]="butTypes.add" v-visible="actionList[actionKeys.add]" type="primary" icon="el-icon-plus" @click="onAdd">
            {{ $t("buttom.add") }}
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.edit" v-visible="actionList[actionKeys.edit]" type="primary" :disabled="isDisabledEdit" icon="el-icon-edit" @click="onEdit">
            {{ $t("buttom.edit") }}
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.deleted" v-visible="actionList[actionKeys.batchDelete]" type="primary" :disabled="isDisabledEelete" icon="el-icon-delete" @click="onBatchDelete">
            {{ $t("buttom.delete") }}
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.imported" v-visible="actionList[actionKeys.import]" type="primary" icon="el-icon-upload" @click="onImported">
            {{ $t("buttom.import") }}
        </el-button>
        <el-dropdown v-visible="[actionList[actionKeys.exportExcel],actionList[actionKeys.exportExcelByIds]]" class="dropdown-box" @command="onCommand">
            <el-button v-assembly:[assembly]="butTypes.export" type="primary">
                <i class="el-icon-download" /> <span>{{ $t("buttom.export") }}</span> <i class="el-icon-arrow-down el-icon--right" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
                <el-dropdown-item v-visible="actionList[actionKeys.exportExcel]" command="onExportAll">
                    {{ $t("buttom.exportAll") }}
                </el-dropdown-item>
                <el-dropdown-item v-visible="actionList[actionKeys.exportExcelByIds]" command="onExport">
                    {{ $t("buttom.exportSelect") }}
                </el-dropdown-item>
            </el-dropdown-menu>
        </el-dropdown>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import ExportExcel from "@/components/page/export/export-excel.vue";
import { butType } from "@/config/enum";
import { IEventFn } from "@/vue-custom/mixin/action-mixin";

@Component({
    components: {
        ExportExcel
    },
    directives: {
        assembly: {
            inserted: (el, binding) => {
                const { arg, value } = binding;
                if (!(arg && arg.includes(value))) {
                    el.style.display = "none";
                }
            }
        }
    }
})
export default class ButBox extends Vue {
    // 增删改查 操作权限集合
    @Prop({ type: Array, default: () => Object.keys(butType) })
    assembly;
    // 父级选中的对象
    @Prop({ type: Array, default: [] })
    selectedData;
    // 导出参数 url地址
    @Prop({
        type: Object,
        default: () => {
            return { params: {}, url: "" };
        }
    })
    exportOption;
    // 权限列表
    @Prop({
        type: Object,
        default: () => {
            return {};
        }
    })
    actionList;
    // 定义 权限列表key
    @Prop({
        type: Object,
        default: () => {
            return {
                add: "add",
                edit: "edit",
                batchDelete: "batchDelete",
                import: "imported",
                exportExcel: "exportExcel",
                exportExcelByIds: "exportExcelByIds"
            };
        }
    })
    actionKeys;

    // 方法事件
    @Prop({
        type: Object,
        default: (): IEventFn => {
            return {
                onAdd: () => {},
                onEdit: () => {},
                onDelete: () => {},
                onBatchDelete: () => {},
                onImported: () => {},
                onExportAll: () => {},
                onExport: () => {}
            };
        }
    })
    events;

    butTypes = butType;
    /**
     * 判断选中数据数量
     */
    get isDisabledEdit() {
        if (this.selectedData.length === 1) {
            return false;
        }
        return true;
    }
    // 判读是否有选中值
    get isDisabledEelete() {
        if (this.selectedData.length > 0) {
            return false;
        }
        return true;
    }

    onAdd() {
        this.events.onAdd ? this.events.onAdd() : this.$emit("onAdd");
    }
    onEdit() {
        if (!this.isDisabledEdit) {
            this.events.onEdit
                ? this.events.onEdit(this.selectedData[0])
                : this.$emit("onEdit", this.selectedData[0]);
        }
    }
    onBatchDelete() {
        if (!this.isDisabledEelete) {
            this.events.onBatchDelete
                ? this.events.onBatchDelete(this.selectedData)
                : this.$emit("onBatchDelete", this.selectedData);
        }
    }

    onDelete() {
        if (!this.isDisabledEelete) {
            this.events.onDelete
                ? this.events.onDelete(this.selectedData[0])
                : this.$emit("onDelete", this.selectedData[0]);
        }
    }
    onImported() {
        this.events.onImported
            ? this.events.onImported()
            : this.$emit("onImported");
    }
    onExportAll() {
        this.events.onExportAll
            ? this.events.onExportAll()
            : this.$emit("onExportAll");
    }
    onExport() {
        this.events.onExport ? this.events.onExport() : this.$emit("onExport");
    }
    onCommand(command) {
        if (command === "onExportAll") {
            this.onExportAll();
        } else if (command === "onExport") {
            this.onExport();
        }
    }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "../../assets/css/mixin.less";
.but-box {
    .flexbox(row, flex-end);
    margin: 15px 0;
    .dropdown-box {
        margin-left: 10px;
    }
}
</style>
