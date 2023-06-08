import request from '/@/utils/request';
import other from '/@/utils/other';

export default function actionlogApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_actionlog/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_actionlog/'+data,
				method: 'get'
			});
		},
	
	};
}
