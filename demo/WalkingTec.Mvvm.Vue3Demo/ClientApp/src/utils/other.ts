import { nextTick, defineAsyncComponent, createApp, h } from 'vue';
import type { App } from 'vue';
import * as svg from '@element-plus/icons-vue';
import router from '/@/router/index';
import pinia from '/@/stores/index';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { i18n } from '/@/i18n/index';
import { Local } from '/@/utils/storage';
import { verifyUrl } from '/@/utils/toolsValidate';
import request from '/@/utils/request';
import ElementPlus, { ElConfigProvider } from 'element-plus';
import { ElDialog } from 'element-plus';
import wtm from '/@/components/index'
import { directive } from '/@/directive/index';
import { Language } from 'element-plus/es/locale';


// 引入组件
const SvgIcon = defineAsyncComponent(() => import('/@/components/svgIcon/index.vue'));

/**
 * 导出全局注册 element plus svg 图标
 * @param app vue 实例
 * @description 使用：https://element-plus.gitee.io/zh-CN/component/icon.html
 */
export function elSvg(app: App) {
	const icons = svg as any;
	for (const i in icons) {
		app.component(`ele-${icons[i].name}`, icons[i]);
	}
	app.component('SvgIcon', SvgIcon);
}

/**
 * 设置浏览器标题国际化
 * @method const title = useTitle(); ==> title()
 */
export function useTitle() {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	nextTick(() => {
		let webTitle = '';
		let globalTitle: string = themeConfig.value.globalTitle;
		const { path, meta } = router.currentRoute.value;
		if (path === '/login') {
			webTitle = <string>meta.title;
		} else {
			webTitle = setTagsViewNameI18n(router.currentRoute.value);
		}
		document.title = `${webTitle} - ${globalTitle}` || globalTitle;
	});
}

/**
 * 设置 自定义 tagsView 名称、 自定义 tagsView 名称国际化
 * @param params 路由 query、params 中的 tagsViewName
 * @returns 返回当前 tagsViewName 名称
 */
export function setTagsViewNameI18n(item: any) {
	let tagsViewName: string = '';
	const { query, params, meta } = item;
	// 修复tagsViewName匹配到其他含下列单词的路由
	// https://gitee.com/lyt-top/vue-next-admin/pulls/44/files
	const pattern = /^\{("(zh-cn|en|zh-tw)":"[^,]+",?){1,3}}$/;
	if (query?.tagsViewName || params?.tagsViewName) {
		if (pattern.test(query?.tagsViewName) || pattern.test(params?.tagsViewName)) {
			// 国际化
			const urlTagsParams = (query?.tagsViewName && JSON.parse(query?.tagsViewName)) || (params?.tagsViewName && JSON.parse(params?.tagsViewName));
			tagsViewName = urlTagsParams[i18n.global.locale.value];
		} else {
			// 非国际化
			tagsViewName = query?.tagsViewName || params?.tagsViewName;
		}
	} else {
		// 非自定义 tagsView 名称
		tagsViewName = i18n.global.t(meta.title);
	}
	return tagsViewName;
}

/**
 * 图片懒加载
 * @param el dom 目标元素
 * @param arr 列表数据
 * @description data-xxx 属性用于存储页面或应用程序的私有自定义数据
 */
export const lazyImg = (el: string, arr: EmptyArrayType) => {
	const io = new IntersectionObserver((res) => {
		res.forEach((v: any) => {
			if (v.isIntersecting) {
				const { img, key } = v.target.dataset;
				v.target.src = img;
				v.target.onload = () => {
					io.unobserve(v.target);
					arr[key]['loading'] = false;
				};
			}
		});
	});
	nextTick(() => {
		document.querySelectorAll(el).forEach((img) => io.observe(img));
	});
};

/**
 * 全局组件大小
 * @returns 返回 `window.localStorage` 中读取的缓存值 `globalComponentSize`
 */
export const globalComponentSize = (): string => {
	const stores = useThemeConfig(pinia);
	const { themeConfig } = storeToRefs(stores);
	return Local.get('themeConfig')?.globalComponentSize || themeConfig.value?.globalComponentSize;
};




