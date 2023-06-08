import { createI18n } from 'vue-i18n';
import pinia from '/@/stores/index';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

// 定义语言国际化内容

/**
 * 说明：
 * 须在 pages 下新建文件夹（建议 `要国际化界面目录` 与 `i18n 目录` 相同，方便查找），
 * 注意国际化定义的字段，不要与原有的定义字段相同。
 * 1、/src/i18n/lang 下的 ts 为框架的国际化内容
 * 2、/src/i18n/pages 下的 ts 为各界面的国际化内容
 */

// element plus 自带国际化
import enLocale from 'element-plus/lib/locale/lang/en';
import zhcnLocale from 'element-plus/lib/locale/lang/zh-cn';


// 定义变量内容
const messages = {};
const element = { en: enLocale, 'zh-cn': zhcnLocale };
const itemize = { en: [], 'zh-cn': []};
const modules: Record<string, any> = import.meta.glob('./**/*.ts', { eager: true });
// 对自动引入的 modules 进行分类 en、zh-cn、zh-tw
// https://vitejs.cn/vite3-cn/guide/features.html#glob-import
for (const path in modules) {
	const key = path.match(/(\S+)\/(\S+).ts/);
	
	if ((<any>itemize)[key![2]]) 
	{
		(<any>itemize)[key![2]].push(modules[path].default);
	}
	else{
		(<any>itemize)[key![2]] = modules[path];
	}
}

function deepMerge(target:any, source:any) {
	if (Object.prototype.toString.call(target) === "[object Object]" && Object.prototype.toString.call(source) === "[object Object]") {
		// 遍历源对象，将源对象存在但目标对象不存在的属性赋值给目标对象
	  for (const key in source) {
		if (!target.hasOwnProperty(key)) {
		  target[key] = source[key];
		}
	  }
	  // 遍历目标对象
	  for (const key in target) {
		if (source.hasOwnProperty(key)) {
			// 判断属性值数据类型，若不是对象类型则赋值给目标对象
		  if (!(Object.prototype.toString.call(source[key]) === "[object Object]")) {
			target[key] = source[key];
		  } else {
			// 是对象类型则递归调用
			deepMerge(target[key], source[key]);
		  }
		}
	  }
	}
  };

// 合并数组对象（非标准数组对象，数组中对象的每项 key、value 都不同）
function mergeArrObj<T>(list: T, key: string) {
	let obj = {};
	(<any>list)[key].forEach((i: EmptyObjectType) => {
		deepMerge(obj,i);
	});
	return obj;
}

// 处理最终格式
for (const key in itemize) {
	(<any>messages)[key] = {
		name: key,
		el: (<any>element)[key].el,
		message: mergeArrObj(itemize, key),
	};
}
// 读取 pinia 默认语言
const stores = useThemeConfig(pinia);
const { themeConfig } = storeToRefs(stores);

// 导出语言国际化
// https://vue-i18n.intlify.dev/guide/essentials/fallback.html#explicit-fallback-with-one-locale
export const i18n = createI18n({
	legacy: false,
	silentTranslationWarn: true,
	missingWarn: false,
	silentFallbackWarn: true,
	fallbackWarn: false,
	locale: themeConfig.value.globalI18n,
	fallbackLocale: zhcnLocale.name,
	messages,
});
