import Vue from 'vue';
import router from '@/router/index';
import store from '@/store/index/index';
import App from '@/pages/index/app.vue';
import '@/assets/css/index.less';
import utilDate from '@/util/date';
import 'babel-polyfill';
// 饿了吗ui
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';

Vue.use(ElementUI);

// 时间格式 | formatTime('yyyy-MM-dd')
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
