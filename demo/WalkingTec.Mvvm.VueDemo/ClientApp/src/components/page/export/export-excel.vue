<template>
  <div class="export-excel">
    <el-button :type="btnType" icon="el-icon-download" @click="onExport">
      {{ btnName }}
    </el-button>
    <el-dialog :title="title" :visible="dialogVisible" width="40%" :append-to-body="appendToBody" @close="onClose">
      <!-- <div class="text-title">
                如果导出的数据过多会导致导出时间稍长，请您耐心等待
            </div> -->
      <div v-show="!isFinish" class="text-title">
        正在导出中，请耐心等待...{{ animatedNumber }}%
      </div>
      <div v-if="isFinish" class="text-title">
        <p class="success-title">
          <i class="el-icon-check success-icon" />成功导出{{ position }}条订单
        </p>
        <p class="success-text">
          请在电脑的下载文件夹中查看
        </p>
      </div>

      <el-progress v-show="!isFinish" class="progress-class" :text-inside="true" :stroke-width="18" :percentage="animatedNumber" />

      <flex-box justify="space-between">
        <el-button type="primary" :disabled="isExporting" :loading="btnLoading" @click="onBegin">
          导出
        </el-button>

        <el-button @click="onClose">
          {{ textTips }}
        </el-button>
      </flex-box>
    </el-dialog>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import store from "@/store/modules/export";
import mixin from "./mixin";
import { TweenLite } from "gsap/TweenMax";
import FlexBox from "@/components/page/FlexBox.vue";
import { getUrlByParamArr } from "@/util/params";
type progressLineType = {
    progressClone: number;
};
type optionsType = {
    count: number;
    batch_type: string;
    export_count: number;
    [property: string]: any;
};
@Component({
    store,
    mixins: [mixin],
    components: { FlexBox }
})
export default class ExportExcel extends Vue {
    @Prop({ type: String, default: "/export/orders.xlsx" })
    exportUrl;
    @Prop({ type: String, default: "导出" })
    btnName;
    @Prop({ type: String, default: "primary" })
    btnType;
    @Prop({ type: String, default: "" })
    querySession;
    @Prop({ type: String, default: "" })
    batchType;
    @Prop({ type: String, default: "提示" })
    title;
    @Prop({ type: String, default: "./" })
    postParam;
    @Prop({ type: String, default: "" })
    type;
    @Prop({ type: Boolean, default: true })
    needModal;
    @Prop({ type: Object, default: () => ({}) })
    params;
    @Prop({ type: Boolean, default: true })
    appendToBody;
    dialogVisible: boolean = false;
    maxNum: number = 5000;
    num: number = 0;
    isExporting: boolean = false;
    btnLoading: boolean = false;
    isCanExport: boolean = false;
    timer: NodeJs.Timeout | null = null;
    textTips: string = "";
    progressLine: progressLineType = {
        progressClone: 0
    };
    animatedNumber: number = 0;
    onBegin() {
        this.onProgress();
        // const options: optionsType = {
        //     count: 10,
        //     batch_type: this.batchType,
        //     export_count: 5000, //导出速度
        //     ...this.params
        // };
        // if (options.order_type)
        //     options.order_type = options.order_type.toUpperCase();
        // if (options.batch_type)
        //     options.batch_type = options.batch_type.toUpperCase();

        // this["getExportInfo"](options)
        //     .then(() => {
        //         if (this["session"]) {
        //             this.onProgress();
        //         } else {
        //             this.$message.error("下载文件出错");
        //         }
        //     })
        //     .catch(data => {
        //         this.$message.error(data || "导出信息获取失败");
        //     });
    }
    onProgress() {
        if (this["exceed"]) {
            window.clearTimeout(this["timer"]);
            this.$message.error(this["exceedMsg"]);
            this.onClose();
        }
        if (this.num < this.maxNum && !this["isFinish"]) {
            this["timer"] = setTimeout(() => {
                // this["getProgress"]();
                this.animatedNumber = this.animatedNumber + 10;
                this.onProgress();
                // console.log("isFinish:", this["isFinish"]);
            }, 500);
        }
        this.isExporting = true;
        this.btnLoading = true;
        if (this.animatedNumber >= 100) {
            window.clearTimeout(this.timer);
            // this["setProgress"](100);
            this.btnLoading = false;
            this.download();
            this.textTips = "确 认";
        }
    }
    onExport() {
        this.textTips = "取 消";
        this.dialogVisible = true;
    }
    onClose() {
        console.log("close....");
        this.dialogVisible = false;
        setTimeout(() => {
            this.maxNum = 100;
            this.num = 0;
            this.isExporting = false;
            this.btnLoading = false;
            this["setSession"]("");
            this["setProgress"](0);
            this["setIsFinish"](false);
            this["setPosition"](0);
            this["setExceedMsg"]("");
            this["setExceed"](false);
            window.clearTimeout(this.timer);
        }, 400);
    }
    download() {
        const link: any = document.createElement("a");
        link.href = getUrlByParamArr(this.exportUrl, this.params);
        link.style = "visibility:hidden";
        // link.download = this.session + ".xlsx";
        document.body.appendChild(link);
        link.click();
        this.$forceUpdate();
    }
    @Watch("progress")
    listenProgress(newVal) {
        TweenLite.to(this.progressLine, 2.8, { progressClone: newVal });
    }
    // get animatedNumber() {
    //     return parseInt(this.progressLine.progressClone.toFixed(0));
    // }
}
</script>
<style lang="less" scoped>
.export-excel {
    display: inline-block;
    margin-left: 10px;
}
.success-title {
    color: #606266;
    font-size: 14px;
}
.success-text {
    color: #909399;
    font-size: 12px;
    margin: 10px 0 10px 25px;
}
.progress-class {
    margin: 10px 0;
}
.success-icon {
    border-radius: 100%;
    background: #0d849a;
    color: #fff;
    margin-right: 10px;
}
</style>
