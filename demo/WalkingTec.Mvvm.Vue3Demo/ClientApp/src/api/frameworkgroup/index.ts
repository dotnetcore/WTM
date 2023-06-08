import request from '/@/utils/request';
import other from '/@/utils/other';

export default function frameworkgroupApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_frameworkgroup/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_frameworkgroup/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/_frameworkgroup/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_frameworkgroup/edit',
				method:'put',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/_frameworkgroup/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_frameworkgroup/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/_frameworkgroup/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_frameworkgroup/Import',
				method: 'post',
				data,
			});
		},
	};
}
