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
import { Layout } from 'antd';
import * as React from 'react';
import Animate from 'rc-animate';
import ContentComponent from './content';
import HeaderComponent from './header';
import SiderComponent from './sider';
import './style.less';
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    App.prototype.render = function () {
        return (React.createElement(Animate, { transitionName: 'fade', transitionAppear: true, component: "" },
            React.createElement(Layout, { className: "app-layout-root" },
                React.createElement(SiderComponent, __assign({}, this.props)),
                React.createElement(Layout, { style: { overflow: "hidden" } },
                    React.createElement(HeaderComponent, __assign({}, this.props)),
                    React.createElement(ContentComponent, __assign({}, this.props))))));
    };
    return App;
}(React.Component));
export default App;
//# sourceMappingURL=index.js.map