<template>
    <div class="but-box">
        <el-button v-assembly:[assembly]="butTypes.add" v-visible="permissionList.add" type="primary" icon="el-icon-plus" @click="onAdd">
            添加
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.edit" v-visible="permissionList.edit" type="primary" :disabled="isDisabledEdit" icon="el-icon-edit" @click="onEdit">
            修改
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.deleted" v-visible="permissionList.batchDelete" type="primary" :disabled="isDisabledEelete" icon="el-icon-delete" @click="onDelete">
            删除
        </el-button>
        <el-button v-assembly:[assembly]="butTypes.imported" type="primary" icon="el-icon-upload" @click="onImported">
            导入
        </el-button>
        <el-dropdown v-visible="[permissionList.exportExcel,permissionList.exportExcelByIds]" class="dropdown-box" @command="onCommand">
            <el-button v-assembly:[assembly]="butTypes.export" type="primary">
                <i class="el-icon-download" /> <span>导出</span> <i class="el-icon-arrow-down el-icon--right" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
                <el-dropdown-item v-visible="permissionList.exportExcel" command="onExportAll">
                    导出全部
                </el-dropdown-item>
                <el-dropdown-item v-visible="permissionList.exportExcelByIds" command="onExport">
                    导出勾选
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

function noop() {}

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
    permissionList;

    // 方法事件
    @Prop({
        type: Object,
        default: (): IEventFn => {
            return {
                onAdd: noop,
                onEdit: noop,
                onDelete: noop,
                onBatchDelete: noop,
                onImported: noop,
                onExportAll: noop,
                onExport: noop
            };
        }
    })
    eventFn;

    butTypes = butType;
    get isDisabledEdit() {
        if (this.selectedData.length === 1) {
            return false;
        }
        return true;
    }
    get isDisabledEelete() {
        if (this.selectedData.length > 0) {
            return false;
        }
        return true;
    }
    onAdd() {
        this.eventFn.onAdd ? this.eventFn.onAdd() : this.$emit("onAdd");
    }
    onEdit() {
        if (!this.isDisabledEdit) {
            this.eventFn.onEdit
                ? this.eventFn.onEdit(this.selectedData[0])
                : this.$emit("onEdit", this.selectedData[0]);
        }
    }

    onBatchDelete() {
        if (!this.isDisabledEelete) {
            this.eventFn.onBatchDelete
                ? this.eventFn.onBatchDelete(this.selectedData)
                : this.$emit("onBatchDelete", this.selectedData);
        }
    }

    onDelete() {
        if (!this.isDisabledEelete) {
            this.eventFn.onDelete
                ? this.eventFn.onDelete(this.selectedData[0])
                : this.$emit("onDelete", this.selectedData[0]);
        }
    }
    onImported() {
        this.eventFn.onImported
            ? this.eventFn.onImported()
            : this.$emit("onImported");
    }
    onExportAll() {
        this.eventFn.onExportAll
            ? this.eventFn.onExportAll()
            : this.$emit("onExportAll");
    }
    onExport() {
        this.eventFn.onExport
            ? this.eventFn.onExport()
            : this.$emit("onExport");
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
