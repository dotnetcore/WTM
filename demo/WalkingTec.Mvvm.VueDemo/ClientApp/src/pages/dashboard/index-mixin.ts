import { Component, Vue } from "vue-property-decorator";
import { RoutesModule } from "@/store/modules/routes";
import EChartsModule from "./views/echarts.vue";
import ChartCard from "./views/chart-card.vue";
import StatsCard from "./views/stats-card.vue";
import { Action, State } from "vuex-class";
import store from "./store/index";
import LOCAL from "./local";

@Component({
    name: "dashboard",
    store,
    components: {
        EChartsModule,
        ChartCard,
        StatsCard
    }
})
export default class extends Vue {
    value: Date = new Date();
    get shortcuts (): any[] {
        return [
            "actionlog",
            "frameworkgroup",
            "frameworkrole",
            "frameworkuser",
            "frameworkmenu",
            "dataprivilege"
        ]
    };
    get shortcutList() {
        return RoutesModule.pageList.filter(item =>
            this.shortcuts.includes(item.Text)
        ).map(item => ({
            ...item,
            Text: this.$t(`route.${item.Text}`)
        }));
    }
    @Action
    getGithubInfo;
    @State
    getGithubInfoData;

    dailySalesChart: Object = {
        data: {
            labels: ["M", "T", "W", "T", "F", "S", "S"],
            series: [[12, 17, 7, 17, 23, 18, 38]]
        },
        options: {
            lineSmooth: this.$Chartist.Interpolation.cardinal({
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

    dataCompletedTasksChart: Object = {
        type: "Bar",
        data: {
            labels: ["12am", "3pm", "6pm", "9pm", "12pm", "3am", "6am", "9am"],
            series: [[230, 750, 450, 300, 280, 240, 200, 190]]
        },

        options: {
            lineSmooth: this.$Chartist.Interpolation.cardinal({
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

    emailsSubscriptionChart: Object = {
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
                        labelInterpolationFnc: function(value) {
                            return value[0];
                        }
                    }
                }
            ]
        ]
    };

    mounted() {
        this.getGithubInfo();
    }

    onPush(path) {
        this.$router.push(path);
    }


    beforeCreate() {
        if (LOCAL && !this.$i18n.getLocaleMessage('en')[this.$options.name]) {
          this.$i18n.mergeLocaleMessage("en", LOCAL.en);
          this.$i18n.mergeLocaleMessage("zh", LOCAL.zh);
        }
      }
}
