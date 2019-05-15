import Vue from 'vue';
import '@/assets/css/index.less';
import router from '@/router/index';
import store from '@/store/index/index';
import App from '@/pages/index/app.vue';
import utilDate from '@/util/date';
import 'element-ui/lib/theme-chalk/index.css';
import 'babel-polyfill';
import {
    Input,
    Tag,
    Table,
    TableColumn,
    Pagination,
    Button,
    Carousel,
    CarouselItem,
    Message,
    Checkbox,
    MessageBox,
    Dialog,
    DatePicker,
    Tooltip,
    Icon
} from 'element-ui';

Vue.use(Input);
Vue.use(Tag);
Vue.use(Table);
Vue.use(TableColumn);
Vue.use(Pagination);
Vue.use(Button);
Vue.use(Carousel);
Vue.use(CarouselItem);
Vue.use(Checkbox);
Vue.use(Dialog);
Vue.use(DatePicker);
Vue.use(Icon);

Vue.use(Tooltip);
Vue.prototype.$message = Message;
Vue.prototype.$confirm = MessageBox.confirm;
Vue.prototype.$alert = MessageBox.alert;

// 时间格式
// | formatTime('yyyy-MM-dd')
Vue.filter(
    'formatTime',
    (value, customFormat = 'yyyy-MM-dd hh:mm:ss', isMsec = true, days = 0) => {
        let time = '';
        if (!isNaN(value)) {
            value = isMsec ? value * 1 : value * 1000;
            value = value + days * 24 * 60 * 60 * 1000;
        } else {
            value = new Date(value).getTime();
        }
        time = value ? utilDate.toFormat(value, customFormat) : '';
        return time;
    }
);

Vue.filter('formatSubStr', (value, num = 6) => {
    if (value && value.length > num) {
        return value.substr(0, num) + '...';
    }
    return value;
});
/* eslint-disable */
const app = new Vue({
    router,
    store,
    render(h) {
        return h(App, {
            props: {
                projectName: 'trade'
            }
        });
    }
});

app.$mount('#App');
