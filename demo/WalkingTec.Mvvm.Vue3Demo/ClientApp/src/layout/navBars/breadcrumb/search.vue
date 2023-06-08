<template>
	<div class="layout-search-dialog">
		<!-- <el-dialog v-model="state.isShowSearch" destroy-on-close :show-close="false">
			<template #footer>
				<el-autocomplete
					v-model="state.menuQuery"
					:fetch-suggestions="menuSearch"
					:placeholder="$t('message._system.user.searchPlaceholder')"
					ref="layoutMenuAutocompleteRef"
					@select="onHandleSelect"
					:fit-input-width="true"
				>
					<template #prefix>
						<el-icon class="el-input__icon">
							<ele-Search />
						</el-icon>
					</template>
					<template #default="{ item }">
						<div>
							<SvgIcon :name="item.meta.icon" class="mr5" />
							{{ $t(item.meta.title) }}
						</div>
					</template>
				</el-autocomplete>
			</template>
		</el-dialog> -->
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbSearch">
import { reactive, ref, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { storeToRefs } from 'pinia';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';

// 定义变量内容
const storesTagsViewRoutes = useTagsViewRoutes();
const { tagsViewRoutes } = storeToRefs(storesTagsViewRoutes);
const layoutMenuAutocompleteRef = ref();
const { t } = useI18n();
const router = useRouter();
const state = reactive<SearchState>({
	isShowSearch: false,
	menuQuery: '',
	tagsViewList: [],
});

// 搜索弹窗打开
const openSearch = () => {
	state.menuQuery = '';
	state.isShowSearch = true;
	initTageView();
	nextTick(() => {
		setTimeout(() => {
			layoutMenuAutocompleteRef.value.focus();
		});
	});
};
// 搜索弹窗关闭
const closeSearch = () => {
	state.isShowSearch = false;
};
// 菜单搜索数据过滤
const menuSearch = (queryString: string, cb: Function) => {
	let results = queryString ? state.tagsViewList.filter(createFilter(queryString)) : state.tagsViewList;
	cb(results);
};
// 菜单搜索过滤
const createFilter = (queryString: string) => {
	return (restaurant: RouteItem) => {
		return (
			restaurant.path.toLowerCase().indexOf(queryString.toLowerCase()) > -1 ||
			restaurant.meta!.title!.toLowerCase().indexOf(queryString.toLowerCase()) > -1 ||
			t(restaurant.meta!.title!).indexOf(queryString.toLowerCase()) > -1
		);
	};
};
// 初始化菜单数据
const initTageView = () => {
	if (state.tagsViewList.length > 0) return false;
	tagsViewRoutes.value.map((v: RouteItem) => {
		if (!v.meta?.isHide) state.tagsViewList.push({ ...v });
	});
};
// 当前菜单选中时
const onHandleSelect = (item: RouteItem) => {
	let { path, redirect } = item;
	if (item.meta?.isLink && !item.meta?.isIframe) window.open(item.meta?.isLink);
	else if (redirect) router.push(redirect);
	else router.push(path);
	closeSearch();
};

// 暴露变量
defineExpose({
	openSearch,
});
</script>

<style scoped lang="scss">
.layout-search-dialog {
	position: relative;
	:deep(.el-dialog) {
		.el-dialog__header,
		.el-dialog__body {
			display: none;
		}
		.el-dialog__footer {
			position: absolute;
			left: 50%;
			transform: translateX(-50%);
			top: -53vh;
		}
	}
	:deep(.el-autocomplete) {
		width: 560px;
		position: absolute;
		top: 150px;
		left: 50%;
		transform: translateX(-50%);
	}
}
</style>
