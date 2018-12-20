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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:27
 * @modify date 2018-09-12 18:52:27
 * @desc [description] .
 */
import { message } from 'antd';
import { action, computed, observable, runInAction } from 'mobx';
import { Request } from 'utils/Request';
var Store = /** @class */ (function () {
    function Store() {
        /** 数据 ID 索引 */
        this.IdKey = 'id';
        /** 页面操按钮 */
        this.Actions = {
            insert: {
                state: true,
                name: "添加"
            },
            update: {
                state: true,
                name: "修改"
            },
            delete: {
                state: true,
                name: "删除"
            },
            import: {
                state: true,
                name: "导入"
            },
            export: {
                state: true,
                name: "导出"
            },
        };
        /** url 地址 */
        this.Urls = {
            search: {
                src: "/test/search",
                method: "post"
            },
            details: {
                src: "/test/details/{id}",
                method: "get"
            },
            insert: {
                src: "/test/insert",
                method: "post"
            },
            update: {
                src: "/test/update",
                method: "post"
            },
            delete: {
                src: "/test/delete",
                method: "post"
            },
            import: {
                src: "/test/import",
                method: "post"
            },
            export: {
                src: "/test/export",
                method: "post"
            },
            template: {
                src: "/test/template",
                method: "post"
            }
        };
        /** 格式化数据参数 */
        this.Format = {
            date: "YYYY-MM-DD",
            dateTime: "YYYY-MM-DD HH:mm:ss",
        };
        /** Ajax   */
        this.Request = new Request("/api");
        /** 搜索数据参数 */
        this.searchParams = {};
        /** 数据列表 */
        this.dataSource = {
            count: 0,
            list: [],
            pageNo: 1,
            pageSize: 10
        };
        /** 多选行 key */
        this.selectedRowKeys = [];
        /**  详情 */
        this.details = {};
        /** 页面动作 */
        this.pageState = {
            visibleEdit: false,
            visiblePort: false,
            loading: false,
            loadingEdit: false,
            isUpdate: false //编辑状态
        };
    }
    /**
     *  修改页面动作状态
     * @param key
     * @param value
     */
    Store.prototype.onPageState = function (key, value) {
        var prevVal = this.pageState[key];
        if (prevVal == value) {
            return;
        }
        if (typeof value == "undefined") {
            value = !prevVal;
        }
        this.pageState[key] = value;
    };
    /**
     * 多选 行
     * @param selectedRowKeys 选中的keys
     */
    Store.prototype.onSelectChange = function (selectedRowKeys) {
        this.selectedRowKeys = selectedRowKeys;
    };
    /**
     * 编辑
     * 需要重写的方法 使用 runInAction 实现 修改Store
     * 使用 @action.bound 装饰器的方法不可重写
     * @param details 详情 有唯一 key 判定为修改
     */
    Store.prototype.onModalShow = function (details) {
        if (details === void 0) { details = {}; }
        return __awaiter(this, void 0, void 0, function () {
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(details[this.IdKey] == null)) return [3 /*break*/, 1];
                        this.onPageState("isUpdate", false);
                        return [3 /*break*/, 3];
                    case 1:
                        this.onPageState("isUpdate", true);
                        return [4 /*yield*/, this.onDetails(details)];
                    case 2:
                        details = _a.sent();
                        _a.label = 3;
                    case 3:
                        runInAction(function () {
                            _this.details = details;
                        });
                        this.onPageState("visibleEdit", true);
                        return [2 /*return*/];
                }
            });
        });
    };
    /**
     * 加载数据 列表
     * @param params 搜索参数
     */
    Store.prototype.onSearch = function (params, page) {
        if (params === void 0) { params = {}; }
        if (page === void 0) { page = { pageNo: 1, pageSize: 10 }; }
        return __awaiter(this, void 0, void 0, function () {
            var method, src, res;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        console.log(this);
                        if (this.pageState.loading == true) {
                            return [2 /*return*/, message.warn('数据正在加载中')];
                        }
                        this.onPageState("loading", true);
                        this.searchParams = __assign({}, this.searchParams, params);
                        params = __assign({}, page, { search: this.searchParams });
                        method = this.Urls.search.method;
                        src = this.Urls.search.src;
                        return [4 /*yield*/, this.Request[method](src, params).map(function (data) {
                                console.log(data);
                                if (data.list) {
                                    data.list = data.list.map(function (x, i) {
                                        // antd table 列表属性需要一个唯一key
                                        return __assign({ key: i }, x);
                                    });
                                }
                                return data;
                            }).toPromise()];
                    case 1:
                        res = _a.sent();
                        runInAction(function () {
                            _this.dataSource = res || _this.dataSource;
                            _this.onPageState("loading", false);
                        });
                        return [2 /*return*/, res];
                }
            });
        });
    };
    /**
     * 详情
     * @param params 数据实体
     */
    Store.prototype.onDetails = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var method, src, res;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.onPageState("loadingEdit", true);
                        method = this.Urls.details.method;
                        src = this.Urls.details.src;
                        return [4 /*yield*/, this.Request[method](src, params).toPromise()];
                    case 1:
                        res = _a.sent();
                        this.onPageState("loadingEdit", false);
                        return [2 /*return*/, res || {}];
                }
            });
        });
    };
    /**
     * 编辑数据
     * @param params 数据实体
     */
    Store.prototype.onEdit = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var details;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.pageState.loadingEdit) {
                            return [2 /*return*/];
                        }
                        details = __assign({}, this.details, params);
                        this.onPageState("loadingEdit", true);
                        if (!this.pageState.isUpdate) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.onUpdate(details)];
                    case 1: return [2 /*return*/, _a.sent()];
                    case 2: return [4 /*yield*/, this.onInsert(details)];
                    case 3: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    /**
     * 添加数据
     * @param params 数据实体
     */
    Store.prototype.onInsert = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var method, src, res;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        method = this.Urls.insert.method;
                        src = this.Urls.insert.src;
                        return [4 /*yield*/, this.Request[method](src, params).toPromise()];
                    case 1:
                        res = _a.sent();
                        if (res) {
                            message.success('添加成功');
                            // 刷新数据
                            this.onSearch();
                            this.onPageState("visibleEdit", false);
                        }
                        else {
                            message.error('添加失败');
                        }
                        this.onPageState("loadingEdit", false);
                        return [2 /*return*/, res];
                }
            });
        });
    };
    /**
     * 更新数据
     * @param params 数据实体
     */
    Store.prototype.onUpdate = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var method, src, res;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        method = this.Urls.update.method;
                        src = this.Urls.update.src;
                        return [4 /*yield*/, this.Request[method](src, params).toPromise()];
                    case 1:
                        res = _a.sent();
                        if (res) {
                            message.success('更新成功');
                            // 刷新数据
                            this.onSearch();
                            this.onPageState("visibleEdit", false);
                        }
                        else {
                            message.error('更新失败');
                        }
                        this.onPageState("loadingEdit", false);
                        return [2 /*return*/, res];
                }
            });
        });
    };
    /**
     * 删除数据
     * @param params 需要删除的数据集合 取 所有的 id
     */
    Store.prototype.onDelete = function (params) {
        return __awaiter(this, void 0, void 0, function () {
            var method, src, res;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        params = params.map(function (x) { return x[_this.IdKey]; });
                        method = this.Urls.delete.method;
                        src = this.Urls.delete.src;
                        return [4 /*yield*/, this.Request[method](src, params).toPromise()];
                    case 1:
                        res = _a.sent();
                        if (res) {
                            message.success('删除成功');
                            this.onSelectChange([]);
                            // 刷新数据
                            this.onSearch();
                        }
                        else {
                            message.success('删除失败');
                        }
                        return [2 /*return*/, res];
                }
            });
        });
    };
    Object.defineProperty(Store.prototype, "importConfig", {
        /**
         * 导入 配置 参数
         * https://ant.design/components/upload-cn/#components-upload-demo-picture-style
         */
        get: function () {
            var _this = this;
            var action = this.Request.address + this.Urls.import.src;
            return {
                name: 'file',
                multiple: true,
                action: action,
                onChange: function (info) {
                    var status = info.file.status;
                    // NProgress.start();
                    if (status !== 'uploading') {
                        console.log(info.file, info.fileList);
                    }
                    if (status === 'done') {
                        var response = info.file.response;
                        // NProgress.done();
                        if (response.status == 200) {
                            // 刷新数据
                            _this.onSearch();
                            message.success(info.file.name + " file uploaded successfully.");
                        }
                        else {
                            message.error(info.file.name + " " + response.message);
                        }
                    }
                    else if (status === 'error') {
                        message.error(info.file.name + " file upload failed.");
                    }
                }
            };
        },
        enumerable: true,
        configurable: true
    });
    /**
     * 导出
     * @param params 筛选参数
     */
    Store.prototype.onExport = function (params) {
        if (params === void 0) { params = this.searchParams; }
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.Request.download({
                            url: this.Request.address + this.Urls.export.src,
                            body: params
                        })];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    /**
    * 数据模板
    */
    Store.prototype.onTemplate = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.Request.download({
                            url: this.Request.address + this.Urls.template.src,
                        })];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    __decorate([
        observable
    ], Store.prototype, "dataSource", void 0);
    __decorate([
        observable
    ], Store.prototype, "selectedRowKeys", void 0);
    __decorate([
        observable
    ], Store.prototype, "details", void 0);
    __decorate([
        observable
    ], Store.prototype, "pageState", void 0);
    __decorate([
        action.bound
    ], Store.prototype, "onPageState", null);
    __decorate([
        action.bound
    ], Store.prototype, "onSelectChange", null);
    __decorate([
        computed
    ], Store.prototype, "importConfig", null);
    return Store;
}());
export default Store;
//# sourceMappingURL=index.js.map