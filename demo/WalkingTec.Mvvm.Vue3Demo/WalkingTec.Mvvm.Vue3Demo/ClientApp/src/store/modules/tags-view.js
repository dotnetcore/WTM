import { __decorate, __extends, __metadata } from "tslib";
import { VuexModule, Module, Mutation, Action, getModule } from "vuex-module-decorators";
import store from "@/store/modules/index";
var TagsView = /** @class */ (function (_super) {
    __extends(TagsView, _super);
    function TagsView() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.visitedViews = [];
        _this.cachedViews = [];
        return _this;
    }
    TagsView.prototype.ADD_VISITED_VIEW = function (view) {
        if (this.visitedViews.some(function (v) { return v.path === view.path; }))
            return;
        this.visitedViews.push(Object.assign({}, view, {
            title: view.meta.title || "no-name"
        }));
    };
    TagsView.prototype.ADD_CACHED_VIEW = function (view) {
        var cachedPath = "";
        if (view.path && _.startsWith(view.path, "/")) {
            cachedPath = view.path.substr(1);
        }
        if (this.cachedViews.includes(cachedPath))
            return;
        if (!view.meta.noCache) {
            this.cachedViews.push(cachedPath);
        }
    };
    TagsView.prototype.DEL_VISITED_VIEW = function (view) {
        for (var _i = 0, _a = Object.entries(this.visitedViews); _i < _a.length; _i++) {
            var _b = _a[_i], i = _b[0], v = _b[1];
            if (v.path === view.path) {
                this.visitedViews.splice(_.parseInt(i), 1);
                break;
            }
        }
    };
    TagsView.prototype.DEL_CACHED_VIEW = function (view) {
        for (var _i = 0, _a = Object.entries(this.cachedViews); _i < _a.length; _i++) {
            var _b = _a[_i], i = _b[0], v = _b[1];
            var cachedPath = view.path ? view.path.substr(1) : view.name;
            if (v === cachedPath) {
                this.cachedViews.splice(_.parseInt(i), 1);
                break;
            }
        }
    };
    TagsView.prototype.DEL_OTHERS_VISITED_VIEWS = function (view) {
        this.visitedViews = this.visitedViews.filter(function (v) {
            return v.meta.affix || v.path === view.path;
        });
    };
    TagsView.prototype.DEL_OTHERS_CACHED_VIEWS = function (view) {
        for (var _i = 0, _a = Object.entries(this.cachedViews); _i < _a.length; _i++) {
            var _b = _a[_i], i = _b[0], v = _b[1];
            if (v === view.name) {
                this.cachedViews = this.cachedViews.slice(_.parseInt(i), _.parseInt(i) + 1);
                break;
            }
        }
    };
    TagsView.prototype.DEL_ALL_VISITED_VIEWS = function () {
        // keep affix tags
        var affixTags = this.visitedViews.filter(function (tag) { return tag.meta.affix; });
        this.visitedViews = affixTags;
    };
    TagsView.prototype.DEL_ALL_CACHED_VIEWS = function () {
        this.cachedViews = [];
    };
    TagsView.prototype.UPDATE_VISITED_VIEW = function (view) {
        for (var _i = 0, _a = this.visitedViews; _i < _a.length; _i++) {
            var v = _a[_i];
            if (v.path === view.path) {
                v = Object.assign(v, view);
                break;
            }
        }
    };
    TagsView.prototype.addView = function (view) {
        this.ADD_VISITED_VIEW(view);
        this.ADD_CACHED_VIEW(view);
    };
    TagsView.prototype.addVisitedView = function (view) {
        this.ADD_VISITED_VIEW(view);
    };
    TagsView.prototype.delView = function (view) {
        this.DEL_VISITED_VIEW(view);
        this.DEL_CACHED_VIEW(view);
    };
    TagsView.prototype.delCachedView = function (view) {
        this.DEL_CACHED_VIEW(view);
    };
    TagsView.prototype.delOthersViews = function (view) {
        this.DEL_OTHERS_VISITED_VIEWS(view);
        this.DEL_OTHERS_CACHED_VIEWS(view);
    };
    TagsView.prototype.delAllViews = function () {
        this.DEL_ALL_VISITED_VIEWS();
        this.DEL_ALL_CACHED_VIEWS();
    };
    TagsView.prototype.delAllCachedViews = function () {
        this.DEL_ALL_CACHED_VIEWS();
    };
    TagsView.prototype.updateVisitedView = function (view) {
        this.UPDATE_VISITED_VIEW(view);
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "ADD_VISITED_VIEW", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "ADD_CACHED_VIEW", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_VISITED_VIEW", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_CACHED_VIEW", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_OTHERS_VISITED_VIEWS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_OTHERS_CACHED_VIEWS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_ALL_VISITED_VIEWS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "DEL_ALL_CACHED_VIEWS", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "UPDATE_VISITED_VIEW", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "addView", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "addVisitedView", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "delView", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "delCachedView", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "delOthersViews", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "delAllViews", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "delAllCachedViews", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], TagsView.prototype, "updateVisitedView", null);
    TagsView = __decorate([
        Module({ dynamic: true, store: store, name: "tagsView" })
    ], TagsView);
    return TagsView;
}(VuexModule));
export var TagsViewModule = getModule(TagsView);
//# sourceMappingURL=tags-view.js.map