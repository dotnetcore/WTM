import importToCDN from 'vite-plugin-cdn-import';

/**
 * 打包相关
 * 注意 prodUrl：使用的是 jsdelivr 还是 unpkg。它们的 path 可能不一致
 * 文章链接：https://blog.csdn.net/qq_34450741/article/details/129766676，使用的是 jsdelivr
 * @description importToCDN https://github.com/mmf-fe/vite-plugin-cdn-import
 * @description cdn 在线引入的 cdn 地址配置。path：https://www.jsdelivr.com/ || https://unpkg.com/
 * @description external 打包时，过滤包导入。参考：https://rollupjs.org/configuration-options/#external
 */
export const buildConfig = {
	cdn() {
		return importToCDN({
			prodUrl: 'https://unpkg.com/{name}@{version}/{path}',
			modules: [
				// autoComplete('vue'),
				// autoComplete('axios'),
				{
					name: 'vue',
					var: 'Vue',
					path: 'dist/vue.global.js',
				},
				{
					name: 'vue-demi',
					var: 'VueDemi',
					path: 'lib/index.iife.js',
				},
				{
					name: 'vue-router',
					var: 'VueRouter',
					path: 'dist/vue-router.global.js',
				},
				{
					name: 'element-plus',
					var: 'ElementPlus',
					path: 'dist/index.full.js',
				},
				// {
				// 	name: '@element-plus/icons-vue',
				// 	var: 'ElementPlusIconsVue',
				// 	path: 'dist/index.iife.min.js',
				// },
				// {
				// 	name: 'echarts',
				// 	var: 'echarts',
				// 	path: 'dist/echarts.min.js',
				// },
				// {
				// 	name: 'echarts-gl',
				// 	var: 'echarts-gl',
				// 	path: 'dist/echarts-gl.min.js',
				// },
				// {
				// 	name: 'echarts-wordcloud',
				// 	var: 'echarts-wordcloud',
				// 	path: 'dist/echarts-wordcloud.min.js',
				// },
				// {
				// 	name: 'vue-i18n',
				// 	var: 'VueI18n',
				// 	path: 'dist/vue-i18n.global.min.js',
				// },
				// {
				// 	name: 'jsplumb',
				// 	var: 'jsPlumb',
				// 	path: 'dist/js/jsplumb.min.js',
				// },
				// {
				// 	name: 'cropperjs',
				// 	var: 'Cropper',
				// 	path: 'dist/cropper.min.js',
				// },
				// {
				// 	name: 'sortablejs',
				// 	var: 'Sortable',
				// 	path: 'Sortable.min.js',
				// },
				// {
				// 	name: 'qrcodejs2-fixes',
				// 	var: 'QRCode',
				// 	path: 'qrcode.min.js',
				// },
				// {
				// 	name: 'print-js',
				// 	var: 'printJS',
				// 	path: 'dist/print.min.js',
				// },
				// {
				// 	name: '@wangeditor/editor',
				// 	var: 'wangEditor',
				// 	path: 'dist/index.min.js',
				// },
				// {
				// 	name: '@wangeditor/editor-for-vue',
				// 	var: 'WangEditorForVue',
				// 	path: 'dist/index.min.js',
				// },
				// {
				// 	name: 'vue-grid-layout',
				// 	var: 'VueGridLayout',
				// 	path: 'https://cdn.jsdelivr.net/npm/vue-grid-layout@3.0.0-beta1/dist/vue-grid-layout.umd.min.js',
				// },
			],
		});
	},
	external: [
		'vue',
		// 'axios',
		'vue-router',
		'element-plus',
		// '@element-plus/icons-vue',
		// 'echarts',
		// 'echarts-gl',
		// 'echarts-wordcloud',
		// 'vue-i18n',
		// 'jsplumb',
		// 'cropperjs',
		// 'sortablejs',
		// 'qrcodejs2-fixes',
		// 'print-js',
		// '@wangeditor/editor',
		// '@wangeditor/editor-for-vue',
		// 'vue-grid-layout',
	],
};
