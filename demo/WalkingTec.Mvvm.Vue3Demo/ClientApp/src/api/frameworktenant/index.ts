import request from '/@/utils/request';
import other from '/@/utils/other';

export default function frameworktenantApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_frameworktenant/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_frameworktenant/'+data,
				method: 'get'
			});
		},
		getFrameworkTenants: (data: number|string) => {
			return request({
				url: '/api/_frameworktenant/GetFrameworkTenants?parent='+data,
				method: 'get'
			});
		},
		setTenant: (tenant: any) => {
			return request({
				url: '/api/_account/SetTenant?tenant='+tenant,
				method: 'get'
			});
		},	
		add:(data: object)=>{
			return request({
				url:'/api/_frameworktenant/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_frameworktenant/edit',
				method:'put',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/_frameworktenant/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_frameworktenant/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/_frameworktenant/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_frameworktenant/Import',
				method: 'post',
				data,
			});
		},
	};
}
