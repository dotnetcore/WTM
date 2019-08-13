<template>
  <article class="order-detail">
    <section class="left-container">
      <el-card class="order-info">
        <div slot="header" class="header-text">
          订单信息
        </div>
        <div>
          <el-row class="info-row">
            <label>订单编号</label>201909023892333
          </el-row>
          <el-row class="info-row">
            <label>意向车型</label>ES8七座版
          </el-row>
          <el-row class="info-row">
            <label>订单状态</label>已支付
          </el-row>
          <el-row class="info-row">
            <label>交车时间</label>2010-01-20
          </el-row>
        </div>
      </el-card>
      <el-card class="up-records">
        <div slot="header">
          <span class="header-text"> 跟进记录</span>
          <span class="header-right">nio跟进Fellow：XXX</span>
        </div>
        <el-steps direction="vertical" :active="0" align-center class="steps-container">
          <el-step icon="fa fa-circle">
            <div slot="title" class="record-title">
              2019/02/29 12:23:21
            </div>
            <div slot="description" class="description">
              支付大定
            </div>
          </el-step>
          <el-step icon="fa fa-circle">
            <div slot="title" class="record-title">
              2019/02/29 12:23:21
            </div>
            <div slot="description" class="description">
              拨打电话
            </div>
          </el-step>
          <el-step icon="fa fa-circle">
            <div slot="title" class="record-title">
              2019/02/29 12:23:21
            </div>
            <div slot="description" class="description">
              支付意向金
            </div>
          </el-step>
        </el-steps>
      </el-card>
    </section>
    <aside class="right-container">
      <el-card class="user-info">
        <div slot="header" class="header-text">
          用户信息
        </div>
        <img src="https://cdn-app-test.nio.com/10000/2016/10/26/2a5b0037-5678-49f8-b9a0-8d89d524f354.png" class="user-header">
        <div class="user-name">
          欧阳娜娜
        </div>
        <div class="user-detail">
          <div class="info-detail">
            <span class="label">手机号</span>
            <span class="info">17628837817</span>
          </div>
          <div class="info-detail">
            <span class="label">地址</span>
            <span class="info">上海浦东XXX</span>
          </div>
        </div>
      </el-card>
    </aside>
  </article>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import store from "@/store/manage/order-manage";
@Component({
    store
})
export default class OrderManage extends Vue {}
</script>

<style lang='less' >
.order-detail {
    display: flex;
    font-size: 14px;
    .header-text {
        font-size: 24px;
        font-weight: 500;
        text-align: left;
        color: rgba(48, 49, 51, 1);
    }
    .left-container {
        flex: 0 1 70%;
        height: 100%;
        .info-row {
            margin-top: 16px;
        }
        label {
            display: inline-block;
            color: #aaa;
            font-weight: 400;
            margin-right: 10px;
            width: 60px;
        }
        .up-records {
            margin-top: 20px;
            .header-right {
                float: right;
            }
            .el-card__header {
                line-height: 25px;
                vertical-align: center;
            }
            .steps-container {
                .el-step__icon-inner {
                    font-size: 10px;
                }
                .el-step__head.is-process {
                    color: #0d849a;
                }
                .description {
                    min-height: 70px;
                    color: #303133;
                }

                .record-title {
                    font-size: 10px;
                    color: #606266;
                }
            }
        }
    }
    .right-container {
        flex: 1;
        text-align: center;
        margin-left: 20px;
        .user-info {
            .user-detail {
                margin-top: 20px;
                .info-detail {
                    height: 50px;
                    line-height: 50px;
                    vertical-align: middle;
                    border-bottom: 1px solid #dcdfe6;
                    .label {
                        float: left;
                    }
                }
            }
            hr {
                color: #dcdfe6;
            }
            .user-header {
                width: 120px;
                height: 120px;
                border-radius: 50%;
            }
            .user-name {
                color: #303133;
                font-size: 20px;
                font-weight: 500;
                margin-top: 10px;
            }
            .label {
                font-size: 14px;
                color: rgba(48, 49, 51, 1);
            }
            .info {
                font-size: 14px;
                color: rgba(48, 49, 51, 1);
                float: right;
            }
        }
    }
}
</style>
