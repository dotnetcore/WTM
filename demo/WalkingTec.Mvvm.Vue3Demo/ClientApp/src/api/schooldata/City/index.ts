import request from '/@/utils/request';
import other from '/@/utils/other';

function CityApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/City/Search',
				method: 'post',
				data,
			});
		},
		get: (data: number|string) => {
			return request({
				url: '/api/City/'+data,
				method: 'get'
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/City/Add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/City/edit',
				method:'put',
				data
			});
		},
     	batchedit:(data: object)=>{
			return request({
				url:'/api/City/batchedit',
				method:'post',
				data
			});
		},    
		delete:(data: Array<number>|Array<string>)=>{
			return request({
				url:'/api/City/BatchDelete',
				method:'post',
				data
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/City/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
				url: '/api/City/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/City/Import',
				method: 'post',
				data,
			});
		},
	};
}

export {CityApi}
