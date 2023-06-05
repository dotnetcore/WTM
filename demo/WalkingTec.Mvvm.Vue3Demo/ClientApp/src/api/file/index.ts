import request from '/@/utils/request';
import other from '/@/utils/other';

/**
 * 文件api接口集合
 * @method getFile 获取文件，返回内部url
 */
export default function fileApi(){
	return {
		getFile: (id: any,width:any=null, height:any=null) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_file/getfile/'+id,
				method: 'get',
				params:{width,height}
			}).then(res=>{
				let rv =URL.createObjectURL(res);
				return rv;
			}).catch(err=>{
				return "";
			});
		},
		getName: (id: any) => {
			return request<any,Blob>({
				url: '/api/_file/GetFileName/'+id,
				method: 'get'
			}).then(res=>{				
				return res;
			}).catch(err=>{
				return "";
			});
		},
		downloadFile: (id: any) => {
			return request<any,Blob>({
				responseType: "blob",
				url: '/api/_file/DownloadFile/'+id,
				method: 'get'
			}).then((data)=>{other.downloadFile(data)});
		},

		deleteFile:(id:string)=>{
			return request({
				url:'/api/_file/DeletedFile/'+id,
				method:'get'				
			});
		}
	};
}
