var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
import { Layout, Menu, Avatar, Row, Col, Dropdown, Icon } from 'antd';
import NoticeIcon from 'ant-design-pro/lib/NoticeIcon';
import * as React from 'react';
import Store from 'store/index';
var Header = Layout.Header;
import moment from 'moment';
import groupBy from 'lodash/groupBy';
import { Tag } from 'antd';
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    App.prototype.shouldComponentUpdate = function () {
        return false;
    };
    App.prototype.render = function () {
        return (React.createElement(Header, { className: "app-layout-header" },
            React.createElement(Row, null,
                React.createElement(Col, { span: 4 },
                    React.createElement(Icon, { onClick: function () { Store.Meun.toggleCollapsed(); }, className: "app-collapsed-trigger", type: "menu-fold", theme: "outlined" })),
                React.createElement(Col, { span: 20, style: { textAlign: "right", padding: "0 20px" } },
                    React.createElement(NoticeIconComponent, null),
                    React.createElement(UserMenu, __assign({}, this.props))))));
    };
    return App;
}(React.Component));
export default App;
var UserMenu = /** @class */ (function (_super) {
    __extends(UserMenu, _super);
    function UserMenu() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    UserMenu.prototype.render = function () {
        var _this = this;
        return (React.createElement(Dropdown, { overlay: React.createElement(Menu, null,
                React.createElement(Menu.Item, null,
                    React.createElement("a", { onClick: function () {
                            _this.props.history.push("/user");
                            console.log(_this.props);
                        } },
                        "  ",
                        React.createElement(Icon, { type: 'appstore' }),
                        "\u4E2A\u4EBA\u4E2D\u5FC3")),
                React.createElement(Menu.Item, null,
                    React.createElement("a", { onClick: function () { Store.User.outLogin(); } },
                        "  ",
                        React.createElement(Icon, { type: 'appstore' }),
                        "\u8BBE\u7F6E")),
                React.createElement(Menu.Item, null,
                    React.createElement("a", { onClick: function () { Store.User.outLogin(); } },
                        "  ",
                        React.createElement(Icon, { type: 'appstore' }),
                        "\u9000\u51FA"))), placement: "bottomCenter" },
            React.createElement("div", { style: { display: "inline-block" } },
                " ",
                React.createElement(Avatar, { size: "large", icon: "user", src: "https://avatars0.githubusercontent.com/u/19631404?s=460&v=4" }),
                " \u00A0",
                React.createElement("span", null, "\u51B7"))));
    };
    return UserMenu;
}(React.Component));
var NoticeIconComponent = /** @class */ (function (_super) {
    __extends(NoticeIconComponent, _super);
    function NoticeIconComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    NoticeIconComponent.prototype.render = function () {
        var data = [{
                id: '000000001',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/ThXAXghbEsBCCSDihZxY.png',
                title: '你收到了 14 份新周报',
                datetime: '2017-08-09',
                type: '通知',
            }, {
                id: '000000002',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/OKJXDXrmkNshAMvwtvhu.png',
                title: '你推荐的 曲妮妮 已通过第三轮面试',
                datetime: '2017-08-08',
                type: '通知',
            }, {
                id: '000000003',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/kISTdvpyTAhtGxpovNWd.png',
                title: '这种模板可以区分多种通知类型',
                datetime: '2017-08-07',
                read: true,
                type: '通知',
            }, {
                id: '000000004',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/GvqBnKhFgObvnSGkDsje.png',
                title: '左侧图标用于区分不同的类型',
                datetime: '2017-08-07',
                type: '通知',
            }, {
                id: '000000005',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/ThXAXghbEsBCCSDihZxY.png',
                title: '内容不要超过两行字，超出时自动截断',
                datetime: '2017-08-07',
                type: '通知',
            }, {
                id: '000000006',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
                title: '曲丽丽 评论了你',
                description: '描述信息描述信息描述信息',
                datetime: '2017-08-07',
                type: '消息',
            }, {
                id: '000000007',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
                title: '朱偏右 回复了你',
                description: '这种模板用于提醒谁与你发生了互动，左侧放『谁』的头像',
                datetime: '2017-08-07',
                type: '消息',
            }, {
                id: '000000008',
                avatar: 'https://gw.alipayobjects.com/zos/rmsportal/fcHMVNCjPOsbUGdEduuv.jpeg',
                title: '标题',
                description: '这种模板用于提醒谁与你发生了互动，左侧放『谁』的头像',
                datetime: '2017-08-07',
                type: '消息',
            }, {
                id: '000000009',
                title: '任务名称',
                description: '任务需要在 2017-01-12 20:00 前启动',
                extra: '未开始',
                status: 'todo',
                type: '待办',
            }, {
                id: '000000010',
                title: '第三方紧急代码变更',
                description: '冠霖提交于 2017-01-06，需在 2017-01-07 前完成代码变更任务',
                extra: '马上到期',
                status: 'urgent',
                type: '待办',
            }, {
                id: '000000011',
                title: '信息安全考试',
                description: '指派竹尔于 2017-01-09 前完成更新并发布',
                extra: '已耗时 8 天',
                status: 'doing',
                type: '待办',
            }, {
                id: '000000012',
                title: 'ABCD 版本发布',
                description: '冠霖提交于 2017-01-06，需在 2017-01-07 前完成代码变更任务',
                extra: '进行中',
                status: 'processing',
                type: '待办',
            }];
        function onItemClick(item, tabProps) {
            console.log(item, tabProps);
        }
        function onClear(tabTitle) {
            console.log(tabTitle);
        }
        function getNoticeData(notices) {
            if (notices.length === 0) {
                return {};
            }
            var newNotices = notices.map(function (notice) {
                var newNotice = __assign({}, notice);
                if (newNotice.datetime) {
                    newNotice.datetime = moment(notice.datetime).fromNow();
                }
                // transform id to item key
                if (newNotice.id) {
                    newNotice.key = newNotice.id;
                }
                if (newNotice.extra && newNotice.status) {
                    var color = ({
                        todo: '',
                        processing: 'blue',
                        urgent: 'red',
                        doing: 'gold',
                    })[newNotice.status];
                    newNotice.extra = React.createElement(Tag, { color: color, style: { marginRight: 0 } }, newNotice.extra);
                }
                return newNotice;
            });
            return groupBy(newNotices, 'type');
        }
        var noticeData = getNoticeData(data);
        return (React.createElement(NoticeIcon, { className: "app-layout-notice-icon", count: 5, onItemClick: onItemClick, onClear: onClear, popupAlign: { offset: [20, -16] } },
            React.createElement(NoticeIcon.Tab, { list: noticeData['通知'], title: "\u901A\u77E5", emptyText: "\u4F60\u5DF2\u67E5\u770B\u6240\u6709\u901A\u77E5", emptyImage: "https://gw.alipayobjects.com/zos/rmsportal/wAhyIChODzsoKIOBHcBk.svg" }),
            React.createElement(NoticeIcon.Tab, { list: noticeData['消息'], title: "\u6D88\u606F", emptyText: "\u60A8\u5DF2\u8BFB\u5B8C\u6240\u6709\u6D88\u606F", emptyImage: "https://gw.alipayobjects.com/zos/rmsportal/sAuJeJzSKbUmHfBQRzmZ.svg" }),
            React.createElement(NoticeIcon.Tab, { list: noticeData['待办'], title: "\u5F85\u529E", emptyText: "\u4F60\u5DF2\u5B8C\u6210\u6240\u6709\u5F85\u529E", emptyImage: "https://gw.alipayobjects.com/zos/rmsportal/HsIsxMZiWKrNUavQUXqx.svg" })));
    };
    return NoticeIconComponent;
}(React.Component));
//# sourceMappingURL=header.js.map