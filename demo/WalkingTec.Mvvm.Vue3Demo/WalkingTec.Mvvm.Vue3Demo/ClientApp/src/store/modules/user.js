import { __assign, __awaiter, __decorate, __extends, __generator, __metadata } from "tslib";
import { VuexModule, Module, Action, Mutation, getModule } from "vuex-module-decorators";
import { getToken, setToken, removeToken } from "@/util/cookie";
import { resetRouter } from "@/router";
import store from "@/store/modules/index";
import _request from "@/util/service";
import serviceUrl from "@/service/modules/user";
var User = /** @class */ (function (_super) {
    __extends(User, _super);
    function User() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.token = getToken() || "";
        _this.name = "";
        _this.roles = [];
        _this.actionList = [];
        _this.menus = [];
        _this.info = {};
        return _this;
        /********************* 单页面 end ***********************/
    }
    User.prototype.SET_TOKEN = function (token) {
        this.token = token;
    };
    User.prototype.SET_NAME = function (name) {
        this.name = name;
    };
    User.prototype.SET_ROLES = function (roles) {
        this.roles = roles;
    };
    User.prototype.SET_ACTIONS = function (actionList) {
        this.actionList = actionList;
    };
    User.prototype.SET_INFO = function (info) {
        this.info = info;
    };
    User.prototype.SET_MENUS = function (menus) {
        this.menus = menus;
    };
    User.prototype.GetUserInfo = function () {
        return __awaiter(this, void 0, void 0, function () {
            var data, Id, ITCode, Name, PhotoId, Roles, Attributes;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, _request(__assign(__assign({}, serviceUrl.loginCheckLogin), { data: { ID: this.token } }))];
                    case 1:
                        data = _a.sent();
                        if (!data) {
                            throw Error("Verification failed, please Login again.");
                        }
                        Id = data.Id, ITCode = data.ITCode, Name = data.Name, PhotoId = data.PhotoId, Roles = data.Roles, Attributes = data.Attributes;
                        this.SET_ROLES(Roles);
                        this.SET_NAME(Name);
                        this.SET_ACTIONS(Attributes.Actions);
                        this.SET_MENUS(Attributes.Menus);
                        this.SET_INFO({ Id: Id, ITCode: ITCode, Name: Name, PhotoId: PhotoId });
                        return [2 /*return*/];
                }
            });
        });
    };
    User.prototype.LogOut = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (this.token === "") {
                            throw Error("LogOut: token is undefined!");
                        }
                        return [4 /*yield*/, _request(__assign(__assign({}, serviceUrl.loginLogout), { data: { ID: this.token } }))];
                    case 1:
                        _a.sent();
                        removeToken();
                        // 单页面
                        this.ResetToken();
                        return [2 /*return*/];
                }
            });
        });
    };
    User.prototype.ChangePassword = function (params) {
        return _request(__assign(__assign({}, serviceUrl.ChangePassword), { data: __assign({}, params) })).catch(function (_a) {
            var response = _a.response;
            return { error: true, data: response.data };
        });
    };
    /********************* 单页面 需要如下***********************/
    User.prototype.Login = function (userInfo) {
        return __awaiter(this, void 0, void 0, function () {
            var username, password, data;
            return __generator(this, function (_a) {
                username = userInfo.username, password = userInfo.password;
                username = username.trim();
                data = { accessToken: "" };
                setToken(data.accessToken);
                this.SET_TOKEN(data.accessToken);
                return [2 /*return*/];
            });
        });
    };
    User.prototype.ResetToken = function () {
        removeToken();
        resetRouter();
        this.SET_TOKEN("");
        this.SET_ROLES([]);
        this.SET_ACTIONS([]);
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_TOKEN", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_NAME", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_ROLES", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Array]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_ACTIONS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_INFO", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "SET_MENUS", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], User.prototype, "GetUserInfo", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], User.prototype, "LogOut", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], User.prototype, "ChangePassword", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", Promise)
    ], User.prototype, "Login", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], User.prototype, "ResetToken", null);
    User = __decorate([
        Module({ dynamic: true, store: store, name: "user" })
    ], User);
    return User;
}(VuexModule));
export var UserModule = getModule(User);
//# sourceMappingURL=user.js.map