import request from '/@/utils/request';
import other from '/@/utils/other';

export default function frameworkroleApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_frameworkrole/search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/_frameworkrole/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/_frameworkrole/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_frameworkrole/edit',
				method:'put',
				data
			});
		},
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/_frameworkrole/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_frameworkrole/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
                responseType: "blob",
				url: '/api/_frameworkrole/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_frameworkrole/Import',
				method: 'post',
				data,
			});
		},
		getPageActions:(data: number|string) => {
			return request({
				url: '/api/_frameworkrole/GetPageActions/'+data,
				method: 'get'
			});
		},
		editPrivilege:(data:object)=>{
			return request({
				url:'/api/_frameworkrole/editPrivilege',
				method:'put',
				data
			});
		}
	};
}
