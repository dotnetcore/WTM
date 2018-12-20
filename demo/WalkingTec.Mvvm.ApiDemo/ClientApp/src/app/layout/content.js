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
import { Layout } from 'antd';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { renderRoutes } from 'react-router-config';
var Header = Layout.Header, Content = Layout.Content, Sider = Layout.Sider;
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    App.prototype.setHeight = function () {
        if (this.body) {
            var content = this.body.querySelector(".app-animate-content");
            if (content) {
                content.style.minHeight = (this.body.offsetHeight - 40) + "px";
            }
        }
    };
    App.prototype.componentDidMount = function () {
        this.setHeight();
    };
    App.prototype.componentDidUpdate = function () {
        this.setHeight();
    };
    App.prototype.render = function () {
        var _this = this;
        return (React.createElement(Layout, { className: "app-layout-body", ref: function (e) { return _this.body = ReactDOM.findDOMNode(e); } },
            React.createElement(Content, { className: "app-layout-content" }, renderRoutes(this.props.route.routes))));
    };
    return App;
}(React.Component));
export default App;
//# sourceMappingURL=content.js.map