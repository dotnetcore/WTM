import request from '/@/utils/request';
import other from '/@/utils/other';

export default function schoolApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/School/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/School/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/School/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/School/edit',
				method:'put',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/School/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/School/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/School/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/School/Import',
				method: 'post',
				data,
			});
		},
	};
}
