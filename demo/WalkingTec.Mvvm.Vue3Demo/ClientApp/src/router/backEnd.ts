import { RouteRecordRaw } from 'vue-router';
import { storeToRefs } from 'pinia';
import pinia from '/@/stores/index';
import { useUserInfo } from '/@/stores/userInfo';
import { useRequestOldRoutes } from '/@/stores/requestOldRoutes';
import { Session, Local } from '/@/utils/storage';
import { NextLoading } from '/@/utils/loading';
import { dynamicRoutes, notFoundAndNoPower } from '/@/router/route';
import { formatTwoStageRoutes, formatFlatteningRoutes, router } from '/@/router/index';
import { useRoutesList } from '/@/stores/routesList';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';

// 后端控制路由

// 引入 api 请求接口

/**
 * 获取目录下的 .vue、.tsx 全部文件
 * @method import.meta.glob
 * @link 参考：https://cn.vitejs.dev/guide/features.html#json
 */
const layouModules: any = import.meta.glob('../layout/routerView/*.{vue,tsx}');
export const viewsModules: any = import.meta.glob('../views/**/*.{vue,tsx}');
const dynamicViewsModules: Record<string, Function> = Object.assign({}, { ...layouModules }, { ...viewsModules });

/**
 * 后端控制路由：初始化方法，防止刷新时路由丢失
 * @method NextLoading 界面 loading 动画开始执行
 * @method useUserInfo().setUserInfos() 触发初始化用户信息 pinia
 * @method useRequestOldRoutes().setRequestOldRoutes() 存储接口原始路由（未处理component），根据需求选择使用
 * @method setAddRoute 添加动态路由
 * @method setFilterMenuAndCacheTagsViewRoutes 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
 */
export async function initBackEndControlRoutes() {



	// 界面 loading 动画开始执行
	if (window.nextLoading === undefined) NextLoading.start();
	// 无 token 停止执行下一步
	if (Local.get('token') === null) return false;
	// 触发初始化用户信息 pinia
	// https://gitee.com/lyt-top/vue-next-admin/issues/I5F1HP
	await useUserInfo().setUserInfos();
	//获取本地数据
	let menu: any[] = [];
	let localMenu = await GetLocalFile();
	if (useUserInfo().userInfos.isDebug == true) {
		menu = localMenu;
	}
	else {
		localMenu = formatFlatteningRoutes(localMenu);
		menu = await getBackEndControlRoutes(useUserInfo().userInfos.menu.filter(x => !x.ParentId || x.ParentId == ''),useUserInfo().userInfos.menu,localMenu);
		menu.push(...localMenu.filter(x=>x.meta.isHide == true));
	}
	// 无登录权限时，添加判断
	// https://gitee.com/lyt-top/vue-next-admin/issues/I64HVO
	//if (menu.length <= 0) return Promise.resolve(true);
	// 存储接口原始路由（未处理component），根据需求选择使用
	useRequestOldRoutes().setRequestOldRoutes(JSON.parse(JSON.stringify(menu)));
	// 处理路由（component），替换 dynamicRoutes（/@/router/route）第一个顶级 children 的路由
	menu = await backEndComponent(menu);
	menu = [{
		path: '/home',
		name: 'home',
		component: () => import('/@/views/home/index.vue'),
		meta: {
			title: 'message._system.router.home',
			isLink: '',
			isHide: false,
			isKeepAlive: true,
			isAffix: true,
			isIframe: false,
			icon: 'fa fa-home',
		},
	}, ...menu]

	dynamicRoutes[0].children = menu;
	// 添加动态路由
	await setAddRoute();
	// 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
	await setFilterMenuAndCacheTagsViewRoutes();
}

/**
 * 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
 * @description 用于左侧菜单、横向菜单的显示
 * @description 用于 tagsView、菜单搜索中：未过滤隐藏的(isHide)
 */
export async function setFilterMenuAndCacheTagsViewRoutes() {
	const storesRoutesList = useRoutesList(pinia);
	storesRoutesList.setRoutesList(dynamicRoutes[0].children as any);
	setCacheTagsViewRoutes();
}

/**
 * 缓存多级嵌套数组处理后的一维数组
 * @description 用于 tagsView、菜单搜索中：未过滤隐藏的(isHide)
 */
export function setCacheTagsViewRoutes() {
	const storesTagsView = useTagsViewRoutes(pinia);
	storesTagsView.setTagsViewRoutes(formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes))[0].children);
}

/**
 * 处理路由格式及添加捕获所有路由或 404 Not found 路由
 * @description 替换 dynamicRoutes（/@/router/route）第一个顶级 children 的路由
 * @returns 返回替换后的路由数组
 */
