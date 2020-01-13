<template>
  <div class="but-box">
    <el-button v-assembly:[assembly]="butTypes.add" v-visible="actionList.add" type="primary" icon="el-icon-plus" @click="onAdd">
      添加
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.edit" v-visible="actionList.edit" type="primary" :disabled="isDisabledEdit" icon="el-icon-edit" @click="onEdit">
      修改
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.deleted" v-visible="actionList.batchDelete" type="primary" :disabled="isDisabledEelete" icon="el-icon-delete" @click="onDelete">
      删除
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.imported" type="primary" icon="el-icon-upload" @click="onImported">
      导入
    </el-button>
    <el-dropdown v-visible="[actionList.exportExcel,actionList.exportExcelByIds]" class="dropdown-box" @command="onCommand">
      <el-button v-assembly:[assembly]="butTypes.export" type="primary">
        <i class="el-icon-download" /> <span>导出</span> <i class="el-icon-arrow-down el-icon--right" />
      </el-button>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item v-visible="actionList.exportExcel" command="onExportAll">
          导出全部
        </el-dropdown-item>
        <el-dropdown-item v-visible="actionList.exportExcelByIds" command="onExport">
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
    // 权限列表
    @Prop({
        type: Object,
        default: () => {
            return {};
        }
    })
    actionList;
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
    onImported() {
        this.$emit("onImported");
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
    margin: 15px 0;
    .dropdown-box {
        margin-left: 10px;
    }
}
</style>
