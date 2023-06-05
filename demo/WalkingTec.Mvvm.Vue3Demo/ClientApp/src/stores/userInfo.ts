import { defineStore } from 'pinia';
import Cookies from 'js-cookie';
import { Session,Local } from '/@/utils/storage';
import loginapi from '/@/api/login';
import fileapi from '/@/api/file';

/**
 * 用户信息
 * @methods setUserInfos 设置用户信息
 */
export const useUserInfo = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {
			userName: '',
			photo: '',
			itCode: '',
			tenantCode:'',
			remoteToken: '',
			currentTenant:'',
			time: 0,
			isDebug: false,
			menu:[],
			roles: [],
			authBtnList: [],
		},
	}),
	actions: {
		async setUserInfos() {
			// 存储用户信息到浏览器缓存
			if (Session.get('userInfo')) {
				this.userInfos = Session.get('userInfo');
			} else {
				const userInfos: any = await this.getApiUserInfo();
				if(userInfos){
				Session.set('userInfo',userInfos);
				this.userInfos = userInfos;
				}
			}
		},
		async getApiUserInfo() {
			return	loginapi().getUserInfo().then(async res=>{				
				const userInfos = {
					userName: res.Name,
					remoteToken: res.RemoteToken??'',
					itCode: res.ITCode,
					tenantCode:res.TenantCode??'',
					currentTenant:res.CurrentTenant??'',
					time: new Date().getTime(),
					photo:'',
					isDebug: true,
					roles: [],
					menu:[],
					authBtnList: []
				};
				if(res.PhotoId !== undefined){
					userInfos.photo = `/api/_file/getfile/${res.PhotoId}?width=25&height=25`;
				}
				if(res.Roles !== undefined){
					userInfos.roles = res.Roles;
				}
				if(res.Attributes?.Actions !== undefined){
					userInfos.authBtnList = res.Attributes.Actions;
				}
				if(res.Attributes?.IsDebug !== undefined){
					userInfos.isDebug = res.Attributes.IsDebug;
				}
				if(res.Attributes?.Menus !== undefined){
					userInfos.menu = res.Attributes.Menus;
				}
				console.log(userInfos);
				return userInfos;
			}).catch(error=>{
				Session.clear(); 
				Local.remove("token");
				window.location.href = '/'; 
			});
			// return new Promise((resolve) => {
			// 	setTimeout(() => {
			// 		// 模拟数据，请求接口时，记得删除多余代码及对应依赖的引入
			// 		const userName = Cookies.get('userName');
			// 		// 模拟数据
			// 		let defaultRoles: Array<string> = [];
			// 		let defaultAuthBtnList: Array<string> = [];
			// 		// admin 页面权限标识，对应路由 meta.roles，用于控制路由的显示/隐藏
			// 		let adminRoles: Array<string> = ['admin'];
			// 		// admin 按钮权限标识
			// 		let adminAuthBtnList: Array<string> = ['btn.add', 'btn.del', 'btn.edit', 'btn.link'];
			// 		// test 页面权限标识，对应路由 meta.roles，用于控制路由的显示/隐藏
			// 		let testRoles: Array<string> = ['common'];
			// 		// test 按钮权限标识
			// 		let testAuthBtnList: Array<string> = ['btn.add', 'btn.link'];
			// 		// 不同用户模拟不同的用户权限
			// 		if (userName === 'admin') {
			// 			defaultRoles = adminRoles;
			// 			defaultAuthBtnList = adminAuthBtnList;
			// 		} else {
			// 			defaultRoles = testRoles;
			// 			defaultAuthBtnList = testAuthBtnList;
			// 		}
			// 		// 用户信息模拟数据
			// 		const userInfos = {
			// 			userName: userName,
			// 			photo:
			// 				userName === 'admin'
			// 					? 'https://img2.baidu.com/it/u=1978192862,2048448374&fm=253&fmt=auto&app=138&f=JPEG?w=504&h=500'
			// 					: 'https://img2.baidu.com/it/u=2370931438,70387529&fm=253&fmt=auto&app=138&f=JPEG?w=500&h=500',
			// 			time: new Date().getTime(),
			// 			roles: defaultRoles,
			// 			authBtnList: defaultAuthBtnList,
			// 		};
			// 		resolve(userInfos);
			// 	}, 0);
			//});
		},
	},
});
