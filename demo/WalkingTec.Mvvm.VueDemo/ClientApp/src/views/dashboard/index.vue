<template>
    <div class="dashboard-container">
        <el-row :gutter="20">
            <el-col :span="8">
                <el-card shadow="hover">
                    <div slot="header">WTM—做最好的.netcore开源框架</div>
                    WalkingTec.Mvvm框架（简称WTM）最早开发与2013年，基于Asp.net MVC3 和 最早的Entity Framework,
                    当初主要是为了解决公司内部开发效率低，代码风格不统一的问题。经历了四年间数十个项目的考验，框架逐步的完善，推出了四个主要版本。
                </el-card>
            </el-col>
            <el-col :span="8">
                <el-card shadow="hover">
                    <div slot="header">WTM开源</div>
                    WTM是纯开源免费框架，如果喜欢，来GitHub赏个星~~
                    <br />
                    <iframe src="https://ghbtns.com/github-btn.html?user=dotnetcore&repo=wtm&type=star&count=true&size=large" frameborder="0" scrolling="0" width="160px" height="30px"></iframe>
                </el-card>
            </el-col>
            <el-col :span="8">
                <el-card shadow="hover">
                    <div slot="header">即刻体验WTM之美</div>
                    现在就点 这里 立刻生成第一个WTM项目吧
                </el-card>
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
import { UserModule } from "@/store/modules/user";

@Component({
    name: "Dashboard"
})
export default class extends Vue {
    value: Date = new Date();
    get info() {
        return UserModule.info;
    }

    get roles() {
        return UserModule.roles;
    }
}
</script>
<style lang="less" rel="stylesheet/less">
.dashboard-container {
    padding: 10px;
    .el-row {
        margin-bottom: 20px;
    }
    .el-card__header {
        color: #333;
        font-size: 14px;
    }
    .el-card {
        font-size: 16px;
        color: #666;
    }
}
</style>
