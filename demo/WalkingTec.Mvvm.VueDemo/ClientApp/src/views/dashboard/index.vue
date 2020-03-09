<template>
  <div class="dashboard-container">
    <el-row :gutter="10">
      <el-col :span="16" :md="16" :xs="24" :sm="24">
        <el-row :gutter="10">
          <el-col :span="12" :xs="24">
            <el-card shadow="hover">
              <div slot="header">快捷方式</div>
              <el-row class="lump-wrap" :gutter="10">
                <el-col
                  :span="6"
                  v-for="item of shortcutList"
                  :key="item.Value"
                >
                  <router-link class="link-a" :to="{ path: item.Url }">
                    <i :class="[item.Icon ? item.Icon : 'el-icon-edit']"></i
                    >{{ item.Text }}</router-link
                  >
                </el-col>
              </el-row>
            </el-card>
          </el-col>
          <el-col :span="12" :xs="24">
            <el-card shadow="hover">
              <div slot="header"><span>WTM开源</span></div>
              <el-row class="lump-wrap cxt-left" :gutter="10">
                <el-col :span="12">
                  <div>
                    <h3>Star</h3>
                    <p>{{ getGithubInfoData.stargazers_count || "" }}</p>
                  </div>
                </el-col>
                <el-col :span="12">
                  <div>
                    <h3>Fork</h3>
                    <p>{{ getGithubInfoData.forks_count || "" }}</p>
                  </div>
                </el-col>
                <el-col :span="12">
                  <div>
                    <h3>Watch</h3>
                    <p>{{ getGithubInfoData.subscribers_count || "" }}</p>
                  </div>
                </el-col>
                <el-col :span="12">
                  <div>
                    <h3>Issue</h3>
                    <p>{{ getGithubInfoData.open_issues_count || "" }}</p>
                  </div>
                </el-col>
              </el-row>
            </el-card>
          </el-col>
          <el-col :span="24" :sm="24">
            <e-charts-module />
          </el-col>
        </el-row>
      </el-col>
      <el-col :span="8" :md="8" :xs="24" :sm="24">
        <el-col :span="24">
          <el-card shadow="hover">
            <div slot="header">框架特点</div>
            <i class="el-icon-check"></i>一键生成WTM项目<br />
            <i class="el-icon-check"></i
            >一键生成增删改查，导入导出，批量操作代码<br />
            <i class="el-icon-check"></i
            >支持一对多，多对多关联模型的识别和代码生成<br />
            <i class="el-icon-check"></i
            >支持分离(React+AntD,Vue+Element)和不分离(LayUI)两种模式<br />
            <i class="el-icon-check"></i
            >支持sqlserver，mysql，pgsql三种数据库<br />
            <i class="el-icon-check"></i
            >封装了Layui，AntD，Element的大部分控件，编写前台更加简便<br />
            <i class="el-icon-check"></i
            >提供了很多基类，封装了绝大部分后台常用操作<br />
            <i class="el-icon-check"></i
            >提供了用户，角色，用户组，菜单，日志等常用模块<br />
            <i class="el-icon-check"></i>支持数据权限的开发和配置<br />
            <i class="el-icon-check"></i>支持读写分离和数据库分库
          </el-card>
        </el-col>
        <el-col :span="24">
          <el-card shadow="hover">
            <div slot="header">作者的话</div>
            <p>
              WTM框架，全称WalkingTec
              MVVM（不是“我特么”的拼音首字母），WTM是一个快速开发框架，有多快？至少目前dotnetcore的开源项目中，我还没有见到更接地气，开发速度更快的框架。WTM的设计理念就是最大程度的加快开发速度，降低开发成本。
            </p>
            <p>
              国内Java一家独大原因很多，有BAT的示范效应，也有微软自己战略的失误。好在微软这两年终于想明白了，
              dotnet
              core的横空出世和收购github都是非常正确的方向。当然要想达到java一样的生态还有很长的路要走，那我就贡献一点绵薄之力吧。
            </p>
            <p>
              WTM开源以来，受到了越来越多开发者的喜爱，WTM必将以更加成熟稳定的姿态回报各位的喜爱。特别鸣谢贤心（layui.com），授权WTM开发的项目可以免费使用其收费版的LayuiAdmin。提高自己，造福他人，吾道不孤！
            </p>
            <p>
              —— 框架开源地址：<el-link
                href="https://github.com/dotnetcore/WTM"
                target="_blank"
                type="primary"
                >https://github.com/dotnetcore/WTM</el-link
              >
            </p>
            <p>
              —— 框架在线文档：<el-link
                href="https://wtmdoc.walkingtec.cn"
                target="_blank"
                type="primary"
                >https://wtmdoc.walkingtec.cn</el-link
              >
            </p>
            <p>
              —— 框架QQ交流群：694148336
            </p>
          </el-card>
        </el-col>
        <el-col :span="24">
          <el-card shadow="hover">
            <div slot="header">即刻体验WTM之美</div>
            <h2>
              现在就点
              <a
                href="https://wtmdoc.walkingtec.cn/setup"
                target="_blank"
                style="color:#009688"
                >这里</a
              >
              立刻生成第一个WTM项目吧
            </h2>
          </el-card>
        </el-col>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="24">
        <el-calendar v-model="value" />
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { RoutesModule } from "@/store/modules/routes";
import EChartsModule from "./echarts";
import { Action, State } from "vuex-class";
import store from "@/store/system/dashboard";
@Component({
  name: "Dashboard",
  store,
  components: {
    EChartsModule
  }
})
export default class extends Vue {
  value: Date = new Date();
  shortcuts: any[] = [
    "日志管理",
    "用户组管理",
    "角色管理",
    "用户管理",
    "菜单管理",
    "数据权限"
  ];
  get shortcutList() {
    return RoutesModule.pageList.filter(item =>
      this.shortcuts.includes(item.Text)
    );
  }
  @Action
  getGithubInfo;
  @State
  getGithubInfoData;
  mounted() {
    this.getGithubInfo();
  }
}
</script>
<style lang="less" rel="stylesheet/less">
@import "~@/assets/css/mixin.less";
.dashboard-container {
  padding: 10px;
  p {
    text-indent: 2em;
  }
  .el-row {
    margin-bottom: 20px;
  }
  .el-card__header {
    color: #333;
    font-size: 14px;
  }
  .el-card {
    font-size: 14px;
    color: #666;
  }
  .lump-wrap {
    margin-bottom: 0;
    min-height: 160px;
    &.cxt-left {
      .el-col {
        .flexalign(flex-start);
        box-sizing: border-box;
        padding-bottom: 10px;
        div {
          background-color: #f8f8f8;
          width: 100%;
          box-sizing: border-box;
          padding: 0 10px;
        }
      }
    }
    .el-col {
      .center(column);
      i {
        width: 100%;
        height: 60px;
        line-height: 60px;
        text-align: center;
        border-radius: 2px;
        font-size: 30px;
        background-color: #f8f8f8;
      }
      h3 {
        padding: 5px 0;
        font-size: 12px;
      }
      p {
        font-style: normal;
        font-size: 30px;
        font-weight: 300;
        color: #009688;
        text-indent: 0;
      }
      .link-a {
        color: #666;
        font-size: 14px;
        text-align: center;
      }
    }
  }
}
</style>
