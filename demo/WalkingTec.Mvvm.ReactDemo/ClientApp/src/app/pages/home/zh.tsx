import { Card, Col, Row, Divider } from 'antd';
import * as React from 'react';
export default class IApp extends React.Component<any, any> {
    public render() {
        return (
            <div>
                <Row gutter={16} >
                    <Col span={12}>
                        <Card title="WTM—做最好的.netcore开源框架" style={{ minHeight: 250 }} >
                            <p>WalkingTec.Mvvm框架（简称WTM）最早开发与2013年，基于Asp.net MVC3 和 最早的Entity Framework, 当初主要是为了解决公司内部开发效率低，代码风格不统一的问题。经历了四年间数十个项目的考验，框架逐步的完善，推出了四个主要版本。</p>
                            <p>2017年9月，我们将代码移植到了.Net Core上，并进行了深度优化和重构，推出了基于Asp.net Core和EF Core的全新框架，新框架在架构，稳定性，速度上都有长足进步，真正成为一款高效开发的利器。</p>
                            <p>框架已开源，欢迎大家提出宝贵意见 <a href="https://github.com/dotnetcore/WTM" target="_blank" >https://github.com/dotnetcore/WTM</a></p>
              </Card>
                    </Col>
                    <Col span={12}>
                        <Card title="WTM解决的问题" style={{ minHeight: 250 }} >
                            <p>WTM框架把常规编码结构化，重复编码自动化，极大地提高了开发效率</p>
                            <p>在不分离的模式下，它连通了前台UI和后台代码。你不需要前后台分离，不需要两个人配合，减少成本，缩短工期。</p>
                            <p>在前后端分离的模式下，同样可以使用代码生成器同时生成前台和后台的代码，极大的降低了前后端人员的沟通成本，从本质上提升了开发效率，让“分离”不再复杂和昂贵。</p>
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
                            <p>  支持sqlserver，mysql，pgsql，sqlite等多种数据库</p>
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
                                —— 框架开源地址：<a href="https://github.com/dotnetcore/WTM" target="_blank" >https://github.com/dotnetcore/WTM</a>

                            </p>
                            <p>
                                —— 框架在线文档：<a href="https://wtmdoc.walkingtec.cn" target="_blank" >https://wtmdoc.walkingtec.cn</a>

                            </p>
                            <p>
                                —— 框架更新日志：<a href="https://github.com/dotnetcore/WTM/blob/develop/CHANGELOG.md" target="_blank">查看更新日志</a>

                            </p>
                            <p>
                                —— 框架QQ交流群：694148336

</p>
                        </Card>
                    </Col>

                </Row>
            </div>
        );
    }
}
