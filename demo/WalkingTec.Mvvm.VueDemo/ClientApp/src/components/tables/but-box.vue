<template>
  <el-card class="but-box">
    <el-button v-assembly:[assembly]="butTypes.add" icon="el-icon-plus" @click="onAdd">
      添加
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.edit" icon="el-icon-edit" @click="onEdit">
      修改
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.delete" icon="el-icon-delete" @click="onDelete">
      删除
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.import" icon="el-icon-upload" @click="onImport">
      导入
    </el-button>
    <el-button v-assembly:[assembly]="butTypes.export" icon="el-icon-download" @click="onExport">
      导出
    </el-button>
  </el-card>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { butType } from "@/config/enum";

@Component({
    directives: {
        assembly: {
            inserted: (el, binding) => {
                if (!(binding.arg && binding.arg.includes(binding.value))) {
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
    butTypes = butType;
    onAdd() {
        this.$emit("onAdd");
    }
    onEdit() {
        this.$emit("onEdit");
    }
    onDelete() {
        this.$emit("onDelete");
    }
    onImport() {
        this.$emit("onImport");
    }
    onExport() {
        this.$emit("onExport");
    }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "../../assets/css/mixin.less";
.but-box {
    .flexbox(row, flex-end);
    margin-top: 30px;
}
</style>
