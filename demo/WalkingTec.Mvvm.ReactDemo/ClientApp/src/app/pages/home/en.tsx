import { Card, Col, Row, Divider } from 'antd';
import * as React from 'react';
export default class IApp extends React.Component<any, any> {
    public render() {
        return (
            <div>
                <Row gutter={16} >
                    <Col span={12}>
                        <Card title="WTM -- make the best. NETCORE open source framework" style={{ minHeight: 250 }} >
                        Walkingtec.mvvm framework (WTM for short) was first developed in 2013. Based on asp.net mvc3 and the earliest Entity Framework, it was mainly used to solve the problems of low development efficiency and inconsistent code style in the company. After four years and dozens of projects, the framework has been gradually improved and four major versions have been launched.
              </Card>
                    </Col>
                    <Col span={12}>
                        <Card title="Problems solved by WTM" style={{ minHeight: 250 }} >
                        WTM framework constructs conventional coding and automates repetitive coding, which greatly improves development efficiency
In a non detached mode, it connects the foreground UI with the background code. You don't need to separate the front and back platform. You don't need two people to cooperate. You can reduce the cost and shorten the construction period.
In the mode of front-end and back-end separation, code generators can also be used to generate front-end and back-end code at the same time, greatly reducing the communication cost of front-end and back-end personnel, essentially improving the development efficiency, so that "separation" is no longer complex and expensive.
              </Card>
                    </Col>

                </Row>
                <Divider />
                <Row gutter={16} >
                    <Col span={12}>
                        <Card title="框架特点" style={{ minHeight: 500 }} >
                            <p>  一键生成WTM项目</p>
                            <p>  一键生成增删改查，导入导出，批量操作代码</p>
                            <p> 支持一对多，多对多关联模型的识别和代码生成</p>
                            <p>支持分离(React+AntD,Vue+Element)和不分离(LayUI)两种模式</p>
                            <p>  支持sqlserver，mysql，pgsql三种数据库</p>
                            <p>   封装了Layui，AntD，Element的大部分控件，编写前台更加简便</p>
                            <p>   提供了很多基类，封装了绝大部分后台常用操作</p>
                            <p>     提供了用户，角色，用户组，菜单，日志等常用模块</p>
                            <p>      支持数据权限的开发和配置</p>
                            <p>    支持读写分离和数据库分库</p>
                        </Card>
                    </Col>
                    <Col span={12}>
                        <Card title="作者的话" style={{ minHeight: 500 }}>
                            <p>
                                WTM框架，全称WalkingTec MVVM（不是“我特么”的拼音首字母），WTM是一个快速开发框架，有多快？至少目前dotnetcore的开源项目中，我还没有见到更接地气，开发速度更快的框架。WTM的设计理念就是最大程度的加快开发速度，降低开发成本。

</p>
                            <p>
                                国内Java一家独大原因很多，有BAT的示范效应，也有微软自己战略的失误。好在微软这两年终于想明白了， dotnet core的横空出世和收购github都是非常正确的方向。当然要想达到java一样的生态还有很长的路要走，那我就贡献一点绵薄之力吧。

</p>
                            <p>
                                WTM开源以来，受到了越来越多开发者的喜爱，WTM必将以更加成熟稳定的姿态回报各位的喜爱。特别鸣谢贤心（layui.com），授权WTM开发的项目可以免费使用其收费版的LayuiAdmin。提高自己，造福他人，吾道不孤！

</p>
                            <p>
                                —— 框架开源地址：<a href="https://github.com/dotnetcore/WTM" target="_block" >https://github.com/dotnetcore/WTM</a>

                            </p>
                            <p>
                                —— 框架在线文档：<a href="https://wtmdoc.walkingtec.cn" target="_block" >https://wtmdoc.walkingtec.cn</a>

                            </p>
                            <p>
                                —— 框架更新日志：<a href="https://github.com/dotnetcore/WTM/blob/develop/CHANGELOG.md" target="_block">查看更新日志</a>

                            </p>
                            <p>
                                —— 框架QQ交流群：694148336

</p>
                        </Card>
                    </Col>

                </Row>
                <Divider />
                <Row>
                    <Col span={24}>
                        <Card title="连接你我" >
                            <div style={{ textAlign: "center" }}>
                                <img src="https://wtmdoc.walkingtec.cn/imgs/WTM-Ali.png" width="300" height="300" />
                                <img src="https://wtmdoc.walkingtec.cn/imgs/gongzhonghao.jpg" width="300" height="300" />
                            </div>

                        </Card>
                    </Col>
                </Row>
            </div>
        );
    }
}
