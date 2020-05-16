import { Card, Col, Row, Divider } from 'antd';
import * as React from 'react';
export default class IApp extends React.Component<any, any> {
    public render() {
        return (
            <div>
                <Row gutter={16} >
                    <Col span={12}>
                        <Card title="WTM -- make the best. NETCORE open source framework" style={{ minHeight: 250 }} >
                            <p>
                                Walkingtec.mvvm framework (WTM for short) was first developed in 2013 ,based on asp.net mvc3 and the earliest Entity Framework. It was mainly used to solve the problems of low coding efficiency and inconsistent code style. Based on dozens of projects,the framework has been gradually improved and four major versions have been launched during 4 years.
                            </p>
                            <p>
                                In September 2017, we transplanted the code to. Net core and carried out deep optimization and reconstruction. We launched a new framework based on asp.net core and EF core. The new framework has made great progress in architecture, stability and speed. It really become a powerful tool for efficient development.
                            </p>
                            <p>
                                We are happy to provide the open-source framework now. Please feel free to send us a note. let us know how we are doing and what you need.Thanks! <a href="https://github.com/dotnetcore/WTM" target="_blank" >https://github.com/dotnetcore/WTM</a>
                            </p>
              </Card>
                    </Col>
                    <Col span={12}>
                        <Card title="Problems solved by WTM" style={{ minHeight: 250 }} >
                            <p>WTM framework greatly improves development efficiency by structures conventional coding and automates repetitive coding.</p>
                            <p>In non-separation mode, it connects front-end UI with back-end code. You don't need to separate the front and back platform. You don't need two people to cooperate. WTM helps you to reduce the cost and shorten the R&D period.</p>
                            <p>In the mode of front-end and back-end separation, both front-end and back-end code can also be generated at the same time by the code generators. It greatly reduce the communication cost of front-end and back-end personnel. In this way , ‘separation’ is no longer complex and expensive.</p>
              </Card>
                    </Col>

                </Row>
                <Divider />
                <Row gutter={16} >
                    <Col span={12}>
                        <Card title="Framework features" style={{ minHeight: 500 }} >
                            <p>  One click to generate WTM project</p>
                            <p>  One click to generate add, delete, modify and query</p>
                            <p>  Support identification and code generation of one-to-many and many-to-many association models</p>
                            <p>  support front-back end separation mode (React+AntD,Vue+Element) and non-separation mode(LayUI)</p>
                            <p>  Support many databases, sqlserver, MySQL ,PgSQL , Sqlite..etc</p>
                            <p>  Encapsulates most of the controls of layUI, antd and element</p>
                            <p>  A lot of base classes are provided ,encapsulate most common background operations</p>
                            <p>  Provide common modules such as users, roles, user groups, menus, logs, etc</p>
                            <p>  Support the development and configuration of data authority</p>
                            <p>  Supportread/ write splitting ;Support database sub Library</p>
                        </Card>
                    </Col>
                    <Col span={12}>
                        <Card title="Author's words" style={{ minHeight: 500 }}>
                            <p>
                                WTM framework, full name walkingtec MVVM. Walkingtec is my own company. WTM is a rapid development framework.How fast ? At least at present, in the open source project of DotNetcore, I haven't seen another faster one. Our goal of WTM is to speed up the development and reduce cost to the greatest extent.
</p>
                            <p>
                                WTM has been loved by more and more developers since its open source. WTM will surely repay your love with a more mature and stable version.Specially thanks to（layui.com Improve yourself, benefit others, we am not alone!</p>
                            <p>
                                —— Open source address of framework:<a href="https://github.com/dotnetcore/WTM" target="_blank" >https://github.com/dotnetcore/WTM</a>

                            </p>
                            <p>
                                —— Framework online document:<a href="https://wtmdoc.walkingtec.cn" target="_blank" >https://wtmdoc.walkingtec.cn</a>

                            </p>
                            <p>
                                —— Framework update log:<a href="https://github.com/dotnetcore/WTM/blob/develop/CHANGELOG.md" target="_blank">view changelog</a>

                            </p>
                            <p>
                                —— Frame QQ communication group:694148336

</p>
                        </Card>
                    </Col>

                </Row>
            </div>
        );
    }
}
