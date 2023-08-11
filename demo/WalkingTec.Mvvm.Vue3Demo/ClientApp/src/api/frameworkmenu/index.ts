import request from '/@/utils/request';
import other from '/@/utils/other';

export default function frameworkmenuApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_frameworkmenu/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_frameworkmenu/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/_frameworkmenu/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_frameworkmenu/edit',
				method:'put',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/_frameworkmenu/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_frameworkmenu/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
                responseType: "blob",
				url: '/api/_frameworkmenu/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_frameworkmenu/Import',
				method: 'post',
				data,
			});
		},
		refresh: () => {
			return request({
				url: '/api/_frameworkmenu/RefreshMenu',
				method: 'get'
			});
		},

	};
}
