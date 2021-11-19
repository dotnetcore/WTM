<template>
    <div v-if="locale === 'zh'">
        <a-row :gutter="20">
            <a-col :md="12">
                <a-card title="WTM—做最好的.netcore开源框架" >
                    <p>WalkingTec.Mvvm框架（简称WTM）最早开发与2013年，基于Asp.net MVC3 和 最早的Entity Framework, 当初主要是为了解决公司内部开发效率低，代码风格不统一的问题。经历了四年间数十个项目的考验，框架逐步的完善，推出了四个主要版本。</p>
                    <p>2017年9月，我们将代码移植到了.Net Core上，并进行了深度优化和重构，推出了基于Asp.net Core和EF Core的全新框架，新框架在架构，稳定性，速度上都有长足进步，真正成为一款高效开发的利器。</p>
                    <p>
                        框架已开源，欢迎大家提出宝贵意见
                        <a
                            href="https://github.com/dotnetcore/WTM"
                            target="_blank"
                        >https://github.com/dotnetcore/WTM</a>
                    </p>
                </a-card>
            </a-col>
            <a-col :md="12">
                <a-card title="WTM解决的问题" >
                    <p>WTM框架把常规编码结构化，重复编码自动化，极大地提高了开发效率</p>
                    <p>在不分离的模式下，它连通了前台UI和后台代码。你不需要前后台分离，不需要两个人配合，减少成本，缩短工期。</p>
                    <p>在前后端分离的模式下，同样可以使用代码生成器同时生成前台和后台的代码，极大的降低了前后端人员的沟通成本，从本质上提升了开发效率，让“分离”不再复杂和昂贵。</p>
                </a-card>
            </a-col>
        </a-row>
        <a-divider></a-divider>
        <a-row :gutter="20">
            <a-col :md="12">
                <a-card title="框架特点" >
                    <p>一键生成WTM项目</p>
                    <p>一键生成增删改查，导入导出，批量操作代码</p>
                    <p>支持一对多，多对多关联模型的识别和代码生成</p>
                    <p>支持React,Vue,LayUI,Blazor等多种模式</p>
                    <p>支持sqlserver，mysql，pgsql，sqlite等多种数据库</p>
                    <p>封装了Layui，AntD，Element的大部分控件，编写前台更加简便</p>
                    <p>提供了很多基类，封装了绝大部分后台常用操作</p>
                    <p>提供了用户，角色，用户组，菜单，日志等常用模块</p>
                    <p>支持数据权限的开发和配置</p>
                    <p>支持读写分离和数据库分库</p>
                </a-card>
            </a-col>
            <a-col :md="12">
                <a-card title="作者的话" >
                    <p>WTM框架，全称WalkingTec MVVM（不是“我特么”的拼音首字母），WTM是一个快速开发框架，有多快？至少目前dotnetcore的开源项目中，我还没有见到更接地气，开发速度更快的框架。WTM的设计理念就是最大程度的加快开发速度，降低开发成本。</p>
                    <p>国内Java一家独大原因很多，有BAT的示范效应，也有微软自己战略的失误。好在微软这两年终于想明白了， dotnet core的横空出世和收购github都是非常正确的方向。当然要想达到java一样的生态还有很长的路要走，那我就贡献一点绵薄之力吧。</p>
                    <p>WTM开源以来，受到了越来越多开发者的喜爱，WTM必将以更加成熟稳定的姿态回报各位的喜爱。提高自己，造福他人，吾道不孤！</p>
                    <p>
                        —— 框架开源地址：
                        <a
                            href="https://github.com/dotnetcore/WTM"
                            target="_blank"
                        >https://github.com/dotnetcore/WTM</a>
                    </p>
                    <p>
                        —— 框架在线文档：
                        <a
                            href="https://wtmdoc.walkingtec.cn"
                            target="_blank"
                        >https://wtmdoc.walkingtec.cn</a>
                    </p>
                    <p>
                        —— 框架更新日志：
                        <a
                            href="https://github.com/dotnetcore/WTM/blob/develop/CHANGELOG.md"
                            target="_blank"
                        >查看更新日志</a>
                    </p>
                    <p>—— 框架QQ交流群：694148336</p>
                </a-card>
            </a-col>
        </a-row>
    </div>
    <div v-else>
        <a-row :gutter="20">
            <a-col :md="12">
                <a-card
                    title="WTM -- make the best. NETCORE open source framework"
                    
                >
                    <p>Walkingtec.mvvm framework (WTM for short) was first developed in 2013 ,based on asp.net mvc3 and the earliest Entity Framework. It was mainly used to solve the problems of low coding efficiency and inconsistent code style. Based on dozens of projects,the framework has been gradually improved and four major versions have been launched during 4 years.</p>
                    <p>In September 2017, we transplanted the code to. Net core and carried out deep optimization and reconstruction. We launched a new framework based on asp.net core and EF core. The new framework has made great progress in architecture, stability and speed. It really become a powerful tool for efficient development.</p>
                    <p>
                        We are happy to provide the open-source framework now. Please feel free to send us a note. let us know how we are doing and what you need.Thanks!
                        <a
                            href="https://github.com/dotnetcore/WTM"
                            target="_blank"
                        >https://github.com/dotnetcore/WTM</a>
                    </p>
                </a-card>
            </a-col>
            <a-col :md="12">
                <a-card title="Problems solved by WTM" >
                    <p>WTM framework greatly improves development efficiency by structures conventional coding and automates repetitive coding.</p>
                    <p>In non-separation mode, it connects front-end UI with back-end code. You don't need to separate the front and back platform. You don't need two people to cooperate. WTM helps you to reduce the cost and shorten the R&D period.</p>
                    <p>In the mode of front-end and back-end separation, both front-end and back-end code can also be generated at the same time by the code generators. It greatly reduce the communication cost of front-end and back-end personnel. In this way , ‘separation’ is no longer complex and expensive.</p>
                </a-card>
            </a-col>
        </a-row>
        <a-divider></a-divider>
        <a-row :gutter="20">
            <a-col :md="12">
                <a-card title="Framework features" >
                    <p>One click to generate WTM project</p>
                    <p>One click to generate add, delete, modify and query</p>
                    <p>Support identification and code generation of one-to-many and many-to-many association models</p>
                    <p>support front-back end separation mode (React+AntD,Vue+Element) and non-separation mode(LayUI)</p>
                    <p>Support many databases, sqlserver, MySQL ,PgSQL , Sqlite..etc</p>
                    <p>Encapsulates most of the controls of layUI, antd and element</p>
                    <p>A lot of base classes are provided ,encapsulate most common background operations</p>
                    <p>Provide common modules such as users, roles, user groups, menus, logs, etc</p>
                    <p>Support the development and configuration of data authority</p>
                    <p>Supportread/ write splitting ;Support database sub Library</p>
                </a-card>
            </a-col>
            <a-col :md="12">
                <a-card title="Author's words" >
                    <p>WTM framework, full name walkingtec MVVM. Walkingtec is my own company. WTM is a rapid development framework.How fast ? At least at present, in the open source project of DotNetcore, I haven't seen another faster one. Our goal of WTM is to speed up the development and reduce cost to the greatest extent.</p>
                    <p>WTM has been loved by more and more developers since its open source. WTM will surely repay your love with a more mature and stable version.Specially thanks to（layui.com Improve yourself, benefit others, we am not alone!</p>
                    <p>
                        —— Open source address of framework:
                        <a
                            href="https://github.com/dotnetcore/WTM"
                            target="_blank"
                        >https://github.com/dotnetcore/WTM</a>
                    </p>
                    <p>
                        —— Framework online document:
                        <a
                            href="https://wtmdoc.walkingtec.cn"
                            target="_blank"
                        >https://wtmdoc.walkingtec.cn</a>
                    </p>
                    <p>
                        —— Framework update log:
                        <a
                            href="https://github.com/dotnetcore/WTM/blob/develop/CHANGELOG.md"
                            target="_blank"
                        >view changelog</a>
                    </p>
                    <p>—— Frame QQ communication group:694148336</p>
                </a-card>
            </a-col>
        </a-row>
    </div>
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
    get locale() {
        return this.$i18n.locale
    }
    created() {

    }
}
</script>
<style lang="less" scoped>
</style>