/**
 * 对象深克隆
 * @param obj 源对象
 * @returns 克隆后的对象
 */
export function deepClone(obj: EmptyObjectType) {
	let newObj: EmptyObjectType;
	try {
		newObj = obj.push ? [] : {};
	} catch (error) {
		newObj = {};
	}
	for (let attr in obj) {
		if (obj[attr] && typeof obj[attr] === 'object') {
			newObj[attr] = deepClone(obj[attr]);
		} else {
			newObj[attr] = obj[attr];
		}
	}
	return newObj;
}

export function setValue(oldObj: any, newObj: any) {
	for (let key in oldObj) {
		if (oldObj[key] && Array.isArray(oldObj[key]) == false && typeof oldObj[key] === 'object') {
			if (Object.hasOwn(newObj,key)) {
				setValue(oldObj[key], newObj[key])
			}
			else {
				oldObj[key] = null;
			}
		}
		else {
			if (Object.hasOwn(newObj,key)) {
				oldObj[key] = newObj[key];
			}
			else {
				oldObj[key] = null;
			}
		}
	}
}

export function clearObj(oldObj: any) {
	for (let key in oldObj) {
		if (oldObj[key] && Array.isArray(oldObj[key]) == false && typeof oldObj[key] === 'object') {
			clearObj(oldObj[key])
		}
		else {
			if (Array.isArray(oldObj[key])) {
				oldObj[key] = [];
			}
			else {
				oldObj[key] = null;
			}
		}
	}
}
/**
 * 判断是否是移动端
 */
export function isMobile() {
	if (
		navigator.userAgent.match(
			/('phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone')/i
		)
	) {
		return true;
	} else {
		return false;
	}
}

/**
 * 判断数组对象中所有属性是否为空，为空则删除当前行对象
 * @description @感谢大黄
 * @param list 数组对象
 * @returns 删除空值后的数组对象
 */
export function handleEmpty(list: EmptyArrayType) {
	const arr = [];
	for (const i in list) {
		const d = [];
		for (const j in list[i]) {
			d.push(list[i][j]);
		}
		const leng = d.filter((item) => item === '').length;
		if (leng !== d.length) {
			arr.push(list[i]);
		}
	}
	return arr;
}

/**
 * 打开外部链接
 * @param val 当前点击项菜单
 */
export function handleOpenLink(val: RouteItem) {
	const { origin, pathname } = window.location;
	router.push(val.path);
	if (verifyUrl(<string>val.meta?.isLink)) window.open(val.meta?.isLink);
	else window.open(`${origin}${pathname}#${val.meta?.isLink}`);
}

export function downloadFile(data: Blob) {
	const match = (<any>data)["contentDisposition"].match(/filename\s*=\s*(.+);/i);
	let filename = ``;
	if (match && match.length > 1) {
		filename = match[1];
	}
	const href = URL.createObjectURL(data);
	const aLink = document.createElement('a');
	aLink.setAttribute('href', href);
	aLink.setAttribute('download', filename);
	aLink.click();
	aLink.remove();
}

export function getSelectList(url: string, ids: Array<number> | Array<string>, istree: boolean, action: string|undefined=undefined) {
	let m = 'get';
	if (action) {
		m = action;
	}
	else {
		if (ids && ids.length > 0) {
			m = 'post';
		}
	}
	let dd = [];
	if (Array.isArray(ids)) {
		dd = [...ids]
	}
	else {
		dd = [ids]
	}
	return request({
		url: url,
		method: m,
		data: dd
	}).then(res => {
		let rv: Array<any> = res.map((x: any) => x);
		return rv;
	});
}

export function setlectToTransfer(data: Array<any>) {
	return data.map((item: any) => {
		return {
			label: item.Text,
			key: item.Value
		}
	})
}

export function setFormError(currentInstance: any, error: any) {
	try {
		for (let key in error.FormError) {
			let name = key.replace('.', '_') + '_FormItem';
			currentInstance.refs[name].validateState = 'error';
			currentInstance.refs[name].validateMessage = error.FormError[key];
		}
	}
	catch { }
}

