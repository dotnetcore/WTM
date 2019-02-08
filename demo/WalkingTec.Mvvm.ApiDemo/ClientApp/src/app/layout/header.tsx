
import NoticeIcon from 'ant-design-pro/lib/NoticeIcon';
import { Avatar, Col, Dropdown, Icon, Layout, Menu, Row, Tag } from 'antd';
import groupBy from 'lodash/groupBy';
import moment from 'moment';
import * as React from 'react';
import Store from 'store/index';
import GlobalConfig from 'global.config';
const { Header } = Layout;
export default class App extends React.Component<any, any> {
    shouldComponentUpdate() {
        return false;
    }
    render() {
        return (
            <Header className="app-layout-header">
                {/* <Menu
                    mode="horizontal"
                    defaultSelectedKeys={['2']}
                    style={{ lineHeight: '64px' }}
                >
                    <Menu.Item key="1">nav 1</Menu.Item>
                    <Menu.Item key="2">nav 2</Menu.Item>
                    <Menu.Item key="3">nav 3</Menu.Item>
                </Menu> */}
                <Row>
                    <Col span={4}><Icon onClick={() => { Store.Meun.toggleCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" /></Col>
                    <Col span={20} style={{ textAlign: "right" }}>
                        {/* <NoticeIconComponent /> */}
                        <UserMenu {...this.props} />
                    </Col>
                </Row>
            </Header>
        );
    }
}
class UserMenu extends React.Component<any, any> {
    render() {
        return (
            <Dropdown overlay={
                <Menu>
                    <Menu.Item>
                        <a onClick={() => {
                            this.props.history.push("/user")
                            console.log(this.props)
                        }}>  <Icon type={'appstore'} />个人中心</a>
                    </Menu.Item>
                    <Menu.Item>
                        <a onClick={() => { Store.User.outLogin() }}>  <Icon type={'appstore'} />设置</a>
                    </Menu.Item>
                    <Menu.Item>
                        <a onClick={() => { Store.User.outLogin() }}>  <Icon type={'appstore'} />退出</a>
                    </Menu.Item>
                    {/* <Menu.Item>
                    <a target="_blank" rel="noopener noreferrer" href="http://www.taobao.com/">2nd menu item</a>
                </Menu.Item>
                <Menu.Item>
                    <a target="_blank" rel="noopener noreferrer" href="http://www.tmall.com/">3rd menu item</a>
                </Menu.Item> */}
                </Menu>
            } placement="bottomCenter">
                <div className="app-user-menu" >
                    <div>
                        <Avatar size="large" icon="user" src={GlobalConfig.default.avatar} />
                        &nbsp;<span>UserName</span>
                    </div>
                </div>
            </Dropdown>
        );
    }
}

// class NoticeIconComponent extends React.Component<any, any> {
//     render() {
//         const data = [{
//             id: '000000001',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/ThXAXghbEsBCCSDihZxY.png',
//             title: '你收到了 14 份新周报',
//             datetime: '2017-08-09',
//             type: '通知',
//         }, {
//             id: '000000002',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/OKJXDXrmkNshAMvwtvhu.png',
//             title: '你推荐的 曲妮妮 已通过第三轮面试',
//             datetime: '2017-08-08',
//             type: '通知',
//         }, {
//             id: '000000003',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/kISTdvpyTAhtGxpovNWd.png',
//             title: '这种模板可以区分多种通知类型',
//             datetime: '2017-08-07',
//             read: true,
//             type: '通知',
//         }, {
//             id: '000000004',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/GvqBnKhFgObvnSGkDsje.png',
//             title: '左侧图标用于区分不同的类型',
//             datetime: '2017-08-07',
//             type: '通知',
//         }, {
//             id: '000000005',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/ThXAXghbEsBCCSDihZxY.png',
//             title: '内容不要超过两行字，超出时自动截断',
//             datetime: '2017-08-07',
//             type: '通知',
//         }, {
//             id: '000000006',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
//             title: '曲丽丽 评论了你',
//             description: '描述信息描述信息描述信息',
//             datetime: '2017-08-07',
//             type: '消息',
//         }, {
//             id: '000000007',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
//             title: '朱偏右 回复了你',
//             description: '这种模板用于提醒谁与你发生了互动，左侧放『谁』的头像',
//             datetime: '2017-08-07',
//             type: '消息',
//         }, {
//             id: '000000008',
//             avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
//             title: '标题',
//             description: '这种模板用于提醒谁与你发生了互动，左侧放『谁』的头像',
//             datetime: '2017-08-07',
//             type: '消息',
//         }, {
//             id: '000000009',
//             title: '任务名称',
//             description: '任务需要在 2017-01-12 20:00 前启动',
//             extra: '未开始',
//             status: 'todo',
//             type: '待办',
//         }, {
//             id: '000000010',
//             title: '第三方紧急代码变更',
//             description: '冠霖提交于 2017-01-06，需在 2017-01-07 前完成代码变更任务',
//             extra: '马上到期',
//             status: 'urgent',
//             type: '待办',
//         }, {
//             id: '000000011',
//             title: '信息安全考试',
//             description: '指派竹尔于 2017-01-09 前完成更新并发布',
//             extra: '已耗时 8 天',
//             status: 'doing',
//             type: '待办',
//         }, {
//             id: '000000012',
//             title: 'ABCD 版本发布',
//             description: '冠霖提交于 2017-01-06，需在 2017-01-07 前完成代码变更任务',
//             extra: '进行中',
//             status: 'processing',
//             type: '待办',
//         }];

//         function onItemClick(item, tabProps) {
//             console.log(item, tabProps);
//         }

//         function onClear(tabTitle) {
//             console.log(tabTitle);
//         }

//         function getNoticeData(notices) {
//             if (notices.length === 0) {
//                 return {};
//             }
//             const newNotices = notices.map((notice) => {
//                 const newNotice = { ...notice };
//                 if (newNotice.datetime) {
//                     newNotice.datetime = moment(notice.datetime).fromNow();
//                 }
//                 // transform id to item key
//                 if (newNotice.id) {
//                     newNotice.key = newNotice.id;
//                 }
//                 if (newNotice.extra && newNotice.status) {
//                     const color = ({
//                         todo: '',
//                         processing: 'blue',
//                         urgent: 'red',
//                         doing: 'gold',
//                     })[newNotice.status];
//                     newNotice.extra = <Tag color={color} style={{ marginRight: 0 }}>{newNotice.extra}</Tag>;
//                 }
//                 return newNotice;
//             });
//             return groupBy(newNotices, 'type');
//         }

//         const noticeData = getNoticeData(data);
//         return (
//             <NoticeIcon
//                 className="app-layout-notice-icon"
//                 count={5}
//                 onItemClick={onItemClick}
//                 onClear={onClear}
//                 popupAlign={{ offset: [20, -16] }}
//             >
//                 <NoticeIcon.Tab
//                     list={noticeData['通知']}
//                     title="通知"
//                     emptyText="你已查看所有通知"
//                     emptyImage="https://gw.alipayobjects.com/zos/rmsportal/wAhyIChODzsoKIOBHcBk.svg"
//                 />
//                 <NoticeIcon.Tab
//                     list={noticeData['消息']}
//                     title="消息"
//                     emptyText="您已读完所有消息"
//                     emptyImage="https://gw.alipayobjects.com/zos/rmsportal/sAuJeJzSKbUmHfBQRzmZ.svg"
//                 />
//                 <NoticeIcon.Tab
//                     list={noticeData['待办']}
//                     title="待办"
//                     emptyText="你已完成所有待办"
//                     emptyImage="https://gw.alipayobjects.com/zos/rmsportal/HsIsxMZiWKrNUavQUXqx.svg"
//                 />
//             </NoticeIcon>
//         );
//     }
// }