export function setFilterRouteEnd() {
	let filterRouteEnd: any = formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes));
	// notFoundAndNoPower 防止 404、401 不在 layout 布局中，不设置的话，404、401 界面将全屏显示
	// 关联问题 No match found for location with path 'xxx'
	filterRouteEnd[0].children = [...filterRouteEnd[0].children, ...notFoundAndNoPower];
	return filterRouteEnd;
}

/**
 * 添加动态路由
 * @method router.addRoute
 * @description 此处循环为 dynamicRoutes（/@/router/route）第一个顶级 children 的路由一维数组，非多级嵌套
 * @link 参考：https://next.router.vuejs.org/zh/api/#addroute
 */
export async function setAddRoute() {
	await setFilterRouteEnd().forEach((route: RouteRecordRaw) => {
		router.addRoute(route);
	});
}

/**
 * 请求后端路由菜单接口
 * @description isRequestRoutes 为 true，则开启后端控制路由
 * @returns 返回后端路由菜单数据
 */
export function getBackEndControlRoutes(menus: any[],allMenus: any[],localMenu: any[]) {
	let rv: any[] = [];
	menus.forEach(element => {
		const newdata = {
			path: element.Url && element.Url != '' ? element.Url : '/' + element.Id,
			name: element.Id,
			component:'',
			meta: {
				title: element.Text,
				isHide: false,
				isKeepAlive: true,
				isAffix: false,
				isIframe: false,
				icon: element.Icon
			},			
			children:getBackEndControlRoutes(allMenus.filter(x=>x.ParentId == element.Id),allMenus,localMenu)
		}
		let comp = localMenu.filter(x=>x.path == newdata.path);
		if(comp.length>0){
			newdata.component = comp[0].component;
		}
		else{
			newdata.component = "layout/routerView/parent.vue";
		}
		rv.push(newdata);
	});
	return rv;
}


/**
 * 后端路由 component 转换
 * @param routes 后端返回的路由表数组
 * @returns 返回处理成函数后的 component
 */
export function backEndComponent(routes: any) {
	if (!routes) return;
	return routes.map((item: any) => {
		if (item.component) item.component = dynamicImport(dynamicViewsModules, item.component as string);
		item.children && backEndComponent(item.children);
		return item;
	});
}

/**
 * 后端路由 component 转换函数
 * @param dynamicViewsModules 获取目录下的 .vue、.tsx 全部文件
 * @param component 当前要处理项 component
 * @returns 返回处理成函数后的 component
 */
export function dynamicImport(dynamicViewsModules: Record<string, Function>, component: string) {
	const keys = Object.keys(dynamicViewsModules);
	const matchKeys = keys.filter((key) => {
		const k = key.replace(/..\/views|../, '');
		return k.startsWith(`${component}`) || k.startsWith(`/${component}`);
	});
	if (matchKeys?.length === 1) {
		const matchKey = matchKeys[0];
		return dynamicViewsModules[matchKey];
	}
	if (matchKeys?.length > 1) {
		return false;
	}
}

async function GetLocalFile() {
	const keys = Object.keys(viewsModules);
	const rv: any[] = [];
	for (let i = 0; i < keys.length; i++) {
		const key = keys[i];
		const k = key.replace(/..\/views|../, '');
		const match = k.match(/\/(.*?)\/.*?/) ?? [];
		if (match.length > 1) {
			const name = match[1];
			if (name == 'home' || name == 'error' || name == 'login') {
				continue;
			}
			let dir = rv.filter((item) => { return item.name == name });
			let cur = null;
			if (dir.length == 0) {
				cur = {
					path: "/" + name,
					name: name,
					component: "layout/routerView/parent.vue",
					meta: {
						title: name,
						isHide: false,
						isKeepAlive: true,
						isAffix: false,
						isIframe: false,
						icon: "fa fa-folder"
					},
					children: []
				}
				rv.push(cur);
			}
			else {
				cur = dir[0];
			}
			const p = k.replace(/\/index.vue/, '').replace(/.vue/, '')

			const cmp = await viewsModules[key]();
			let ishide = false;
			let className = '';
			let title = '';
			if (cmp.default && cmp.default.name) {
				const narray = cmp.default.name.split(',');
				title = narray[0] ? narray[0] : p.replace(/\//, '_');
				ishide = (narray.length > 1 && narray[1] == "false") ? true : false;
				className = narray.length > 2 ? narray[2] : '';
				if (narray.length > 3) {
					className += "," + narray[3];
				}
			}
			else {
				title = p.replace(/\//, '_');
			}
			cur.children.push({
				path: p,
				name: p.replace(/\//, '_'),
				component: k,
				meta: {
					title: title,
					isHide: ishide,
					isKeepAlive: !ishide,
					className: className,
					icon: "fa fa-file"
				}
			});
		}
	}
	return rv;
}
