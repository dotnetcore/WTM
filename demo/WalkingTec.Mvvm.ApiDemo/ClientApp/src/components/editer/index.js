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
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:42
 * @modify date 2018-09-12 18:53:42
 * @desc [description]
*/
import { message } from 'antd';
import BraftEditor, { EditorState } from 'braft-editor';
import * as React from 'react';
require('braft-editor/dist/index.css');
/**
 * https://github.com/margox/braft-editor
 * 富文本编辑
 */
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            responseList: [],
        };
        _this.onChange = function (content) {
            _this.receiveHtml(content.toHTML());
            console.log('onChange', content.toHTML());
        };
        _this.onRawChange = function (rawContent) {
            console.log('onRawChange', rawContent);
        };
        return _this;
    }
    default_1.prototype.receiveHtml = function (object) {
        var onChange = this.props.onChange;
        onChange(object == '<p></p>' ? null : object);
    };
    default_1.prototype.shouldComponentUpdate = function () {
        return false;
    };
    default_1.prototype.render = function () {
        var editorProps = {
            height: 500,
            contentFormat: 'html',
            // initialContent: this.props.value,
            defaultValue: EditorState.createFrom(this.props.value),
            onChange: this.onChange,
            media: {
                allowPasteImage: true,
                image: true,
                video: false,
                audio: false,
                validateFn: null,
                uploadFn: function (param) {
                    var serverURL = '/admin/upload';
                    var xhr = new XMLHttpRequest;
                    var fd = new FormData();
                    // libraryId可用于通过mediaLibrary示例来操作对应的媒体内容
                    // console.log(param.libraryId)
                    var successFn = function (response) {
                        console.log(xhr.response);
                        response = JSON.parse(xhr.response);
                        // 假设服务端直接返回文件上传后的地址
                        // 上传成功后调用param.success并传入上传后的文件地址
                        if (response.code == 200) {
                            param.success({
                                url: response.data.fullUrl,
                                meta: {
                                    id: 'xxx',
                                    title: 'xxx',
                                    alt: 'xxx',
                                    loop: true,
                                    autoPlay: true,
                                    controls: true,
                                    poster: 'http://xxx/xx.png',
                                }
                            });
                        }
                        else {
                            message.error("\u56FE\u7247\u4E0A\u4F20\u5931\u8D25");
                            param.error({
                                msg: 'unable to upload.'
                            });
                        }
                    };
                    var progressFn = function (event) {
                        // 上传进度发生变化时调用param.progress
                        param.progress(event.loaded / event.total * 100);
                    };
                    var errorFn = function (response) {
                        // 上传发生错误时调用param.error
                        param.error({
                            msg: 'unable to upload.'
                        });
                        message.error("\u56FE\u7247\u4E0A\u4F20\u5931\u8D25");
                    };
                    xhr.upload.addEventListener("progress", progressFn, false);
                    xhr.addEventListener("load", successFn, false);
                    xhr.addEventListener("error", errorFn, false);
                    xhr.addEventListener("abort", errorFn, false);
                    fd.append('file', param.file);
                    xhr.open('POST', serverURL, true);
                    xhr.send(fd);
                },
                removeConfirmFn: null,
                onRemove: null,
                onChange: null,
                onInsert: null,
            }
        };
        return (React.createElement(BraftEditor, __assign({}, editorProps)));
    };
    return default_1;
}(React.Component));
export default default_1;
//# sourceMappingURL=index.js.map