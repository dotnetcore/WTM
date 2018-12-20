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
import Login from 'ant-design-pro/lib/Login';
import { Alert, Checkbox } from 'antd';
import Animate from 'rc-animate';
import * as React from 'react';
import store from 'store/index';
import './style.less';
var Tab = Login.Tab, UserName = Login.UserName, Password = Login.Password, Mobile = Login.Mobile, Captcha = Login.Captcha, Submit = Login.Submit;
var LoginDemo = /** @class */ (function (_super) {
    __extends(LoginDemo, _super);
    function LoginDemo() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            notice: '',
            type: 'tab1',
            autoLogin: true,
        };
        _this.onSubmit = function (err, values) {
            console.log('value collected ->', __assign({}, values, { autoLogin: _this.state.autoLogin }));
            if (_this.state.type === 'tab1') {
                _this.setState({
                    notice: '',
                }, function () {
                    if (!err && (values.username !== 'admin' || values.password !== '888888')) {
                        setTimeout(function () {
                            store.User.Login(values);
                        }, 500);
                    }
                });
            }
        };
        _this.onTabChange = function (key) {
            _this.setState({
                type: key,
            });
        };
        _this.changeAutoLogin = function (e) {
            _this.setState({
                autoLogin: e.target.checked,
            });
        };
        return _this;
    }
    LoginDemo.prototype.render = function () {
        return (React.createElement(Animate, { transitionName: "fade", transitionAppear: true, component: "" },
            React.createElement("div", { className: "app-login" },
                React.createElement("div", { className: "app-login-form" },
                    React.createElement(Login, { defaultActiveKey: this.state.type, onTabChange: this.onTabChange, onSubmit: this.onSubmit },
                        React.createElement(Tab, { key: "tab1", tab: "Account" },
                            this.state.notice &&
                                React.createElement(Alert, { style: { marginBottom: 24 }, message: this.state.notice, type: "error", showIcon: true, closable: true }),
                            React.createElement(UserName, { name: "username" }),
                            React.createElement(Password, { name: "password" })),
                        React.createElement(Tab, { key: "tab2", tab: "Mobile" },
                            React.createElement(Mobile, { name: "mobile" }),
                            React.createElement(Captcha, { onGetCaptcha: function () { return console.log('Get captcha!'); }, name: "captcha" })),
                        React.createElement("div", null,
                            React.createElement(Checkbox, { checked: this.state.autoLogin, onChange: this.changeAutoLogin }, "Keep me logged in"),
                            React.createElement("a", { style: { float: 'right' }, href: "" }, "Forgot password")),
                        React.createElement(Submit, null, "Login"),
                        React.createElement("div", null,
                            "Other login methods",
                            React.createElement("span", { className: "icon icon-alipay" }),
                            React.createElement("span", { className: "icon icon-taobao" }),
                            React.createElement("span", { className: "icon icon-weibo" }),
                            React.createElement("a", { style: { float: 'right' }, href: "" }, "Register")))))));
    };
    return LoginDemo;
}(React.Component));
export default LoginDemo;
//# sourceMappingURL=index.js.map