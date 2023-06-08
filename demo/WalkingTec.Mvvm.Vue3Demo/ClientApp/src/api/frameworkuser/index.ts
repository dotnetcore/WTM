import request from '/@/utils/request';
import other from '/@/utils/other';

export default function frameworkuserApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_FrameworkUser/Search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_Admin/FrameworkUser/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/_Admin/FrameworkUser/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_Admin/FrameworkUser/edit',
				method:'put',
				data
			});
		},
		batchedit:(data: object)=>{
			return request({
				url:'/api/_FrameworkUser/batchedit',
				method:'post',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/_Admin/FrameworkUser/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_Admin/FrameworkUser/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/_Admin/FrameworkUser/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_Admin/FrameworkUser/Import',
				method: 'post',
				data,
			});
		},
	};
}
