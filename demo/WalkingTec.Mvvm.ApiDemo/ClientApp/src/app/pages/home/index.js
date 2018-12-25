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
import { Row, Col, Card } from 'antd';
import Antv1 from './antv1';
import Antv2 from './antv2';
import Antv3 from './antv3';
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    IApp.prototype.render = function () {
        console.log("Home");
        return (React.createElement("div", null,
            React.createElement(Row, { type: "flex", gutter: 16 },
                React.createElement(Col, { span: 24 },
                    React.createElement(Card, { bordered: false },
                        React.createElement("div", { style: {
                                textAlign: "center",
                                fontSize: 60
                            } },
                            React.createElement("a", { href: "https://wtm-front.github.io/wtm-cli/", target: "_block" }, "WTM\u6587\u6863\u5730\u5740"))))),
            React.createElement(Row, { type: "flex", gutter: 16 },
                React.createElement(Col, { span: 12 },
                    React.createElement(Card, { bordered: false },
                        "  ",
                        React.createElement(Antv1, null))),
                React.createElement(Col, { span: 12 },
                    React.createElement(Card, { bordered: false },
                        "  ",
                        React.createElement(Antv3, null)))),
            React.createElement(Row, { type: "flex", gutter: 16 },
                React.createElement(Col, { span: 24 },
                    React.createElement(Card, { bordered: false },
                        "    ",
                        React.createElement(Antv2, null))))));
    };
    return IApp;
}(React.Component));
export default IApp;
//# sourceMappingURL=index.js.map