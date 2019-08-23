<template>
  <el-card class="but-box">
    <el-button v-assembly:[assembly]="butTypes.add" icon="el-icon-plus" @click="onAdd">
      添加
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.edit" :disabled="isDisabledEdit" icon="el-icon-edit" @click="onEdit">
      修改
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.delete" :disabled="isDisabledEelete" icon="el-icon-delete" @click="onDelete">
      删除
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.import" icon="el-icon-upload" @click="onImport">
      导入
    </el-button>
    <el-dropdown class="dropdown-box" @command="onCommand">
      <el-button v-assembly:[assembly]="butTypes.export">
        <i class="el-icon-download" /> <span>导出</span> <i class="el-icon-arrow-down el-icon--right" />
      </el-button>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item command="onExportAll">
          导出全部
        </el-dropdown-item>
        <el-dropdown-item command="onExport">
          导出勾选
        </el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <!-- <export-excel v-assembly:[assembly]="butTypes.export" :params="exportOption.params" :export-url="exportOption.url" btn-name="导出" batch-type="EXPORT_REPLACEMENT" /> -->
  </el-card>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import ExportExcel from "@/components/common/export/export-excel.vue";
import { butType } from "@/config/enum";

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
    @Prop({
        type: Array,
        default: () => Object.keys(butType)
    })
    assembly;
    @Prop({
        type: Array,
        default: []
    })
    selectedData;
    @Prop({
        type: Object,
        default: () => {
            return { params: {}, url: "" };
        }
    })
    exportOption; // 导出参数 url地址

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
        this.$emit("onAdd");
    }
    onEdit() {
        if (!this.isDisabledEdit) {
            this.$emit("onEdit", this.selectedData[0]);
        }
    }
    onDelete() {
        if (!this.isDisabledEelete) {
            this.$emit("onDelete", this.selectedData);
        }
    }
    onImport() {
        this.$emit("onImport");
    }
    onCommand(command) {
        this.$emit(command);
    }
    // onExportAll() {
    //     console.log("onExportAll");
    //     this.$emit("onExportAll");
    // }
    // onExport() {
    //     this.$emit("onExport", this.selectedData);
    // }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "../../assets/css/mixin.less";
.but-box {
    .flexbox(row, flex-end);
    margin-top: 30px;
    .dropdown-box {
        margin-left: 10px;
    }
}
</style>
