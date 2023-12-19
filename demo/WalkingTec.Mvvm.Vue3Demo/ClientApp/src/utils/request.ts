import axios, { AxiosInstance } from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Local, Session } from '/@/utils/storage';
import qs from 'qs';
import { i18n } from '/@/i18n/index';


// 配置新建一个 axios 实例
const service: AxiosInstance = axios.create({
	baseURL: import.meta.env.VITE_API_URL,
	timeout: 50000,
	headers: { 'Content-Type': 'application/json' },
	paramsSerializer: {
		serialize(params) {
			return qs.stringify(params, { allowDots: true });
		},
	},
});

// 添加请求拦截器
service.interceptors.request.use(
	(config) => {
		// 在发送请求之前做些什么 token
        if (Local.get('token') && config.url?.toLocaleLowerCase().includes("/api/_account/loginjwt") == false) {
			config.headers!['Authorization'] = `Bearer ${Local.get('token')}`;
		}
		config.headers!['Accept-Language'] = Local.get('themeConfig')['globalI18n'];
		return config;
	},
	(error) => {
		// 对请求错误做些什么
		return Promise.reject(error);
	}
);

// 添加响应拦截器
service.interceptors.response.use(
	(response) => {
		if (response.headers['content-disposition']) {
			response.data.contentDisposition = response.headers['content-disposition']
		}
		console.log(response.data);
		return response.data;
	},
	(error) => {
		// 对响应错误做点什么
		if (error.response.status === 400) {
			if (typeof error.response.data === 'string') {
				ElMessage.error(error.response.data);
			}
			else {
				if (error.response.data.Message && error.response.data.Message.constructor == Array && error.response.data.Message.length > 0) {
					ElMessage.error(error.response.data.Message[0]);
				}
				else if (error.response.data.Form && Object.getOwnPropertyNames(error.response.data.Form).length > 0) {
					ElMessage.error(error.response.data.Form[Object.getOwnPropertyNames(error.response.data.Form)[0]]);
				}
			}
			error.FormError = error.response.data.Form;
		}
		else if (error.response.status === 401 || error.response.status === 4001) {

			ElMessageBox.alert(i18n.global.t('message._system.common.out'), '', {})
				.then(() => {
					Session.clear();
					Local.remove("token");
					window.location.href = '/';
				})
				.catch(() => { });
		}
		else if (error.response.status === 403) {
			ElMessage.error(i18n.global.t('message._system.common.noright'));
		}
		else {
			if (error.response.data) ElMessage.error(error.response.statusText);
			else ElMessage.error(i18n.global.t('message._system.common.requesterror'));
		}
		return Promise.reject(error);
	}
);

// 导出 axios 实例
export default service;
