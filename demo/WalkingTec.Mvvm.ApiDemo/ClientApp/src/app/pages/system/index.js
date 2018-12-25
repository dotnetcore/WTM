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
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-10 05:07:39
 * @modify date 2018-09-10 05:07:39
 * @desc [description]
*/
import { Tabs } from 'antd';
import * as React from 'react';
import SubMenu from './subMenu';
import Authorize from './authorize';
var TabPane = Tabs.TabPane;
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    IApp.prototype.render = function () {
        var TreeNodeConfig = {
            disableCheckbox: true,
            selectable: false
        };
        return (React.createElement(Tabs, { defaultActiveKey: "subMenu", tabPosition: "left" },
            React.createElement(TabPane, { tab: "\u83DC\u5355\u8BBE\u7F6E", key: "subMenu" },
                React.createElement(SubMenu, null)),
            React.createElement(TabPane, { tab: "\u6743\u9650\u8BBE\u7F6E", key: "authorize" },
                React.createElement(Authorize, null))));
    };
    return IApp;
}(React.Component));
export default IApp;
//# sourceMappingURL=index.js.map