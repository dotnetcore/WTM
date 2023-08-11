import request from '/@/utils/request';
import other from '/@/utils/other';

export default function dataprivilegeApi() {
	return {
		search: (data: object) => {
			return request({
				url: '/api/_dataprivilege/search',
				method: 'post',
				data,
			});
		},
		get: (data: any) => {
			return request({
				url: '/api/_dataprivilege/get',
				method: 'get',
				params:{TableName:data.TableName,TargetId:data.TargetId,DpType:data.DpType}
			});
		},
		add:(data: object)=>{
			return request({
				url:'/api/_dataprivilege/add',
				method:'post',
				data
			});
		},
		edit:(data: object)=>{
			return request({
				url:'/api/_dataprivilege/edit',
				method:'put',
				data
			});
		},
		delete:(data: any)=>{
			return request({
				url:'/api/_dataprivilege/delete',
				method:'post',
				data:{ModelName:data.TableName,Id:data.TargetId,Type:data.DpType}
			});
		},
		export: (data: object) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_dataprivilege/ExportExcel',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});;
		},
		exportById: (data: Array<number>|Array<string>) => {
			return request<any,Blob>({
                responseType: "blob",
				url: '/api/_dataprivilege/ExportExcelByIds',
				method: 'post',
				data,
			}).then((data)=>{other.downloadFile(data)});
		},
		import: (data: object) => {
			return request({
				url: '/api/_dataprivilege/Import',
				method: 'post',
				data,
			});
		},
	};
}