export function clearFormError(currentInstance: any) {
	try {
		for (let key in currentInstance.refs) {
			currentInstance.refs[key].validateState = 'valid';
			currentInstance.refs[key].validateMessage = null;
		}
	}
	catch { }
}

export function openDialog(title: string, component: any, wtmdata: any, refreshFunc: any,width:any) {
	const div = document.createElement('div');
	document.body.appendChild(div);
	const close = () => {
		//@ts-ignore
		app.unmount(div);
		div.remove();
	};
	const app = createApp({
		render() {
			return h(ElConfigProvider,{
			},()=>h(
				ElDialog,
				{
					title: title,
					draggable: true,
					modelValue: true,
					width:width?width:'50%',
					onClosed: () => { close(); }
				},
				() => h(component, { wtmdata: wtmdata, onCloseDialog: () => { close(); }, onRefresh: () => { if (refreshFunc) refreshFunc(); } })
			));
		}
	});
	directive(app);
	app.use(router).use(ElementPlus).use(i18n).use(wtm).mount(div);
}

export function flatTree(arr:any,intentkey:any=null,intent:number=4){
	if (arr.length <= 0) return false;
	for (let i = 0; i < arr.length; i++) {
		if (arr[i].children && arr[i].children.length>0) {
			let temp = [...arr[i].children];
			if(intentkey !== null){
				for(let j=0;j<intent;j++){
					temp.forEach(element => {
						element[intentkey] = "-"+element[intentkey]
					});
				}
			}
			arr = arr.slice(0, i + 1).concat(temp, arr.slice(i + 1));
		}
	}
	return arr;
}

/**
 * 统一批量导出
 * @method elSvg 导出全局注册 element plus svg 图标
 * @method useTitle 设置浏览器标题国际化
 * @method setTagsViewNameI18n 设置 自定义 tagsView 名称、 自定义 tagsView 名称国际化
 * @method lazyImg 图片懒加载
 * @method globalComponentSize() element plus 全局组件大小
 * @method deepClone 对象深克隆
 * @method isMobile 判断是否是移动端
 * @method handleEmpty 判断数组对象中所有属性是否为空，为空则删除当前行对象
 * @method handleOpenLink 打开外部链接
 */
const other = {
	elSvg: (app: App) => {
		elSvg(app);
	},
	useTitle: () => {
		useTitle();
	},
	setTagsViewNameI18n(route: RouteToFrom) {
		return setTagsViewNameI18n(route);
	},
	lazyImg: (el: string, arr: EmptyArrayType) => {
		lazyImg(el, arr);
	},
	globalComponentSize: () => {
		return globalComponentSize();
	},
	deepClone: (obj: EmptyObjectType) => {
		return deepClone(obj);
	},
	isMobile: () => {
		return isMobile();
	},
	handleEmpty: (list: EmptyArrayType) => {
		return handleEmpty(list);
	},
	handleOpenLink: (val: RouteItem) => {
		handleOpenLink(val);
	},
	downloadFile: (data: Blob) => {
		downloadFile(data);
	},
	getSelectList: (url: string, ids: Array<number> | Array<string>, istree: boolean, action: string|undefined=undefined) => {
		return getSelectList(url, ids, istree, action);
	},
	setlectToTransfer: (data: Array<any>) => {
		return setlectToTransfer(data);
	},
	setValue(oldObj: any, newObj: any) {
		setValue(oldObj, newObj);
	},
	setFormError(currentInstance: any, error: any) {
		setFormError(currentInstance, error);
	},
	clearFormError(currentInstance: any) {
		clearFormError(currentInstance);
	},
	clearObj(oldObj: any) {
		clearObj(oldObj);
	},
	openDialog(title: string, component: any, wtmdata: any = null, refreshFunc: any = null, width: any=null) {
		openDialog(title, component, wtmdata, refreshFunc,width);
	},
	flatTree(arr:any,intentkey:any=null,intent:number=2) : any[]{
		return flatTree(arr,intentkey,intent);
	}
};

// 统一批量导出
export default other;
