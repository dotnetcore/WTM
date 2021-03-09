import { __assign, __decorate, __extends, __metadata } from "tslib";
import { Component, Vue } from "vue-property-decorator";
import { RoutesModule } from "@/store/modules/routes";
import EChartsModule from "./views/echarts.vue";
import ChartCard from "./views/chart-card.vue";
import StatsCard from "./views/stats-card.vue";
import { Action, State } from "vuex-class";
import store from "./store/index";
import LOCAL from "./local";
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.value = new Date();
        _this.dailySalesChart = {
            data: {
                labels: ["M", "T", "W", "T", "F", "S", "S"],
                series: [[12, 17, 7, 17, 23, 18, 38]]
            },
            options: {
                lineSmooth: _this.$Chartist.Interpolation.cardinal({
                    tension: 0
                }),
                low: 0,
                high: 50,
                chartPadding: {
                    top: 0,
                    right: 0,
                    bottom: 0,
                    left: 0
                }
            },
            type: "Line"
        };
        _this.dataCompletedTasksChart = {
            type: "Bar",
            data: {
                labels: ["12am", "3pm", "6pm", "9pm", "12pm", "3am", "6am", "9am"],
                series: [[230, 750, 450, 300, 280, 240, 200, 190]]
            },
            options: {
                lineSmooth: _this.$Chartist.Interpolation.cardinal({
                    tension: 0
                }),
                low: 0,
                high: 1000,
                chartPadding: {
                    top: 0,
                    right: 0,
                    bottom: 0,
                    left: 0
                }
            }
        };
        _this.emailsSubscriptionChart = {
            data: {
                labels: [
                    "Ja",
                    "Fe",
                    "Ma",
                    "Ap",
                    "Mai",
                    "Ju",
                    "Jul",
                    "Au",
                    "Se",
                    "Oc",
                    "No",
                    "De"
                ],
                series: [
                    [542, 443, 320, 780, 553, 453, 326, 434, 568, 610, 756, 895]
                ]
            },
            options: {
                axisX: {
                    showGrid: false
                },
                low: 0,
                high: 1000,
                chartPadding: {
                    top: 0,
                    right: 5,
                    bottom: 0,
                    left: 0
                }
            },
            responsiveOptions: [
                [
                    "screen and (max-width: 640px)",
                    {
                        seriesBarDistance: 5,
                        axisX: {
                            labelInterpolationFnc: function (value) {
                                return value[0];
                            }
                        }
                    }
                ]
            ]
        };
        return _this;
    }
    Object.defineProperty(default_1.prototype, "shortcuts", {
        get: function () {
            return [
                "actionlog",
                "frameworkgroup",
                "frameworkrole",
                "frameworkuser",
                "frameworkmenu",
                "dataprivilege"
            ];
        },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(default_1.prototype, "shortcutList", {
        get: function () {
            var _this = this;
            return RoutesModule.pageList.filter(function (item) {
                return _this.shortcuts.includes(item.Text);
            }).map(function (item) { return (__assign(__assign({}, item), { Text: _this.$t("route." + item.Text) })); });
        },
        enumerable: false,
        configurable: true
    });
    default_1.prototype.mounted = function () {
        this.getGithubInfo();
    };
    default_1.prototype.onPush = function (path) {
        this.$router.push(path);
    };
    default_1.prototype.beforeCreate = function () {
        if (LOCAL && !this.$i18n.getLocaleMessage('en')[this.$options.name]) {
            this.$i18n.mergeLocaleMessage("en", LOCAL.en);
            this.$i18n.mergeLocaleMessage("zh", LOCAL.zh);
        }
    };
    __decorate([
        Action,
        __metadata("design:type", Object)
    ], default_1.prototype, "getGithubInfo", void 0);
    __decorate([
        State,
        __metadata("design:type", Object)
    ], default_1.prototype, "getGithubInfoData", void 0);
    default_1 = __decorate([
        Component({
            name: "dashboard",
            store: store,
            components: {
                EChartsModule: EChartsModule,
                ChartCard: ChartCard,
                StatsCard: StatsCard
            }
        })
    ], default_1);
    return default_1;
}(Vue));
export default default_1;
//# sourceMappingURL=index-mixin.js.map