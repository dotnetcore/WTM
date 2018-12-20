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
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spread = (this && this.__spread) || function () {
    for (var ar = [], i = 0; i < arguments.length; i++) ar = ar.concat(__read(arguments[i]));
    return ar;
};
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-10 05:07:36
 * @modify date 2018-09-10 05:07:36
 * @desc [description]
*/
import { Alert, Button, Drawer, Icon, message, Tabs, Tree } from 'antd';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
var TabPane = Tabs.TabPane;
var TreeNode = Tree.TreeNode;
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.onDragEnter = function (info) {
        };
        _this.onDrop = function (info) {
            var dropKey = info.node.props.eventKey;
            var dragKey = info.dragNode.props.eventKey;
            var dropPos = info.node.props.pos.split('-');
            if (dropPos.length > 2) {
                return message.warning('只支持二级菜单');
            }
            if (dragKey == "system" || dropKey == "system") {
                return message.warning('系统设置不可更改');
            }
            var dropPosition = info.dropPosition - Number(dropPos[dropPos.length - 1]);
            // const dragNodesKeys = info.dragNodesKeys;
            var loop = function (data, key, callback) {
                data.forEach(function (item, index, arr) {
                    if (item.Key === key) {
                        return callback(item, index, arr);
                    }
                    if (item.Children) {
                        return loop(item.Children, key, callback);
                    }
                });
            };
            var data = __spread(toJS(Store.Meun.subMenu));
            var dragObj;
            loop(data, dragKey, function (item, index, arr) {
                arr.splice(index, 1);
                dragObj = item;
            });
            // debugger
            if (info.dropToGap) {
                var ar_1;
                var i_1;
                loop(data, dropKey, function (item, index, arr) {
                    ar_1 = arr;
                    i_1 = index;
                });
                if (dropPosition === -1) {
                    ar_1.splice(i_1, 0, dragObj);
                }
                else {
                    ar_1.splice(i_1 + 1, 0, dragObj);
                }
            }
            else {
                loop(data, dropKey, function (item) {
                    item.Children = item.Children || [];
                    // where to insert 示例添加到尾部，可以是随意位置
                    item.Children.push(dragObj);
                });
            }
            Store.Meun.setSubMenu(data);
        };
        return _this;
    }
    IApp.prototype.onSelect = function (selectedKeys, e) {
        console.log(selectedKeys);
    };
    IApp.prototype.render = function () {
        var TreeNodeConfig = {
            disableCheckbox: true,
        };
        return (React.createElement("div", null,
            React.createElement(Alert, { message: "\u9664\u4E86Home \u548C system \u4E4B\u5916 \u4EC5\u7528\u4E8E \u5F00\u53D1\u914D\u7F6E ", type: "info", showIcon: true }),
            React.createElement(Tree, { showLine: true, showIcon: true, draggable: true, defaultExpandedKeys: ['0-0-0'], onSelect: this.onSelect.bind(this), onDragEnter: this.onDragEnter, onDrop: this.onDrop }, Store.Meun.subMenu.map(function (x, i) { return React.createElement(TreeNode, __assign({}, TreeNodeConfig, { title: x.Name, key: x.Key, icon: React.createElement(Icon, { type: x.Icon }) }), x.Children && x.Children.map(function (y, yi) { return React.createElement(TreeNode, __assign({}, TreeNodeConfig, { title: y.Name, key: y.Key, icon: React.createElement(Icon, { type: y.Icon }) })); })); })),
            React.createElement(DrawerComponent, null)));
    };
    IApp = __decorate([
        observer
    ], IApp);
    return IApp;
}(React.Component));
export default IApp;
var DrawerComponent = /** @class */ (function (_super) {
    __extends(DrawerComponent, _super);
    function DrawerComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = { visible: false, childrenDrawer: false };
        _this.showDrawer = function () {
            _this.setState({
                visible: true,
            });
        };
        _this.onClose = function () {
            _this.setState({
                visible: false,
            });
        };
        _this.showChildrenDrawer = function () {
            _this.setState({
                childrenDrawer: true,
            });
        };
        _this.onChildrenDrawerClose = function () {
            _this.setState({
                childrenDrawer: false,
            });
        };
        return _this;
    }
    DrawerComponent.prototype.render = function () {
        return (React.createElement("div", null,
            React.createElement(Button, { type: "primary", onClick: this.showDrawer }, "\u4FDD\u5B58\u83DC\u5355"),
            React.createElement(Drawer, { title: "Multi-level drawer", width: 520, closable: false, onClose: this.onClose, visible: this.state.visible },
                React.createElement(Button, { type: "primary", onClick: this.showChildrenDrawer }, "Two-level drawer"),
                React.createElement(Drawer, { title: "Two-level Drawer", width: 320, closable: false, onClose: this.onChildrenDrawerClose, visible: this.state.childrenDrawer }, "This is two-level drawer"),
                React.createElement("div", { style: {
                        position: 'absolute',
                        bottom: 0,
                        width: '100%',
                        borderTop: '1px solid #e8e8e8',
                        padding: '10px 16px',
                        textAlign: 'right',
                        left: 0,
                        background: '#fff',
                        borderRadius: '0 0 4px 4px',
                    } },
                    React.createElement(Button, { style: {
                            marginRight: 8,
                        }, onClick: this.onClose }, "Cancel"),
                    React.createElement(Button, { onClick: this.onClose, type: "primary" }, "Submit")))));
    };
    return DrawerComponent;
}(React.Component));
//# sourceMappingURL=subMenu.js.map