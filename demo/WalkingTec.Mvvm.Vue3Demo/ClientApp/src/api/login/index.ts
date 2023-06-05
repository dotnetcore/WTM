import request from '/@/utils/request';

/**
 * （不建议写成 request.post(xxx)，因为这样 post 时，无法 params 与 data 同时传参）
 *
 * 登录api接口集合
 * @method signIn 用户登录
 * @method signOut 用户退出登录
 */
export default function useLoginApi() {
	return {
		signIn: (data: object) => {
			return request({
				url: '/api/_account/loginjwt',
				method: 'post',
				data,
			});
		},
		signInSSO: (remoteToken: any) => {
			return request({
				url: '/api/_account/loginjwt?_remotetoken='+remoteToken,
				method: 'post',
				data:{
					Account:'',
					Password:'',
					RemoteToken:remoteToken
				},
			});
		},	
		signOut: (data: object) => {
			return request({
				url: '/user/signOut',
				method: 'post',
				data,
			});
		},
		changePassword: (data: object) => {
			return request({
				url: '/api/_account/ChangePassword',
				method: 'post',
				data,
			});
		},
		getUserInfo:()=>{
			return request({
				url:'/api/_account/checkuserinfo',
				method:'get'
			});
		}
	};
}
