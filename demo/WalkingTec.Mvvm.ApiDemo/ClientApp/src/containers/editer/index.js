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
import * as React from 'react';
import { Form, Button } from 'antd';
import Editor from 'components/editer';
var FormItem = Form.Item;
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.handleSubmit = function (e) {
            e.preventDefault();
            _this.props.form.validateFields(function (err, values) {
                if (!err) {
                    console.log(values);
                }
                else {
                    console.log(values);
                }
            });
        };
        return _this;
    }
    IApp.prototype.render = function () {
        var getFieldDecorator = this.props.form.getFieldDecorator;
        var formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 6 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 16 },
            },
        };
        return (React.createElement(Form, { onSubmit: this.handleSubmit },
            React.createElement(FormItem, { label: '\u5185\u5BB9' }, getFieldDecorator('content', {
                initialValue: '<p>默认内容</p>',
                rules: [{ required: true, message: 'Please input content note!' }],
            })(React.createElement(Editor, null))),
            React.createElement(FormItem, { label: '\u5185\u5BB9' }, getFieldDecorator('content', {
                initialValue: '<p>默认内容</p>',
                rules: [{ required: true, message: 'Please input content note!' }],
            })(React.createElement(Editor, null))),
            React.createElement(FormItem, { label: '\u5185\u5BB9' }, getFieldDecorator('content', {
                initialValue: '<p>默认内容</p>',
                rules: [{ required: true, message: 'Please input content note!' }],
            })(React.createElement(Editor, null))),
            React.createElement(FormItem, { label: '\u5185\u5BB9' }, getFieldDecorator('content', {
                initialValue: '<p>默认内容</p>',
                rules: [{ required: true, message: 'Please input content note!' }],
            })(React.createElement(Editor, null))),
            React.createElement(Button, { type: "primary", htmlType: "submit" }, "\u63D0\u4EA4")));
    };
    return IApp;
}(React.Component));
export default Form.create()(IApp);
//# sourceMappingURL=index.js.map