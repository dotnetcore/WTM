import request from '/@/utils/request';
import other from '/@/utils/other';

function SchoolVue3Api() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/SchoolVue3/Search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/SchoolVue3/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/SchoolVue3/Add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/SchoolVue3/edit',
				method:'put',
				data
			});
		},
     	batchedit:(data: object)=>{
			return request({
				url:'/api/SchoolVue3/batchedit',
				method:'post',
				data
			});
		},    
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/SchoolVue3/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/SchoolVue3/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/SchoolVue3/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/SchoolVue3/Import',
				method: 'post',
				data,
			});
		},
	};
}

export {SchoolVue3Api}
