<template>
  <el-row class="stats-card">
    <div class="card-header" :class="[backgroundColor]">
      <i :class="[elIcon]"></i>
    </div>
    <div class="card-content">
      <slot name="content"></slot>
    </div>
    <el-col :span="24" class="card-actions">
      <slot name="footer"></slot>
    </el-col>
  </el-row>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component({
  name: "StatsCard"
})
export default class extends Vue {
  @Prop({ default: "" })
  backgroundColor: string;
  @Prop({ default: "" })
  elIcon: string;
}
</script>

<style lang="less" scoped>
.stats-card {
  background-color: #fff;
  display: inline-block;
  position: relative;
  width: 100%;
  margin: 25px 0;
  overflow: unset;
  box-shadow: 0 1px 4px 0 rgba(0, 0, 0, 0.14);
  border-radius: 3px;
  color: rgba(0, 0, 0, 0.87);
  .card-header {
    float: left;
    text-align: center;
    margin: -20px 15px 0;
    border-radius: 3px;
    padding: 15px;
    background-color: #999999;
    i {
      font-size: 36px !important;
      line-height: 56px;
      width: 56px;
      height: 56px;
      color: #ffffff !important;
    }
  }
  .card-content {
    text-align: right;
    padding-top: 10px;
    padding: 15px 20px;
    font-size: 16px;
    line-height: 22px;
    display: block;
  }
  .card-actions {
    margin: 0 20px 10px;
    width: calc(100% - 40px);
    padding: 10px 0 0 0;
    border-top: 1px solid #eee;
  }

  .purple {
    background: linear-gradient(60deg, #ab47bc, #8e24aa);
  }
  .blue {
    background: linear-gradient(60deg, #26c6da, #00acc1);
  }
  .green {
    background: linear-gradient(60deg, #66bb6a, #43a047);
  }
  .orange {
    background: linear-gradient(60deg, #ffa726, #fb8c00);
  }
  .red {
    background: linear-gradient(60deg, #ef5350, #e53935);
  }
}
</style>
